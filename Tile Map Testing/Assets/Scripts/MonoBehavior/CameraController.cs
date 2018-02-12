using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public int cameraSpeed = 15;
	public int setShiftMultiplier = 2;
	public int maximumZoom;
	public int minimumZoom;

	int pixelsPerUnit = 32;
	int ppuScale = 2;

	float[] orthoScales;

	int orthoRef;

	int shiftMultiplier;

	int currentLevel = TileGenerator.startingLevel - 1;
	int newLevel;

	List<Vector2> uvs;

	void Start (){
		InitializeCamera ();
		uvs = new List<Vector2> ();
	}

	void Update () {
		//Shift Multiplier
		if (Input.GetKey ("left shift")) {
			shiftMultiplier = setShiftMultiplier;
		} else {
			shiftMultiplier = 1;
		}
		//Moving The Camera Around
		if (Input.GetKey ("w")) {
			Camera.main.transform.Translate (0, cameraSpeed * Time.deltaTime * shiftMultiplier, 0);
		}
		if (Input.GetKey ("s")) {
			Camera.main.transform.Translate (0, -cameraSpeed * Time.deltaTime * shiftMultiplier, 0);
		}
		if (Input.GetKey ("a")) {
			Camera.main.transform.Translate (-cameraSpeed * Time.deltaTime * shiftMultiplier, 0 , 0);
		}
		if (Input.GetKey ("d")) {
			Camera.main.transform.Translate (cameraSpeed * Time.deltaTime * shiftMultiplier, 0, 0);
		}
		//Zooming The Camera In and Out
		if (Input.GetAxis ("Mouse ScrollWheel") > 0 && Camera.main.orthographicSize > orthoScales [orthoScales.Length - 1]) {
			orthoRef++;
			Camera.main.orthographicSize = orthoScales [orthoRef];
		}
		if (Input.GetAxis ("Mouse ScrollWheel") < 0 && Camera.main.orthographicSize < orthoScales [0]) {
			orthoRef--;
			Camera.main.orthographicSize = orthoScales [orthoRef];
		}
		//Switch Between Levels
		if (Input.GetKeyDown ("[+]") && currentLevel < TileGenerator.mapLevels - 1) {
			newLevel = currentLevel + 1;
			//Debug.Log (currentLevel + " " + newLevel + "+");
			TileGenerator.instance.ChangeLevel (currentLevel, newLevel);
			currentLevel = newLevel;
		}
		if (Input.GetKeyDown ("[-]") && currentLevel > 0) {
			newLevel = currentLevel - 1;
			//Debug.Log (currentLevel + " " + newLevel + "-");
			TileGenerator.instance.ChangeLevel (currentLevel, newLevel);
			currentLevel = newLevel;
		}
		//Clicking (Getting World Co-Ords)
		if (Input.GetMouseButtonDown (0) && 
			Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= 0 && 
			Camera.main.ScreenToWorldPoint(Input.mousePosition).x < TileGenerator.mapWidth && 
			Camera.main.ScreenToWorldPoint(Input.mousePosition).y >= 0 &&
			Camera.main.ScreenToWorldPoint(Input.mousePosition).y < TileGenerator.mapHeight) {
			int camX = Mathf.FloorToInt (Camera.main.ScreenToWorldPoint (Input.mousePosition).x);
			int camY = Mathf.FloorToInt (Camera.main.ScreenToWorldPoint (Input.mousePosition).y);

			Tile.BuildWall (TileGenerator.tiles [currentLevel, camX, camY]);
			TileGenerator.RefreshMeshAtTile (TileGenerator.tiles [currentLevel, camX, camY]);
		}
	}

	public void InitializeCamera(){
		orthoRef = ppuScale;
		Camera.main.orthographicSize = (Camera.main.pixelHeight / (float)(ppuScale * pixelsPerUnit)) / 2f;

		CalculateValidOrthoScales ();
	}

	public void CalculateValidOrthoScales(){
		orthoScales = new float[maximumZoom];

		for (int i = 0; i < orthoScales.Length; i++) {
			int scale = i + 1;
			orthoScales [i] = (Camera.main.pixelHeight / (float)(scale * pixelsPerUnit)) / 2f;
		}
	}

	public void RefreshMesh(Tile t){
		Mesh mesh = MeshData.GetMeshAtTile (t);

		int posChunkX = Mathf.FloorToInt (TileGenerator.tiles [currentLevel, t.X, t.Y].MESH.transform.position.x);
		int posChunkY = Mathf.FloorToInt (TileGenerator.tiles [currentLevel, t.X, t.Y].MESH.transform.position.y);
		int chunkX = 0;
		int chunkY = 0;

		for (int i = 0; i < TileGenerator.tiles.GetLength(1); i++) {
			if (mesh == TileGenerator.tiles [currentLevel, i, t.Y].MESH.GetComponent<MeshFilter> ().mesh) {
				chunkX++;				
			}					
		}
		for (int i = 0; i < TileGenerator.tiles.GetLength(2); i++) {
			if (mesh == TileGenerator.tiles [currentLevel, t.X, i].MESH.GetComponent<MeshFilter> ().mesh) {
				chunkY++;
			}
		}

		for (int i = 0; i < chunkX; i++) {
			for (int o = 0; o < chunkY; o++) {
				uvs.AddRange (SpriteLoader.instance.GetUVS (TileGenerator.tiles [currentLevel, i + posChunkX, o + posChunkY]));
			}
		}

		mesh.uv = uvs.ToArray ();
		uvs.Clear ();
	}

}
