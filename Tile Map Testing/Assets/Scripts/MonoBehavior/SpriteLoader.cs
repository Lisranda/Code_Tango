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

	public Vector2[] GetWallUVS (Tile tile){
		string key = tile.WALL.ToString ();

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
