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
	public GameObject inventoryBar;
	public GameObject landscape;

	private InputField input;

	public static int textureWidth;
	public static int textureHeight;
	public static int textureTileSize = 16;
	public static Dictionary<string, SpriteData> textures;

	World world;

	// load textures made by Texture Packer
	void loadTextures(string jsonPath, string imagePath) {
		// Attention: Resources.Load couldn't have file extension
		string textureJson = (Resources.Load(jsonPath) as TextAsset).text; // File.ReadAllText (Application.streamingAssetsPath + jsonPath);
		JsonData textureJsonData = JsonMapper.ToObject (textureJson);
		textureWidth = (int)textureJsonData ["meta"] ["size"] ["w"];
		textureHeight = (int)textureJsonData ["meta"] ["size"] ["h"];

		Texture2D tex = Resources.Load(imagePath) as Texture2D;
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
		loadTextures("texture_json", "texture");
		loadTextures ("items_json", "items");

		input = commandBox.GetComponent<InputField> ();

		// TODO: load Town world 
		world = new World("test");
		GameObject.Find ("Landscape").GetComponent<Generate_Landscape> ().startGeneratingLandscape (world);



		// set left hand and right hand
		GameObject rightHand = GameObject.Find ("RightHandItem");
		generate3DMeshFrom2D (rightHand, textures ["iron_pickaxe"]);

		GameObject leftHand = GameObject.Find ("LeftHandItem");
		generate3DMeshFrom2D (leftHand, textures ["iron_sword"]);

		// set inventory bar items
		inventoryBar.GetComponent<InventoryBar>().setItem(textures["dirt"].texture_2d, 0);
		inventoryBar.GetComponent<InventoryBar>().setItem(textures["sand"].texture_2d, 1);
		inventoryBar.GetComponent<InventoryBar>().setItem(textures["log_jungle"].texture_2d, 2);
		inventoryBar.GetComponent<InventoryBar>().setItem(textures["planks_jungle"].texture_2d, 3);
		inventoryBar.GetComponent<InventoryBar>().setItem(textures["log_oak"].texture_2d, 4);
		inventoryBar.GetComponent<InventoryBar>().setItem(textures["planks_oak"].texture_2d, 5);
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
		char[] delimiter = { ' ' };
		string[] commands = command.Split (delimiter, 10);

		if (commands[0] == "save") {
			world.saveWorld ();
		}

		input.text = "";
	}

	public static void generate3DMeshFrom2D(GameObject g, SpriteData sprite, float depth = 0.0625f) {

		g.AddComponent<ExtrudeSprite> ();
		// gameObject.AddComponent<MeshFilter> ();
		// gameObject.AddComponent<MeshRenderer> ();
		g.GetComponent<ExtrudeSprite> ().GenerateMesh (sprite.texture_2d, depth);
	}

	public static void generate3DMeshFrom2DTexture(GameObject g, Texture2D texture_2d, float depth = 0.0625f) {

		g.AddComponent<ExtrudeSprite> ();

		g.GetComponent<ExtrudeSprite> ().GenerateMesh (texture_2d, depth);
	}
}
