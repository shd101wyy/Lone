using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer)/*, typeof(MeshCollider)*/)]
public class DropItem : MonoBehaviour {

	private bool startMoving;
	private float speed;
	private int count; 
	private bool findPlayer;
	private Vector3 step;

	private MeshData meshData;

	// Use this for initialization
	void Start () {
		startMoving = false;
		speed = 0.5f;
		count = 0;
		findPlayer = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (findPlayer == false) {
			count++;
			if (count == 25) {
				float y = transform.position.y;
				speed = -speed;
				count = 0;
			}
			transform.position = transform.position + new Vector3 (0, speed * Time.deltaTime, 0);
			transform.Rotate (new Vector3 (0, 30f * Time.deltaTime, 0));

			checkNearbyPlayer ();
		} else {
			if (count == 0) { // destroy self gameObject
				Destroy(transform.gameObject); 
			} else {
				transform.position = transform.position + step;
				count--;
			}
		}
	}

	public void generate3DMesh(Block block, Vector3 blockPos) {
		/*
		Texture2D texture_2d = block.getTexture();

		transform.position = blockPos;
		name = "drop " + blockPos;

		Game.generate3DMeshFrom2DTexture (this.transform.gameObject, texture_2d, 1);

		transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

		startMoving = true;
		*/
		name = "drop " + blockPos;

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

		/*
		// collision
		MeshCollider coll = GetComponent<MeshCollider>();// transform.gameObject.AddComponent< MeshCollider >() as MeshCollider;
		coll.sharedMesh = null; 
		Mesh coll_mesh = new Mesh(); 
		coll_mesh.vertices = meshData.colVertices.ToArray();
		coll_mesh.triangles = meshData.colTriangles.ToArray();
		coll_mesh.RecalculateNormals();
		coll.sharedMesh = coll_mesh;
		*/

		transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
		transform.position = blockPos;
		startMoving = true;
	}

	void checkNearbyPlayer() {
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 4f);
		int i = 0;
		while (i < hitColliders.Length) {
			if (hitColliders [i].tag == "Player") {
				// Debug.Log (hitColliders [i].name);
				findPlayer = true;
				count = 20;
				step = (hitColliders [i].transform.position - transform.position) / count;
			}
			i++;
		}
	}
}
