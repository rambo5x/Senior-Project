using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateQuad
{
    public Mesh mesh;
    MeshFilter theMF;
    MeshRenderer theMR;
    Vector3[] vertices, normals;
    Vector2[] uvs;
    int[] tris;

    public CreateQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, MeshController.BlockType bType){
        mesh = new Mesh(); //creating the mesh
        mesh.name = "GeneratedQuad";

        vertices = new Vector3[4];
        normals = new Vector3[4];
        uvs = new Vector2[4];
        tris = new int[6];
        Vector3 normal = -Vector3.Cross(p1-p0, p2-p0);

        vertices = new Vector3[] { p0, p1, p2, p3 };
        normals = new Vector3[] { normal, normal, normal, normal };
        uvs = new Vector2[] { MeshController.blockUVs[(int)bType, 0], MeshController.blockUVs[(int)bType, 1], MeshController.blockUVs[(int)bType, 2], MeshController.blockUVs[(int)bType, 3] };
        tris = new int[] { 3, 1, 0, 3, 2, 1 };

        mesh.vertices = vertices; //assign our mesh the 4 required pieces of data for creating a quad
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = tris;

        mesh.RecalculateBounds(); //recalculating collision values
    }
    public CreateQuad(MeshController.BlockSide side, Vector3 offset, MeshController.BlockType bType)
    {
        mesh = new Mesh(); //creating the mesh
        mesh.name = "GeneratedQuad";

        vertices = new Vector3[4]; //4 each for the corners of a quad for the following three vectors
        normals = new Vector3[4];
        uvs = new Vector2[4];
        tris = new int[6]; //6 since each tri has 3 vertices and there 2 tris per quad

        Vector2 uv00 = MeshController.blockUVs[(int)bType, 0];
        Vector2 uv10 = MeshController.blockUVs[(int)bType, 1];
        Vector2 uv01 = MeshController.blockUVs[(int)bType, 2];
        Vector2 uv11 = MeshController.blockUVs[(int)bType, 3];

    // Points on a cube
    // p6      +-------+ p7
    //       /|       /|
    //      / |      / |
    // p5  +--------+  | p4
    //     |  |     |  |       Left -x
    // p2  |  +-----|--+ p3
    //     | /      | /
    //     |/       |/
    // p1  +--------+  p0
    //        Front +z

        Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f) + offset; //create vertices for each corner of a cube & add offset for moving the cube in a different position
        Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f) + offset;
        Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f) + offset;
        Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f) + offset;
        Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f) + offset;
        Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f) + offset;
        Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f) + offset;
        Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f) + offset;

        switch (side)
        {
            // block sides
            case MeshController.BlockSide.FRONT:
            
                vertices = new Vector3[] { p4, p5, p1, p0 }; //create a side of a cube to be a quad
                normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward }; //show normals in the forward direction
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; //for displaying a texture
                tris = new int[] { 3, 1, 0, 3, 2, 1 }; //create triangle data based on vertices array index.
                break; 
            case MeshController.BlockSide.BACK:
            
                vertices = new Vector3[] { p6, p7, p3, p2 }; //create a side of a cube to be a quad
                normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back }; //show normals in the back direction
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; //for displaying a texture
                tris = new int[] { 3, 1, 0, 3, 2, 1 }; //create triangle data based on vertices array index.
                break;
            case MeshController.BlockSide.LEFT:
            
                vertices = new Vector3[] { p7, p4, p0, p3 }; //create a side of a cube to be a quad
                normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left }; //show normals in the left direction
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; //for displaying a texture
                tris = new int[] { 3, 1, 0, 3, 2, 1 }; //create triangle data based on vertices array index.
                break; 
            case MeshController.BlockSide.RIGHT:
            
                vertices = new Vector3[] { p5, p6, p2, p1 }; //create a side of a cube to be a quad
                normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right }; //show normals in the right direction
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; //for displaying a texture
                tris = new int[] { 3, 1, 0, 3, 2, 1 }; //create triangle data based on vertices array index.
                break;   
            case MeshController.BlockSide.TOP:
            
                vertices = new Vector3[] { p7, p6, p5, p4 }; //create a side of a cube to be a quad
                normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up }; //show normals in the top direction
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; //for displaying a texture
                tris = new int[] { 3, 1, 0, 3, 2, 1 }; //create triangle data based on vertices array index.
                break;
            case MeshController.BlockSide.BOTTOM:
            
                vertices = new Vector3[] { p0, p1, p2, p3 }; //create a side of a cube to be a quad
                normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down }; //show normals in the bottom direction
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; //for displaying a texture
                tris = new int[] { 3, 1, 0, 3, 2, 1 }; //create triangle data based on vertices array index.
                break;
            
            // diagonal sides
            case MeshController.BlockSide.DIAG_FRONT:
            
                vertices = new Vector3[] { p0, p7, p6, p1 }; //create a side of a cube to be a quad
                normals = new Vector3[] { Vector3.forward + Vector3.up, Vector3.forward + Vector3.up, Vector3.forward + Vector3.up, Vector3.forward + Vector3.up }; //show normals in the front direction
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; //for displaying a texture
                tris = new int[] { 3, 1, 0, 3, 2, 1 }; //create triangle data based on vertices array index.
                break;
            case MeshController.BlockSide.DIAG_BACK:
            
                vertices = new Vector3[] { p3, p2, p5, p4 }; //create a side of a cube to be a quad
                normals = new Vector3[] { Vector3.back + Vector3.up, Vector3.back + Vector3.up, Vector3.back + Vector3.up, Vector3.back + Vector3.up }; //show normals in the bottom direction
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; //for displaying a texture
                tris = new int[] { 3, 1, 0, 3, 2, 1 }; //create triangle data based on vertices array index.
                break;
            case MeshController.BlockSide.DIAG_LEFT:
            
                vertices = new Vector3[] { p3, p6, p5, p0 }; //create a side of a cube to be a quad
                normals = new Vector3[] { Vector3.left + Vector3.up, Vector3.left + Vector3.up, Vector3.left + Vector3.up, Vector3.left + Vector3.up }; //show normals in the bottom direction
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; //for displaying a texture
                tris = new int[] { 3, 1, 0, 3, 2, 1 }; //create triangle data based on vertices array index.
                break;
            case MeshController.BlockSide.DIAG_RIGHT:
            
                vertices = new Vector3[] { p1, p4, p7, p2 }; //create a side of a cube to be a quad
                normals = new Vector3[] { Vector3.right + Vector3.up, Vector3.right + Vector3.up, Vector3.right + Vector3.up, Vector3.right + Vector3.up }; //show normals in the bottom direction
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; //for displaying a texture
                tris = new int[] { 3, 1, 0, 3, 2, 1 }; //create triangle data based on vertices array index.
                break;
        }

        mesh.vertices = vertices; //assign our mesh the 4 required pieces of data for creating a quad
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = tris;

        mesh.RecalculateBounds(); //recalculating collision values
    }
}


