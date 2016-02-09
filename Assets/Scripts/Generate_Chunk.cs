using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum Type {GRASS, SAND, SNOW, DIRT};

public class Block {
	public Type type;
	public bool visible;
	public Vector3 pos;

	public GameObject meshObject;

	protected GameObject front;
	protected GameObject back;
	protected GameObject left;
	protected GameObject right;
	protected GameObject top;
	protected GameObject bottom;

	private GameObject clone;

	public Block(Type type, bool visible, Vector3 pos) {
		this.type = type;
		this.visible = visible;
		this.pos = pos;


		front = null;
		back = null;
		left = null;
		right = null;
		top = null;
		bottom = null;
	}

	public void setClone(GameObject clone) {
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

	public void disableInvisibleFaces(Dictionary<Vector3, Block> chunk) {
		front.SetActive (true);
		back.SetActive (true);
		left.SetActive (true);
		right.SetActive (true);
		top.SetActive (true);
		bottom.SetActive (true);


		if (chunk.ContainsKey (new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z - 1))) {
			front.SetActive(false);
		}
		if (chunk.ContainsKey (new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z + 1))) {
			back.SetActive(false);
		}
		if (chunk.ContainsKey (new Vector3 ((int)pos.x + 1, (int)pos.y, (int)pos.z))) {
			// right.GetComponent<Renderer> ().enabled = false;
			// Destroy(right);
			right.SetActive(false);
		}
		if (chunk.ContainsKey (new Vector3 ((int)pos.x - 1, (int)pos.y, (int)pos.z))) {
			// left.GetComponent<Renderer> ().enabled = false;
			// Destroy(left);
			left.SetActive(false);
		}
		if (chunk.ContainsKey (new Vector3 ((int)pos.x, (int)pos.y - 1, (int)pos.z))) {
			// bottom.GetComponent<Renderer> ().enabled = false;
			// Destroy(bottom);
			bottom.SetActive(false);
		}
	}
}

public class Grass: Block {
	public Grass(bool visible, Vector3 pos) : base(Type.GRASS, visible, pos) {
		this.meshObject = GameObject.Find("Grass");
	}
}

public class Dirt: Block {
	public Dirt(bool visible, Vector3 pos) : base(Type.DIRT, visible, pos) {
		this.meshObject = GameObject.Find("Dirt");
	}
}

public class Snow: Block {
	public Snow(bool visible, Vector3 pos) : base(Type.SNOW, visible, pos) {
		this.meshObject = GameObject.Find("Snow");
	}
}

public class Sand: Block {
	public Sand(bool visible, Vector3 pos) : base(Type.SAND, visible, pos) {
		this.meshObject = GameObject.Find("Sand");
	}
}

public class Generate_Chunk : MonoBehaviour {
	private int width;
	private int depth;
	private int height;
	private int chunkX;
	private int chunkZ;

	public int heightScale = 20;
	public float detailScale = 25.0f;

	Dictionary<Vector3, Block> worldBlocks = new Dictionary<Vector3, Block>();

	private GameObject player;


	// Use this for initialization
	void Start () {
	}

	public void startGeneratingChunk(int seed, int chunkWidth, int chunkDepth, int chunkHeight, int chunkX, int chunkZ,
		GameObject player) {
		this.width = chunkWidth;
		this.depth = chunkDepth;
		this.height = chunkHeight;
		this.chunkX = chunkX;
		this.chunkZ = chunkZ;
		this.player = player;

		// int seed = (int)Network.time * 10;
		ArrayList visibleBlocks = new ArrayList ();

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

		foreach (Vector3 blockPos in visibleBlocks) {
			Block block = worldBlocks [blockPos];
			block.visible = true;
			drawBlock (block.type, blockPos);
		}
	}

	void createBlock(Vector3 blockPos, bool isTop) {
		int y = (int)blockPos.y;
		Block block;
		if (y > 15) {
			block = new Snow (false, blockPos);
		} else if (isTop && y > 5) {
			block = new Grass (false, blockPos);
		} else if (y > 5) {
			block = new Dirt (false, blockPos);
		} else {
			block = new Sand (false, blockPos);
		}

		worldBlocks[blockPos] = block;
	}

	void drawBlock(Type type, Vector3 blockPos) {
		Block block = worldBlocks [blockPos];
		GameObject clone = (GameObject)Instantiate (block.meshObject, block.pos, Quaternion.identity);
		block.setClone (clone);
		clone.name = blockPos.ToString ();

		block.disableInvisibleFaces (worldBlocks);

	}

	void drawInvisibleBlock(Vector3 blockPos) {
		if (worldBlocks.ContainsKey (blockPos) == false)
			return;


		Block block = worldBlocks[blockPos];
		Type type = block.type;
		if (block.visible) {
			block.disableInvisibleFaces (worldBlocks);
			return;
		}
		block.visible = true; 
		drawBlock (block.type, blockPos);
	}

	void drawAroundInvisibleBlocks(Vector3 blockPos) {
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
	}

	// Update is called once per frame
	void Update () {

		// left click
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2.0f, Screen.height / 2.0f, 0));

			if (Physics.Raycast (ray, out hit, 10.0f)) {

				GameObject block = hit.transform.parent.gameObject;
				if (!worldBlocks.ContainsKey (block.transform.position))
					return;
				
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
				if (!worldBlocks.ContainsKey (block.transform.position))
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