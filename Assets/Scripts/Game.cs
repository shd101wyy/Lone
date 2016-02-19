using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using LitJson;
using System.Collections.Generic;

public class SpriteData {
	// public string name; // without .png 
	public Vector2 _00;
	public Vector2 _01;
	public Vector2 _10;
	public Vector2 _11;


	// coordinate in Texture2D
	// bottom left is the (0, 0) origin
	public int textureX; 
	public int textureY;

	public Texture2D texture_2d;

	public SpriteData(int offsetX, int offsetY, int spriteWidth, int spriteHeight, int textureWidth, int textureHeight, Texture2D imageTexture) {

		_00 = new Vector2(offsetX * spriteWidth / (float)Game.textureWidth, 1-(offsetY + 1) * spriteHeight / (float)Game.textureHeight);
		_01 = new Vector2(offsetX * spriteWidth / (float)Game.textureWidth, 1-offsetY * spriteHeight / (float)Game.textureHeight);
		_10 = new Vector2((offsetX + 1) * spriteWidth / (float)Game.textureWidth, 1-(offsetY + 1) * spriteHeight / (float)Game.textureHeight);
		_11 = new Vector2((offsetX + 1) * spriteWidth / (float)Game.textureWidth, 1-offsetY * spriteHeight / (float)Game.textureHeight);

		textureX = offsetX * spriteWidth;
		textureY = textureHeight - (offsetY * spriteHeight + spriteHeight);


		Color[] colors = imageTexture.GetPixels (textureX, textureY, spriteWidth, spriteHeight);
		texture_2d = new Texture2D (spriteWidth, spriteHeight, TextureFormat.RGBA32, false);
		texture_2d.SetPixels (colors);
		texture_2d.Apply ();
		texture_2d.filterMode = FilterMode.Point;
		texture_2d.wrapMode = TextureWrapMode.Clamp;
	}
}

public class Game : MonoBehaviour {
	public GameObject commandBox;
	public GameObject player;

	private InputField input;

	public static int textureWidth;
	public static int textureHeight;
	public static int textureTileSize = 16;
	public static Dictionary<string, SpriteData> textures;

	// load textures made by Texture Packer
	void loadTextures(string jsonPath, string imagePath) {
		string textureJson = File.ReadAllText (Application.streamingAssetsPath + jsonPath);
		JsonData textureJsonData = JsonMapper.ToObject (textureJson);
		textureWidth = (int)textureJsonData ["meta"] ["size"] ["w"];
		textureHeight = (int)textureJsonData ["meta"] ["size"] ["h"];

		Texture2D tex = new Texture2D (2, 2, TextureFormat.ARGB32, false);// Resources.Load(Application.dataPath + "/Textures/PIXIE_1/assets/minecraft/textures/items/iron_sword.png") as Texture2D;
		byte[] data = File.ReadAllBytes(Application.streamingAssetsPath + imagePath);
		tex.LoadImage (data);
		tex.filterMode = FilterMode.Point;
		tex.wrapMode = TextureWrapMode.Clamp;

		for (int i = 0; i < textureJsonData ["frames"].Count; i++) {
			JsonData d = textureJsonData ["frames"] [i];
			string name = (string)d ["filename"];
			name = name.Substring (0, name.IndexOf ("."));
			int x = (int)d["frame"]["x"];
			int y = (int)d["frame"]["y"];

			// these two should be equal to 16.
			int spriteWidth = (int)d ["frame"] ["w"];
			int spriteHeight = (int)d ["frame"] ["h"];

			int offsetX = x / spriteWidth;
			int offsetY = y / spriteHeight;
			textures.Add (name, new SpriteData (offsetX, offsetY, spriteWidth, spriteHeight, textureWidth, textureHeight, tex));
		}
	}

	// Use this for initialization
	void Start () {
		textures = new Dictionary<string, SpriteData> ();

		// load texture data from StreamingAssets folder
		loadTextures("/texture.json", "/texture.png");
		loadTextures ("/items.json", "/items.png");
			
		input = commandBox.GetComponent<InputField> ();
		GameObject.Find ("Landscape").GetComponent<Generate_Landscape> ().startGeneratingLandscape (/*0*/ /*(int)Network.time * 10*/ (int)System.DateTime.Now.Millisecond * 1000);


		GameObject rightHand = GameObject.Find ("RightHandItem");
		generate3DMeshFrom2D (rightHand, textures ["iron_pickaxe"]);

		GameObject leftHand = GameObject.Find ("LeftHandItem");
		generate3DMeshFrom2D (leftHand, textures ["iron_sword"]);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.T)) {
			if (!input.isFocused) {
				input.ActivateInputField ();
				input.Select ();
				input.text = "";
			}
		}
	}

	public void GetCommandBoxInput(string command) {
		Debug.Log ("@: " + command);
		command = command.Trim ();
		if (command == "start") {
			/*
			GameObject landscape = GameObject.Find ("Landscape");
			Destroy (landscape);
			landscape.GetComponent<Generate_Landscape> ().startGeneratingLandscape ((int)Network.time * 10);
			*/
		}

		input.text = "";
	}

	public static void generate3DMeshFrom2D(GameObject gameObject, SpriteData sprite, float depth = 0.0625f) {
		gameObject.AddComponent<ExtrudeSprite> ();
		// gameObject.AddComponent<MeshFilter> ();
		// gameObject.AddComponent<MeshRenderer> ();
		gameObject.GetComponent<ExtrudeSprite> ().GenerateMesh (sprite.texture_2d);
	}
}
