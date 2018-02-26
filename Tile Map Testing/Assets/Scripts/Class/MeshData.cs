using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData {

	public List<Vector3> vertices;
	public List<int> triangles;
	public List<Vector2> uvs;

	public MeshData(Tile[,,] tiles, Tile[,,] chunkTiles, int currentLevel, int xindex, int yindex, string key){
		vertices = new List<Vector3>();
		triangles = new List<int>();
		uvs = new List<Vector2> ();

		if (key == "Floor") {
			for (int x = 0; x < chunkTiles.GetLength (1); x++) {
				for (int y = 0; y < chunkTiles.GetLength (2); y++) {
					GenerateWorldSquare (tiles [currentLevel, x + xindex, y + yindex], x, y);
				}
			}
		} else if (key == "Wall") {
			for (int x = 0; x < chunkTiles.GetLength (1); x++) {
				for (int y = 0; y < chunkTiles.GetLength (2); y++) {
					GenerateWallSquare (tiles [currentLevel, x + xindex, y + yindex], x, y);
				}
			}
		} else if (key == "Deployables") {
			for (int x = 0; x < chunkTiles.GetLength (1); x++) {
				for (int y = 0; y < chunkTiles.GetLength (2); y++) {
					GenerateDeployablesSquare (tiles [currentLevel, x + xindex, y + yindex], x, y);
				}
			}
		} else if (key == "Overlay") {
			for (int x = 0; x < chunkTiles.GetLength (1); x++) {
				for (int y = 0; y < chunkTiles.GetLength (2); y++) {
					GenerateOverlaySquare (tiles [currentLevel, x + xindex, y + yindex], x, y);
				}
			}
		} else {
			Debug.LogError ("Tried to generate squares, unknown what type of mesh to generate for.");
		}
	}

	public static Mesh GetMeshAtTile(Tile t){
		Mesh m = t.MESH [TileGenerator.floorMeshArrayRef].GetComponent<MeshFilter> ().mesh;
		return m;
	}

	void GenerateWorldSquare (Tile tile, int x, int y){
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

		uvs.AddRange (SpriteLoader.instance.GetWorldUVS (tile));
	}
		
	void GenerateWallSquare (Tile tile, int x, int y){
		Tile[] neighbours = TileGenerator.GetTileNeighbours (tile, true);

		GenerateQuads (neighbours, tile, x, y, 1);
		GenerateQuads (neighbours, tile, x + 0.5f, y, 2);
		GenerateQuads (neighbours, tile, x, y + 0.5f, 3);
		GenerateQuads (neighbours, tile, x + 0.5f, y + 0.5f, 4);
	}

	void GenerateQuads (Tile[] neighbours, Tile tile, float x, float y, int quadrant){
		vertices.Add (new Vector3 (x + 0, y + 0));
		vertices.Add (new Vector3 (x + 0.5f, y + 0));
		vertices.Add (new Vector3 (x + 0, y + 0.5f));
		vertices.Add (new Vector3 (x + 0.5f, y + 0.5f));

		triangles.Add (vertices.Count - 1);
		triangles.Add (vertices.Count - 3);
		triangles.Add (vertices.Count - 4);

		triangles.Add (vertices.Count - 2);
		triangles.Add (vertices.Count - 1);
		triangles.Add (vertices.Count - 4);

		uvs.AddRange (SpriteLoader.instance.GetWallUVS (neighbours, tile.WALL, quadrant));
	}

	void GenerateOverlaySquare (Tile tile, int x, int y){
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

		uvs.AddRange (SpriteLoader.instance.GetOverlayUVS (tile));
	}

	void GenerateDeployablesSquare (Tile tile, int x, int y){
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

		uvs.AddRange (SpriteLoader.instance.GetDeployablesUVS (tile));
	}
}