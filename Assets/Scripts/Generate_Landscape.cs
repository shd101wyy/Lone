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

public class World {
	public Dictionary<Vector2, Tile> tiles;
	public Dictionary<Vector2, HeightMap> heightMaps;

	public World() {
		tiles = new Dictionary<Vector2, Tile> ();
		heightMaps = new Dictionary<Vector2, HeightMap> ();
	}

	public void addHeightMap(Vector2 chunkPos, HeightMap heightMap) {
		heightMaps.Add (chunkPos, heightMap);
	}

	public void addChunkGameObject(Vector2 chunkPos, GameObject chunkObject, float creationTime) {
		Tile tile = new Tile (chunkObject, creationTime);
		tiles.Add(chunkPos, tile);
	}

	// chunkPos like (0, 0), (0, 1), (1, 1)...
	public Chunk getChunk(Vector2 chunkPos) {
		return tiles [chunkPos].chunkObject.GetComponent<Generate_Chunk>().heightMap.chunk;
	}

	public bool hasChunkAtPosition(Vector2 chunkPos) {
		return tiles.ContainsKey (chunkPos);
	}

	public void setCreationTime(Vector2 chunkPos, float creationTime) {
		tiles [chunkPos].creationTime = creationTime;
	}

	// get block
	public Block getBlock(Vector3 blockPos) {
		Vector2 chunkPos = new Vector2 (Mathf.Floor (blockPos.x / Generate_Landscape.chunkWidth),
										Mathf.Floor (blockPos.z / Generate_Landscape.chunkDepth));
		if (!heightMaps.ContainsKey (chunkPos))
			return null;
		Chunk chunk = heightMaps [chunkPos].chunk;
		return chunk.getBlock (blockPos);
	}

	// add/set block
	public void addBlock(Vector3 blockPos, Block block, bool needRender = false) {
		Vector2 chunkPos = new Vector2 (Mathf.Floor (blockPos.x / Generate_Landscape.chunkWidth),
			Mathf.Floor (blockPos.z / Generate_Landscape.chunkDepth));
		
		Chunk chunk = heightMaps [chunkPos].chunk;
		if (chunk.blocks.ContainsKey (blockPos))
			return;
		chunk.addBlock (blockPos, block);
		chunk.needRender = needRender;
	}

	// remove block
	public void removeBlock(Vector3 blockPos, bool needRender = false) {
		Vector2 chunkPos = new Vector2 (Mathf.Floor (blockPos.x / Generate_Landscape.chunkWidth),
			Mathf.Floor (blockPos.z / Generate_Landscape.chunkDepth));
		Chunk chunk = heightMaps [chunkPos].chunk;
		chunk.removeBlock (blockPos);
		chunk.needRender = needRender;

		int chunkWidth = Generate_Landscape.chunkWidth;
		int chunkDepth = Generate_Landscape.chunkDepth;

		//Debug.Log (blockPos);
		//Debug.Log (chunkPos);
		// render left chunk
		if (Mathf.Floor((blockPos.x-1)/chunkWidth) != chunkPos.x  && 
			heightMaps.ContainsKey(chunkPos + new Vector2(-1, 0)) && 
			this.getBlock(blockPos + new Vector3(-1, 0, 0)) != null) {

			//Debug.Log ("left");
			heightMaps [chunkPos + new Vector2 (-1, 0)].chunk.needRender = true;
		
		} else if (Mathf.Floor((blockPos.x+1)/chunkWidth) != chunkPos.x && 
			heightMaps.ContainsKey(chunkPos + new Vector2(1, 0)) && 
			this.getBlock(blockPos + new Vector3(1, 0, 0)) != null) {

			//Debug.Log ("right");
			heightMaps [chunkPos + new Vector2 (1, 0)].chunk.needRender = true;
		}

		if (Mathf.Floor((blockPos.z-1)/chunkDepth) != chunkPos.y && 
			heightMaps.ContainsKey(chunkPos + new Vector2(0, -1)) && 
			this.getBlock(blockPos + new Vector3(0, 0, -1)) != null) {

			//Debug.Log ("front");
			heightMaps [chunkPos + new Vector2 (0, -1)].chunk.needRender = true;
		
		} else if (Mathf.Floor((blockPos.z+1)/chunkDepth) != chunkPos.y && 
			heightMaps.ContainsKey(chunkPos + new Vector2(0, 1)) && 
			this.getBlock(blockPos + new Vector3(0, 0, 1)) != null) {

			//Debug.Log ("back");
			heightMaps [chunkPos + new Vector2 (0, 1)].chunk.needRender = true;
		}
	}
}

public class Generate_Landscape : MonoBehaviour {
	public GameObject player;
	public GameObject chunkPrefab;

	public static GameObject commandBox;


	public static int chunkWidth = 16;
	public static int chunkDepth = 16;
	public static int chunkHeight = 128;

	private Vector3 startPos;

	// private int planeSize = 6;
	private int planeSize = 6;// 20;
	private int seed = 0;

	private ArrayList heightMaps;

	private World world = new World();

	// Use this for initialization
	void Start () {
	}

	public void startGeneratingLandscape(int seed) {
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

		this.heightMaps = new ArrayList();

		for (int x = -planeSize / 2; x < planeSize / 2; x++) {
			for (int z = -planeSize / 2; z < planeSize / 2; z++) {
				int chunkX = x + playerX;
				int chunkZ = z + playerZ;
				generateHeightMap(chunkX, chunkZ);
			}
		}

		for (int i = 0; i < this.heightMaps.Count; i++) {
			HeightMap heightMap = this.heightMaps [i] as HeightMap;
			int chunkX = heightMap.chunkX;
			int chunkZ = heightMap.chunkZ;

			GameObject chunkClone = (GameObject)Instantiate (chunkPrefab, /*new Vector3 (chunkX*chunkWidth, 0, chunkZ*chunkDepth)*/Vector3.zero, Quaternion.identity);
			Vector2 chunkPos = new Vector2 (chunkX, chunkZ);
			chunkClone.GetComponent<Generate_Chunk> ().startGeneratingChunk (heightMap, player);
			chunkClone.name = "chunk_" + chunkPos;
			world.addChunkGameObject (chunkPos, chunkClone, updateTime);
		}

	}

	void loadBlockPrefabs() {
		commandBox = GameObject.Find ("CommandBox");
	}

	void generateHeightMap(int chunkX, int chunkZ) {
		HeightMap heightMap = new HeightMap (chunkX, chunkZ, seed, world);
		heightMap.generateHeightMap ();

		// lock (heightMaps) {
		heightMaps.Add (heightMap);

		world.addHeightMap (new Vector2 (chunkX, chunkZ), heightMap);
		// }
	}

	Vector3 getHitObjectPos(Vector3 hit1, Vector3 hit2, Vector3 hit3, Vector3 normal) {
		float x, y, z;
		if (normal == new Vector3 (0, 0, 1)) {
			z = hit1.z - 0.5f;
			x = Mathf.Min (hit1.x, Mathf.Min(hit2.x, hit3.x)) + 0.5f;
			y = Mathf.Min (hit1.y, Mathf.Min(hit2.y, hit3.y)) + 0.5f;
		} else if (normal == new Vector3 (0, 0, -1)) {
			z = hit1.z + 0.5f;
			x = Mathf.Min (hit1.x, Mathf.Min(hit2.x, hit3.x)) + 0.5f;
			y = Mathf.Min (hit1.y, Mathf.Min(hit2.y, hit3.y)) + 0.5f;
		} else if (normal == new Vector3 (1, 0, 0)) {
			x = hit1.x - 0.5f;
			y = Mathf.Min (hit1.y, Mathf.Min(hit2.y, hit3.y)) + 0.5f;
			z = Mathf.Min (hit1.z, Mathf.Min(hit2.z, hit3.z)) + 0.5f;
		} else if (normal == new Vector3 (-1, 0, 0)) {
			x = hit1.x + 0.5f;
			y = Mathf.Min (hit1.y, Mathf.Min(hit2.y, hit3.y)) + 0.5f;
			z = Mathf.Min (hit1.z, Mathf.Min(hit2.z, hit3.z)) + 0.5f;
		} else if (normal == new Vector3 (0, 1, 0)) {
			y = hit1.y - 0.5f;
			x = Mathf.Min (hit1.x, Mathf.Min(hit2.x, hit3.x)) + 0.5f;
			z = Mathf.Min (hit1.z, Mathf.Min(hit2.z, hit3.z)) + 0.5f;
		} else/* if (normal == new Vector3 (0, -1, 0))*/ {
			y = hit1.y + 0.5f;
			x = Mathf.Min (hit1.x, Mathf.Min(hit2.x, hit3.x)) + 0.5f;
			z = Mathf.Min (hit1.z, Mathf.Min(hit2.z, hit3.z)) + 0.5f;
		}

		return new Vector3 (x, y, z);
	}

	// Update is called once per frame
	void Update () {
		int playerX = (int)(player.transform.position.x / chunkWidth);
		int playerZ = (int)(player.transform.position.z / chunkDepth);

		int xMove = (int)Mathf.Abs(playerX - startPos.x);
		int zMove = (int)Mathf.Abs(playerZ - startPos.z);

		int cmp = Mathf.Max(0, planeSize / 4);

		if (xMove > cmp || zMove > cmp) {
			// StartCoroutine (updateTerrain ());
			startPos = new Vector3 (playerX, 0, playerZ);

			StartCoroutine (joinThreads ());
		}

		// right click
		if (Input.GetMouseButtonDown (1)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2.0f, Screen.height / 2.0f, 0));

			if (Physics.Raycast (ray, out hit, 6.0f)) {

				MeshCollider collider = hit.collider as MeshCollider;
				if (collider != null) {
					Mesh mesh = collider.sharedMesh;

					// Debug.DrawRay(player.transform.position, hit.point - player.transform.position);
					int index = hit.triangleIndex * 3;

					Vector3 hit1 = mesh.vertices[mesh.triangles[index    ]];
					Vector3 hit2 = mesh.vertices[mesh.triangles[index + 1]];
					Vector3 hit3 = mesh.vertices[mesh.triangles[index + 2]];

					Vector3 blockPos =  getHitObjectPos(hit1, hit2, hit3, hit.normal);
					Vector3 newPos = blockPos + hit.normal;

					if ((player.transform.position.x < newPos.x + 0.5f && player.transform.position.x > newPos.x - 0.5f) &&
						(player.transform.position.y < newPos.y + 0.5f && player.transform.position.y > newPos.y - 0.5f) &&
						(player.transform.position.z < newPos.z + 0.5f && player.transform.position.z > newPos.z - 0.5f)) {
						// Debug.Log ("Inside");
						return;
					}
						
					world.addBlock (newPos, new Water (world, newPos), true);
				}

			}
		}

		// left click
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2.0f, Screen.height / 2.0f, 0));

			if (Physics.Raycast (ray, out hit, 6.0f)) {

				MeshCollider collider = hit.collider as MeshCollider;
				if (collider != null) {
					Mesh mesh = collider.sharedMesh;

					// Debug.DrawRay(player.transform.position, hit.point - player.transform.position);
					int index = hit.triangleIndex * 3;

					Vector3 hit1 = mesh.vertices[mesh.triangles[index    ]];
					Vector3 hit2 = mesh.vertices[mesh.triangles[index + 1]];
					Vector3 hit3 = mesh.vertices[mesh.triangles[index + 2]];

					Vector3 blockPos =  getHitObjectPos(hit1, hit2, hit3, hit.normal);
					if (blockPos.y == 0)
						return;
					
					world.removeBlock (blockPos, true);
				}
			}
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
				if (!world.hasChunkAtPosition (chunkPos)) {
					int chunkX = x + playerX;
					int chunkZ = z + playerZ;
					// chunkPosList.Add(new Vector2(chunkX, chunkZ));
					generateHeightMap(chunkX, chunkZ);
				} else {
					world.setCreationTime (chunkPos, updateTime);
				}
			}
		}

		for (int i = 0; i < this.heightMaps.Count; i++) {
			HeightMap heightMap = this.heightMaps [i] as HeightMap;
			int chunkX = heightMap.chunkX;
			int chunkZ = heightMap.chunkZ;

			GameObject chunkClone = (GameObject)Instantiate (chunkPrefab, /*new Vector3 (chunkX*chunkWidth, 0, chunkZ*chunkDepth)*/Vector3.zero, Quaternion.identity);
			Vector2 chunkPos = new Vector2 (chunkX, chunkZ);
			chunkClone.GetComponent<Generate_Chunk> ().startGeneratingChunk (heightMap, player);
			chunkClone.name = "chunk_" + chunkPos;
			world.addChunkGameObject (chunkPos, chunkClone, updateTime);

			yield return new WaitForEndOfFrame();
		}
			
		Dictionary<Vector2, Tile> newTiles = new Dictionary<Vector2, Tile>();

		foreach (KeyValuePair<Vector2, Tile> entry in world.tiles) {
			if (entry.Value.creationTime != updateTime) { // remove old terrain. TODO: save that generated terrain.
				Destroy (entry.Value.chunkObject);
			} else {
				newTiles.Add (entry.Key, entry.Value);
			}
		}
		world.tiles = newTiles;
	}
}
