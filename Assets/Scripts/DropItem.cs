using UnityEngine;
using System.Collections;

public class DropItem : MonoBehaviour {

	private bool startMoving;
	private float speed;
	private int count; 
	private bool findPlayer;
	private Vector3 step;

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

	public void generate3DMesh(Block block) {
		Vector3 blockPos = block.pos;
		Texture2D texture_2d = block.texture_2d;

		transform.position = blockPos;
		name = "drop " + blockPos;

		Game.generate3DMeshFrom2DTexture (this.transform.gameObject, texture_2d, 1);

		transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

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
