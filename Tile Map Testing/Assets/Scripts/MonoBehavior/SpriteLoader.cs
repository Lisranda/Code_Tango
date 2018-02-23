using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader : MonoBehaviour {

	public static SpriteLoader instance;

	public static Sprite[] SpriteArray;
	public static Material worldMaterial;

	Dictionary<string, Vector2[]> uvMap;

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
	}

	void InitializeWorldMaterial(){
		worldMaterial = new Material (Shader.Find ("Sprites/Default"));
		worldMaterial.mainTexture = SpriteArray [0].texture;
	}

	void InitializeSpriteUVs(){
		double spriteWidth = 0d;
		double spriteHeight = 0d;

		//Calculate spritesheet width
		foreach (Sprite sprite in SpriteArray) {
			if (sprite.rect.x + sprite.rect.width > spriteWidth)
				spriteWidth = sprite.rect.x + sprite.rect.width;

			if (sprite.rect.y + sprite.rect.height > spriteHeight)
				spriteHeight = sprite.rect.y + sprite.rect.height;
		}

		//Set UVs for each sprite
		foreach (Sprite sprite in SpriteArray) {
			Vector2[] uvs = new Vector2[4];

			uvs [0] = new Vector2 ((float)(sprite.rect.x / spriteWidth), (float)(sprite.rect.y / spriteHeight));
			uvs [1] = new Vector2 ((float)((sprite.rect.x + sprite.rect.width) / spriteWidth), (float)(sprite.rect.y / spriteHeight));
			uvs [2] = new Vector2 ((float)(sprite.rect.x / spriteWidth), (float)((sprite.rect.y + sprite.rect.height) / spriteHeight));
			uvs [3] = new Vector2 ((float)((sprite.rect.x + sprite.rect.width) / spriteWidth), (float)((sprite.rect.y + sprite.rect.height) / spriteHeight));

			uvMap.Add (sprite.name, uvs);
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
