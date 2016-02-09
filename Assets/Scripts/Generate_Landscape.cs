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

	// blocks 
	public static GameObject prefab_Grass;// = GameObject.Find("Grass");
	public static GameObject prefab_Sand;// = GameObject.Find("Sand");
	public static GameObject prefab_Snow;// = GameObject.Find("Snow");
	public static GameObject prefab_Dirt;// = GameObject.Find("Dirt");
	public static GameObject prefab_Fern;// = GameObject.Find("Fern");


	public static int chunkWidth = 16;
	public static int chunkDepth = 16;
	public static int chunkHeight = 128;

	private Vector3 startPos;

	private int planeSize = 4;
	private int seed = 0;

	Dictionary<Vector2, Tile> tiles = new Dictionary<Vector2, Tile>();

	// Use this for initialization
	void Start () {
		loadBlockPrefabs ();


		startPos = Vector3.zero;

		this.seed = (int)Network.time * 10;

		float updateTime = Time.realtimeSinceStartup;

		player.transform.position = new Vector3 (0, 30, 0);


		// for debug
		// GameObject chunkClone = (GameObject)Instantiate (chunkPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
		// chunkClone.GetComponent<Generate_Chunk> ().startGeneratingChunk (seed, chunkWidth, chunkDepth, chunkHeight, 0, 0, player);

		for (int x = -planeSize / 2; x < planeSize / 2; x++) {
			for (int z = -planeSize / 2; z < planeSize / 2; z++) {
				GameObject chunkClone = (GameObject)Instantiate (chunkPrefab, new Vector3 (x * chunkWidth, 0, z * chunkDepth), Quaternion.identity);
				chunkClone.GetComponent<Generate_Chunk> ().startGeneratingChunk (seed, chunkWidth, chunkDepth, chunkHeight, x, z, player);

				Tile c = new Tile (chunkClone, updateTime);
				tiles.Add (new Vector2 (x, z), c);
			}
		}
	}

	void loadBlockPrefabs() {
		prefab_Grass = GameObject.Find("Grass");
		prefab_Sand = GameObject.Find("Sand");
		prefab_Snow = GameObject.Find("Snow");
		prefab_Dirt = GameObject.Find("Dirt");
		prefab_Fern = GameObject.Find("Fern");
	}

	void helper(int chunkX, int chunkZ, float updateTime) {
		GameObject chunkClone = (GameObject)Instantiate (chunkPrefab, new Vector3 (chunkX*chunkWidth, 0, chunkZ*chunkDepth), Quaternion.identity);
		chunkClone.GetComponent<Generate_Chunk> ().startGeneratingChunk (seed, chunkWidth, chunkDepth, chunkHeight, chunkX, chunkZ, player);

		Tile c = new Tile (chunkClone, updateTime);
		tiles.Add (new Vector2 (chunkX, chunkZ), c);
	}

	// Update is called once per frame
	void Update () {
		int playerX = (int)(player.transform.position.x / chunkWidth);
		int playerZ = (int)(player.transform.position.z / chunkDepth);

		int xMove = (int)Mathf.Abs(playerX - startPos.x);
		int zMove = (int)Mathf.Abs(playerZ - startPos.z);

		if (xMove > 0 || zMove > 0) {
			// StartCoroutine (updateTerrain ());
			startPos = new Vector3 (playerX, 0, playerZ);

			float updateTime = Time.realtimeSinceStartup;

			for (int x = -planeSize / 2; x < planeSize / 2; x++) {
				for (int z = -planeSize / 2; z < planeSize / 2; z++) {
					Vector2 chunkPos = new Vector2 (x + playerX, z + playerZ);
					if (!tiles.ContainsKey (chunkPos)) {
						// Thread thread = new Thread (()=> helper(x + playerX, z + playerZ, updateTime));
						// thread.Start ();
						helper(x + playerX, z + playerZ, updateTime);
					} else {
						tiles [chunkPos].creationTime = updateTime;
					}

					// yield return new WaitForEndOfFrame();
				}
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

	IEnumerator updateTerrain() {
		int playerX = (int)(player.transform.position.x / chunkWidth);
		int playerZ = (int)(player.transform.position.z / chunkDepth);
		startPos = new Vector3 (playerX, 0, playerZ);

		float updateTime = Time.realtimeSinceStartup;

		for (int x = -planeSize / 2; x < planeSize / 2; x++) {
			for (int z = -planeSize / 2; z < planeSize / 2; z++) {
				Vector2 chunkPos = new Vector2 (x + playerX, z + playerZ);
				if (!tiles.ContainsKey (chunkPos)) {
					GameObject chunkClone = (GameObject)Instantiate (chunkPrefab, new Vector3 ((x+playerX)*chunkWidth, 0, (z+playerZ)*chunkDepth), Quaternion.identity);
					chunkClone.GetComponent<Generate_Chunk> ().startGeneratingChunk (seed, chunkWidth, chunkDepth, chunkHeight, x+playerX, z+playerZ, player);

					Tile c = new Tile (chunkClone, updateTime);
					tiles.Add (new Vector2 (x+playerX, z+playerZ), c);
				} else {
					tiles [chunkPos].creationTime = updateTime;
				}

				// yield return new WaitForEndOfFrame();
			}
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

		yield return new WaitForEndOfFrame();
	}
}
