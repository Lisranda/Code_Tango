using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Designations {
	public static void Mine(Tile t){
		if (t != null) {			
			t.WALL = Tile.Wall.Empty;
			MeshRefresh.refreshList.Add (TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [1]);
			MeshRefresh.refreshList.AddRange (TileGenerator.GetMeshNeighbours(TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [1], true));
			Debug.Log (TileGenerator.GetMeshNeighbours (TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [1], true).Count);
		}
	}
}
