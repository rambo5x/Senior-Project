using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBlock
{
    public Mesh blockMesh;
    private CreateChunk chunk;
    // thoughts

    /*
        - find all top blocks after first chunk data pass
        - determine which blocks are subject to have a smoothed block added on top
            - if the block above the top block does not have any neighbors, do not add a smoothing block
            - if the block above the top block DOES have neighbors, a smoothing block should be added
    */
    public CreateBlock(Vector3 position, CreateChunk.BlockData blockData, CreateChunk chunk){
        this.chunk = chunk;
        
        if (blockData.BlockType == MeshController.BlockType.AIR || blockData.Smoothed) return; // no need to check if quads should be generated

        List<CreateQuad> quads = new List<CreateQuad>(); //create quad array to pass all sides

        // generate regular block
        if (chunk.NeighCheck((int)position.x, (int)position.y - 1, (int)position.z) != MeshController.BlockState.OTHER) quads.Add(new CreateQuad(MeshController.BlockSide.BOTTOM, position, blockData.BlockType));
        if (chunk.NeighCheck((int)position.x, (int)position.y + 1, (int)position.z) != MeshController.BlockState.OTHER) quads.Add(new CreateQuad(MeshController.BlockSide.TOP, position, blockData.BlockType));
        if (chunk.NeighCheck((int)position.x - 1, (int)position.y, (int)position.z) != MeshController.BlockState.OTHER) quads.Add(new CreateQuad(MeshController.BlockSide.LEFT, position, blockData.BlockType));
        if (chunk.NeighCheck((int)position.x + 1, (int)position.y, (int)position.z) != MeshController.BlockState.OTHER) quads.Add(new CreateQuad(MeshController.BlockSide.RIGHT, position, blockData.BlockType));
        if (chunk.NeighCheck((int)position.x, (int)position.y, (int)position.z + 1) != MeshController.BlockState.OTHER) quads.Add(new CreateQuad(MeshController.BlockSide.FRONT, position, blockData.BlockType));
        if (chunk.NeighCheck((int)position.x, (int)position.y, (int)position.z - 1) != MeshController.BlockState.OTHER) quads.Add(new CreateQuad(MeshController.BlockSide.BACK, position, blockData.BlockType));

        if (quads.Count == 0) return; // no need to allocate memory for face generation

        Mesh[] sideMeshes = new Mesh[quads.Count]; //assign quads to meshes
        for (int i = 0; i < quads.Count; i++){
            sideMeshes[i] = quads[i].mesh;
        }

        blockMesh = MeshController.MergeMeshes(sideMeshes); //merge meshes
        blockMesh.name = $"Cube_{position.x}_{position.y}_{position.z}";
    }
}
