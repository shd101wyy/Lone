using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Tile {
	public float creationTime;
	public GameObject chunkObject;

	public Tile(GameObject chunkObject, float creationTime) {
		this.chunkObject = chunkObject;
		this.creationTime = creationTime;
	}
}

public class Generate_Landscape : MonoBehaviour {
	public GameObject player;
	public GameObject chunkPrefab;
	public GameObject loadingText;


	// blocks 
	public static GameObject prefab_Grass;// = GameObject.Find("Grass");
	public static GameObject prefab_Sand;// = GameObject.Find("Sand");
	public static GameObject prefab_Snow;// = GameObject.Find("Snow");
	public static GameObject prefab_Dirt;// = GameObject.Find("Dirt");
	public static GameObject prefab_Fern;// = GameObject.Find("Fern");
	public static GameObject prefab_Rose;

	public static GameObject commandBox;


	public static int chunkWidth = 16;
	public static int chunkDepth = 16;
	public static int chunkHeight = 128;

	private Vector3 startPos;

	// private int planeSize = 6;
	private int planeSize = 16;
	private int seed = 0;

	private ArrayList heightMaps;

	Dictionary<Vector2, Tile> tiles = new Dictionary<Vector2, Tile>();

	// Use this for initialization
	void Start () {
	}

	public void startGeneratingLandscape(int seed) {
		this.loadingText.SetActive (true);
		loadBlockPrefabs ();


		startPos = Vector3.zero;

		// seed == 0 的话生成平面
		this.seed = seed;

		float updateTime = Time.realtimeSinceStartup;

		player.transform.position = new Vector3 (0, 30, 0);

		int playerX = (int)(player.transform.position.x / chunkWidth);
		int playerZ = (int)(player.transform.position.z / chunkDepth);

		// for debug
		// GameObject chunkClone = (GameObject)Instantiate (chunkPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
		// chunkClone.GetComponent<Generate_Chunk> ().startGeneratingChunk (seed, chunkWidth, chunkDepth, chunkHeight, 0, 0, player);

		for (int x = -planeSize / 2; x < planeSize / 2; x++) {
			for (int z = -planeSize / 2; z < planeSize / 2; z++) {
				int chunkX = x + playerX;
				int chunkZ = z + playerZ;

				HeightMap heightMap = new HeightMap (chunkX, chunkZ, chunkWidth, chunkDepth, seed);
				heightMap.generateHeightMap ();

				GameObject chunkClone = (GameObject)Instantiate (chunkPrefab, /*new Vector3 (chunkX * chunkWidth, 0, chunkZ * chunkDepth)*/ Vector3.zero, Quaternion.identity);
				chunkClone.GetComponent<Generate_Chunk> ().startGeneratingChunk (heightMap, player);
				chunkClone.name = "chunk_" + new Vector3 (chunkX, 0, chunkZ );

				Tile c = new Tile (chunkClone, updateTime);
				tiles.Add (new Vector2 (x, z), c);
			}
		}

		this.loadingText.SetActive (false);
	}

	void loadBlockPrefabs() {
		prefab_Grass = GameObject.Find("Grass");
		prefab_Sand = GameObject.Find("Sand");
		prefab_Snow = GameObject.Find("Snow");
		prefab_Dirt = GameObject.Find("Dirt");
		prefab_Fern = GameObject.Find("Fern");
		prefab_Rose = GameObject.Find ("Rose");

		commandBox = GameObject.Find ("CommandBox");
	}

	void generateHeightMap(int chunkX, int chunkZ) {
		HeightMap heightMap = new HeightMap (chunkX, chunkZ, chunkWidth, chunkDepth, seed);
		heightMap.generateHeightMap ();

		// lock (heightMaps) {
		heightMaps.Add (heightMap);
		// }
	}

	// Update is called once per frame
	void Update () {
		int playerX = (int)(player.transform.position.x / chunkWidth);
		int playerZ = (int)(player.transform.position.z / chunkDepth);

		int xMove = (int)Mathf.Abs(playerX - startPos.x);
		int zMove = (int)Mathf.Abs(playerZ - startPos.z);

		int cmp = Mathf.Max(0, planeSize / 4);

		if (xMove > cmp || zMove > cmp) {
			Debug.Log ("render");
			// StartCoroutine (updateTerrain ());
			startPos = new Vector3 (playerX, 0, playerZ);

			StartCoroutine (joinThreads ());
		}
	}

	IEnumerator joinThreads() {
		int playerX = (int)(player.transform.position.x / chunkWidth);
		int playerZ = (int)(player.transform.position.z / chunkDepth);

		float updateTime = Time.realtimeSinceStartup;

		this.heightMaps = new ArrayList();

		for (int x = -planeSize / 2; x < planeSize / 2; x++) {
			for (int z = -planeSize / 2; z < planeSize / 2; z++) {
				Vector2 chunkPos = new Vector2 (x + playerX, z + playerZ);
				if (!tiles.ContainsKey (chunkPos)) {
					int chunkX = x + playerX;
					int chunkZ = z + playerZ;
					// chunkPosList.Add(new Vector2(chunkX, chunkZ));
					generateHeightMap(chunkX, chunkZ);
				} else {
					tiles [chunkPos].creationTime = updateTime;
				}
			}
		}

		for (int i = 0; i < this.heightMaps.Count; i++) {
			HeightMap heightMap = this.heightMaps [i] as HeightMap;
			int chunkX = heightMap.chunkX;
			int chunkZ = heightMap.chunkZ;

			GameObject chunkClone = (GameObject)Instantiate (chunkPrefab, /*new Vector3 (chunkX*chunkWidth, 0, chunkZ*chunkDepth)*/Vector3.zero, Quaternion.identity);
			chunkClone.GetComponent<Generate_Chunk> ().startGeneratingChunk (heightMap, player);
			chunkClone.name = "chunk_" + new Vector3 (chunkX, 0, chunkZ);

			Tile c = new Tile (chunkClone, updateTime);
			tiles.Add (new Vector2 (chunkX, chunkZ), c);

			yield return new WaitForEndOfFrame();
		}

		Dictionary<Vector2, Tile> newTiles = new Dictionary<Vector2, Tile>();

		foreach (KeyValuePair<Vector2, Tile> entry in tiles) {
			if (entry.Value.creationTime != updateTime) { // remove old terrain. TODO: save that generated terrain.
				Destroy (entry.Value.chunkObject);
			} else {
				newTiles.Add (entry.Key, entry.Value);
			}
		}
		tiles = newTiles;
	}
}
