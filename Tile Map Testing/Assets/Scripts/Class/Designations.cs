using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Designations {
	public static void Mine(Tile t){
		if (t != null) {			
			t.WALL = Tile.Wall.Empty;
			//MeshRefresh.refreshList.Add (TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [1]);

			foreach (GameObject go in TileGenerator.meshList [t.LEVEL]) {
				if (go.GetComponent<DataTracker>().layer == DataTracker.Layer.Wall)
					MeshRefresh.refreshList.Add (go);
			}
		}
	}
}
