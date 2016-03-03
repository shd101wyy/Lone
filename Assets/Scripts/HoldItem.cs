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

	public void generate3DMesh(Item item) {
		if (item.itemType == ItemType.BLOCK) {
			generate3DMeshFromBlock ((Block)item);
			transform.localScale = new Vector3 (0.6f, 0.6f, 0.6f);
		} else {
			generate3DMeshFromTexture (item.getTexture ());
			transform.localScale = new Vector3 (1f, 1f, 1f);
		}
	}

	public void generate3DMeshFromBlock(Block block) {
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
	}

	public void generate3DMeshFromTexture(Texture2D texture_2d, float depth = 0.065f) {
		ExtrudeSprite extrudeSprite = this.transform.gameObject.AddComponent<ExtrudeSprite> () as ExtrudeSprite;
		extrudeSprite.GenerateMesh (texture_2d, depth);
	}
}
