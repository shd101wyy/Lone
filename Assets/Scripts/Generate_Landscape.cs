using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	public static int chunkWidth = 24;
	public static int chunkDepth = 24;
	public static int chunkHeight = 128;

	private Vector3 startPos;

	private int planeSize = 6;
	private int seed = 0;

	Dictionary<Vector2, Tile> tiles = new Dictionary<Vector2, Tile>();

	// Use this for initialization
	void Start () {
		startPos = Vector3.zero;

		this.seed = (int)Network.time * 10;

		float updateTime = Time.realtimeSinceStartup;

		player.transform.position = new Vector3 (0, 30, 0);

		for (int x = -planeSize / 2; x < planeSize / 2; x++) {
			for (int z = -planeSize / 2; z < planeSize / 2; z++) {
				GameObject chunkClone = (GameObject)Instantiate (chunkPrefab, new Vector3 (x * chunkWidth, 0, z * chunkDepth), Quaternion.identity);
				chunkClone.GetComponent<Generate_Chunk> ().startGeneratingChunk (seed, chunkWidth, chunkDepth, chunkHeight, x, z, player);

				Tile c = new Tile (chunkClone, updateTime);
				tiles.Add (new Vector2 (x, z), c);

			}
		}
	}

	// Update is called once per frame
	void Update () {
		int playerX = (int)(player.transform.position.x / chunkWidth);
		int playerZ = (int)(player.transform.position.z / chunkDepth);

		int xMove = (int)(playerX - startPos.x);
		int zMove = (int)(playerZ - startPos.z);

		if (xMove > 0 || zMove > 0) {
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
			startPos = new Vector3 (playerX, 0, playerZ);
		}
	}
}
