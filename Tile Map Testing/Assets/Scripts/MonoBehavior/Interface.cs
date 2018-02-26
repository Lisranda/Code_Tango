using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface : MonoBehaviour {
	public static Interface instance;

	int setShiftMultiplier = 2;
	int shiftMultiplier;

	int mouseStartL;
	int mouseStartX;
	int mouseStartY;

	List<Tile> mouseSelected;
	List<Tile> actionSelected;

	Tile tS;
	Tile tE;

	enum SelectionType {Off, Tile, Filled, Border, Object};
	SelectionType selectionType;
	Designations.DesigType designation;

	void Awake (){
		instance = this;
		mouseSelected = new List<Tile> ();
		actionSelected = new List<Tile> ();
		selectionType = SelectionType.Off;
	}

	void Update (){
		SetShiftMultiplier ();
		CameraMovementInputs ();
		CameraZoomInputs ();
		ChangeLevelInputs ();
		DesignationSelector ();
		MouseBoxSelect ();
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
		if (Input.GetKeyDown ("e")) {
			if (selectionType == SelectionType.Filled) {
				selectionType = SelectionType.Off;
			} else {
				designation = Designations.DesigType.Mine;
				selectionType = SelectionType.Filled;
			}
		}

		if (Input.GetKeyDown ("b")) {
			if (selectionType == SelectionType.Border) {
				selectionType = SelectionType.Off;
			} else {
				designation = Designations.DesigType.BuildWall;
				selectionType = SelectionType.Border;
			}
		}

		if (Input.GetKeyDown ("c")) {
			if (selectionType == SelectionType.Object) {
				selectionType = SelectionType.Off;
			} else {
				designation = Designations.DesigType.PlaceDeployable;
				selectionType = SelectionType.Object;
			}
		}
	}

	void MouseHoverSelector(bool desigOn){
		if (tE != null) {
			tE.OVERLAY = Tile.Overlay.Empty;
			MeshRefresh.AddForRefresh (tE.MESH [TileGenerator.overlayMeshArrayRef]);
			tE = null;
		}

		if (desigOn && TileGenerator.GetTileAt (MouseL (), MouseX (), MouseY ()) != null) {
			tS = TileGenerator.GetTileAt (MouseL (), MouseX (), MouseY ());
			tE = tS;
			tS.OVERLAY = Tile.Overlay.SelectTile;
			MeshRefresh.AddForRefresh (tS.MESH [TileGenerator.overlayMeshArrayRef]);
		}
	}

	public void MouseBoxSelect(){
		if (selectionType == SelectionType.Filled || selectionType == SelectionType.Border) {
			MouseHoverSelector (true);

			if (Input.GetMouseButtonDown (0)) {
				mouseStartL = MouseL ();
				mouseStartX = MouseX ();
				mouseStartY = MouseY ();				
			}

			if (Input.GetMouseButton (0)) {				
				int lS = mouseStartL;
				int lE = MouseL ();
				if (lE < lS) {
					int swap = lS;
					lS = lE;
					lE = swap;
				}
				int xS = mouseStartX;
				int xE = MouseX ();
				if (xE < xS) {
					int swap = xS;
					xS = xE;
					xE = swap;
				}
				int yS = mouseStartY;
				int yE = MouseY ();
				if (yE < yS) {
					int swap = yS;
					yS = yE;
					yE = swap;
				}

				for (int u = lS; u <= lE; u++) {
					for (int i = xS; i <= xE; i++) {
						for (int o = yS; o <= yE; o++) {
							if (TileGenerator.GetTileAt (u, i, o) != null) {
								if (selectionType == SelectionType.Filled) {
									mouseSelected.Add (TileGenerator.GetTileAt (u, i, o));
								} else if (selectionType == SelectionType.Border) {
									if (i == xS || i == xE || o == yS || o == yE)
										mouseSelected.Add (TileGenerator.GetTileAt (u, i, o));
								}
							}															
						}					
					}
				}					

				foreach (Tile t in actionSelected) {
					t.OVERLAY = Tile.Overlay.Empty;
					MeshRefresh.AddForRefresh (t.MESH [TileGenerator.overlayMeshArrayRef]);
				}

				foreach (Tile t in mouseSelected) {
					t.OVERLAY = Tile.Overlay.SelectTile;
					MeshRefresh.AddForRefresh (t.MESH [TileGenerator.overlayMeshArrayRef]);
				}

				actionSelected.Clear ();
				actionSelected.AddRange (mouseSelected);
				mouseSelected.Clear ();				
			}

			if (Input.GetMouseButtonUp (0)) {
				foreach (Tile t in actionSelected) {
					Designations.DesignationCaller (t, designation);
					t.OVERLAY = Tile.Overlay.Empty;
					MeshRefresh.AddForRefresh (t.MESH [TileGenerator.overlayMeshArrayRef]);
				}
			}
		} else if (selectionType == SelectionType.Object) {
			MouseHoverSelector (true);

			if (Input.GetMouseButtonUp (0) && TileGenerator.InWorldBounds (MouseL (), MouseX (), MouseY ())) {
				Tile t = TileGenerator.GetTileAt (MouseL (), MouseX (), MouseY ());
				if (t.DEPLOYABLE == null) {
					Designations.DesignationCaller (t, designation);
				}
			}

		} else
			MouseHoverSelector (false);
	}
}
