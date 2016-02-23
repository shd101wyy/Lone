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
	private MeshData meshData = null;
	private World world = null;

	// Use this for initialization
	void Start () {
	}

	public void bindChunk(Chunk chunk, World world) {
		this.world = world;
		this.chunk = chunk;
		// renderChunk ();
	}

	void drawBlock(Vector3 blockPos) {
		Block block = chunk.getBlock(blockPos);
		block.generateMesh (meshData, blockPos, world);
	}

	public void renderChunk() {
		meshData = new MeshData ();

		foreach (var blockPos in chunk.blocks.Keys) {
			drawBlock (blockPos.ToVector3());
		}

		MeshFilter filter = transform.GetComponent< MeshFilter >();
		Mesh mesh = filter.mesh;
		mesh.Clear();

		mesh.vertices = meshData.vertices.ToArray();
		mesh.triangles = meshData.triangles.ToArray();

		// mesh.normals = meshData.normals.ToArray ();
		mesh.uv = meshData.uvs.ToArray();

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

		// set chunk as rendered
		chunk.rendered = true;
	}
		
	// Update is called once per frame
	void Update () {
		if (chunk != null && chunk.needRender) {
			renderChunk ();
			chunk.needRender = false;

			/*
			if (chunk.water != null) {
				Water water = chunk.water;
				chunk.water = null;

				// StartCoroutine(spreadWater (water));
			}
			*/
		}
	}
	/*
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

		
	}
*/
}