using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


public enum Type {GRASS, SAND, SNOW, DIRT, PLANT_FERN};

public abstract class Block {
	public Type type;
	public bool visible;
	public Vector3 pos;

	public GameObject meshObject;

	protected GameObject clone;

	public Block(Type type, bool visible, Vector3 pos) {
		this.type = type;
		this.visible = visible;
		this.pos = pos;
	}

	public abstract void setClone (GameObject clone);

	public abstract void disableInvisibleFaces (Dictionary<Vector3, Block> chunk);
}

public class CubeBlock: Block {
	protected GameObject front;
	protected GameObject back;
	protected GameObject left;
	protected GameObject right;
	protected GameObject top;
	protected GameObject bottom;

	public CubeBlock(Type type, bool visible, Vector3 pos) : base(type, visible, pos) {
		front = null;
		back = null;
		left = null;
		right = null;
		top = null;
		bottom = null;
	}

	public override void setClone(GameObject clone) {
		this.clone = clone;

		GameObject top = (GameObject)clone.transform.FindChild ("Top").gameObject;
		GameObject bottom = (GameObject)clone.transform.FindChild ("Bottom").gameObject;
		GameObject left = (GameObject)clone.transform.FindChild ("Left").gameObject;
		GameObject right = (GameObject)clone.transform.FindChild ("Right").gameObject;
		GameObject front = (GameObject)clone.transform.FindChild ("Front").gameObject;
		GameObject back = (GameObject)clone.transform.FindChild ("Back").gameObject;

		addFaces (front, back, left, right, top, bottom);
	}

	public void addFaces( GameObject front, 
		GameObject back,
		GameObject left,
		GameObject right,
		GameObject top,
		GameObject bottom) {

		this.front = front;
		this.back = back;
		this.left = left;
		this.right = right;
		this.top = top;
		this.bottom = bottom;
	}

	public override void disableInvisibleFaces(Dictionary<Vector3, Block> chunk) {
		front.SetActive (true);
		back.SetActive (true);
		left.SetActive (true);
		right.SetActive (true);
		top.SetActive (true);
		bottom.SetActive (true);


		Vector3 frontDir = new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z - 1);
		Vector3 backDir = new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z + 1);
		Vector3 leftDir = new Vector3 ((int)pos.x - 1, (int)pos.y, (int)pos.z);
		Vector3 rightDir = new Vector3 ((int)pos.x + 1, (int)pos.y, (int)pos.z);
		Vector3 bottomDir = new Vector3 ((int)pos.x, (int)pos.y - 1, (int)pos.z);

		if (chunk.ContainsKey (frontDir) &&
			chunk[frontDir] is CubeBlock) {
			front.SetActive(false);
		}
		if (chunk.ContainsKey (backDir)  &&
			chunk[backDir] is CubeBlock) {
			back.SetActive(false);
		}
		if (chunk.ContainsKey (rightDir) && 
			chunk[rightDir] is CubeBlock) {
			right.SetActive(false);
		}
		if (chunk.ContainsKey (leftDir) &&
			chunk[leftDir] is CubeBlock) {
			left.SetActive(false);
		}
		if (chunk.ContainsKey (bottomDir) && 
			chunk[bottomDir] is CubeBlock) {
			bottom.SetActive(false);
		}
	}
}

public class Fern: Block {
	public Fern(bool visible, Vector3 pos) : base(Type.PLANT_FERN, visible, pos) {
		// this.meshObject = GameObject.Find("Fern");
		this.meshObject = Generate_Landscape.prefab_Fern;
	}

	public override void setClone(GameObject clone) {
		this.clone = clone;
	}

	public override void disableInvisibleFaces (Dictionary<Vector3, Block> chunk) {
		// nothing happened
	}
}

public class Grass: CubeBlock {
	public Grass(bool visible, Vector3 pos) : base(Type.GRASS, visible, pos) {
		this.meshObject = Generate_Landscape.prefab_Grass;
	}
}

public class Dirt: CubeBlock {
	public Dirt(bool visible, Vector3 pos) : base(Type.DIRT, visible, pos) {
		this.meshObject = Generate_Landscape.prefab_Dirt;
	}
}

public class Snow: CubeBlock {
	public Snow(bool visible, Vector3 pos) : base(Type.SNOW, visible, pos) {
		this.meshObject = Generate_Landscape.prefab_Snow;
	}
}

public class Sand: CubeBlock {
	public Sand(bool visible, Vector3 pos) : base(Type.SAND, visible, pos) {
		this.meshObject = Generate_Landscape.prefab_Sand;
	}
}

public class Generate_Chunk : MonoBehaviour {
	private int width;
	private int depth;
	// private int height;
	private int chunkX;
	private int chunkZ;

	public int heightScale = 20;
	public float detailScale = 25.0f;

	Dictionary<Vector3, Block> worldBlocks;
	ArrayList visibleBlocks;

	private GameObject player;
	private int seed;


	// Use this for initialization
	void Start () {
	}

	void generateHeightMap() {
		int startX = width * chunkX;
		int startZ = depth * chunkZ;

		for (int z = 0; z < depth; z++) {
			for (int x = 0; x < width; x++) {
				int y = (int)(Mathf.PerlinNoise ((x+seed+startX)/detailScale, (z+seed+startZ)/detailScale) * heightScale);
				Vector3 blockPos = new Vector3 (x+startX, y, z+startZ);

				visibleBlocks.Add(blockPos);
				createBlock (blockPos, true);
				while (y > 0) {
					y--; 
					blockPos = new Vector3 (x+startX, y, z+startZ);
					createBlock (blockPos, false);
				}
			}
		}
	}

	public void startGeneratingChunk(int seed, int chunkWidth, int chunkDepth, int chunkHeight, int chunkX, int chunkZ,
		GameObject player) {
		this.width = chunkWidth;
		this.depth = chunkDepth;
		// this.height = chunkHeight;
		this.chunkX = chunkX;
		this.chunkZ = chunkZ;
		this.player = player;
		this.seed = seed;

		// int seed = (int)Network.time * 10;
		worldBlocks = new Dictionary<Vector3, Block>();
		visibleBlocks = new ArrayList ();


		/*
		int startX = width * this.chunkX;
		int startZ = depth * this.chunkZ;

		for (int z = 0; z < depth; z++) {
			for (int x = 0; x < width; x++) {
				int y = (int)(Mathf.PerlinNoise ((x+seed+startX)/detailScale, (z+seed+startZ)/detailScale) * heightScale);
				Vector3 blockPos = new Vector3 (x+startX, y, z+startZ);

				visibleBlocks.Add(blockPos);
				createBlock (blockPos, true);
				while (y > 0) {
					y--; 
					blockPos = new Vector3 (x+startX, y, z+startZ);
					createBlock (blockPos, false);
				}
			}
		}*/

		Thread thread = new Thread (() => generateHeightMap ());
		thread.Start ();
		thread.Join ();
		// generateHeightMap ();

		foreach (Vector3 blockPos in visibleBlocks) {
			Block block = worldBlocks [blockPos];
			block.visible = true;
			drawBlock (block.type, blockPos);


			// check blocks below

			int y = (int)blockPos.y - 1;
			while (y > 0) {
				Vector3 pos = new Vector3 (blockPos.x, y, blockPos.z);
				block = worldBlocks [pos];

				// 下面这个满段豫剧很关键，要不然会重复创建。
				if (block.visible)
					break;

				int[,] delta = {{1, 0, 0}, {-1, 0, 0}, {0, -1, 0}, {0, 0, 1}, {0, 0, -1}};
				bool needToBeVisible = false;
				for (int i = 0; i < 5; i++) {
					int deltaX = delta [i, 0];
					int deltaY = delta [i, 1];
					int deltaZ = delta [i, 2];

					Vector3 p = new Vector3 (pos.x + deltaX, pos.y + deltaY, pos.z + deltaZ);
					if (!worldBlocks.ContainsKey (p) || !(worldBlocks[p] is CubeBlock)) {
						needToBeVisible = true;
						break;
					}
				}
				if (needToBeVisible) {
					block.visible = true;
					drawBlock (block.type, pos);
					y--;
				} else {
					break;
				}
			}
		}
	}

	void createBlock(Vector3 blockPos, bool isTop) {
		int y = (int)blockPos.y;
		Block block;
		if (y > 15) {
			block = new Snow (false, blockPos);
		} else if (isTop && y > 5) {
			block = new Grass (false, blockPos);
			System.Random rnd = new System.Random ();
			if (rnd.Next(0, 10) <= 2) { // FERN
				Vector3 pos = blockPos + new Vector3 (0, 1, 0);
				Block fern = new Fern(true, pos);
				worldBlocks.Add(pos, fern);
				visibleBlocks.Add (pos);
			}
		} else if (y > 5) {
			block = new Dirt (false, blockPos);
		} else {
			block = new Sand (false, blockPos);
		}

		worldBlocks.Add(blockPos, block);
	}

	void drawBlock(Type type, Vector3 blockPos) {
		Block block = worldBlocks [blockPos];
		GameObject clone = (GameObject)Instantiate (block.meshObject, block.pos, Quaternion.identity);
		clone.transform.parent = this.transform;
		block.setClone (clone);
		clone.name = blockPos.ToString ();

		block.disableInvisibleFaces (worldBlocks);

	}

	void drawInvisibleBlock(Vector3 blockPos) {
		if (worldBlocks.ContainsKey (blockPos) == false)
			return;


		Block block = worldBlocks[blockPos];
		if (block.visible) {
			block.disableInvisibleFaces (worldBlocks);
			return;
		}
		block.visible = true; 
		drawBlock (block.type, blockPos);
	}

	void drawAroundInvisibleBlocks(Vector3 blockPos) {
		int[,] delta = {{1, 0, 0}, {-1, 0, 0}, {0, -1, 0}, {0, 1, 0}, {0, 0, 1}, {0, 0, -1}};

		for (int i = 0; i < 6; i++) {
			Vector3 neighbour = new Vector3 ((int)blockPos.x + delta[i, 0], (int)blockPos.y + delta[i, 1], (int)blockPos.z + delta[i, 2]);
			drawInvisibleBlock (neighbour);
		}
		/*
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				for(int z = -1; z <= 1; z++) {
					if (!(x == 0 && y == 0 && z == 0)) {
						Vector3 neighbour = new Vector3 ((int)blockPos.x + x, (int)blockPos.y + y, (int)blockPos.z + z);
						drawInvisibleBlock (neighbour);
					}
				}
			}
		}
		*/
	}

	// Update is called once per frame
	void Update () {

		// left click
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2.0f, Screen.height / 2.0f, 0));

			if (Physics.Raycast (ray, out hit, 10.0f)) {

				GameObject block = hit.transform.parent.gameObject;
				if (worldBlocks.ContainsKey (block.transform.position) == false) {
					return;
				}
				
				Vector3 blockPos = block.transform.position;
				
				// this is the bottom block, don't delete it 
				if ((int)blockPos.y == 0) return;

				worldBlocks.Remove(blockPos);
				Destroy (block);

				drawAroundInvisibleBlocks (blockPos);
			}
		}

		// right click
		if (Input.GetMouseButtonDown (1)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2.0f, Screen.height / 2.0f, 0));

			if (Physics.Raycast (ray, out hit, 10.0f)) {

				GameObject block = hit.transform.parent.gameObject;
				if (!worldBlocks.ContainsKey (block.transform.position) || !(worldBlocks[block.transform.position] is CubeBlock))
					return;
				
				string faceName = hit.transform.gameObject.name;
				Vector3 delta = new Vector3(0, 0, 0);

				switch (faceName) {
				case "Top": 
					delta = new Vector3 (0, 1, 0);
					break;
				case "Bottom": 
					delta = new Vector3 (0, -1, 0);
					break;
				case "Left": 
					delta = new Vector3 (-1, 0, 0);
					break;
				case "Right": 
					delta = new Vector3 (+1, 0, 0);
					break;
				case "Front": 
					delta = new Vector3 (0, 0, -1);
					break;
				case "Back": 
					delta = new Vector3 (0, 0, +1);
					break;
				}

				Vector3 newPos = hit.transform.parent.gameObject.transform.position + delta;
				if ((player.transform.position.x < newPos.x + 0.5f && player.transform.position.x > newPos.x - 0.5f) &&
				    (player.transform.position.y < newPos.y + 0.5f && player.transform.position.y > newPos.y - 0.5f) &&
				    (player.transform.position.z < newPos.z + 0.5f && player.transform.position.z > newPos.z - 0.5f)) {
					// Debug.Log ("Inside");
					return;
				}
		
				Block newBlock = new Dirt (true, newPos);
				worldBlocks [newPos] = newBlock;
				newBlock.visible = true;
				drawBlock (newBlock.type, newPos);

				drawAroundInvisibleBlocks (newPos);

			}
		}
	}
}