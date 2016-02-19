using UnityEngine;
using System.Collections;

public class RightHandAnimation : MonoBehaviour {

	private Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Debug.Log ("wave right hand");
			anim.Play ("RightHandWave", -1, 0f);
		}
	}
}
