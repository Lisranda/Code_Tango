using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deployables  {
	public string name { get; private set; }
	int posLevel;
	int posX;
	int posY;
	int sizeX;
	int sizeY;
	float travelSpeed;
	Tile tile;

	protected Deployables () {		
	}

	public static Deployables LoadDeployables (string name, float travelSpeed, int sizeX, int sizeY) {
		Deployables obj = new Deployables ();
		obj.name = name;
		obj.travelSpeed = travelSpeed;
		obj.sizeX = sizeX;
		obj.sizeY = sizeY;
		return obj;
	}

	public static Deployables PlaceDeployable (Deployables model, Tile tile) {
		Deployables obj = new Deployables ();
		obj.name = model.name;
		obj.travelSpeed = model.travelSpeed;
		obj.sizeX = model.sizeX;
		obj.sizeY = model.sizeY;
		obj.tile = tile;

		if (!tile.AssignDeployableToTile (obj))
			return null;
		else
			return obj;
	}
}
