using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader : MonoBehaviour {

	public static SpriteLoader instance;

	public static Sprite[] SpriteArray;
	public static Material worldMaterial;

	Dictionary<string, int> spriteMap = new Dictionary<string, int>();
	Dictionary<string, Vector2[]> uvMap;

	void Awake (){
		instance = this;
		uvMap = new Dictionary<string, Vector2[]>();
		SpriteArray = Resources.LoadAll<Sprite> ("Sprites");
		worldMaterial = new Material (Shader.Find ("Sprites/Default"));
		worldMaterial.mainTexture = SpriteArray [0].texture;

		float spriteWidth = 0f;
		float spriteHeight = 0f;

		foreach (Sprite sprite in SpriteArray) {
			if (sprite.rect.x + sprite.rect.width > spriteWidth)
				spriteWidth = sprite.rect.x + sprite.rect.width;

			if (sprite.rect.y + sprite.rect.height > spriteHeight)
				spriteHeight = sprite.rect.y + sprite.rect.height;
		}

		foreach (Sprite sprite in SpriteArray) {
			Vector2[] uvs = new Vector2[4];

			uvs [0] = new Vector2 (sprite.rect.x / spriteWidth, sprite.rect.y / spriteHeight);
			uvs [1] = new Vector2 ((sprite.rect.x + sprite.rect.width) / spriteWidth, sprite.rect.y / spriteHeight);
			uvs [2] = new Vector2 (sprite.rect.x / spriteWidth, (sprite.rect.y + sprite.rect.height) / spriteHeight);
			uvs [3] = new Vector2 ((sprite.rect.x + sprite.rect.width) / spriteWidth, (sprite.rect.y + sprite.rect.height) / spriteHeight);

			uvMap.Add (sprite.name, uvs);
		}






		for (int i = 0; i < SpriteArray.Length; i++) {
			spriteMap.Add(SpriteArray[i].name,i);
		}
	}
		
	public int GetSpriteIndex(string key){
		int index;
		if (spriteMap.TryGetValue (key, out index)) {
			return index;
		} else {
			Debug.LogError ("The dictionary does not have a sprite with key: " + key);
			return 2;
		}
	}

	public Vector2[] GetUVS (Tile tile){
		string key = tile.MAT.ToString ();

		if (uvMap.ContainsKey (key)) {
			return uvMap [key];
		}
		else {
			Debug.LogError ("There is no UV for tile type: " + key);
			return uvMap ["NullSprite"];
		}
	}
}
