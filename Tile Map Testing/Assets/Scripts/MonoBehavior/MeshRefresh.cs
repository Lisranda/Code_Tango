﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeshRefresh : MonoBehaviour {
	
	public static MeshRefresh instance;
	public static List<GameObject> refreshList;
	public static List<GameObject> uniqueRefreshList;
	static List<Vector2> uvs;
	Tile[] neighbours;

	void Awake(){
		instance = this;
		refreshList = new List<GameObject> ();
		uniqueRefreshList = new List<GameObject> ();
		uvs = new List<Vector2> ();
	}

	void LateUpdate(){
		RefreshMeshFromList ();
	}

	public static void AddForRefresh(GameObject meshGO){
		refreshList.Add (meshGO);
	}

	public static void AddForRefreshRange(List<GameObject> meshGOList){
		refreshList.AddRange (meshGOList);
	}
		
	static void RemoveDuplicates(){
		uniqueRefreshList.AddRange (refreshList.Distinct ().ToList ());
		refreshList.Clear ();
	}

	void RefreshMeshFromList(){
		if (refreshList.Count > 0)
			RemoveDuplicates ();
		if (uniqueRefreshList.Count > 0) {
			foreach (GameObject go in uniqueRefreshList) {
				Mesh mesh = go.GetComponent<MeshFilter> ().mesh;
				int level = go.GetComponent<DataTracker> ().level;
				DataTracker.Layer layer = go.GetComponent<DataTracker> ().layer;

				int posChunkX = Mathf.FloorToInt (go.transform.position.x);
				int posChunkY = Mathf.FloorToInt (go.transform.position.y);

				for (int i = 0; i < mesh.bounds.size.x; i++) {
					for (int o = 0; o < mesh.bounds.size.y; o++) {
						if (layer == DataTracker.Layer.Floor) {
							uvs.AddRange (SpriteLoader.instance.GetWorldUVS (TileGenerator.tiles [level, i + posChunkX, o + posChunkY]));
						} else if (layer == DataTracker.Layer.Wall) {
							neighbours = TileGenerator.GetTileNeighbours (TileGenerator.tiles [level, i + posChunkX, o + posChunkY], true);
							uvs.AddRange (SpriteLoader.instance.GetWallUVS (neighbours, TileGenerator.tiles [level, i + posChunkX, o + posChunkY].WALL, 1));
							uvs.AddRange (SpriteLoader.instance.GetWallUVS (neighbours, TileGenerator.tiles [level, i + posChunkX, o + posChunkY].WALL, 2));
							uvs.AddRange (SpriteLoader.instance.GetWallUVS (neighbours, TileGenerator.tiles [level, i + posChunkX, o + posChunkY].WALL, 3));
							uvs.AddRange (SpriteLoader.instance.GetWallUVS (neighbours, TileGenerator.tiles [level, i + posChunkX, o + posChunkY].WALL, 4));
						} else if (layer == DataTracker.Layer.Deployables) {
							uvs.AddRange (SpriteLoader.instance.GetDeployablesUVS (TileGenerator.tiles [level, i + posChunkX, o + posChunkY]));
						} else if (layer == DataTracker.Layer.Overlay) {
							uvs.AddRange (SpriteLoader.instance.GetOverlayUVS (TileGenerator.tiles [level, i + posChunkX, o + posChunkY]));
						} else {
							Debug.LogError ("RefreshMeshAtTile failed because it is trying to get UVS for a layer that doesn't exist in the tile.MESH array.");
						}
					}
				}
				mesh.uv = uvs.ToArray ();
				uvs.Clear ();
			}
			uniqueRefreshList.Clear ();
		}		
	}
}
