using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour {

	public static TileGenerator instance;
			
	public static int mapWidth = 50;
	public static int mapHeight = 50;
	public static int mapLevels = 60;

	public bool randomSeed;
	public string seed;
	public float frequency;
	public float lacunarity;
	public float amplitude;
	public float persistance;
	public int octaves;

	int worldChunkMax = 25;
	int worldMeshCountX;
	int worldMeshCountY;

	public static int floorMeshArrayRef = 0;
	public static int wallMeshArrayRef = 1;
	public static int overlayMeshArrayRef = 2;

	int worldMeshZ = 3;
	int wallMeshZ = 2;
	int overlayMeshZ=1;

	public readonly static int startingLevel = 52;

	public static Tile[,,] tiles = new Tile[mapLevels, mapWidth, mapHeight];
	public static GameObject[] levelArray = new GameObject[mapLevels];
	public static GameObject[,,,] meshArray;

	Noise elevation;

	void Awake (){
		instance = this;
		worldMeshCountX = Mathf.CeilToInt (mapWidth / (float)worldChunkMax);
		worldMeshCountY = Mathf.CeilToInt (mapHeight / (float)worldChunkMax);
		meshArray = new GameObject[3, mapLevels, worldMeshCountX, worldMeshCountY];
		GenerateElevationNoise ();
		GenerateTiles ();
	}

	void Start () {
		GenerateLevels ();
	}

	void Update(){
	}

	public static bool InWorldBounds(int x, int y){
		if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
			return true;
		else
			return false;
	}

	public static Tile GetTileAt(int level, int x, int y){
		if (InWorldBounds (x, y)) {
			return tiles [level, x, y];
		} else {
			return null;
		}
	}

	public static GameObject GetMeshAt(int arrayRef, int level, int x, int y){
		if (x >= 0 && x < meshArray.GetLength (2) && y >= 0 && y < meshArray.GetLength (3)) {
			return meshArray [arrayRef, level, x, y];
		} else {
			return null;
		}
	}
						
	void GenerateLevels(){
		for (int level = 0; level < mapLevels; level++) {
			levelArray [level] = new GameObject ("Level: " + (level + 1));
			//meshList.Add (new List<List<List<GameObject>>> ());
			levelArray [level].transform.position = Vector3.zero;
			levelArray [level].transform.parent = this.transform;
			DivideWorldArray (0, 0, level, levelArray [level]);
			DivideWallArray (0, 0, level, levelArray [level]);
			DivideOverlayArray (0, 0, level, levelArray [level]);
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

	public static List<GameObject> GetMeshNeighbours (GameObject m, bool diagonal = false){
		List<GameObject> neighbours = new List<GameObject> ();
		DataTracker mData = m.GetComponent<DataTracker> ();
		int arrayRef = 0;

		if (mData.layer == DataTracker.Layer.Floor)
			arrayRef = 0;
		else if (mData.layer == DataTracker.Layer.Wall)
			arrayRef = 1;
		else if (mData.layer == DataTracker.Layer.Overlay)
			arrayRef = 2;

		if (GetMeshAt (arrayRef, mData.level, mData.x, mData.y + 1) != null)
			neighbours.Add (GetMeshAt (arrayRef, mData.level, mData.x, mData.y + 1));
		if (GetMeshAt (arrayRef, mData.level, mData.x + 1, mData.y) != null)
			neighbours.Add(GetMeshAt (arrayRef, mData.level, mData.x + 1, mData.y));
		if (GetMeshAt (arrayRef, mData.level, mData.x, mData.y - 1) != null)
			neighbours.Add(GetMeshAt (arrayRef, mData.level, mData.x, mData.y - 1));
		if (GetMeshAt (arrayRef, mData.level, mData.x - 1, mData.y) != null)
			neighbours.Add(GetMeshAt (arrayRef, mData.level, mData.x - 1, mData.y));		

		if (diagonal) {
			if (GetMeshAt (arrayRef, mData.level, mData.x + 1, mData.y + 1) != null)
				neighbours.Add(GetMeshAt (arrayRef, mData.level, mData.x + 1, mData.y + 1));
			if (GetMeshAt (arrayRef, mData.level, mData.x + 1, mData.y - 1) != null)
				neighbours.Add(GetMeshAt (arrayRef, mData.level, mData.x + 1, mData.y - 1));
			if (GetMeshAt (arrayRef, mData.level, mData.x - 1, mData.y - 1) != null)
				neighbours.Add(GetMeshAt (arrayRef, mData.level, mData.x - 1, mData.y - 1));
			if (GetMeshAt (arrayRef, mData.level, mData.x - 1, mData.y + 1) != null)
				neighbours.Add(GetMeshAt (arrayRef, mData.level, mData.x - 1, mData.y + 1));			
		}
		return neighbours;
	}

	public static Tile[] GetTileNeighbours (Tile t, bool diagonal = false){
		Tile[] neighbours = new Tile[diagonal ? 8 : 4];

		neighbours [0] = GetTileAt(t.LEVEL, t.X, t.Y + 1);
		neighbours [1] = GetTileAt(t.LEVEL, t.X + 1, t.Y);
		neighbours [2] = GetTileAt(t.LEVEL, t.X, t.Y - 1);
		neighbours [3] = GetTileAt(t.LEVEL, t.X - 1, t.Y);

		if (diagonal) {
			neighbours [4] = GetTileAt(t.LEVEL, t.X + 1, t.Y + 1);
			neighbours [5] = GetTileAt(t.LEVEL, t.X + 1, t.Y - 1);
			neighbours [6] = GetTileAt(t.LEVEL, t.X - 1, t.Y - 1);
			neighbours [7] = GetTileAt(t.LEVEL, t.X - 1, t.Y + 1);
		}

		return neighbours;
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
		DataTracker tracker = meshGO.AddComponent<DataTracker> ();
		tracker.level = currentLevel;
		tracker.x = x / worldChunkMax;
		tracker.y = y / worldChunkMax;
		tracker.layer = DataTracker.Layer.Floor;
		meshArray[floorMeshArrayRef, currentLevel, x / worldChunkMax, y / worldChunkMax] = meshGO;

		for (int i = 0 + x; i < tilesChunk.GetLength (1) + x; i++) {
			for (int o = 0 + y; o < tilesChunk.GetLength (2) + y; o++) {
				tiles [currentLevel, i, o].MESH [floorMeshArrayRef] = meshGO;
			}
		}

		MeshFilter filter = meshGO.AddComponent<MeshFilter> ();
		MeshRenderer render = meshGO.AddComponent<MeshRenderer> ();
		//MeshCollider collider = meshGO.AddComponent<MeshCollider> ();
		meshGO.transform.SetParent (levelGO.transform);
		meshGO.transform.position = new Vector3 (x, y, worldMeshZ);
		Mesh mesh = filter.mesh;
		//collider.convex = true;
		//collider.sharedMesh = mesh;
		render.material = SpriteLoader.worldMaterial;

		mesh.vertices = data.vertices.ToArray ();
		mesh.triangles = data.triangles.ToArray ();
		mesh.uv = data.uvs.ToArray ();
	}

	void DivideWallArray(int dex1, int dex2, int currentLevel, GameObject levelGO){
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

		GenerateWallMesh (chunk, dex1, dex2, currentLevel, levelGO);

		if (tiles.GetLength (1) > dex1 + worldChunkMax) {
			DivideWallArray (dex1 + worldChunkMax, dex2, currentLevel, levelGO);
			return;
		}
		if (tiles.GetLength (2) > dex2 + worldChunkMax) {
			DivideWallArray (0, dex2 + worldChunkMax, currentLevel, levelGO);
			return;
		}
	}

	void GenerateWallMesh(Tile[,,] tilesChunk, int x, int y, int currentLevel, GameObject levelGO){
		MeshData data = new MeshData (tiles, tilesChunk, currentLevel, x, y, "Wall");
		GameObject meshGO = new GameObject ("MESH WALL " + (currentLevel + 1) + " " + x + " " + y);
		DataTracker tracker = meshGO.AddComponent<DataTracker> ();
		tracker.level = currentLevel;
		tracker.x = x / worldChunkMax;
		tracker.y = y / worldChunkMax;
		tracker.layer = DataTracker.Layer.Wall;
		meshArray[wallMeshArrayRef, currentLevel, x / worldChunkMax, y / worldChunkMax] = meshGO;

		for (int i = 0 + x; i < tilesChunk.GetLength (1) + x; i++) {
			for (int o = 0 + y; o < tilesChunk.GetLength (2) + y; o++) {
				tiles [currentLevel, i, o].MESH [wallMeshArrayRef] = meshGO;
			}
		}

		MeshFilter filter = meshGO.AddComponent<MeshFilter> ();
		MeshRenderer render = meshGO.AddComponent<MeshRenderer> ();
		//MeshCollider collider = meshGO.AddComponent<MeshCollider> ();
		meshGO.transform.SetParent (levelGO.transform);
		meshGO.transform.position = new Vector3 (x, y, wallMeshZ);
		Mesh mesh = filter.mesh;
		//collider.convex = true;
		//collider.sharedMesh = mesh;
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
		DataTracker tracker = meshGO.AddComponent<DataTracker> ();
		tracker.level = currentLevel;
		tracker.x = x / worldChunkMax;
		tracker.y = y / worldChunkMax;
		tracker.layer = DataTracker.Layer.Overlay;
		meshArray[overlayMeshArrayRef, currentLevel, x / worldChunkMax, y / worldChunkMax] = meshGO;

		for (int i = 0 + x; i < tilesChunk.GetLength (1) + x; i++) {
			for (int o = 0 + y; o < tilesChunk.GetLength (2) + y; o++) {
				tiles [currentLevel, i, o].MESH [overlayMeshArrayRef] = meshGO;
			}
		}

		MeshFilter filter = meshGO.AddComponent<MeshFilter> ();
		MeshRenderer render = meshGO.AddComponent<MeshRenderer> ();
		//MeshCollider collider = meshGO.AddComponent<MeshCollider> ();
		meshGO.transform.SetParent (levelGO.transform);
		meshGO.transform.position = new Vector3 (x, y, overlayMeshZ);
		Mesh mesh = filter.mesh;
		//collider.convex = true;
		//collider.sharedMesh = mesh;
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
}
