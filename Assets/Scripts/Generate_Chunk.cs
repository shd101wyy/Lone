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

		if (block is CubeBlock) {
			((CubeBlock)block).generateMesh (meshData, blockPos, world);
		} else if (block is Plant) {
			// GameObject clone = (GameObject)Instantiate (((Plant)block).getGameObject(), blockPos, Quaternion.identity);
			((Plant)block).generateMesh(meshData, blockPos, world);
		} else {
			Debug.Log ("Error drawBlock");
		}
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

		mesh.normals = meshData.normals.ToArray ();
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
		}
	}
}