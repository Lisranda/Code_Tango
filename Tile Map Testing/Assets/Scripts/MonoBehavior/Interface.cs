using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface : MonoBehaviour {
	public static Interface instance;

	int setShiftMultiplier = 2;
	int shiftMultiplier;

	List<Tile> mouseSelected;

	void Awake (){
		instance = this;
		mouseSelected = new List<Tile> ();
	}

	void LateUpdate () {
		SetShiftMultiplier ();
		CameraMovementInputs ();
		CameraZoomInputs ();
		ChangeLevelInputs ();
		MouseLeftClickInputs ();
	}

	#region Mouse Coords To Int

	int MouseX(){
		return Mathf.FloorToInt (Camera.main.ScreenToWorldPoint (Input.mousePosition).x);
	}

	int MouseY(){
		return Mathf.FloorToInt (Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
	}

	int MouseL(){
		return CameraController.currentLevel;
	}

	#endregion

	void SetShiftMultiplier(){
		if (Input.GetKey ("left shift")) {
			shiftMultiplier = setShiftMultiplier;
		} else {
			shiftMultiplier = 1;
		}
	}

	void CameraMovementInputs(){
		if (Input.GetKey ("w"))
			CameraController.CameraMovement ("w", shiftMultiplier);
		if (Input.GetKey ("s"))
			CameraController.CameraMovement ("s", shiftMultiplier);
		if (Input.GetKey ("a"))
			CameraController.CameraMovement ("a", shiftMultiplier);
		if (Input.GetKey ("d"))
			CameraController.CameraMovement ("d", shiftMultiplier);
	}

	void CameraZoomInputs(){
		if (Input.GetAxis ("Mouse ScrollWheel") > 0)
			CameraController.CameraZoom (true, shiftMultiplier);
		if (Input.GetAxis ("Mouse ScrollWheel") < 0)
			CameraController.CameraZoom (false, shiftMultiplier);
	}

	void ChangeLevelInputs(){
		if (Input.GetKeyDown ("[+]"))
			CameraController.CameraLevelChange (true);
		if (Input.GetKeyDown ("[-]"))
			CameraController.CameraLevelChange (false);
	}

	void MouseLeftClickInputs(){
		if (Input.GetMouseButtonDown (0)) {
			if (TileGenerator.GetTileAt(MouseL (), MouseX (), MouseY ()) != null) {
				mouseSelected.Add (TileGenerator.GetTileAt(MouseL (), MouseX (), MouseY ()));
			}
		}
		if (Input.GetMouseButton (0)) {
			if (MouseX () != mouseSelected [0].X || MouseY () != mouseSelected [0].Y) {
				for (int i = mouseSelected [0].X; i <= MouseX (); i++) {
					for (int o = mouseSelected [0].Y; o <= MouseY (); o++) {
						if (!mouseSelected.Contains (TileGenerator.GetTileAt (MouseL (), i, o))) {
							mouseSelected.Add (TileGenerator.GetTileAt (MouseL (), i, o));
						}
					}					
				}
			}

			foreach (Tile t in mouseSelected) {
				t.OVERLAY = Tile.Overlay.SelectTile;
				MeshRefresh.refreshList.Add (t.MESH [2]);
			}
			
		}
		if (Input.GetMouseButtonUp (0)) {
			foreach (Tile t in mouseSelected) {
				Designations.Mine (t);
				t.OVERLAY = Tile.Overlay.Empty;
				MeshRefresh.refreshList.Add (t.MESH [2]);
			}
			mouseSelected.Clear ();
		}
	}
}
