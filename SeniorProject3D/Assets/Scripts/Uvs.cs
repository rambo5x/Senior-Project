using UnityEngine;
using System.Collections;

[RequireComponent(typeof (MeshFilter))]
[RequireComponent(typeof (MeshRenderer))]
public class Uvs : MonoBehaviour {

	// The Base Tile will be the Bottom side upper left vertex of the Cube.
    public float BasetileX = 1;
    public float BasetileY = 1;


	void Start () {

		float pixelSize = 16;
		float tilePerc = 1/pixelSize;

		float size = 1;
		Vector3[] vertices = {
			new Vector3(0, size, 0),
			new Vector3(0, 0, 0),
			new Vector3(size, size, 0),
			new Vector3(size, 0, 0),

			new Vector3(0, 0, size),
			new Vector3(size, 0, size),
			new Vector3(0, size, size),
			new Vector3(size, size, size),

			new Vector3(0, size, 0),
			new Vector3(size, size, 0),

			new Vector3(0, size, 0),
			new Vector3(0, size, size),

			new Vector3(size, size, 0),
			new Vector3(size, size, size),
		};

		int[] triangles = {
			0, 2, 1, // front
			1, 2, 3,
			4, 5, 6, // back
			5, 7, 6,
			6, 7, 8, //top
			7, 9 ,8, 
			1, 3, 4, //bottom
			3, 5, 4,
			1, 11,10,// left
			1, 4, 11,
			3, 12, 5,//right
			5, 12, 13


		};

           // (u,v) -> (x,y)

		Vector2[] uvs = {
			
			// Bot *
			// Top **
			
			//side 1
			new Vector2(tilePerc * (BasetileX - 1) , tilePerc * BasetileY),  // top left
			new Vector2(tilePerc * BasetileX, tilePerc * BasetileY), // top right * // <-- input Vertex
			new Vector2(tilePerc * (BasetileX - 1) , tilePerc * (BasetileY - 1)), // Bot left
			new Vector2(tilePerc * BasetileX, tilePerc * (BasetileY - 1)), // Bot Right *

			//side 2
			new Vector2(tilePerc * (BasetileX + 1), tilePerc * BasetileY), // top left *
			new Vector2(tilePerc * (BasetileX + 1),  tilePerc * (BasetileY - 1)), // Bot left *
			new Vector2(tilePerc * (BasetileX + 2), tilePerc * BasetileY), // top right **
			new Vector2(tilePerc * (BasetileX + 2), tilePerc * (BasetileY - 1)), // Bot Right **

			//Top
			new Vector2(tilePerc * (BasetileX + 3), tilePerc * BasetileY),  // top right **
			new Vector2(tilePerc * (BasetileX + 3), tilePerc * (BasetileY - 1)),  // Bot Right **

			//side 3
			new Vector2(tilePerc * BasetileX, tilePerc * (BasetileY  + 1)),  // top left
			new Vector2(tilePerc * (BasetileX + 1), tilePerc * (BasetileY  + 1)),  // top right

			//side 4
			new Vector2(tilePerc * BasetileX, tilePerc * (BasetileY - 2)), // Bot left
			new Vector2(tilePerc * (BasetileX + 1), tilePerc * (BasetileY - 2)), // Bot Right
		};	

		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		mesh.Clear ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.Optimize ();
		mesh.RecalculateNormals ();
	}
}