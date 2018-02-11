using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
	public enum Type {Empty, Floor, Wall};
	public Type TYPE;
	public enum Stuff {Empty, Grass, Dirt, Stone, Sand, Water};
	public Stuff MAT;
	public readonly int X;
	public readonly int Y;
	public readonly int LEVEL;
	public readonly float ELEVATION;
	//public readonly float TEMPERATURE;
	//public readonly float HUMIDITY;

	public Tile(int level, int x, int y, float elevation){
		this.X = x;
		this.Y = y;
		this.LEVEL = level;
		this.ELEVATION = elevation;

		if (LEVEL < 50) {
			this.TYPE = Type.Wall;
			this.MAT = Stuff.Stone;
		}

		for (int i = 0; i < 10; i++) {
			if (LEVEL == (50 + i)) {
				if (ELEVATION < i / 10f) {
					this.TYPE = Type.Empty;
					this.MAT = Stuff.Empty;
				} else if (ELEVATION < (i + 1f) / 10f) {
					this.TYPE = Type.Floor;
					this.MAT = Stuff.Grass;
				} else {
					this.TYPE = Type.Wall;
					this.MAT = Stuff.Stone;
				}

			}
		}

		//if (ELEVATION > 0.4f) {
		//	this.MAT = Stuff.Grass;
		//} else {
		//	this.MAT = Stuff.Dirt;
		//}


		//Use this debug to see the levels as separate materials
		//if (LEVEL % 2 == 1) {
		//	this.TYPE = Type.Dirt;
		//} else {
		//	this.TYPE = Type.Grass;
		//}
	}		

	public Vector3 Position(){
		return new Vector3 (this.X, this.Y, 0);
	}

	public static void BuildWall(Tile tile){
		tile.TYPE = Type.Wall;
		tile.MAT = Stuff.Stone;
	}
}
