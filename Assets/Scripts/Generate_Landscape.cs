using UnityEngine;
using System.Collections;


public enum Type {GRASS, SAND, SNOW};

public class Block {
	public Type type;
	public bool visible;

	public Block(Type t, bool v) {
		type = t;
		visible = v;
	}
}


public class Generate_Landscape : MonoBehaviour {
	public static int width = 64;
	public static int depth = 64;
	public static int height = 128;
	public int heightScale = 20;
	public float detailScale = 25.0f;

	public GameObject grassBlock;
	public GameObject snowBlock;
	public GameObject sandBlock;

	Block[,,] worldBlocks = new Block[width, height, depth];


	// Use this for initialization
	void Start () {
		int seed = (int)Network.time * 10;

		for (int z = 0; z < depth; z++) {
			for (int x = 0; x < width; x++) {
				int y = (int)(Mathf.PerlinNoise ((x+seed)/detailScale, (z+seed)/detailScale) * heightScale);
				Vector3 blockPos = new Vector3 (x, y, z);

				createBlock (blockPos, true);
				while (y > 0) {
					y--; 
					blockPos = new Vector3 (x, y, z);
					createBlock (blockPos, false);
				}
			}
		}
	}

	void createBlock(Vector3 blockPos, bool create) {
		int y = (int)blockPos.y;
		if (y > 15) {
			if (create) {
				drawBlock (Type.SNOW, blockPos);
			}

			worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block (Type.SNOW, create);

		} else if (y > 5) {
			if (create) {
				drawBlock (Type.GRASS, blockPos);
			}

			worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block (Type.GRASS, create);

		} else {
			if (create) {
				drawBlock (Type.SAND, blockPos);
			}

			worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block (Type.SAND, create);

		}
	}

	void drawBlock(Type type, Vector3 blockPos) {
		switch (type) {
		case Type.GRASS:
			Instantiate (grassBlock, blockPos, Quaternion.identity);
			break;
		case Type.SAND:
			Instantiate (sandBlock, blockPos, Quaternion.identity);
			break;
		case Type.SNOW:
			Instantiate (snowBlock, blockPos, Quaternion.identity);
			break;
		}
	}

	void drawInvisibleBlock(Vector3 blockPos) {
		if (blockPos.x < 0 || blockPos.x >= width ||
		    blockPos.y < 0 || blockPos.y >= height ||
		    blockPos.z < 0 || blockPos.z >= depth)
			return;
		
		Block block = worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z];
		if (block == null)
			return;

		if (!block.visible) {
			block.visible = true; 
			drawBlock (block.type, blockPos);
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2.0f, Screen.height / 2.0f, 0));
			if (Physics.Raycast (ray, out hit, 1000.0f)) {
				Vector3 blockPos = hit.transform.position;

				// this is the bottom block, don't delete it 
				if ((int)blockPos.y == 0) return;
				worldBlocks [(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;

				Destroy (hit.transform.gameObject);

				for (int x = -1; x <= 1; x++) {
					for (int y = -1; y <= 1; y++) {
						for(int z = -1; z <= 1; z++) {
							if (!(x == 0 && y == 0 && z == 0)) {
								Vector3 neighbour = new Vector3 (blockPos.x + x, blockPos.y + y, blockPos.z + z);
								drawInvisibleBlock (neighbour);
							}
						}
					}
				}
			}
		}
	}
}
