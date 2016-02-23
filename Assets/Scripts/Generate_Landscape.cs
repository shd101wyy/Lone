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

	// private int planeSize = 6;
	// private int planeSize = 6; // 2; // 4; /*6;*/ /*20;*/

	public World world;

	public int selectedBlock = 1;

	public List<Vector2> buildList = new List<Vector2>();
	public List<Chunk> renderList = new List<Chunk>(); // Chunk need to be rendered.
	public Dictionary<Vector2, GameObject> chunkObjects = new Dictionary<Vector2, GameObject>();
	public static int halfPlaneSize = 4;
	int timer = 0;

	static  Vector3[] chunkPositions= {   new Vector3( 0, 0,  0), new Vector3(-1, 0,  0), new Vector3( 0, 0, -1), new Vector3( 0, 0,  1), new Vector3( 1, 0,  0),
		new Vector3(-1, 0, -1), new Vector3(-1, 0,  1), new Vector3( 1, 0, -1), new Vector3( 1, 0,  1), new Vector3(-2, 0,  0),
		new Vector3( 0, 0, -2), new Vector3( 0, 0,  2), new Vector3( 2, 0,  0), new Vector3(-2, 0, -1), new Vector3(-2, 0,  1),
		new Vector3(-1, 0, -2), new Vector3(-1, 0,  2), new Vector3( 1, 0, -2), new Vector3( 1, 0,  2), new Vector3( 2, 0, -1),
		new Vector3( 2, 0,  1), new Vector3(-2, 0, -2), new Vector3(-2, 0,  2), new Vector3( 2, 0, -2), new Vector3( 2, 0,  2),
		new Vector3(-3, 0,  0), new Vector3( 0, 0, -3), new Vector3( 0, 0,  3), new Vector3( 3, 0,  0), new Vector3(-3, 0, -1),
		new Vector3(-3, 0,  1), new Vector3(-1, 0, -3), new Vector3(-1, 0,  3), new Vector3( 1, 0, -3), new Vector3( 1, 0,  3),
		new Vector3( 3, 0, -1), new Vector3( 3, 0,  1), new Vector3(-3, 0, -2), new Vector3(-3, 0,  2), new Vector3(-2, 0, -3),
		new Vector3(-2, 0,  3), new Vector3( 2, 0, -3), new Vector3( 2, 0,  3), new Vector3( 3, 0, -2), new Vector3( 3, 0,  2),
		new Vector3(-4, 0,  0), new Vector3( 0, 0, -4), new Vector3( 0, 0,  4), new Vector3( 4, 0,  0), new Vector3(-4, 0, -1),
		new Vector3(-4, 0,  1), new Vector3(-1, 0, -4), new Vector3(-1, 0,  4), new Vector3( 1, 0, -4), new Vector3( 1, 0,  4),
		new Vector3( 4, 0, -1), new Vector3( 4, 0,  1), new Vector3(-3, 0, -3), new Vector3(-3, 0,  3), new Vector3( 3, 0, -3),
		new Vector3( 3, 0,  3), new Vector3(-4, 0, -2), new Vector3(-4, 0,  2), new Vector3(-2, 0, -4), new Vector3(-2, 0,  4),
		new Vector3( 2, 0, -4), new Vector3( 2, 0,  4), new Vector3( 4, 0, -2), new Vector3( 4, 0,  2), new Vector3(-5, 0,  0),
		new Vector3(-4, 0, -3), new Vector3(-4, 0,  3), new Vector3(-3, 0, -4), new Vector3(-3, 0,  4), new Vector3( 0, 0, -5),
		new Vector3( 0, 0,  5), new Vector3( 3, 0, -4), new Vector3( 3, 0,  4), new Vector3( 4, 0, -3), new Vector3( 4, 0,  3),
		new Vector3( 5, 0,  0), new Vector3(-5, 0, -1), new Vector3(-5, 0,  1), new Vector3(-1, 0, -5), new Vector3(-1, 0,  5),
		new Vector3( 1, 0, -5), new Vector3( 1, 0,  5), new Vector3( 5, 0, -1), new Vector3( 5, 0,  1), new Vector3(-5, 0, -2),
		new Vector3(-5, 0,  2), new Vector3(-2, 0, -5), new Vector3(-2, 0,  5), new Vector3( 2, 0, -5), new Vector3( 2, 0,  5),
		new Vector3( 5, 0, -2), new Vector3( 5, 0,  2), new Vector3(-4, 0, -4), new Vector3(-4, 0,  4), new Vector3( 4, 0, -4),
		new Vector3( 4, 0,  4), new Vector3(-5, 0, -3), new Vector3(-5, 0,  3), new Vector3(-3, 0, -5), new Vector3(-3, 0,  5),
		new Vector3( 3, 0, -5), new Vector3( 3, 0,  5), new Vector3( 5, 0, -3), new Vector3( 5, 0,  3), new Vector3(-6, 0,  0),
		new Vector3( 0, 0, -6), new Vector3( 0, 0,  6), new Vector3( 6, 0,  0), new Vector3(-6, 0, -1), new Vector3(-6, 0,  1),
		new Vector3(-1, 0, -6), new Vector3(-1, 0,  6), new Vector3( 1, 0, -6), new Vector3( 1, 0,  6), new Vector3( 6, 0, -1),
		new Vector3( 6, 0,  1), new Vector3(-6, 0, -2), new Vector3(-6, 0,  2), new Vector3(-2, 0, -6), new Vector3(-2, 0,  6),
		new Vector3( 2, 0, -6), new Vector3( 2, 0,  6), new Vector3( 6, 0, -2), new Vector3( 6, 0,  2), new Vector3(-5, 0, -4),
		new Vector3(-5, 0,  4), new Vector3(-4, 0, -5), new Vector3(-4, 0,  5), new Vector3( 4, 0, -5), new Vector3( 4, 0,  5),
		new Vector3( 5, 0, -4), new Vector3( 5, 0,  4), new Vector3(-6, 0, -3), new Vector3(-6, 0,  3), new Vector3(-3, 0, -6),
		new Vector3(-3, 0,  6), new Vector3( 3, 0, -6), new Vector3( 3, 0,  6), new Vector3( 6, 0, -3), new Vector3( 6, 0,  3),
		new Vector3(-7, 0,  0), new Vector3( 0, 0, -7), new Vector3( 0, 0,  7), new Vector3( 7, 0,  0), new Vector3(-7, 0, -1),
		new Vector3(-7, 0,  1), new Vector3(-5, 0, -5), new Vector3(-5, 0,  5), new Vector3(-1, 0, -7), new Vector3(-1, 0,  7),
		new Vector3( 1, 0, -7), new Vector3( 1, 0,  7), new Vector3( 5, 0, -5), new Vector3( 5, 0,  5), new Vector3( 7, 0, -1),
		new Vector3( 7, 0,  1), new Vector3(-6, 0, -4), new Vector3(-6, 0,  4), new Vector3(-4, 0, -6), new Vector3(-4, 0,  6),
		new Vector3( 4, 0, -6), new Vector3( 4, 0,  6), new Vector3( 6, 0, -4), new Vector3( 6, 0,  4), new Vector3(-7, 0, -2),
		new Vector3(-7, 0,  2), new Vector3(-2, 0, -7), new Vector3(-2, 0,  7), new Vector3( 2, 0, -7), new Vector3( 2, 0,  7),
		new Vector3( 7, 0, -2), new Vector3( 7, 0,  2), new Vector3(-7, 0, -3), new Vector3(-7, 0,  3), new Vector3(-3, 0, -7),
		new Vector3(-3, 0,  7), new Vector3( 3, 0, -7), new Vector3( 3, 0,  7), new Vector3( 7, 0, -3), new Vector3( 7, 0,  3),
		new Vector3(-6, 0, -5), new Vector3(-6, 0,  5), new Vector3(-5, 0, -6), new Vector3(-5, 0,  6), new Vector3( 5, 0, -6),
		new Vector3( 5, 0,  6), new Vector3( 6, 0, -5), new Vector3( 6, 0,  5) };

	// Use this for initialization
	void Start () {
	}

	public void startGeneratingLandscape(World world) {
		this.world = world;

		player.transform.position = new Vector3 (world.worldData.playerX, world.worldData.playerY, world.worldData.playerZ);
		player.transform.localEulerAngles = new Vector3 (world.worldData.playerRX, world.worldData.playerRY, world.worldData.playerRZ);

		// generate 4 blocks around player
		InitiateWorld();
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

	void CheckLeftClick() {
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
	}

	void CheckRightClick() {
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
	}

	void CheckKeyboard() {
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

	// Update is called once per frame
	void Update () {
		world.worldData.updatePlayerTransformationData (player);


		CheckLeftClick ();
		CheckRightClick ();
		CheckKeyboard ();	


		DeleteChunks ();
		FindChunksToLoad ();
		LoadAndRenderChunks ();

	}

	void InitiateWorld() {
		int playerChunkX = (int)(player.transform.position.x / chunkWidth);
		int playerChunkZ = (int)(player.transform.position.z / chunkDepth);
		for (int x = -1; x < 1; x++) {
			for (int z = -1; z < 1; z++) {
				BuildChunk (new Vector2 (playerChunkX + x, playerChunkZ + z));
			}
		}

		LoadAndRenderChunks ();
	}

	void FindChunksToLoad() {
		int playerChunkX = (int)(player.transform.position.x / chunkWidth);
		int playerChunkZ = (int)(player.transform.position.z / chunkDepth);

		if (buildList.Count == 0) {
			for (int i = 0; i < chunkPositions.Length; i++) {
				Vector3 chunkPos = new Vector2 (playerChunkX + chunkPositions[i].x, playerChunkZ + chunkPositions[i].z);
				Chunk newChunk = world.getChunk (chunkPos);

				if (newChunk != null && (newChunk.rendered || renderList.Contains (newChunk)))
					continue;
				else
					buildList.Add (chunkPos); // TODO add something

				return;
			}
		}
	}

	void BuildChunk(Vector2 chunkPos) {
		if (world.hasChunkAtPosition (chunkPos))
			return;
		else {
			Chunk chunk = world.CreateChunk (chunkPos);
			GameObject chunkClone = (GameObject)Instantiate (chunkPrefab, Vector3.zero, Quaternion.identity);
			chunkClone.GetComponent<Generate_Chunk> ().bindChunk (chunk, world);
			chunkClone.name = "chunk_" + chunkPos;
			chunkObjects.Add (chunkPos, chunkClone);

			renderList.Add (chunk);
		}
	}

	void LoadAndRenderChunks() {
		for (int i = 0; i < 4; i++) {  // render 4 chunks per frame
			if (buildList.Count != 0) {
				BuildChunk (buildList [0]);
				buildList.RemoveAt (0);
			}
		}

		for (int i = 0; i < renderList.Count; i++) {
			Chunk chunk = renderList [0];
			if (!chunk.rendered)
				chunk.needRender = true;
			renderList.RemoveAt (0);
		}
	}

	void DeleteChunks() {
		if (timer == 10) {
			var chunksToDelete = new List<Vector2> ();
			foreach (var item in world.chunks) {
				Chunk chunk = item.Value;
				float distance = Vector3.Distance (
					new Vector3(player.transform.position.x, 0, player.transform.position.z),
					new Vector3(chunk.chunkX * chunkWidth, 0, chunk.chunkZ * chunkDepth)
				);

				if (distance > 256) {
					chunksToDelete.Add (item.Key);
				}
			}

			for (int i = 0; i < chunksToDelete.Count; i++) {
				world.RemoveChunk (chunksToDelete [i]);
				Destroy (chunkObjects [chunksToDelete [i]]);
				chunkObjects.Remove (chunksToDelete [i]);
			}

			timer = 0;
		}
		timer++;
	}
}
