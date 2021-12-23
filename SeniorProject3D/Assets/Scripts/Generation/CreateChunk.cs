using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

public class CreateChunk
{
    private int vertexStart, triangleStart, blockIndex;
    public int width,height,depth;
    private Vector3 blockPos = Vector3.zero;
    private int seed, amplitudeMulti;
    private float frequencyMulti;
    private List<Mesh> inputMeshes;
    private CombineMeshJobs jobs;
    public BlockData[] chunkBlockData;
    public List<Vector3> topBlockPositions = new List<Vector3>();
    public Dictionary<Vector2,Vector3> topBlockXZToVec = new Dictionary<Vector2, Vector3>();
    // Dictionary for keeping track of highest y vertice for each x,z coordinate
    public Dictionary<(float, float), float> topVerticeMap = new Dictionary<(float,float), float>();

    // Job for speeding up mesh execution time, NativeArrays must be deallocated at end of job
    [BurstCompile]
    struct CombineMeshJobs : IJobParallelFor {
        [ReadOnly] public Mesh.MeshDataArray meshData; // read only, will not be changed during job
        public Mesh.MeshData mesh;
        public NativeArray<int> vertexStarts;
        public NativeArray<int> triangleStarts;

        public void Execute(int index) {
            Mesh.MeshData data = meshData[index];
            int vertexCount = data.vertexCount;
            int vertexStart = vertexStarts[index];

            // getting data out of mesh and reinterpreting Vector3 as float3
            NativeArray<float3> vertices = new NativeArray<float3>(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetVertices(vertices.Reinterpret<Vector3>());
            NativeArray<float3> normals = new NativeArray<float3>(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetNormals(normals.Reinterpret<Vector3>());
            NativeArray<float3> uvs = new NativeArray<float3>(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetUVs(0, uvs.Reinterpret<Vector3>()); // only one texture, so only channel we need

            NativeArray<Vector3> outputVertices = mesh.GetVertexData<Vector3>(stream: 0);
            NativeArray<Vector3> outputNormals = mesh.GetVertexData<Vector3>(stream: 1);
            NativeArray<Vector3> outputUVs = mesh.GetVertexData<Vector3>(stream: 2);

            for (int i = 0; i < vertexCount; i++){
                outputVertices[i + vertexStart] = vertices[i];
                outputNormals[i + vertexStart] = normals[i];
                outputUVs[i + vertexStart] = uvs[i];
            }

            int triangleCount = data.GetSubMesh(0).indexCount;
            int triangleStart = triangleStarts[index];
            NativeArray<int> outputTriangles = mesh.GetIndexData<int>();

            NativeArray<ushort> triangles = data.GetIndexData<ushort>();
            for (int i = 0; i < triangleCount; i++){
                int triangleIndex = triangles[i];
                outputTriangles[i + triangleStart] = vertexStart + triangleIndex;
            }

            Dispose();

            void Dispose() {
                vertices.Dispose();
                normals.Dispose();
                uvs.Dispose();
            }
        } 
    }

    public struct BlockData {
        public MeshController.BlockType BlockType;
        public bool Smoothed;
        public BlockData(MeshController.BlockType blockType, bool smoothed){
            this.BlockType = blockType;
            this.Smoothed = smoothed;
        }
    }

    public CreateChunk(
        GameObject gameObject,
        Material atlas,
        ChunkController controller,
        Vector3? startPos = null,
        int width = ChunkController.maxWidth, 
        int height = ChunkController.maxHeight,
        int depth = ChunkController.maxDepth, 
        MeshController.BlockType[,,] customBlocks = null
    ){
        this.width = width;
        this.height = height;
        this.depth = depth;
        int meshCount = width * height * depth;
        vertexStart = 0;
        triangleStart = 0;
        blockIndex = 0;

        inputMeshes = new List<Mesh>();
        
        int blockCount = meshCount;

        chunkBlockData = new BlockData[blockCount];

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>(); //create the mesh filter
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>(); //create the mesh renderer
        meshRenderer.material = atlas; //add the material for the atlas

        // Move object to target position and update offset
        Vector3 offset = Vector3.zero;
        if (startPos != null){
            gameObject.transform.position = (Vector3) startPos;
            offset = (Vector3) startPos;
        }

        // Initialization for jobs, for processing meshes
        jobs = new CombineMeshJobs();
        jobs.vertexStarts = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        jobs.triangleStarts = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        // Generate block data
        if (customBlocks != null){
            ParseBlockData(customBlocks);
        } else {
            seed = (int) UnityEngine.Random.Range(20, 100); //Allows for a range of translation values to make the world unique every time.
            GenerateBlockData();
        }
        
        // Generate regular blocks and prepare top blocks for smoothing
        for (int i = 0; i < width; i++){
            for (int j = 0; j < height; j++){
                for (int k = 0; k < depth; k++){
                    if (chunkBlockData[CoordToId(i,j,k)].Smoothed){
                        float centerHeight = j + offset.y;
                        float centerWidth = i + offset.x;
                        float centerDepth = k + offset.z;

                        // front-left corner
                        if (topVerticeMap.ContainsKey((centerWidth - 0.5f, centerDepth + 0.5f))){
                            if (centerHeight + 0.5f > topVerticeMap[(centerWidth - 0.5f, centerDepth + 0.5f)]){
                                topVerticeMap[(centerWidth - 0.5f, centerDepth + 0.5f)] = centerHeight + 0.5f;
                            }
                        } else {
                            topVerticeMap.Add((centerWidth - 0.5f, centerDepth + 0.5f), centerHeight + 0.5f);
                        }
                        
                        // front-right corner
                        if (topVerticeMap.ContainsKey((centerWidth + 0.5f, centerDepth + 0.5f))){
                            if (centerHeight + 0.5f > topVerticeMap[(centerWidth + 0.5f, centerDepth + 0.5f)]){
                                topVerticeMap[(centerWidth + 0.5f, centerDepth + 0.5f)] = centerHeight + 0.5f;
                            }
                        } else {
                            topVerticeMap.Add((centerWidth + 0.5f, centerDepth + 0.5f), centerHeight + 0.5f);
                        }
                        
                        // back-left corner
                        if (topVerticeMap.ContainsKey((centerWidth - 0.5f, centerDepth - 0.5f))){
                            if (centerHeight + 0.5f > topVerticeMap[(centerWidth - 0.5f, centerDepth - 0.5f)]){
                                topVerticeMap[(centerWidth - 0.5f, centerDepth - 0.5f)] = centerHeight + 0.5f;
                            }
                        } else {
                            topVerticeMap.Add((centerWidth - 0.5f, centerDepth - 0.5f), centerHeight + 0.5f);
                        }
                        
                        // back-right corner
                        if (topVerticeMap.ContainsKey((centerWidth + 0.5f, centerDepth - 0.5f))){
                            if (centerHeight + 0.5f > topVerticeMap[(centerWidth + 0.5f, centerDepth - 0.5f)]){
                                topVerticeMap[(centerWidth + 0.5f, centerDepth - 0.5f)] = centerHeight + 0.5f;
                            }
                        } else {
                            topVerticeMap.Add((centerWidth + 0.5f, centerDepth - 0.5f), centerHeight + 0.5f);
                        }
                    } else {
                        blockPos = new Vector3(i + offset.x, j + offset.y, k + offset.z);
                        GenerateBlock(i,j,k);
                    }
                }
            }
        }

        // Generate top smooth layer
        GenerateTopSmoothLayer();

        jobs.meshData = Mesh.AcquireReadOnlyMeshData(inputMeshes);
        Mesh.MeshDataArray meshData = Mesh.AllocateWritableMeshData(1);
        jobs.mesh = meshData[0];
        jobs.mesh.SetIndexBufferParams(triangleStart, IndexFormat.UInt32);
        jobs.mesh.SetVertexBufferParams(vertexStart,
            new VertexAttributeDescriptor(VertexAttribute.Position, stream: 0),
            new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, stream: 2));

        JobHandle meshHandler = jobs.Schedule(inputMeshes.Count, 4);
        Mesh chunkMesh = new Mesh();
        chunkMesh.name = "Terrain";
        SubMeshDescriptor subMeshDescriptor = new SubMeshDescriptor(0, triangleStart, MeshTopology.Triangles);
        subMeshDescriptor.firstVertex = 0;
        subMeshDescriptor.vertexCount = vertexStart;

        meshHandler.Complete();

        jobs.mesh.subMeshCount = 1;
        jobs.mesh.SetSubMesh(0, subMeshDescriptor);

        Mesh.ApplyAndDisposeWritableMeshData(meshData, new[] {chunkMesh});
        jobs.meshData.Dispose();
        jobs.vertexStarts.Dispose();
        jobs.triangleStarts.Dispose();
        chunkMesh.RecalculateBounds(); // recalculates colliders

        meshFilter.mesh = chunkMesh; // setting mesh

        // update player and enemy spawn locations
        controller.playerStartPosition = topBlockPositions[topBlockPositions.Count / 3];

        // add mesh collider
        gameObject.AddComponent<MeshCollider>();
        gameObject.transform.position = new Vector3(0,0,0); // fix chunk position offset error

        void ParseBlockData(MeshController.BlockType[,,] customBlocks){
            for (int i = 0; i < depth; i++){
                for (int j = 0; j < width; j++){
                    for (int k = 0; k < height; k++){
                        chunkBlockData[CoordToId(j,k,i)] = new BlockData(customBlocks[j,k,i], false);
                        if (k + 1 < height){
                            if (customBlocks[j,k+1,i] == MeshController.BlockType.AIR && customBlocks[j,k,i] != MeshController.BlockType.AIR){
                                topBlockPositions.Add(new Vector3(j,k,i));
                            }
                        }
                    }
                }
            }
        }

        void GenerateBlockData(){
            Dictionary<Vector2, float> noise = new Dictionary<Vector2, float>();
            int octaves = (int) ChunkController.Instance.octaves;
            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            // Get noiseHeights for each column, find max and min noiseHeights
            for (int i = 0; i < depth; i++){
                for (int j = 0; j < width; j++){
                    float amplitude = ChunkController.Instance.amplitude;
                    float frequency = ChunkController.Instance.frequency;
                    float persistence = ChunkController.Instance.persistence;
                    float lacunarity = ChunkController.Instance.lacunarity;
                    float noiseHeight = 0.0f;

                    for (int o = 0; o < octaves; o++){
                        float perlinValue = Mathf.PerlinNoise((j + seed) / frequency, (i + seed) / frequency);
                        noiseHeight += perlinValue * amplitude;
                        amplitude *= persistence;
                        frequency /= lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight){
                        maxNoiseHeight = noiseHeight;
                    } else if (noiseHeight < minNoiseHeight){
                        minNoiseHeight = noiseHeight;
                    }
                    noise.Add(new Vector2(i,j), noiseHeight);
                }
            }

            // Normalize noiseHeights and input block data
            for (int i = 0; i < depth; i++){
                for (int j = 0; j < width; j++){
                    float noiseHeight = (noise[new Vector2(i,j)] - minNoiseHeight) / (maxNoiseHeight - minNoiseHeight);

                    int columnMaxHeight = (int) (noiseHeight * height) + 1; 
                    if (columnMaxHeight > height) columnMaxHeight = height;
                    if (columnMaxHeight < 1) columnMaxHeight = 1;

                    // bounds 0 to perlinHeight for solid blocks, perlinHeight to maxHeight for air blocks
                    int dirtLayerRange = columnMaxHeight - (int) UnityEngine.Random.Range(3,5);

                    for (int k = height - 1; k >= 0; k--){
                        if (k > columnMaxHeight){
                            chunkBlockData[CoordToId(j, k, i)] = new BlockData(MeshController.BlockType.AIR, false);
                            continue;
                        }
                        if (k == columnMaxHeight || k == height - 1){
                            MeshController.BlockType topBlockType;
                            float paintHeight = noiseHeight + (UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f ? 
                                -(Mathf.PerlinNoise(UnityEngine.Random.Range(0.0f, 500.0f), UnityEngine.Random.Range(0.0f, 500.0f)) / 50) : 
                                (Mathf.PerlinNoise(UnityEngine.Random.Range(0.0f, 500.0f), UnityEngine.Random.Range(0.0f, 500.0f)) / 50));

                            if (paintHeight >= 0.95f){
                                topBlockType = MeshController.BlockType.SNOW;
                            } 
                            else if (paintHeight > 0.85f){
                                topBlockType = MeshController.BlockType.DARK_STONE;
                            }
                            else if (paintHeight > 0.7f){
                                topBlockType = MeshController.BlockType.STONE;
                            }
                            else if (paintHeight > 0.55f){
                                topBlockType = MeshController.BlockType.GRASSTOP_DARK;
                            }
                            else if (paintHeight > 0.30f){
                                topBlockType = MeshController.BlockType.GRASS_MED;
                            }
                            else if (paintHeight > 0.15f){
                                topBlockType = MeshController.BlockType.GRASSTOP;
                            }
                            else {
                                topBlockType = MeshController.BlockType.SAND;
                            } 
                            chunkBlockData[CoordToId(j, k, i)] = new BlockData(topBlockType, true);
                            topBlockPositions.Add(new Vector3(j,k,i)); // Add block as suitable top block to check for smoothing
                            topBlockXZToVec.Add(new Vector2(j,i), new Vector3(j,k,i));
                        }
                        else if (k > dirtLayerRange){
                            chunkBlockData[CoordToId(j, k, i)] = new BlockData(MeshController.BlockType.DIRT, false);
                        }
                        else {
                            chunkBlockData[CoordToId(j, k, i)] = new BlockData(MeshController.BlockType.STONE, false);
                        }
                    }
                }
            }
        }

        void GenerateBlock(int x, int y, int z) {
            CreateBlock currentBlock = new CreateBlock(blockPos, chunkBlockData[CoordToId(x,y,z)], this);
            if (currentBlock.blockMesh != null) {
                inputMeshes.Add(currentBlock.blockMesh);
                jobs.vertexStarts[blockIndex] = vertexStart;
                jobs.triangleStarts[blockIndex] = triangleStart;
                vertexStart += currentBlock.blockMesh.vertexCount;
                triangleStart += (int) currentBlock.blockMesh.GetIndexCount(0); // number of triangles within this mesh, 0 for only block mesh
                blockIndex++;
            }
        }

        void GenerateTopSmoothLayer() {
            // Generate top smoothed layer
            for(int i = 0; i < topBlockPositions.Count; i++){

                MeshController.BlockType blockType = chunkBlockData[CoordToId((int)topBlockPositions[i].x,(int)topBlockPositions[i].y,(int)topBlockPositions[i].z)].BlockType;

                //  p[0] ---------- p[1]
                //     |            |
                //     |            |
                //     |            |
                //  p[3] ---------- p[2]

                // Grab coordinates of highest points for a grid tile
                List<Vector3> points = new List<Vector3>();
                points.Add(new Vector3((int)topBlockPositions[i].x-0.5f, topVerticeMap[((int)topBlockPositions[i].x-0.5f, (int)topBlockPositions[i].z+0.5f)], (int)topBlockPositions[i].z+0.5f)); // FL,p0
                points.Add(new Vector3((int)topBlockPositions[i].x+0.5f, topVerticeMap[((int)topBlockPositions[i].x+0.5f, (int)topBlockPositions[i].z+0.5f)], (int)topBlockPositions[i].z+0.5f)); // FR,p1
                points.Add(new Vector3((int)topBlockPositions[i].x+0.5f, topVerticeMap[((int)topBlockPositions[i].x+0.5f, (int)topBlockPositions[i].z-0.5f)], (int)topBlockPositions[i].z-0.5f)); // BR,p2
                points.Add(new Vector3((int)topBlockPositions[i].x-0.5f, topVerticeMap[((int)topBlockPositions[i].x-0.5f, (int)topBlockPositions[i].z-0.5f)], (int)topBlockPositions[i].z-0.5f)); // BL,p3

                // In order to generate a mesh in the correct direction, we need
                // to input the vertices in a clockwise order
                Dictionary<Vector3, Vector3> edgesDirectedClockwise = new Dictionary<Vector3, Vector3>();
                edgesDirectedClockwise.Add(points[0], points[1]);
                edgesDirectedClockwise.Add(points[1], points[2]);
                edgesDirectedClockwise.Add(points[2], points[3]);
                edgesDirectedClockwise.Add(points[3], points[0]);

                // Create edges between vertices diagonally
                Dictionary<Vector3, Vector3> edgesUndirectedDiagonal = new Dictionary<Vector3, Vector3>();
                edgesUndirectedDiagonal.Add(points[0], points[2]);
                edgesUndirectedDiagonal.Add(points[1], points[3]);
                edgesUndirectedDiagonal.Add(points[2], points[0]);
                edgesUndirectedDiagonal.Add(points[3], points[1]);

                // Find highest point and lowest point of four vertices
                int highestPointIndex = 0;
                for (int j = 1; j < 4; j++){
                    if (points[j].y == points[highestPointIndex].y){
                        if (edgesUndirectedDiagonal[points[j]].y < edgesUndirectedDiagonal[points[highestPointIndex]].y){
                            highestPointIndex = j;
                        }
                    } else if (points[j].y > points[highestPointIndex].y){
                        highestPointIndex = j;
                    }
                }

                // Generate triangles based on highest and lowest points
                Vector3 v0 = points[highestPointIndex];
                Vector3 v1 = edgesDirectedClockwise[v0];
                Vector3 v2 = edgesDirectedClockwise[v1];
                Vector3 v3 = edgesDirectedClockwise[v2];
                Mesh[] meshes = new Mesh[2];
                meshes[0] = new CreateTriangle(v0, v1, v2, blockType).mesh;
                meshes[1] = new CreateTriangle(v2, v3, v0, blockType).mesh;
                Mesh mesh = MeshController.MergeMeshes(meshes);
                if (mesh != null) {
                    inputMeshes.Add(mesh);
                    jobs.vertexStarts[blockIndex] = vertexStart;
                    jobs.triangleStarts[blockIndex] = triangleStart;
                    vertexStart += mesh.vertexCount;
                    triangleStart += (int) mesh.GetIndexCount(0); // number of triangles within this mesh, 0 for only block mesh
                    blockIndex++;
                }

                // Update top block vertices to allow for object instantiation at correct positions
                topBlockPositions[i] = new Vector3(topBlockPositions[i].x, ((v0.y + v2.y) / 2) + 1, topBlockPositions[i].z);
            }
        }
    }

    // Coordinate parsing functions
    public int CoordToId(int x, int y, int z) {
        return (z * width * height) + (y * width) + x;
    }

    public int[] IdToCoord(int id) {
        int z = id / (width * height);
        id -= z * width * height;
        int y = id / width;
        int x = id % width;
        return new int[3] {x,y,z};
    }
    public MeshController.BlockState NeighCheck(int x, int y, int z) {
        // Check if block is out of bounds or is air
        if (x < 0 || x >= width || y < 0 || y >= height || z < 0 || z >= depth) return MeshController.BlockState.NONE;
        if (chunkBlockData[CoordToId(x,y,z)].BlockType == MeshController.BlockType.AIR) return MeshController.BlockState.AIR;

        return MeshController.BlockState.OTHER;
    }
}
