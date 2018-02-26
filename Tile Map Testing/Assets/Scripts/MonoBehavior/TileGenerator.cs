using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour {

	public static TileGenerator instance;

	#region VAR: WORLD DIMENSIONS

	public static int mapWidth = 50;
	public static int mapHeight = 50;
	public static int mapLevels = 60;

	public readonly static int startingLevel = 52;

	#endregion

	#region VAR: NOISE VARIABLES

	Noise elevation;
	public bool randomSeed;
	public string seed;
	public float frequency;
	public float lacunarity;
	public float amplitude;
	public float persistance;
	public int octaves;

	#endregion

	#region VAR: MESH VARIABLES

	int worldChunkMax = 10;
	int worldMeshCountX;
	int worldMeshCountY;

	enum Layer {Floor, Wall, Deployables, Overlay};

	public static int numberOfMeshLayers = 4;
	public static int floorMeshArrayRef = 0;
	public static int wallMeshArrayRef = 1;
	public static int deployablesMeshArrayRef = 2;
	public static int overlayMeshArrayRef = 3;

	int floorMeshZ = 3;
	int wallMeshZ = 2;
	int deployablesMeshZ = 1;
	int overlayMeshZ = 0;

	#endregion

	#region VAR: TILE/LEVEL/MESH ARRAYS

	public static Tile[,,] tiles = new Tile[mapLevels, mapWidth, mapHeight];
	public static GameObject[] levelArray = new GameObject[mapLevels];
	public static GameObject[,,,] meshArray;

	#endregion

	#region MONOBEHAVIOR

	void Awake (){
		instance = this;
		worldMeshCountX = Mathf.CeilToInt (mapWidth / (float)worldChunkMax);
		worldMeshCountY = Mathf.CeilToInt (mapHeight / (float)worldChunkMax);
		meshArray = new GameObject[numberOfMeshLayers, mapLevels, worldMeshCountX, worldMeshCountY];
		GenerateElevationNoise ();
		GenerateTiles ();
	}

	void Start () {
		GenerateLevels ();
	}

	void Update(){
	}

	#endregion

	#region HELPERS

	public static bool InWorldBounds(int l, int x, int y){
		if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight && l >= 0 && l < mapLevels)
			return true;
		else
			return false;
	}

	public static Tile GetTileAt(int level, int x, int y){
		if (InWorldBounds (level, x, y)) {
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

	public static List<GameObject> GetMeshNeighbours (GameObject m, bool diagonal = false){
		List<GameObject> neighbours = new List<GameObject> ();
		DataTracker mData = m.GetComponent<DataTracker> ();
		int arrayRef = 0;

		if (mData.layer == DataTracker.Layer.Floor)
			arrayRef = floorMeshArrayRef;
		else if (mData.layer == DataTracker.Layer.Wall)
			arrayRef = wallMeshArrayRef;
		else if (mData.layer == DataTracker.Layer.Deployables)
			arrayRef = deployablesMeshArrayRef;
		else if (mData.layer == DataTracker.Layer.Overlay)
			arrayRef = overlayMeshArrayRef;

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

	public void ChangeLevel(int currentLevel, int newLevel){
		levelArray [currentLevel].SetActive (false);
		levelArray [newLevel].SetActive (true);
	}

	#endregion

	#region WORLD-GEN

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

	void GenerateElevationNoise(){
		if (randomSeed) {
			int value = Random.Range (-1000, 1000);
			seed = value.ToString ();
		}
		elevation = new Noise (seed.GetHashCode (), frequency, lacunarity, amplitude, persistance, octaves);
	}

	void GenerateLevels(){
		for (int level = 0; level < mapLevels; level++) {
			levelArray [level] = new GameObject ("Level: " + (level + 1));
			levelArray [level].transform.position = Vector3.zero;
			levelArray [level].transform.parent = this.transform;
			DivideTileArrayToChunks (0, 0, level, levelArray [level], Layer.Floor);
			DivideTileArrayToChunks (0, 0, level, levelArray [level], Layer.Wall);
			DivideTileArrayToChunks (0, 0, level, levelArray [level], Layer.Deployables);
			DivideTileArrayToChunks (0, 0, level, levelArray [level], Layer.Overlay);
			if (level != startingLevel - 1) {
				levelArray [level].SetActive (false);
			}
		}
	}

	void DivideTileArrayToChunks(int dex1, int dex2, int currentLevel, GameObject levelGO, Layer layer){
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

		GenerateMesh (chunk, dex1, dex2, currentLevel, levelGO, layer);

		if (tiles.GetLength (1) > dex1 + worldChunkMax) {
			DivideTileArrayToChunks (dex1 + worldChunkMax, dex2, currentLevel, levelGO, layer);
			return;
		}
		if (tiles.GetLength (2) > dex2 + worldChunkMax) {
			DivideTileArrayToChunks (0, dex2 + worldChunkMax, currentLevel, levelGO, layer);
			return;
		}
	}
		
	void GenerateMesh(Tile[,,] tilesChunk, int x, int y, int currentLevel, GameObject levelGO, Layer layer){
		MeshData data = new MeshData (tiles, tilesChunk, currentLevel, x, y, layer.ToString ());
		GameObject meshGO = new GameObject ("MESH: " + layer.ToString() + " | " + (currentLevel + 1) + " | " + x + ":" + y);
		DataTracker tracker = meshGO.AddComponent<DataTracker> ();
		tracker.level = currentLevel;
		tracker.x = x / worldChunkMax;
		tracker.y = y / worldChunkMax;

		int arrayRef = 0;
		int meshZ = 0;
		Material meshMat = SpriteLoader.worldMaterial;
		if (layer == Layer.Floor) {
			tracker.layer = DataTracker.Layer.Floor;
			arrayRef = floorMeshArrayRef;
			meshZ = floorMeshZ;
			meshMat = SpriteLoader.worldMaterial;
		} else if (layer == Layer.Wall) {
			tracker.layer = DataTracker.Layer.Wall;
			arrayRef = wallMeshArrayRef;
			meshZ = wallMeshZ;
			meshMat = SpriteLoader.worldMaterial;
		} else if (layer == Layer.Deployables) {
			tracker.layer = DataTracker.Layer.Deployables;
			arrayRef = deployablesMeshArrayRef;
			meshZ = deployablesMeshZ;
			meshMat = SpriteLoader.worldMaterial;
		} else if (layer == Layer.Overlay) {
			tracker.layer = DataTracker.Layer.Overlay;
			arrayRef = overlayMeshArrayRef;
			meshZ = overlayMeshZ;
			meshMat = SpriteLoader.worldMaterial;
		}
			
		meshArray[arrayRef, currentLevel, x / worldChunkMax, y / worldChunkMax] = meshGO;

		for (int i = 0 + x; i < tilesChunk.GetLength (1) + x; i++) {
			for (int o = 0 + y; o < tilesChunk.GetLength (2) + y; o++) {
				tiles [currentLevel, i, o].MESH [arrayRef] = meshGO;
			}
		}

		MeshFilter filter = meshGO.AddComponent<MeshFilter> ();
		MeshRenderer render = meshGO.AddComponent<MeshRenderer> ();
		meshGO.transform.SetParent (levelGO.transform);
		meshGO.transform.position = new Vector3 (x, y, meshZ);
		Mesh mesh = filter.mesh;
		render.material = meshMat;

		mesh.vertices = data.vertices.ToArray ();
		mesh.triangles = data.triangles.ToArray ();
		mesh.uv = data.uvs.ToArray ();
	}

	#endregion
}
