using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Designations {

	public enum DesigType {Mine, BuildWall};

	public static void DesignationCaller(Tile t, DesigType desigType){
		if (desigType == DesigType.Mine)
			Mine (t);
		if (desigType == DesigType.BuildWall)
			BuildWall (t);
	}

	static void Mine(Tile t){
		if (t != null) {			
			t.WALL = Tile.Wall.Empty;
			MeshRefresh.AddForRefresh (TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [1]);
			MeshRefresh.AddForRefreshRange (TileGenerator.GetMeshNeighbours(TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [1], true));
		}
	}

	static void BuildWall(Tile t){
		if (t != null) {			
			t.WALL = Tile.Wall.Wall;
			MeshRefresh.AddForRefresh (TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [1]);
			MeshRefresh.AddForRefreshRange (TileGenerator.GetMeshNeighbours(TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [1], true));
		}
	}
}
