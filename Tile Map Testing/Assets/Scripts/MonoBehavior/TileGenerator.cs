using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour {

	public static TileGenerator instance;
			
	public static int mapWidth = 125;
	public static int mapHeight = 125;
	public static int mapLevels = 60;

	public bool randomSeed;
	public string seed;
	public float frequency;
	public float lacunarity;
	public float amplitude;
	public float persistance;
	public int octaves;

	public readonly static int startingLevel = 52;

	public static Tile[,,] tiles = new Tile[mapLevels, mapWidth, mapHeight];
	public static GameObject[] levelArray = new GameObject[mapLevels];
	static List<Vector2> uvs;

	Noise elevation;

	void Awake (){
		instance = this;
		uvs = new List<Vector2> ();
		GenerateElevationNoise ();
		GenerateTiles ();
	}

	void Start () {
		GenerateLevels ();
	}

	void GenerateLevels(){
		for (int level = 0; level < mapLevels; level++) {
			GameObject levelGO = new GameObject ("Level: " + (level + 1));
			levelGO.transform.position = new Vector3 (0, 0, 0);
			levelArray [level] = levelGO;
			DivideTilesArray (0, 0, level, levelGO);
			if (level != startingLevel - 1) {
				levelGO.SetActive (false);
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

	void DivideTilesArray(int dex1, int dex2, int currentLevel, GameObject levelGO){
		Tile[,,] chunk;
		int sizeX;
		int sizeY;

		if (tiles.GetLength (1) - dex1 > 125) {
			sizeX = 125;
		} else {
			sizeX = tiles.GetLength (1) - dex1;
		}

		if (tiles.GetLength (2) - dex2 > 125) {
			sizeY = 125;
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

		if (tiles.GetLength (1) > dex1 + 125) {
			DivideTilesArray (dex1 + 125, dex2, currentLevel, levelGO);
			return;
		}
		if (tiles.GetLength (2) > dex2 + 125) {
			DivideTilesArray (0, dex2 + 125, currentLevel, levelGO);
			return;
		}
	}

	void GenerateWorldMesh(Tile[,,] tilesChunk, int x, int y, int currentLevel, GameObject levelGO){
		MeshData data = new MeshData (tiles, tilesChunk, currentLevel, x, y);
		GameObject meshGO = new GameObject ("MESH " + (currentLevel + 1) + " " + x + " " + y);

		for (int i = 0 + x; i < tilesChunk.GetLength (1) + x; i++) {
			for (int o = 0 + y; o < tilesChunk.GetLength (2) + y; o++) {
				tiles [currentLevel, i, o].MESH = meshGO;
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
		Mesh mesh = MeshData.GetMeshAtTile (t);

		int posChunkX = Mathf.FloorToInt (tiles [t.LEVEL, t.X, t.Y].MESH.transform.position.x);
		int posChunkY = Mathf.FloorToInt (tiles [t.LEVEL, t.X, t.Y].MESH.transform.position.y);
		int chunkX = 0;
		int chunkY = 0;

		for (int i = 0; i < tiles.GetLength(1); i++) {
			if (mesh == tiles [t.LEVEL, i, t.Y].MESH.GetComponent<MeshFilter> ().mesh) {
				chunkX++;				
			}					
		}
		for (int i = 0; i < tiles.GetLength(2); i++) {
			if (mesh == tiles [t.LEVEL, t.X, i].MESH.GetComponent<MeshFilter> ().mesh) {
				chunkY++;
			}
		}

		for (int i = 0; i < chunkX; i++) {
			for (int o = 0; o < chunkY; o++) {
				uvs.AddRange (SpriteLoader.instance.GetUVS (TileGenerator.tiles [t.LEVEL, i + posChunkX, o + posChunkY]));
			}
		}

		mesh.uv = uvs.ToArray ();
		uvs.Clear ();
	}
}
