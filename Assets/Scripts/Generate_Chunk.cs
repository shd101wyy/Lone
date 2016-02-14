using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

// render chunk
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Generate_Chunk : MonoBehaviour {
	Dictionary<Vector3, Block> chunk;
	ArrayList visibleBlocks;

	private GameObject player;

	private MeshData meshData;

	// Use this for initialization
	void Start () {
	}

	public void startGeneratingChunk(HeightMap heightMap,
		GameObject player) {

		this.player = player;

		meshData = new MeshData ();
		chunk = heightMap.chunk;
		visibleBlocks = heightMap.visibleBlocks;

		int i;

		foreach (Vector3 blockPos in visibleBlocks) {
			Block block = chunk [blockPos];
			block.visible = true;
			drawBlock (blockPos);


			// check blocks below
			int y = (int)blockPos.y - 1;
			while (y > 0) {
				Vector3 pos = new Vector3 (blockPos.x, y, blockPos.z);
				block = chunk [pos];

				// 下面这个满段豫剧很关键，要不然会重复创建。
				if (block.visible)
					break;

				int[,] delta = {{1, 0, 0}, {-1, 0, 0}, {0, -1, 0}, {0, 0, 1}, {0, 0, -1}};
				bool needToBeVisible = false;
				for (i = 0; i < 5; i++) {
					int deltaX = delta [i, 0];
					int deltaY = delta [i, 1];
					int deltaZ = delta [i, 2];

					Vector3 p = new Vector3 (pos.x + deltaX, pos.y + deltaY, pos.z + deltaZ);
					if (!chunk.ContainsKey (p) || !(chunk[p] is CubeBlock)) {
						needToBeVisible = true;
						break;
					}
				}
				if (needToBeVisible) {
					block.visible = true;
					drawBlock (pos);
					y--;
				} else {
					break;
				}
			}
		}

		MeshFilter filter = transform.GetComponent< MeshFilter >();
		Mesh mesh = filter.mesh;
		mesh.Clear();

		mesh.vertices = meshData.vertices.ToArray();
		mesh.normals = meshData.normals.ToArray ();
		mesh.uv = meshData.uvs.ToArray();
		mesh.triangles = meshData.triangles.ToArray();

		mesh.RecalculateBounds();
		mesh.Optimize();

		// collision
		MeshCollider coll = transform.GetComponent<MeshCollider> ();
		coll.sharedMesh = null; 
		Mesh coll_mesh = new Mesh(); 
		coll_mesh.vertices = meshData.colVertices.ToArray();
		coll_mesh.triangles = meshData.colTriangles.ToArray();
		coll_mesh.RecalculateNormals();
		coll.sharedMesh = coll_mesh;
	}

	void drawBlock(Vector3 blockPos) {
		Block block = chunk [blockPos];
		block.generateMesh (meshData);
	}

	void drawInvisibleBlock(Vector3 blockPos) {
		if (chunk.ContainsKey (blockPos) == false)
			return;


		Block block = chunk[blockPos];
		if (block.visible) {
			block.generateMesh (meshData);
			return;
		}
		block.visible = true; 
		drawBlock (blockPos);
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
		/*
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width / 2.0f, Screen.height / 2.0f, 0));

			if (Physics.Raycast (ray, out hit, 16.0f)) {

				GameObject block = hit.transform.parent.gameObject;
				if (chunk.ContainsKey (block.transform.position) == false) {
					return;
				}
				
				Vector3 blockPos = block.transform.position;
				
				// this is the bottom block, don't delete it 
				if ((int)blockPos.y == 0) return;

				chunk.Remove(blockPos);
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
				if (!chunk.ContainsKey (block.transform.position) || !(chunk[block.transform.position] is CubeBlock))
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
				chunk [newPos] = newBlock;
				newBlock.visible = true;
				drawBlock (newPos);

				drawAroundInvisibleBlocks (newPos);

			}
		}
		*/
	}
}