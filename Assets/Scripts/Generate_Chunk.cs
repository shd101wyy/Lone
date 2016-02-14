using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

// render chunk
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Generate_Chunk : MonoBehaviour {
	private Dictionary<Vector3, Block> chunk = null;
	// private ArrayList visibleBlocks = null;
	private GameObject player = null;
	private MeshData meshData = null;
	private HeightMap heightMap = null;

	// Use this for initialization
	void Start () {
	}

	public void startGeneratingChunk(HeightMap heightMap,
		GameObject player) {

		this.player = player;
		this.heightMap = heightMap;

		chunk = heightMap.chunk;
		// visibleBlocks = heightMap.visibleBlocks;

		renderChunk ();
	}

	void drawBlock(Vector3 blockPos) {
		Block block = chunk [blockPos];
		block.generateMesh (meshData);
	}

	void renderChunk() {
		meshData = new MeshData ();

		foreach (var blockPos in chunk.Keys) {
			drawBlock (blockPos);
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
		if (player == null)
			return;
		
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

					if (!(blockPos.x <= (heightMap.chunkX + 1) * Generate_Landscape.chunkWidth && blockPos.x >= heightMap.chunkX * Generate_Landscape.chunkWidth &&
						blockPos.z <= (heightMap.chunkZ + 1) * Generate_Landscape.chunkDepth && blockPos.z >= heightMap.chunkZ * Generate_Landscape.chunkDepth)) {
						return;
					}

					Vector3 newPos = blockPos + hit.normal;

					if ((player.transform.position.x < newPos.x + 0.5f && player.transform.position.x > newPos.x - 0.5f) &&
						(player.transform.position.y < newPos.y + 0.5f && player.transform.position.y > newPos.y - 0.5f) &&
						(player.transform.position.z < newPos.z + 0.5f && player.transform.position.z > newPos.z - 0.5f)) {
						// Debug.Log ("Inside");
						return;
					}

					if (this.chunk.ContainsKey (newPos) == false) {
						Block newBlock = new Dirt (true, newPos, chunk);
						this.chunk.Add(newPos, newBlock);
						// this.visibleBlocks.Add (newPos);
						renderChunk ();
					}
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

					if (!(blockPos.x <= (heightMap.chunkX + 1) * Generate_Landscape.chunkWidth && blockPos.x >= heightMap.chunkX * Generate_Landscape.chunkWidth &&
						blockPos.z <= (heightMap.chunkZ + 1) * Generate_Landscape.chunkDepth && blockPos.z >= heightMap.chunkZ * Generate_Landscape.chunkDepth)) {
						return;
					}

					if (blockPos.y == 0)
						return;

					chunk.Remove (blockPos);
					renderChunk ();
				}
			}
		}
	}
}