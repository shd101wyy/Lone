using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

// render chunk
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Generate_Chunk : MonoBehaviour {
	private Chunk chunk = null;
	// private ArrayList visibleBlocks = null;
	private GameObject player = null;
	private MeshData meshData = null;
	public HeightMap heightMap = null;

	// Use this for initialization
	void Start () {
	}

	public void startGeneratingChunk(HeightMap heightMap,
		GameObject player) {

		this.player = player;
		this.heightMap = heightMap;

		chunk = heightMap.chunk;

		renderChunk ();
	}

	void drawBlock(Vector3 blockPos) {
		Block block = chunk.getBlock(blockPos);
		block.generateMesh (meshData);
	}

	public void renderChunk() {
		meshData = new MeshData ();

		foreach (var blockPos in chunk.blocks.Keys) {
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
		if (chunk != null && chunk.needRender) {
			renderChunk ();
			chunk.needRender = false;
		}
	}
}