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

	void ImportSprites(){
		SpriteArray = Resources.LoadAll<Sprite> ("Sprites");
	}

	void InitializeWorldMaterial(){
		worldMaterial = new Material (Shader.Find ("Sprites/Default"));
		worldMaterial.mainTexture = SpriteArray [0].texture;
	}

	void InitializeSpriteUVs(){
		float spriteWidth = 0f;
		float spriteHeight = 0f;

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

			uvs [0] = new Vector2 (sprite.rect.x / spriteWidth, sprite.rect.y / spriteHeight);
			uvs [1] = new Vector2 ((sprite.rect.x + sprite.rect.width) / spriteWidth, sprite.rect.y / spriteHeight);
			uvs [2] = new Vector2 (sprite.rect.x / spriteWidth, (sprite.rect.y + sprite.rect.height) / spriteHeight);
			uvs [3] = new Vector2 ((sprite.rect.x + sprite.rect.width) / spriteWidth, (sprite.rect.y + sprite.rect.height) / spriteHeight);

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
			return uvMap ["NullSprite"];
		}
	}

	public Vector2[] GetWallUVS (Tile[] neighbours, Tile.Wall wall, int quadrant){
		if (wall == Tile.Wall.Empty) {
			return uvMap ["Empty"];
		}			

		string key = wall.ToString () + "_" + quadrant.ToString () + "_";
		if (quadrant == 1) {
			if (neighbours [2] == null || neighbours [2].WALL == Tile.Wall.Empty)
				key += "S";
			if (neighbours [3] == null || neighbours [3].WALL == Tile.Wall.Empty)
				key += "W";
			if (neighbours [6] == null || neighbours [6].WALL == Tile.Wall.Empty)
				key += "";
		}else if (quadrant == 2) {
			if (neighbours [1] == null || neighbours [1].WALL == Tile.Wall.Empty)
				key += "";
			if (neighbours [2] == null || neighbours [2].WALL == Tile.Wall.Empty)
				key += "";
			if (neighbours [5] == null || neighbours [5].WALL == Tile.Wall.Empty)
				key += "";
		} else if (quadrant == 3) {
			if (neighbours [0] == null || neighbours [0].WALL == Tile.Wall.Empty)
				key += "";
			if (neighbours [3] == null || neighbours [3].WALL == Tile.Wall.Empty)
				key += "";
			if (neighbours [7] == null || neighbours [7].WALL == Tile.Wall.Empty)
				key += "";
		}else if (quadrant == 4) {
			if (neighbours [0] == null || neighbours [0].WALL == Tile.Wall.Empty)
				key += "";
			if (neighbours [1] == null || neighbours [1].WALL == Tile.Wall.Empty)
				key += "";
			if (neighbours [4] == null || neighbours [4].WALL == Tile.Wall.Empty)
				key += "";
		}





		if (uvMap.ContainsKey (key)) {
			return uvMap [key];
		}
		else {
			Debug.LogError ("There is no UV for tile type: " + key);
			return uvMap ["NullSprite"];
		}
	}

	public Vector2[] GetOverlayUVS (Tile tile){
		string key = tile.OVERLAY.ToString ();

		if (uvMap.ContainsKey (key)) {
			return uvMap [key];
		}
		else {
			Debug.LogError ("There is no UV for tile type: " + key);
			return uvMap ["NullSprite"];
		}
	}


}
