using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public GameObject leftHand;
	public GameObject rightHand;
	public HoldItem leftHandHoldItem;
	public HoldItem rightHandHoldItem;
	InventoryBarController inventoryBarController;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha1) || 
			Input.GetKeyDown (KeyCode.Alpha2) || 
			Input.GetKeyDown (KeyCode.Alpha3) || 
			Input.GetKeyDown (KeyCode.Alpha4) || 
			Input.GetKeyDown (KeyCode.Alpha5) || 
			Input.GetKeyDown (KeyCode.Alpha6) || 
			Input.GetKeyDown (KeyCode.Alpha7) || 
			Input.GetKeyDown (KeyCode.Alpha8) || 
			Input.GetKeyDown (KeyCode.Alpha9)) {

			Item item = inventoryBarController.getSelectedItem ();
			rightHandHoldItem.generate3DMesh (item);

		}
	}

	public void initializePlayer() {
		inventoryBarController = GameObject.Find ("InventoryBar").GetComponent<InventoryBarController>() as InventoryBarController;

		leftHandHoldItem = leftHand.GetComponent<HoldItem> () as HoldItem;
		rightHandHoldItem = rightHand.GetComponent<HoldItem> () as HoldItem;

		rightHandHoldItem.generate3DMesh (inventoryBarController.getItem (0));
	}
}
