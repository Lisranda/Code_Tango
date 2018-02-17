using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	static int cameraSpeed = 15;
	static int numberOfZoomLevels = 10;

	static int pixelsPerUnit = 32;
	static int ppuScale = 1;

	static float[] orthoScales;

	static int orthoRef;


	public static int currentLevel = TileGenerator.startingLevel - 1;
	static int newLevel;


	void Start (){
		InitializeCamera ();
	}

	public void InitializeCamera(){
		orthoRef = ppuScale;
		Camera.main.orthographicSize = (Camera.main.pixelHeight / (float)(ppuScale * pixelsPerUnit)) / 2f;

		CalculateValidOrthoScales ();
	}

	public void CalculateValidOrthoScales(){
		orthoScales = new float[numberOfZoomLevels];

		for (int i = 0; i < orthoScales.Length; i++) {
			int scale = i + 1;
			orthoScales [i] = (Camera.main.pixelHeight / (float)(scale * pixelsPerUnit)) / 2f;
		}
	}

	public static void CameraMovement(string key, int multiplier){
		if (key == "w")
			Camera.main.transform.Translate (0, cameraSpeed * Time.deltaTime * multiplier, 0);
		if (key == "s")
			Camera.main.transform.Translate (0, -cameraSpeed * Time.deltaTime * multiplier, 0);
		if (key == "a")
			Camera.main.transform.Translate (-cameraSpeed * Time.deltaTime * multiplier, 0, 0);
		if (key == "d")
			Camera.main.transform.Translate (cameraSpeed * Time.deltaTime * multiplier, 0, 0);
	}

	public static void CameraZoom(bool zoomIn, int multiplier){
		if (zoomIn && Camera.main.orthographicSize > orthoScales [orthoScales.Length - 1]) {
			orthoRef++;
			Camera.main.orthographicSize = orthoScales [orthoRef];
		}
		if (!zoomIn && Camera.main.orthographicSize < orthoScales [0]) {
			orthoRef--;
			Camera.main.orthographicSize = orthoScales [orthoRef];
		}
	}

	public static void CameraLevelChange(bool goUp){
		if (goUp && currentLevel < TileGenerator.mapLevels - 1) {
			newLevel = currentLevel + 1;
			TileGenerator.instance.ChangeLevel (currentLevel, newLevel);
			currentLevel = newLevel;
		}
		if (!goUp && currentLevel > 0) {
			newLevel = currentLevel - 1;
			TileGenerator.instance.ChangeLevel (currentLevel, newLevel);
			currentLevel = newLevel;
		}
	}
}
