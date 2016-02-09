using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum Type {GRASS, SAND, SNOW};

public class Block {
	public Type type;
	public bool visible;
	public Vector3 pos;

	public GameObject front;
	public GameObject back;
	public GameObject left;
	public GameObject right;
	public GameObject top;
	public GameObject bottom;


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


public class Generate_Landscape : MonoBehaviour {
	public static int width = 25;
	public static int depth = 25;
	public static int height = 128;
	public int heightScale = 20;
	public float detailScale = 25.0f;

	private GameObject grassBlock;
	private GameObject snowBlock;
	private GameObject sandBlock;

	Dictionary<Vector3, Block> worldBlocks = new Dictionary<Vector3, Block>();

	// Use this for initialization
	void Start () {
		grassBlock = GameObject.Find("Grass");
		snowBlock = GameObject.Find("Snow");
		sandBlock = GameObject.Find("Sand");

		int seed = (int)Network.time * 10;

		ArrayList visibleBlocks = new ArrayList ();

		for (int z = 0; z < depth; z++) {
			for (int x = 0; x < width; x++) {
				int y = (int)(Mathf.PerlinNoise ((x+seed)/detailScale, (z+seed)/detailScale) * heightScale);
				Vector3 blockPos = new Vector3 (x, y, z);

				visibleBlocks.Add(blockPos);
				createBlock (blockPos);
				while (y > 0) {
					y--; 
					blockPos = new Vector3 (x, y, z);
					createBlock (blockPos);
				}
			}
		}

		foreach (Vector3 blockPos in visibleBlocks) {
			Block block = worldBlocks [blockPos];
			block.visible = true;
			drawBlock (block.type, blockPos);
		}
	}

	void createBlock(Vector3 blockPos) {
		int y = (int)blockPos.y;
		Block block;
		if (y > 15) {
			block = new Block (Type.SNOW, false, blockPos);
		} else if (y > 5) {
			block = new Block (Type.GRASS, false, blockPos);
		} else {
			block = new Block (Type.SAND, false, blockPos);
		}

		worldBlocks[blockPos] = block;
	}

	void drawBlock(Type type, Vector3 blockPos) {
		GameObject clone = null;

		switch (type) {
		case Type.GRASS:
			clone = (GameObject)Instantiate (grassBlock, blockPos, Quaternion.identity);
			//clone.name = "grass";
			break;
		case Type.SAND:
			clone = (GameObject)Instantiate (sandBlock, blockPos, Quaternion.identity);
			//clone.name = "sand";
			break;
		case Type.SNOW:
			clone = (GameObject)Instantiate (snowBlock, blockPos, Quaternion.identity);
			//clone.name = "snow";
			break;
		}

		clone.name = blockPos.ToString ();
			
		GameObject top = (GameObject)clone.transform.FindChild ("Top").gameObject;
		GameObject bottom = (GameObject)clone.transform.FindChild ("Bottom").gameObject;
		GameObject left = (GameObject)clone.transform.FindChild ("Left").gameObject;
		GameObject right = (GameObject)clone.transform.FindChild ("Right").gameObject;
		GameObject front = (GameObject)clone.transform.FindChild ("Front").gameObject;
		GameObject back = (GameObject)clone.transform.FindChild ("Back").gameObject;

		Block block = worldBlocks [blockPos];
		block.addFaces (front, back, left, right, top, bottom);
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

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2.0f, Screen.height / 2.0f, 0));
			if (Physics.Raycast (ray, out hit, 200.0f)) {
				GameObject block = hit.collider.gameObject.transform.parent.gameObject;
				Vector3 blockPos = block.transform.position;

				// this is the bottom block, don't delete it 
				if ((int)blockPos.y == 0) return;

				worldBlocks.Remove(blockPos);
				Destroy (block);

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
		}
	}
}
