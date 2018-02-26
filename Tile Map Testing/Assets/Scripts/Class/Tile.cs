using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
	public enum Wall {Empty, Wall};
	public Wall WALL;
	public enum Floor {Empty, Grass, Dirt, Stone, Sand, Water};
	public Floor FLOOR;
	public enum Overlay {Empty, SelectTile};
	public Overlay OVERLAY;
	public readonly int X;
	public readonly int Y;
	public readonly int LEVEL;
	public GameObject[] MESH = new GameObject[TileGenerator.numberOfMeshLayers];
	public readonly float ELEVATION;
	public readonly float TEMPERATURE;
	public readonly float HUMIDITY;
	public Deployables DEPLOYABLE;

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
					this.WALL = Wall.Wall;
					this.FLOOR = Floor.Stone;
					this.OVERLAY = Overlay.Empty;
				}
			}
		}
	}

	public bool AssignDeployableToTile (Deployables obj){
		if (obj == null) {
			DEPLOYABLE = null;
			return true;
		} else if (DEPLOYABLE != null) {
			return false;
		} else {
			DEPLOYABLE = obj;
			return true;
		}
	}
}
