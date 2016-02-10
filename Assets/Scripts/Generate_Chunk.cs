using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Generate_Chunk : MonoBehaviour {
	Dictionary<Vector3, Block> worldBlocks;
	ArrayList visibleBlocks;

	private GameObject player;

	// Use this for initialization
	void Start () {
	}

	public void startGeneratingChunk(HeightMap heightMap,
		GameObject player) {

		this.player = player;

		worldBlocks = heightMap.worldBlocks;
		visibleBlocks = heightMap.visibleBlocks;

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
	}

	// Update is called once per frame
	void Update () {

		// left click
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2.0f, Screen.height / 2.0f, 0));

			if (Physics.Raycast (ray, out hit, 16.0f)) {

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

			if (Physics.Raycast (ray, out hit, 16.0f)) {

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