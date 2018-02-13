using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour {

	public static TileGenerator instance;
			
	public static int mapWidth = 200;
	public static int mapHeight = 200;
	public static int mapLevels = 60;

	public bool randomSeed;
	public string seed;
	public float frequency;
	public float lacunarity;
	public float amplitude;
	public float persistance;
	public int octaves;

	int worldChunkMax = 25;
	int wallChunkMax = 25;

	public static int floorMeshArrayRef = 0;
	public static int wallMeshArrayRef = 1;
	public static int overlayMeshArrayRef = 2;

	public readonly static int startingLevel = 52;

	public static Tile[,,] tiles = new Tile[mapLevels, mapWidth, mapHeight];
	public static GameObject[] levelArray = new GameObject[mapLevels];
	static List<Vector2> uvs;
	static List<List<GameObject>> meshList;

	Noise elevation;

	void Awake (){
		instance = this;
		uvs = new List<Vector2> ();
		meshList = new List<List<GameObject>> ();
		GenerateElevationNoise ();
		GenerateTiles ();
	}

	void Start () {
		GenerateLevels ();
	}
						
	void GenerateLevels(){
		for (int level = 0; level < mapLevels; level++) {
			levelArray [level] = new GameObject ("Level: " + (level + 1));
			meshList.Add (new List<GameObject> ());
			levelArray [level].transform.position = Vector3.zero;
			levelArray [level].transform.parent = this.transform;
			DivideWorldArray (0, 0, level, levelArray [level]);
			if (level != startingLevel - 1) {
				levelArray [level].SetActive (false);
			}
		}
	}

	void GenerateTiles(){
		float[,] elevationValues = elevation.GetPerlin (mapWidth, mapHeight);

		for (int level = 0; level < mapLevels; level++) {
			for (int x = 0; x < mapWidth; x++) {
				for (int y = 0; y < mapHeight; y++) {
					tiles [level, x, y] = new Tile (level, x, y, elevationValues [x, y]);
				}
			}
		}
	}
		
	public void ChangeLevel(int currentLevel, int newLevel){
		levelArray [currentLevel].SetActive (false);
		levelArray [newLevel].SetActive (true);
	}

	void DivideWorldArray(int dex1, int dex2, int currentLevel, GameObject levelGO){
		Tile[,,] chunk;
		int sizeX;
		int sizeY;

		if (tiles.GetLength (1) - dex1 > worldChunkMax) {
			sizeX = worldChunkMax;
		} else {
			sizeX = tiles.GetLength (1) - dex1;
		}

		if (tiles.GetLength (2) - dex2 > worldChunkMax) {
			sizeY = worldChunkMax;
		} else {
			sizeY = tiles.GetLength (2) - dex2;
		}

		chunk = new Tile[mapLevels, sizeX, sizeY];

		//for (int i = 0; i < sizeX; i++) {
		//	for (int o = 0; o < sizeY; o++) {
		//		chunk [currentLevel, i, o] = tiles [currentLevel, i + dex1, o + dex2];
		//	}		
		//}

		GenerateWorldMesh (chunk, dex1, dex2, currentLevel, levelGO);

		if (tiles.GetLength (1) > dex1 + worldChunkMax) {
			DivideWorldArray (dex1 + worldChunkMax, dex2, currentLevel, levelGO);
			return;
		}
		if (tiles.GetLength (2) > dex2 + worldChunkMax) {
			DivideWorldArray (0, dex2 + worldChunkMax, currentLevel, levelGO);
			return;
		}
	}

	void GenerateWorldMesh(Tile[,,] tilesChunk, int x, int y, int currentLevel, GameObject levelGO){
		MeshData data = new MeshData (tiles, tilesChunk, currentLevel, x, y, "Floor");
		GameObject meshGO = new GameObject ("MESH WORLD " + (currentLevel + 1) + " " + x + " " + y);
		meshList [currentLevel].Add (meshGO);

		for (int i = 0 + x; i < tilesChunk.GetLength (1) + x; i++) {
			for (int o = 0 + y; o < tilesChunk.GetLength (2) + y; o++) {
				tiles [currentLevel, i, o].MESH [floorMeshArrayRef] = meshGO;
			}
		}

		MeshFilter filter = meshGO.AddComponent<MeshFilter> ();
		MeshRenderer render = meshGO.AddComponent<MeshRenderer> ();
		MeshCollider collider = meshGO.AddComponent<MeshCollider> ();
		meshGO.transform.SetParent (levelGO.transform);
		meshGO.transform.position = new Vector3 (x, y);
		Mesh mesh = filter.mesh;
		collider.convex = true;
		collider.sharedMesh = mesh;
		render.material = SpriteLoader.worldMaterial;

		mesh.vertices = data.vertices.ToArray ();
		mesh.triangles = data.triangles.ToArray ();
		mesh.uv = data.uvs.ToArray ();
	}

	void DivideWallArray(int dex1, int dex2, int currentLevel, GameObject levelGO){
		Tile[,,] chunk;
		int sizeX;
		int sizeY;

		if (tiles.GetLength (1) - dex1 > wallChunkMax) {
			sizeX = wallChunkMax;
		} else {
			sizeX = tiles.GetLength (1) - dex1;
		}

		if (tiles.GetLength (2) - dex2 > wallChunkMax) {
			sizeY = wallChunkMax;
		} else {
			sizeY = tiles.GetLength (2) - dex2;
		}

		chunk = new Tile[mapLevels, sizeX, sizeY];

		//for (int i = 0; i < sizeX; i++) {
		//	for (int o = 0; o < sizeY; o++) {
		//		chunk [currentLevel, i, o] = tiles [currentLevel, i + dex1, o + dex2];
		//	}		
		//}

		GenerateWallMesh (chunk, dex1, dex2, currentLevel, levelGO);

		if (tiles.GetLength (1) > dex1 + wallChunkMax) {
			DivideWallArray (dex1 + wallChunkMax, dex2, currentLevel, levelGO);
			return;
		}
		if (tiles.GetLength (2) > dex2 + wallChunkMax) {
			DivideWallArray (0, dex2 + wallChunkMax, currentLevel, levelGO);
			return;
		}
	}

	void GenerateWallMesh(Tile[,,] tilesChunk, int x, int y, int currentLevel, GameObject levelGO){
		MeshData data = new MeshData (tiles, tilesChunk, currentLevel, x, y, "Wall");
		GameObject meshGO = new GameObject ("MESH WALL " + (currentLevel + 1) + " " + x + " " + y);
		meshList [currentLevel].Add (meshGO);

		for (int i = 0 + x; i < tilesChunk.GetLength (1) + x; i++) {
			for (int o = 0 + y; o < tilesChunk.GetLength (2) + y; o++) {
				tiles [currentLevel, i, o].MESH [wallMeshArrayRef] = meshGO;
			}
		}

		MeshFilter filter = meshGO.AddComponent<MeshFilter> ();
		MeshRenderer render = meshGO.AddComponent<MeshRenderer> ();
		MeshCollider collider = meshGO.AddComponent<MeshCollider> ();
		meshGO.transform.SetParent (levelGO.transform);
		meshGO.transform.position = new Vector3 (x, y);
		Mesh mesh = filter.mesh;
		collider.convex = true;
		collider.sharedMesh = mesh;
		render.material = SpriteLoader.worldMaterial;

		mesh.vertices = data.vertices.ToArray ();
		mesh.triangles = data.triangles.ToArray ();
		mesh.uv = data.uvs.ToArray ();
	}

	void DivideOverlayArray(int dex1, int dex2, int currentLevel, GameObject levelGO){
		Tile[,,] chunk;
		int sizeX;
		int sizeY;

		if (tiles.GetLength (1) - dex1 > worldChunkMax) {
			sizeX = worldChunkMax;
		} else {
			sizeX = tiles.GetLength (1) - dex1;
		}

		if (tiles.GetLength (2) - dex2 > worldChunkMax) {
			sizeY = worldChunkMax;
		} else {
			sizeY = tiles.GetLength (2) - dex2;
		}

		chunk = new Tile[mapLevels, sizeX, sizeY];

		//for (int i = 0; i < sizeX; i++) {
		//	for (int o = 0; o < sizeY; o++) {
		//		chunk [currentLevel, i, o] = tiles [currentLevel, i + dex1, o + dex2];
		//	}		
		//}

		GenerateOverlayMesh (chunk, dex1, dex2, currentLevel, levelGO);

		if (tiles.GetLength (1) > dex1 + worldChunkMax) {
			DivideOverlayArray (dex1 + worldChunkMax, dex2, currentLevel, levelGO);
			return;
		}
		if (tiles.GetLength (2) > dex2 + worldChunkMax) {
			DivideOverlayArray (0, dex2 + worldChunkMax, currentLevel, levelGO);
			return;
		}
	}

	void GenerateOverlayMesh(Tile[,,] tilesChunk, int x, int y, int currentLevel, GameObject levelGO){
		MeshData data = new MeshData (tiles, tilesChunk, currentLevel, x, y, "Overlay");
		GameObject meshGO = new GameObject ("MESH OVERLAY " + (currentLevel + 1) + " " + x + " " + y);
		meshList [currentLevel].Add (meshGO);

		for (int i = 0 + x; i < tilesChunk.GetLength (1) + x; i++) {
			for (int o = 0 + y; o < tilesChunk.GetLength (2) + y; o++) {
				tiles [currentLevel, i, o].MESH [overlayMeshArrayRef] = meshGO;
			}
		}

		MeshFilter filter = meshGO.AddComponent<MeshFilter> ();
		MeshRenderer render = meshGO.AddComponent<MeshRenderer> ();
		MeshCollider collider = meshGO.AddComponent<MeshCollider> ();
		meshGO.transform.SetParent (levelGO.transform);
		meshGO.transform.position = new Vector3 (x, y);
		Mesh mesh = filter.mesh;
		collider.convex = true;
		collider.sharedMesh = mesh;
		render.material = SpriteLoader.worldMaterial;

		mesh.vertices = data.vertices.ToArray ();
		mesh.triangles = data.triangles.ToArray ();
		mesh.uv = data.uvs.ToArray ();
	}

	void GenerateElevationNoise(){
		if (randomSeed) {
			int value = Random.Range (-1000, 1000);
			seed = value.ToString ();
		}

		elevation = new Noise (seed.GetHashCode (), frequency, lacunarity, amplitude, persistance, octaves);
	}
		
	public static void RefreshMeshAtTile(Tile t){
		GameObject go = t.MESH [0];
		Mesh mesh = go.GetComponent<MeshFilter> ().mesh;

		int posChunkX = Mathf.FloorToInt (go.transform.position.x);
		int posChunkY = Mathf.FloorToInt (go.transform.position.y);

		for (int i = 0; i < mesh.bounds.size.x; i++) {
			for (int o = 0; o < mesh.bounds.size.y; o++) {
				uvs.AddRange (SpriteLoader.instance.GetWorldUVS (tiles [t.LEVEL, i + posChunkX, o + posChunkY]));
			}
		}

		mesh.uv = uvs.ToArray ();
		uvs.Clear ();
	}

	public static void RefreshFloorMeshes(){
		int currentLevel = CameraController.currentLevel;

		for (int i = 0; i < meshList[currentLevel].Count; i++) {
			GameObject go = meshList [currentLevel] [i];
			Mesh mesh = go.GetComponent<MeshFilter> ().sharedMesh;

			int posChunkX = Mathf.FloorToInt(go.transform.position.x);
			int posChunkY = Mathf.FloorToInt(go.transform.position.y);

			for (int x = 0; x < mesh.bounds.size.x; x++) {
				for (int y = 0; y < mesh.bounds.size.y; y++) {
					uvs.AddRange (SpriteLoader.instance.GetWorldUVS (tiles [currentLevel, x + posChunkX, y + posChunkY]));
				}
			}

			mesh.uv = uvs.ToArray ();
			uvs.Clear ();
		}



	}
}
