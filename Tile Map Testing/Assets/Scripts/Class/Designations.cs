using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Designations {
	public static void Mine(Tile t){
		if (t != null) {			
			t.WALL = Tile.Wall.Empty;
			MeshRefresh.AddForRefresh (TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [1]);
			MeshRefresh.AddForRefreshRange (TileGenerator.GetMeshNeighbours(TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [1], true));
		}
	}
}
