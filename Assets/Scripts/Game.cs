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
	PlayerController playerController;

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

		// 下面 initialization 的顺序不能变

		// set inventory bar items
		InventoryBarController inventoryBarController = inventoryBar.GetComponent<InventoryBarController>() as InventoryBarController;
		inventoryBarController.setItem (new Dirt(), 0);
		inventoryBarController.setItem (new Sand(), 1);
		inventoryBarController.setItem (new LogJungle(), 2);
		inventoryBarController.setItem (new PlanksJungle(), 3);
		inventoryBarController.setItem (new LogOak(), 4);
		inventoryBarController.setItem (new PlanksOak(), 5);

		// TODO: load Town world 
		world = new World("test");
		GameObject.Find ("Landscape").GetComponent<Generate_Landscape> ().startGeneratingLandscape (world);

		// initialize player
		playerController = player.GetComponent<PlayerController> () as PlayerController;
		playerController.initializePlayer ();
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

	public static void generate3DMeshFromBlock(GameObject g, Block block) {
		HoldItem holdItem = g.AddComponent<HoldItem> () as HoldItem;
		holdItem.generate3DMesh (block);

	}

	public static void generate3DMeshFrom2D(GameObject g, SpriteData sprite, float depth = 0.0625f) {

		g.AddComponent<ExtrudeSprite> ();
		g.GetComponent<ExtrudeSprite> ().GenerateMesh (sprite.texture_2d, depth);
	}

	public static void generate3DMeshFrom2DTexture(GameObject g, Texture2D texture_2d, float depth = 0.0625f) {

		g.AddComponent<ExtrudeSprite> ();
		g.GetComponent<ExtrudeSprite> ().GenerateMesh (texture_2d, depth);
	}
}
