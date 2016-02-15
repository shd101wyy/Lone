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

			if (chunk.water != null) {
				Water water = chunk.water;
				chunk.water = null;

				StartCoroutine(spreadWater (water));
			}
		}
	}

	IEnumerator spreadWater(Water water) {
		World world = this.heightMap.chunk.world;
		Vector3 pos = water.pos;
		if (water.energy - 1 == 0 && world.getBlock(pos + new Vector3(0, -1, 0)) != null)
			yield break;
		int energy = water.energy;

		Vector3 frontDir = new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z - 1);
		Vector3 backDir = new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z + 1);
		Vector3 leftDir = new Vector3 ((int)pos.x - 1, (int)pos.y, (int)pos.z);
		Vector3 rightDir = new Vector3 ((int)pos.x + 1, (int)pos.y, (int)pos.z);
		Vector3 bottomDir = new Vector3 ((int)pos.x, (int)pos.y - 1, (int)pos.z);

		Block front = world.getBlock (frontDir);
		Block back = world.getBlock (backDir);
		Block left = world.getBlock (leftDir);
		Block right = world.getBlock (rightDir);
		Block bottom = world.getBlock (bottomDir);

		float seconds = 1f;

		ArrayList spread = new ArrayList();

		if (energy > 1 && (!(front != null && front is CubeBlock)) && bottom != null) {
			yield return new WaitForSeconds (seconds);

			Water w = new Water (energy - 1);
			world.addBlock (frontDir, w, true);

			spread.Add(w);
		}

		if (energy > 1 && (!(back != null && back is CubeBlock)) && bottom != null) {
			yield return new WaitForSeconds (seconds);

			Water w = new Water (energy - 1);
			world.addBlock (backDir, w, true);

			spread.Add(w);
		}

		if (energy > 1 && (!(left != null && left is CubeBlock)) && bottom != null) {
			yield return new WaitForSeconds (seconds);

			Water w = new Water (energy - 1);
			world.addBlock (leftDir, w, true);

			spread.Add(w);
		}

		if (energy > 1 && (!(right != null && right is CubeBlock)) && bottom != null) {
			yield return new WaitForSeconds (seconds);

			Water w = new Water (energy - 1);
			world.addBlock (rightDir, w, true);

			spread.Add(w);
		}

		if (bottom == null) {
			Water w = new Water (energy - 1);
			world.addBlock (bottomDir, w, true);

			spread.Add(w);
		}

		/*
		this.chunk.needRender = true;
		yield return new WaitForSeconds (seconds);
		this.chunk.needRender = false;
		for (int i = 0; i < spread.Count; i++) {
			StartCoroutine(spreadWater (spread[i] as Water));
		}
		*/
		
	}
}