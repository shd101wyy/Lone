using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HoldItem : MonoBehaviour {
	MeshData meshData;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void generate3DMesh(Block block) {
		meshData = new MeshData ();
		block.generateMesh (meshData, Vector3.zero, null, false, true);

		MeshFilter filter = GetComponent<MeshFilter> (); // transform.gameObject.AddComponent< MeshFilter >() as MeshFilter;
		Mesh mesh = filter.mesh;
		mesh.Clear();

		mesh.vertices = meshData.vertices.ToArray();
		mesh.triangles = meshData.triangles.ToArray();

		// mesh.normals = meshData.normals.ToArray ();
		mesh.uv = meshData.uvs.ToArray();

		mesh.RecalculateBounds();
		mesh.Optimize();

		Material material = GetComponent<Renderer>().material as Material;
		material.mainTexture = GameObject.Find ("Chunk").GetComponent<Renderer> ().material.mainTexture; 

		transform.localScale = new Vector3 (0.6f, 0.6f, 0.6f);
	}
}
