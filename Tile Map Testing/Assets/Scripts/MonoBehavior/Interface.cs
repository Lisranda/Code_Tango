﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface : MonoBehaviour {
	public static Interface instance;

	int setShiftMultiplier = 2;
	int shiftMultiplier;

	bool mineDesignation = false;
	bool buildDesignation = false;

	List<Tile> mouseSelected;
	List<Tile> actionSelected;

	Tile tS;
	Tile tE;

	void Awake (){
		instance = this;
		mouseSelected = new List<Tile> ();
		actionSelected = new List<Tile> ();
	}

	void LateUpdate () {
		SetShiftMultiplier ();
		CameraMovementInputs ();
		CameraZoomInputs ();
		ChangeLevelInputs ();
		DesignationSelector ();
//		MouseHoverSelector ();
		if (mineDesignation)
			MineDesignation ();
		if (buildDesignation)
			BuildDesignation ();
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

	void DesignationSelector(){
		if (Input.GetKeyDown ("b")) {
			if (!buildDesignation) {
				mineDesignation = false;
				buildDesignation = true;
			}				
			else
				buildDesignation = false;
		}
		if (Input.GetKeyDown ("m")) {
			if (!mineDesignation) {
				buildDesignation = false;
				mineDesignation = true;
			}					
			else
				mineDesignation = false;			
		}
	}

	void MouseHoverSelector(){
		if (tE != null) {
			tE.OVERLAY = Tile.Overlay.Empty;
			MeshRefresh.AddForRefresh (tE.MESH [2]);
		}

		if (TileGenerator.GetTileAt (MouseL (), MouseX (), MouseY ()) != null) {
			tS = TileGenerator.GetTileAt (MouseL (), MouseX (), MouseY ());
			tE = tS;
			tS.OVERLAY = Tile.Overlay.SelectTile;
			MeshRefresh.AddForRefresh (tS.MESH [2]);
		}
	}

	void MineDesignation(){
		MouseHoverSelector ();

		if (Input.GetMouseButtonDown (0)) {
			if (TileGenerator.GetTileAt(MouseL (), MouseX (), MouseY ()) != null) {
				mouseSelected.Add (TileGenerator.GetTileAt(MouseL (), MouseX (), MouseY ()));
			}
		}

		if (Input.GetMouseButton (0)) {
			if (TileGenerator.GetTileAt (MouseL (), MouseX (), MouseY ()) != null) {
				int xS = mouseSelected [0].X;
				int xE = MouseX ();
				if (xE < xS) {
					int swap = xS;
					xS = xE;
					xE = swap;
				}
				int yS = mouseSelected [0].Y;
				int yE = MouseY ();
				if (yE < yS) {
					int swap = yS;
					yS = yE;
					yE = swap;
				}

				for (int i = xS; i <= xE; i++) {
					for (int o = yS; o <= yE; o++) {						
						mouseSelected.Add (TileGenerator.GetTileAt (MouseL (), i, o));						
					}					
				}

				foreach (Tile t in actionSelected) {
					t.OVERLAY = Tile.Overlay.Empty;
					MeshRefresh.AddForRefresh (t.MESH [2]);
				}

				foreach (Tile t in mouseSelected) {
					t.OVERLAY = Tile.Overlay.SelectTile;
					MeshRefresh.AddForRefresh (t.MESH [2]);
				}

				actionSelected.Clear ();
				actionSelected.AddRange (mouseSelected);
				mouseSelected.Clear ();
				mouseSelected.Add (actionSelected [0]);
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			foreach (Tile t in actionSelected) {
				Designations.Mine (t);
				t.OVERLAY = Tile.Overlay.Empty;
				MeshRefresh.AddForRefresh (t.MESH [2]);
			}
			mouseSelected.Clear ();
		}
	}

	void BuildDesignation(){
		MouseHoverSelector ();

		if (Input.GetMouseButtonDown (0)) {
			if (TileGenerator.GetTileAt(MouseL (), MouseX (), MouseY ()) != null) {
				mouseSelected.Add (TileGenerator.GetTileAt(MouseL (), MouseX (), MouseY ()));
			}
		}

		if (Input.GetMouseButton (0)) {
			if (TileGenerator.GetTileAt (MouseL (), MouseX (), MouseY ()) != null) {
				int xS = mouseSelected [0].X;
				int xE = MouseX ();
				if (xE < xS) {
					int swap = xS;
					xS = xE;
					xE = swap;
				}
				int yS = mouseSelected [0].Y;
				int yE = MouseY ();
				if (yE < yS) {
					int swap = yS;
					yS = yE;
					yE = swap;
				}

				for (int i = xS; i <= xE; i++) {
					for (int o = yS; o <= yE; o++) {
						if ((i == xS || i == xE) || (o == yS || o == yE))
							mouseSelected.Add (TileGenerator.GetTileAt (MouseL (), i, o));						
					}					
				}

				foreach (Tile t in actionSelected) {
					t.OVERLAY = Tile.Overlay.Empty;
					MeshRefresh.AddForRefresh (t.MESH [2]);
				}

				foreach (Tile t in mouseSelected) {
					t.OVERLAY = Tile.Overlay.SelectTile;
					MeshRefresh.AddForRefresh (t.MESH [2]);
				}

				actionSelected.Clear ();
				actionSelected.AddRange (mouseSelected);
				mouseSelected.Clear ();
				mouseSelected.Add (actionSelected [0]);
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			foreach (Tile t in actionSelected) {
				Designations.Mine (t);
				t.OVERLAY = Tile.Overlay.Empty;
				MeshRefresh.AddForRefresh (t.MESH [2]);
			}
			mouseSelected.Clear ();
		}
	}
}
