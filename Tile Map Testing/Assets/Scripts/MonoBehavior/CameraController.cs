using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public int cameraSpeed = 15;
	public int setShiftMultiplier = 2;
	public int maximumZoom;
	public int minimumZoom;

	int shiftMultiplier;

	int currentLevel = TileGenerator.startingLevel - 1;
	int newLevel;

	void Start (){
	}

	void Update () {
		if (Input.GetKey ("left shift")) {
			shiftMultiplier = setShiftMultiplier;
		} else {
			shiftMultiplier = 1;
		}

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
		if (Input.GetAxis ("Mouse ScrollWheel") > 0 && Camera.main.orthographicSize > maximumZoom) {
			Camera.main.orthographicSize = Mathf.Clamp((Camera.main.orthographicSize - 1 * shiftMultiplier),maximumZoom,minimumZoom);
		}
		if (Input.GetAxis ("Mouse ScrollWheel") < 0 && Camera.main.orthographicSize < minimumZoom) {
			Camera.main.orthographicSize = Mathf.Clamp((Camera.main.orthographicSize + 1 * shiftMultiplier),maximumZoom,minimumZoom);
		}

		//Switch Between Levels
		if (Input.GetKeyDown ("[+]") && currentLevel < TileGenerator.mapLevels - 1) {
			newLevel = currentLevel + 1;
			Debug.Log (currentLevel + " " + newLevel + "+");
			TileGenerator.instance.ChangeLevel (currentLevel, newLevel);
			currentLevel = newLevel;
		}
		if (Input.GetKeyDown ("[-]") && currentLevel > 0) {
			newLevel = currentLevel - 1;
			Debug.Log (currentLevel + " " + newLevel + "-");
			TileGenerator.instance.ChangeLevel (currentLevel, newLevel);
			currentLevel = newLevel;
		}

		if (Input.GetMouseButtonDown (0) && 
			Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= 0 && 
			Camera.main.ScreenToWorldPoint(Input.mousePosition).x < TileGenerator.mapWidth && 
			Camera.main.ScreenToWorldPoint(Input.mousePosition).y >= 0 &&
			Camera.main.ScreenToWorldPoint(Input.mousePosition).y < TileGenerator.mapHeight) {
			int camX = Mathf.FloorToInt (Camera.main.ScreenToWorldPoint (Input.mousePosition).x);
			int camY = Mathf.FloorToInt (Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
		}


	}
}
