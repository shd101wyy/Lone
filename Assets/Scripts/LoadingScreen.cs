using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (Countdown ());
	}
	
	private IEnumerator Countdown() {
		yield return new WaitForSeconds (8);
		SceneManager.LoadSceneAsync("Main");
	}
}
