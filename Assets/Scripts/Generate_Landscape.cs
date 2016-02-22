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

	public static int chunkWidth = 16;
	public static int chunkDepth = 16;
	public static int chunkHeight = 128;

	private Vector3 startPos;

	// private int planeSize = 6;
	private int planeSize = 10; // 2; // 4; /*6;*/ /*20;*/
	private int seed = 0;

	private ArrayList heightMaps;

	public World world;
	private Dictionary<Vector2, Tile> tiles = new Dictionary<Vector2, Tile> (); 

	public int selectedBlock = 1;

	// Use this for initialization
	void Start () {
	}

	public void startGeneratingLandscape(World world) {
		this.world = world;
		// seed == 0 的话生成平面
		this.seed = world.worldData.seed;

		startPos = Vector3.zero; 
		player.transform.position = new Vector3 (world.worldData.playerX, world.worldData.playerY, world.worldData.playerZ);
		player.transform.localEulerAngles = new Vector3 (world.worldData.playerRX, world.worldData.playerRY, world.worldData.playerRZ);

		StartCoroutine (joinThreads (false));
	}

	void addChunkGameObject(Vector2 chunkPos, GameObject chunkObject, float creationTime) {
		Tile tile = new Tile (chunkObject, creationTime);
		tiles.Add(chunkPos, tile);
	}

	void setCreationTime(Vector2 chunkPos, float creationTime) {
		tiles [chunkPos].creationTime = creationTime;
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

		world.worldData.updatePlayerTransformationData (player);

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

					Block block;
					switch (selectedBlock) {
					case 1:
						block = new Dirt ();
						break;
					case 2:
						block = new Sand ();
						break;
					case 3:
						block = new LogJungle ();
						break;
					case 4:
						block = new PlanksJungle ();
						break;
					case 5:
						block = new LogOak ();
						break;
					case 6:
						block = new PlanksOak ();
						break;
					default:
						block = new Dirt ();
						break;
					}

					world.addBlock (newPos, block, true);
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

					Block block = world.getBlock (blockPos);
					if (block != null) { // draw small one 
						DropItem dropItem = (new GameObject ()).AddComponent<DropItem> () as DropItem;
						dropItem.generate3DMesh (block, blockPos);

						world.removeBlock (blockPos, true);
					}
				}
			}
		}

		// TODO: change the code below in the future
		if (Input.GetKeyDown (KeyCode.Alpha1)) { // 点击了数字键盘 数字 1
			selectedBlock = 1;
		} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			selectedBlock = 2;
		} else if (Input.GetKeyDown (KeyCode.Alpha3)) {
			selectedBlock = 3;
		} else if (Input.GetKeyDown (KeyCode.Alpha4)) {
			selectedBlock = 4;
		} else if (Input.GetKeyDown (KeyCode.Alpha5)) {
			selectedBlock = 5;
		} else if (Input.GetKeyDown (KeyCode.Alpha6)) {
			selectedBlock = 6;
		} else if (Input.GetKeyDown (KeyCode.Alpha7)) {
			selectedBlock = 7;
		} else if (Input.GetKeyDown (KeyCode.Alpha8)) {
			selectedBlock = 8;
		} else if (Input.GetKeyDown (KeyCode.Alpha9)) {
			selectedBlock = 9;
		}
	}

	IEnumerator joinThreads(bool asyncFlag = true) {
		int playerX = (int)(player.transform.position.x / chunkWidth);
		int playerZ = (int)(player.transform.position.z / chunkDepth);

		float updateTime = Time.realtimeSinceStartup;

		this.heightMaps = new ArrayList();

		for (int x = -planeSize / 2; x < planeSize / 2; x++) {
			for (int z = -planeSize / 2; z < planeSize / 2; z++) {

				int chunkX = x + playerX;
				int chunkZ = z + playerZ;
				Vector2 chunkPos = new Vector2 (chunkX, chunkZ);

				if (!world.hasChunkAtPosition (chunkPos)) {
					Chunk chunk = Serialization.LoadChunk (chunkPos, world);
					if (chunk != null) { // loaded chunk from disk 
						chunk.changed = false; 
						HeightMap heightMap = new HeightMap (chunkX, chunkZ, chunk);

						// lock (heightMaps) {
						heightMaps.Add (heightMap);

						world.addHeightMap (chunkPos, heightMap);
					} else {
						generateHeightMap(chunkX, chunkZ);
					}
				} else {
					setCreationTime (chunkPos, updateTime);
				}
			}
		}

		for (int i = this.heightMaps.Count - 1; i >= 0; i--) {
			HeightMap heightMap = this.heightMaps [i] as HeightMap;
			int chunkX = heightMap.chunkX;
			int chunkZ = heightMap.chunkZ;

			GameObject chunkClone = (GameObject)Instantiate (chunkPrefab, /*new Vector3 (chunkX*chunkWidth, 0, chunkZ*chunkDepth)*/Vector3.zero, Quaternion.identity);
			Vector2 chunkPos = new Vector2 (chunkX, chunkZ);
			chunkClone.GetComponent<Generate_Chunk> ().startGeneratingChunk (heightMap, world);
			chunkClone.name = "chunk_" + chunkPos;
			addChunkGameObject (chunkPos, chunkClone, updateTime);

			if (asyncFlag)
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

		if (!asyncFlag)
			yield return new WaitForEndOfFrame();
	}
}
