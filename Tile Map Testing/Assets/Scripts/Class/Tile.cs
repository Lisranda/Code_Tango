using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
	public enum Type {Empty, Floor, Wall};
	public Type TYPE;
	public enum Stuff {Grass, Dirt, Stone, Sand};
	public Stuff MAT;
	public readonly int X;
	public readonly int Y;
	public readonly int LEVEL;
	//public readonly float ELEVATION;
	//public readonly float TEMPERATURE;
	//public readonly float HUMIDITY;

	public Tile(int level, int x, int y){
		this.X = x;
		this.Y = y;
		this.LEVEL = level;

		if (Random.Range (0, 2) == 0) {
			this.MAT = Stuff.Grass;
		} else {
			this.MAT = Stuff.Dirt;
		}

		//if (LEVEL % 2 == 1) {
		//	this.TYPE = Type.Dirt;
		//} else {
		//	this.TYPE = Type.Grass;
		//}
	}		

	public Vector3 Position(){
		return new Vector3 (this.X, this.Y, 0);
	}

	public void BuildWall(){
		this.MAT = Stuff.Stone;
	}
}
