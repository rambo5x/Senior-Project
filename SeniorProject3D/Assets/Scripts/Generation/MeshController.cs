using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2>; //add library for required vertex data

public class MeshController
{
    public enum BlockType
    {
        GRASSTOP, GRASS_MED, GRASSTOP_DARK, GRASSSIDE, DIRT, WATER, STONE, DARK_STONE, SAND, SNOW, AIR
    };
    public enum BlockState {NONE, AIR, OTHER};

    public enum BlockSide { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK, DIAG_FRONT, DIAG_BACK, DIAG_LEFT, DIAG_RIGHT};

    public enum BlockTriangle {
        DIAG_FRONT, DIAG_BACK, DIAG_LEFT, DIAG_RIGHT,
        DIAG_RT_FRONT_HFL, DIAG_RT_FRONT_HFR, DIAG_RT_BACK_HFL, DIAG_RT_BACK_HFR, DIAG_RT_LEFT_HFL, DIAG_RT_LEFT_HFR, DIAG_RT_RIGHT_HFL, DIAG_RT_RIGHT_HFR,
        TRI_FRONT_LEFT, TRI_FRONT_RIGHT, TRI_BACK_LEFT, TRI_BACK_RIGHT, TRI_LEFT_LEFT, TRI_LEFT_RIGHT, TRI_RIGHT_LEFT, TRI_RIGHT_RIGHT
    };

    public static Vector2[,] blockUVs = {
        /*GRASS_LITE*/{ new Vector2(0.125f, 0.375f), new Vector2(0.1875f,0.375f),
                        new Vector2(0.125f, 0.4375f), new Vector2(0.1875f,0.4375f) },
        /*GRASS_MED*/ { new Vector2(0.125f, 0.4375f), new Vector2(0.1875f,0.4375f),
                        new Vector2(0.125f, 0.5f), new Vector2(0.1875f,0.5f) },
        /*GRASS_DARK*/{ new Vector2(0.0625f, 0.375f), new Vector2(0.125f,0.375f),
                        new Vector2(0.0625f, 0.4375f), new Vector2(0.125f,0.4375f) },
        /*GRASSSIDE*/ { new Vector2( 0.1875f, 0.9375f ), new Vector2( 0.25f, 0.9375f),
                        new Vector2( 0.1875f, 1.0f ),new Vector2( 0.25f, 1.0f )},
        /*DIRT*/	  { new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
                        new Vector2( 0.125f, 1.0f ),new Vector2( 0.1875f, 1.0f )},
        /*WATER*/	  { new Vector2(0.875f,0.125f),  new Vector2(0.9375f,0.125f),
                        new Vector2(0.875f,0.1875f), new Vector2(0.9375f,0.1875f)},
        /*STONE*/	  { new Vector2( 0.0625f, 0.9375f ), new Vector2( 0.125f, 0.9375f),
                        new Vector2( 0.0625f, 1.0f ),new Vector2( 0.125f, 1.0f )},
        /*DARK_STONE*/{ new Vector2( 0.0625f, 0.875f ), new Vector2( 0.125f, 0.875f),
                        new Vector2( 0.0625f, 0.9375f ),new Vector2( 0.125f, 0.9375f )},
        /*SAND*/	  { new Vector2(0.125f,0.875f),  new Vector2(0.1875f,0.875f),
                        new Vector2(0.125f,0.9375f), new Vector2(0.1875f,0.9375f)},
        /*SNOW*/      { new Vector2(0.125f, 0.6875f ), new Vector2( 0.1875f, 0.6875f),
                        new Vector2(0.125f, 0.75f ),new Vector2( 0.1875f, 0.75f )},
    };

    public static Mesh MergeMeshes(Mesh[] meshes) //merge meshes
    {
        Mesh mesh = new Mesh();

        Dictionary<VertexData, int> pointsOrder = new Dictionary<VertexData, int>(); //keep track of the order of the points when new vertices get added
        HashSet<VertexData> pointsHash = new HashSet<VertexData>(); //hash the vertex data to quickly find if vertex already exists
        List<int> tris = new List<int>(); //handle any duplicate or append tris

        int pIndex = 0;
        for (int i = 0; i < meshes.Length; i++) //loop through each mesh
        {
            if (meshes[i] == null)
                continue;
            for (int j = 0; j < meshes[i].vertices.Length; j++) //loop through each vertex of the current mesh
            {
                Vector3 v = meshes[i].vertices[j];
                Vector3 n = meshes[i].normals[j];
                Vector3 u = meshes[i].uv[j];
                VertexData p = new VertexData(v, n, u);
                if (!pointsHash.Contains(p)) //check if hash set contains data already and if not add it
                {
                    pointsOrder.Add(p, pIndex);
                    pointsHash.Add(p);

                    pIndex++;
                }
            }

            for (int t = 0; t < meshes[i].triangles.Length; t++) //keeep track of which vertices the triangles are pointing towards
            {
                int triPoint = meshes[i].triangles[t];
                Vector3 v = meshes[i].vertices[triPoint];
                Vector3 n = meshes[i].normals[triPoint];
                Vector3 u = meshes[i].uv[triPoint];
                VertexData p = new VertexData(v, n, u);

                int index;
                pointsOrder.TryGetValue(p, out index);
                tris.Add(index);

            }
            meshes[i] = null;
        }
        ExtractArrays(pointsOrder, mesh);
        mesh.triangles = tris.ToArray();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static void ExtractArrays(Dictionary<VertexData, int> list, Mesh mesh) //extract arrays from dictionary and assign to mesh
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        foreach (VertexData v in list.Keys)
        {
            verts.Add(v.Item1);
            norms.Add(v.Item2);
            uvs.Add(v.Item3);
        }
        mesh.vertices = verts.ToArray();
        mesh.normals = norms.ToArray();
        mesh.uv = uvs.ToArray();
    }
}
