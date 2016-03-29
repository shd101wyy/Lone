using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

// render chunk
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Generate_Chunk : MonoBehaviour {
	public GameObject player;
	private Chunk chunk = null;
	private MeshData cubeBlockMeshData = null;
	private MeshData plantsMeshData = null;
	private World world = null;
	private bool needRender = false;

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

		/*if (block is Air) {
			return;
		} else*/ if (block is CubeBlock) {
			((CubeBlock)block).generateMesh (cubeBlockMeshData, blockPos, world);
		} else if (block is Plant) {
			// GameObject clone = (GameObject)Instantiate (((Plant)block).getGameObject(), blockPos, Quaternion.identity);
			((Plant)block).generateMesh(plantsMeshData, blockPos, world);
		} else {
			Debug.Log ("Error drawBlock");
		}
	}

	public void renderChunk() {
		cubeBlockMeshData = new MeshData ();
		plantsMeshData = new MeshData ();

		// chunk.updateChunksMask (world);
		foreach (var blockPos in chunk.blocks.Keys) {
			drawBlock (blockPos.ToVector3());
		}

		/*
		if (chunk.isAllAir()) {
			chunk.rendered = true;
			return;
		}
		*/

		/*
		for (int x = 0; x < Chunk.width; x++) {
			for (int y = 0; y < Chunk.height; y++) {
				for (int z = 0; z < Chunk.depth; z++) {
					drawBlock (new Vector3 (x + chunk.x, y + chunk.y, z + chunk.z));
				}
			}
		}
		*/

		// Cubeblock
		MeshFilter filter = transform.GetComponent< MeshFilter >();
		Mesh mesh = filter.mesh;
		mesh.Clear();

		mesh.vertices = cubeBlockMeshData.vertices.ToArray();
		mesh.triangles = cubeBlockMeshData.triangles.ToArray();

		mesh.normals = cubeBlockMeshData.normals.ToArray ();
		mesh.uv = cubeBlockMeshData.uvs.ToArray();

		mesh.RecalculateBounds();
		mesh.Optimize();

		// collision
		MeshCollider coll = transform.GetComponent<MeshCollider> ();
		coll.sharedMesh = null; 
		Mesh coll_mesh = new Mesh(); 
		coll_mesh.vertices = cubeBlockMeshData.colVertices.ToArray();
		coll_mesh.triangles = cubeBlockMeshData.colTriangles.ToArray();
		coll_mesh.RecalculateNormals();
		coll.sharedMesh = coll_mesh;

		// plants mesh
		MeshFilter plantsMesh = transform.FindChild ("Plants").GetComponent<MeshFilter> ();
		mesh = plantsMesh.mesh;
		mesh.Clear();

		mesh.vertices = plantsMeshData.vertices.ToArray();
		mesh.triangles = plantsMeshData.triangles.ToArray();

		mesh.normals = plantsMeshData.normals.ToArray ();
		mesh.uv = plantsMeshData.uvs.ToArray();

		mesh.RecalculateBounds();
		mesh.Optimize();


		// collision
		coll = transform.FindChild ("Plants").GetComponent<MeshCollider> ();
		coll.sharedMesh = null; 
		coll_mesh = new Mesh(); 
		coll_mesh.vertices = plantsMeshData.colVertices.ToArray();
		coll_mesh.triangles = plantsMeshData.colTriangles.ToArray();
		coll_mesh.RecalculateNormals();
		coll.sharedMesh = coll_mesh;

		// ignore the collider between player and plants
		Physics.IgnoreCollision(transform.FindChild ("Plants").GetComponent<Collider>(), player.GetComponent<Collider>());

		// set chunk as rendered
		chunk.rendered = true;
	}
		
	// Update is called once per frame
	void Update () {
		if (chunk != null && chunk.needRender) {
			renderChunk ();
			chunk.needRender = false;
		}
	}
}