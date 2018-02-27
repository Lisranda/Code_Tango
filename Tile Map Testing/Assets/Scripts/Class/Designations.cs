using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Designations {

	public enum DesigType {Mine, BuildWall, PlaceDeployable};

	public static void DesignationCaller(Tile t, DesigType desigType){
		if (desigType == DesigType.Mine)
			Mine (t);
		if (desigType == DesigType.BuildWall)
			BuildWall (t);
		if (desigType == DesigType.PlaceDeployable)
			PlaceDeployable (t);
	}

	static void Mine(Tile t){
		if (t != null) {			
			t.WALL = Tile.Wall.Empty;
			MeshRefresh.AddForRefresh (TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [TileGenerator.wallMeshArrayRef]);
			MeshRefresh.AddForRefreshRange (TileGenerator.GetMeshNeighbours(TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [TileGenerator.wallMeshArrayRef], true));
		}
	}

	static void BuildWall(Tile t){
		if (t != null) {			
			t.WALL = Tile.Wall.Wall;
			MeshRefresh.AddForRefresh (TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [TileGenerator.wallMeshArrayRef]);
			MeshRefresh.AddForRefreshRange (TileGenerator.GetMeshNeighbours(TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [TileGenerator.wallMeshArrayRef], true));
		}
	}

	static void PlaceDeployable(Tile t){
		if (t != null) {
			Deployables.PlaceDeployable (DeployableLoader.instance.GetModel ("Bed"), t);
			MeshRefresh.AddForRefresh (TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [TileGenerator.deployablesMeshArrayRef]);
			MeshRefresh.AddForRefreshRange (TileGenerator.GetMeshNeighbours(TileGenerator.tiles [t.LEVEL, t.X, t.Y].MESH [TileGenerator.deployablesMeshArrayRef], true));
		}
	}
}
