using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
	public enum Wall {Empty, Wall};
	public Wall WALL;
	public enum Floor {Empty, Grass, Dirt, Stone, Sand, Water};
	public Floor FLOOR;
	public enum Overlay {Empty, Grass};
	public Overlay OVERLAY;
	public readonly int X;
	public readonly int Y;
	public readonly int LEVEL;
	public GameObject[] MESH = new GameObject[3];
	public readonly float ELEVATION;
	public readonly float TEMPERATURE;
	public readonly float HUMIDITY;

	public Tile(int level, int x, int y, float elevation){
		this.X = x;
		this.Y = y;
		this.LEVEL = level;
		this.ELEVATION = elevation;

		if (LEVEL < 50) {
			this.WALL = Wall.Wall;
			this.FLOOR = Floor.Stone;
			this.OVERLAY = Overlay.Empty;
		}

		for (int i = 0; i < 10; i++) {
			if (LEVEL == (50 + i)) {
				if (ELEVATION < i / 10f) {
					this.WALL = Wall.Empty;
					this.FLOOR = Floor.Empty;
					this.OVERLAY = Overlay.Empty;
				} else if (ELEVATION < (i + 1f) / 10f) {
					this.WALL = Wall.Empty;
					this.FLOOR = Floor.Grass;
					this.OVERLAY = Overlay.Empty;
				} else {
					this.WALL = Wall.Empty;
					this.FLOOR = Floor.Stone;
					this.OVERLAY = Overlay.Empty;
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

	public static Tile GetTileAt(int level, int x, int y){
		Tile t = TileGenerator.tiles [level, x, y];
		return t;
	}

	public static void BuildWall(Tile tile){
		tile.WALL = Wall.Wall;
		tile.FLOOR = Floor.Stone;
	}
}
