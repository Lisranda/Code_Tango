using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deployables  {
	public string name { get; private set; }
	int posLevel;
	int posX;
	int posY;
	public int sizeX { get; private set; }
	public int sizeY { get; private set; }
	float travelSpeed;
	public Tile tile { get; private set; }
	public enum Rotation { Normal, Right, Up, Left };
	public Rotation rotation { get; private set; }

	protected Deployables () {		
	}

	public void ChangeModelRotation (Rotation r) {
		int swap = 0;
		if (r == Rotation.Normal) {
			rotation = r;
			swap = sizeX;
			sizeX = sizeY;
			sizeY = swap;
			Debug.Log ("Changing Rotation " + rotation.ToString () + sizeX + " : " + sizeY);
		} else if (r == Rotation.Right) {
			rotation = r;
			swap = sizeX;
			sizeX = sizeY;
			sizeY = swap;
			Debug.Log ("Changing Rotation " + rotation.ToString () + sizeX + " : " + sizeY);
		} else if (r == Rotation.Up) {
			rotation = r;
			swap = sizeX;
			sizeX = sizeY;
			sizeY = -swap;
			Debug.Log ("Changing Rotation " + rotation.ToString () + sizeX + " : " + sizeY);
		} else if (r == Rotation.Left) {
			rotation = r;
			swap = sizeX;
			sizeX = sizeY;
			sizeY = swap;
			Debug.Log ("Changing Rotation " + rotation.ToString () + sizeX + " : " + sizeY);
		}
	}

	public static Deployables LoadDeployables (string name, float travelSpeed, int sizeX, int sizeY) {
		Deployables obj = new Deployables ();
		obj.name = name;
		obj.travelSpeed = travelSpeed;
		obj.sizeX = sizeX;
		obj.sizeY = sizeY;
		obj.rotation = Rotation.Normal;
		return obj;
	}

	public static Deployables PlaceDeployable (Deployables model, Tile tile) {
		Deployables obj = new Deployables ();
		obj.name = model.name;
		obj.travelSpeed = model.travelSpeed;
		obj.sizeX = model.sizeX;
		obj.sizeY = model.sizeY;
		obj.rotation = model.rotation;
		obj.tile = tile;


		for (int x = 0; x < obj.sizeX; x++) {
			for (int y = 0; y < obj.sizeY; y++) {
				if (!TileGenerator.tiles [tile.LEVEL, tile.X + x, tile.Y - y].CheckDeployablesEmptyOnTile ())
					return null;
			}			
		}

		for (int x = 0; x < obj.sizeX; x++) {
			for (int y = 0; y < obj.sizeY; y++) {
				if (!TileGenerator.tiles [tile.LEVEL, tile.X + x, tile.Y - y].AssignDeployableToTile (obj))
					return null;
			}			
		}

//		if (!tile.AssignDeployableToTile (obj))
//			return null;

		return obj;
	}
}
