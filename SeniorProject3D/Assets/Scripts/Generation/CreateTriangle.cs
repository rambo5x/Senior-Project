using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTriangle
{
    public Mesh mesh;
    MeshFilter theMF;
    MeshRenderer theMR;
    Vector3[] vertices, normals;
    Vector2[] uvs;
    int[] tris;

    // Create triangle based on vertices
    public CreateTriangle(Vector3 p0, Vector3 p1, Vector3 p2, MeshController.BlockType bType){
        mesh = new Mesh(); //creating the mesh
        mesh.name = "GeneratedTriangle";

        vertices = new Vector3[3];
        normals = new Vector3[3];
        uvs = new Vector2[3];
        tris = new int[3];
        Vector3 normal = Vector3.Cross(p1-p0, p2-p0);

        vertices = new Vector3[] { p0, p1, p2 };
        normals = new Vector3[] { normal, normal, normal };
        uvs = new Vector2[] { MeshController.blockUVs[(int)bType, 0], MeshController.blockUVs[(int)bType, 1], MeshController.blockUVs[(int)bType, 2] };
        tris = new int[] { 0, 1, 2 };

        mesh.vertices = vertices; //assign our mesh the 4 required pieces of data for creating a quad
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = tris;

        mesh.RecalculateBounds(); //recalculating collision values
    }
}
