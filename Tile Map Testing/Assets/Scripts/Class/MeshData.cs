using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData {

	public List<Vector3> vertices;
	public List<int> triangles;
	public List<Vector2> uvs;

	public MeshData(Tile[,,] tiles, Tile[,,] chunkTiles, int currentLevel, int xindex, int yindex){
		vertices = new List<Vector3>();
		triangles = new List<int>();
		uvs = new List<Vector2> ();

		for (int x = 0; x < chunkTiles.GetLength(1); x++) {
			for (int y = 0; y < chunkTiles.GetLength(2); y++) {
				GenerateSquare (tiles [currentLevel, x + xindex, y + yindex], x, y);
			}
		}
	}

	public static Mesh GetMeshAtTile(Tile t){
		Mesh m = t.MESH.GetComponent<MeshFilter> ().mesh;
		return m;
	}

	void GenerateSquare (Tile tile, int x, int y){
		vertices.Add (new Vector3 (x + 0, y + 0));
		vertices.Add (new Vector3 (x + 1, y + 0));
		vertices.Add (new Vector3 (x + 0, y + 1));
		vertices.Add (new Vector3 (x + 1, y + 1));

		triangles.Add (vertices.Count - 1);
		triangles.Add (vertices.Count - 3);
		triangles.Add (vertices.Count - 4);

		triangles.Add (vertices.Count - 2);
		triangles.Add (vertices.Count - 1);
		triangles.Add (vertices.Count - 4);

		uvs.AddRange (SpriteLoader.instance.GetUVS (tile));
	}

}