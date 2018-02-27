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

	List<Tile> tSL;
	List<Tile> tEL;

	Tile tS;
	Tile tE;

	enum SelectionType {Off, Tile, Filled, Border, Object};
	SelectionType selectionType;
	Designations.DesigType designation;

	Deployables model;
	int xS;
	int yS;

	void Awake (){
		instance = this;
		mouseSelected = new List<Tile> ();
		actionSelected = new List<Tile> ();
		tSL = new List<Tile> ();
		tEL = new List<Tile> ();
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
				model = DeployableLoader.instance.GetModel ("Bed");
			}
		}
	}

	void MouseHoverSelector(bool desigOn){
		if (tE != null) {
			tE.OVERLAY = Tile.Overlay.Empty;
			MeshRefresh.AddForRefresh (tE.MESH [TileGenerator.overlayMeshArrayRef]);
			tE = null;
		}
		if (tEL.Count > 0) {
			foreach (Tile t in tEL) {
				t.OVERLAY = Tile.Overlay.Empty;
				MeshRefresh.AddForRefresh (t.MESH [TileGenerator.overlayMeshArrayRef]);
			}
			tEL.Clear ();
		}
		if (selectionType != SelectionType.Object) {
			if (desigOn && TileGenerator.GetTileAt (MouseL (), MouseX (), MouseY ()) != null) {
				tS = TileGenerator.GetTileAt (MouseL (), MouseX (), MouseY ());
				tE = tS;
				tS.OVERLAY = Tile.Overlay.SelectTile;
				MeshRefresh.AddForRefresh (tS.MESH [TileGenerator.overlayMeshArrayRef]);
			}
		} else if (selectionType == SelectionType.Object) {

			int xS = 0;
			int yS = 0;
			int xE = model.sizeX;
			int yE = model.sizeY;
			if (xE < xS) {
				int swap = xS;
				xS = xE;
				xE = swap;
			}
			if (yE < yS) {
				int swap = yS;
				yS = yE;
				yE = swap;
			}
				

			for (int x = xS; x < xE; x++) {
				for (int y = yS; y < yE; y++) {
					if (TileGenerator.GetTileAt (MouseL (), MouseX () + x, MouseY () - y) != null)
						tSL.Add (TileGenerator.GetTileAt (MouseL (), MouseX () + x, MouseY () - y));
					else {
						tSL.Clear ();
						goto BreakObjectHoverSelection;
					}						
				}
			}
			BreakObjectHoverSelection:
			tEL.AddRange (tSL);

			bool allValid = true;
			foreach (Tile t in tSL) {
				if (!t.ValidForDeployables ()) {
					allValid = false;
					break;
				} else
					allValid = true;					
			}

			foreach (Tile t in tSL) {
				if (t.ValidForDeployables () && allValid)
					t.OVERLAY = Tile.Overlay.SelectTile;
				else
					t.OVERLAY = Tile.Overlay.BadSelectTile;
				
				MeshRefresh.AddForRefresh (t.MESH [TileGenerator.overlayMeshArrayRef]);
			}
			tSL.Clear ();
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

			if (Input.GetKeyDown ("r")) {
				if (model.rotation == Deployables.Rotation.Normal)
					model.ChangeModelRotation (Deployables.Rotation.Right);
				else if (model.rotation == Deployables.Rotation.Right)
					model.ChangeModelRotation (Deployables.Rotation.Up);
				else if (model.rotation == Deployables.Rotation.Up)
					model.ChangeModelRotation (Deployables.Rotation.Left);
				else if (model.rotation == Deployables.Rotation.Left)
					model.ChangeModelRotation (Deployables.Rotation.Normal);
			}

			if (Input.GetMouseButtonUp (0) && TileGenerator.InWorldBounds (MouseL (), MouseX (), MouseY ())) {
				for (int x = 0; x < model.sizeX; x++) {
					for (int y = 0; y < model.sizeY; y++) {
						Tile t = TileGenerator.GetTileAt (MouseL (), MouseX () + x, MouseY () - y);
						if (t == null || !t.ValidForDeployables ())
							return;
					}
				}

				Tile ti = TileGenerator.GetTileAt (MouseL (), MouseX (), MouseY ());
				Designations.DesignationCaller (ti, designation);
			}

		} else
			MouseHoverSelector (false);
	}
}
