using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game : MonoBehaviour {
	public GameObject commandBox;
	public GameObject player;

	private InputField input;

	// Use this for initialization
	void Start () {
		input = commandBox.GetComponent<InputField> ();
		GameObject.Find ("Landscape").GetComponent<Generate_Landscape> ().startGeneratingLandscape (/*0*/ (int)Network.time * 10);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.T)) {
			if (!input.isFocused) {
				input.ActivateInputField ();
				input.Select ();
				input.text = "";
			}
		}
	}

	public void GetCommandBoxInput(string command) {
		Debug.Log ("@: " + command);
		command = command.Trim ();
		if (command == "start") {
			/*
			GameObject landscape = GameObject.Find ("Landscape");
			Destroy (landscape);
			landscape.GetComponent<Generate_Landscape> ().startGeneratingLandscape ((int)Network.time * 10);
			*/
		}

		input.text = "";

	}
}
