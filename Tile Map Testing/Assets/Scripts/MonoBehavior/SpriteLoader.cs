using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader : MonoBehaviour {

	public static SpriteLoader instance;

	public static Sprite[] SpriteArray;
	public static Material worldMaterial;

	Dictionary<string, Vector2[]> uvMap;

	Texture2D atlas;
	Rect[] rects;

	void Awake (){
		instance = this;
		uvMap = new Dictionary<string, Vector2[]>();


		ImportSprites ();
		InitializeWorldMaterial ();
		InitializeSpriteUVs ();
	}

	bool IsEmptyOrNull(Tile[] neighbours, int arrayPos){
		if (neighbours [arrayPos] == null || neighbours [arrayPos].WALL == Tile.Wall.Empty) {
			return true;
		} else
			return false;
	}

	void ImportSprites(){
		SpriteArray = Resources.LoadAll<Sprite> ("Sprites");
		Texture2D[] atlasTextures = new Texture2D[SpriteArray.Length];

		for (int i = 0; i < SpriteArray.Length; i++) 
			atlasTextures [i] = SpriteArray [i].texture;		

		atlas = new Texture2D (8192, 8192);
		atlas.filterMode = FilterMode.Point;
		rects = atlas.PackTextures (atlasTextures, 0, 8192);
	}

	void InitializeWorldMaterial(){
		worldMaterial = new Material (Shader.Find ("Sprites/Default"));
		worldMaterial.mainTexture = atlas;
	}

	void InitializeSpriteUVs(){
		for (int i = 0; i < rects.Length; i++) {
			Vector2[] uvs = new Vector2[4];

			uvs [0] = new Vector2 (rects [i].xMin, rects [i].yMin);
			uvs [1] = new Vector2 (rects [i].xMax, rects [i].yMin);
			uvs [2] = new Vector2 (rects [i].xMin, rects [i].yMax);
			uvs [3] = new Vector2 (rects [i].xMax, rects [i].yMax);

			uvMap.Add (SpriteArray [i].name, uvs);
		}
	}

	public Vector2[] GetWorldUVS (Tile tile){
		string key = tile.FLOOR.ToString ();

		if (uvMap.ContainsKey (key)) {
			return uvMap [key];
		}
		else {
			Debug.LogError ("There is no UV for tile type: " + key);
			return uvMap ["NullTile"];
		}
	}

	public Vector2[] GetWallUVS (Tile[] neighbours, Tile.Wall wall, int quadrant){
		if (wall == Tile.Wall.Empty) {
			return uvMap ["Empty"];
		}			

		string key = wall.ToString () + "_" + quadrant.ToString () + "_";

		if (quadrant == 1) {
			if (IsEmptyOrNull (neighbours, 2) && IsEmptyOrNull (neighbours, 3)) {
				key += "Cor";
			} else if (!IsEmptyOrNull (neighbours, 2) && !IsEmptyOrNull (neighbours, 3) && IsEmptyOrNull (neighbours, 6)) {
				key += "InvCor";
			} else if (IsEmptyOrNull (neighbours, 2)) {
				key += "S";
			} else if (IsEmptyOrNull (neighbours, 3)) {
				key += "W";
			}
		} else if (quadrant == 2) {
			if (IsEmptyOrNull (neighbours, 1) && IsEmptyOrNull (neighbours, 2)) {
				key += "Cor";
			} else if (!IsEmptyOrNull (neighbours, 1) && !IsEmptyOrNull (neighbours, 2) && IsEmptyOrNull (neighbours, 5)) {
				key += "InvCor";
			} else if (IsEmptyOrNull (neighbours, 1)) {
				key += "E";
			} else if (IsEmptyOrNull (neighbours, 2)) {
				key += "S";
			}
		} else if (quadrant == 3) {
			if (IsEmptyOrNull (neighbours, 0) && IsEmptyOrNull (neighbours, 3)) {
				key += "Cor";
			} else if (!IsEmptyOrNull (neighbours, 0) && !IsEmptyOrNull (neighbours, 3) && IsEmptyOrNull (neighbours, 7)) {
				key += "InvCor";
			} else if (IsEmptyOrNull (neighbours, 0)) {
				key += "N";
			} else if (IsEmptyOrNull (neighbours, 3)) {
				key += "W";
			}
		} else if (quadrant == 4) {
			if (IsEmptyOrNull (neighbours, 0) && IsEmptyOrNull (neighbours, 1)) {
				key += "Cor";
			} else if (!IsEmptyOrNull (neighbours, 0) && !IsEmptyOrNull (neighbours, 1) && IsEmptyOrNull (neighbours, 4)) {
				key += "InvCor";
			} else if (IsEmptyOrNull (neighbours, 0)) {
				key += "N";
			} else if (IsEmptyOrNull (neighbours, 1)) {
				key += "E";
			}
		}

		if (uvMap.ContainsKey (key)) {
			return uvMap [key];
		}
		else {
			Debug.LogError ("There is no UV for tile type: " + key);
			return uvMap ["NullTile"];
		}
	}

	public Vector2[] GetOverlayUVS (Tile tile){
		string key = tile.OVERLAY.ToString ();

		if (uvMap.ContainsKey (key)) {
			return uvMap [key];
		}
		else {
			Debug.LogError ("There is no UV for tile type: " + key);
			return uvMap ["NullTile"];
		}
	}


}
