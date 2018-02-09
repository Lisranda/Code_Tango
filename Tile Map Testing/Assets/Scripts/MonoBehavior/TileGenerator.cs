using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour {

	public static TileGenerator instance;
		
	public static int mapWidth = 125;
	public static int mapHeight = 125;
	public static int mapLevels = 1;

	public readonly static int startingLevel = 1;

	public Material worldMaterial;

	public static Tile[,,] tiles = new Tile[mapLevels, mapWidth, mapHeight];
	public static GameObject[] levelArray = new GameObject[mapLevels];

	void Awake (){
		instance = this;
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
		for (int level = 0; level < mapLevels; level++) {
			for (int x = 0; x < mapWidth; x++) {
				for (int y = 0; y < mapHeight; y++) {
					Tile tile = new Tile (level, x, y);
					tiles [level, x, y] = tile;
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

		for (int i = dex1; i < sizeX; i++) {
			for (int o = dex2; o < sizeY; o++) {
				chunk [currentLevel, i, o] = tiles [currentLevel, i + dex1, o + dex2];
			}		
		}

		GenerateWorldMesh (chunk, dex1, dex2, currentLevel, levelGO);

		if (tiles.GetLength (1) >= dex1 + 125) {
			DivideTilesArray (dex1 + 125, dex2, currentLevel, levelGO);
			return;
		}
		if (tiles.GetLength (2) >= dex2 + 125) {
			DivideTilesArray (0, dex2 + 125, currentLevel, levelGO);
			return;
		}
	}

	void GenerateWorldMesh(Tile[,,] tilesChunk, int x, int y, int currentLevel, GameObject levelGO){
		MeshData data = new MeshData (tiles, tilesChunk, currentLevel);
		GameObject meshGO = new GameObject ("WORLD");
		MeshFilter filter = meshGO.AddComponent<MeshFilter> ();
		MeshRenderer render = meshGO.AddComponent<MeshRenderer> ();
		meshGO.transform.SetParent (levelGO.transform);
		meshGO.transform.position = new Vector3 (x, y);
		Mesh mesh = filter.mesh;
		render.material = worldMaterial;

		mesh.vertices = data.vertices.ToArray ();
		mesh.triangles = data.triangles.ToArray ();
		mesh.uv = data.uvs.ToArray ();
	}
}
