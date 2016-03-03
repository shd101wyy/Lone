using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryBarController : MonoBehaviour {

	public Image selectBox;
	public Image[] slots;
	public Item[] items;
	int slotIndex;

	// Use this for initialization
	void Start () {
		Color color = slots [0].color;
		color.a = 1f;
		slots [0].color = color;

		items = new Item[slots.Length];
		for (int i = 0; i < slots.Length; i++) {
			items [i] = null;
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			for (int i = 0; i < slots.Length; i++) {
				if (isMouseOverSlot (i)) {
					Debug.Log ("click slot: " + i);
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			setSelectedSlot (0);
		} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			setSelectedSlot (1);
		} else if (Input.GetKeyDown (KeyCode.Alpha3)) {
			setSelectedSlot (2);
		} else if (Input.GetKeyDown (KeyCode.Alpha4)) {
			setSelectedSlot (3);
		} else if (Input.GetKeyDown (KeyCode.Alpha5)) {
			setSelectedSlot (4);
		} else if (Input.GetKeyDown (KeyCode.Alpha6)) {
			setSelectedSlot (5);
		} else if (Input.GetKeyDown (KeyCode.Alpha7)) {
			setSelectedSlot (6);
		} else if (Input.GetKeyDown (KeyCode.Alpha8)) {
			setSelectedSlot (7);
		} else if (Input.GetKeyDown (KeyCode.Alpha9)) {
			setSelectedSlot (8);
		}
	}

	public void setItem(Item item, int slotIndex) {
		if (item.itemType == ItemType.BLOCK) {
			Texture2D tex = ((Block)item).getTexture ();
			Sprite s = Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
			this.slots [slotIndex].sprite = s;


			Color color = slots [slotIndex].color;
			color.a = 1f;
			slots [slotIndex].color = color;

			items [slotIndex] = item;
		} else {
			Texture2D tex = item.getTexture ();
			Sprite s = Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
			this.slots [slotIndex].sprite = s;


			Color color = slots [slotIndex].color;
			color.a = 1f;
			slots [slotIndex].color = color;

			items [slotIndex] = item;
		}
	}

	void setSelectedSlot(int slotIndex) {
		RectTransform slotRT = slots [slotIndex].GetComponent<RectTransform> () as RectTransform;
		RectTransform selectBoxRT = selectBox.GetComponent<RectTransform> () as RectTransform;

		selectBoxRT.position = slotRT.position;

		this.slotIndex = slotIndex;
	}

	public Item getSelectedItem() {
		return items [slotIndex];	
	}

	public Item getItem(int slotIndex) {
		return items [slotIndex];
	}


	public bool isMouseOverSlot(int slotIndex) {
		RectTransform rt = slots [slotIndex].GetComponent<RectTransform> ();
		if (Input.mousePosition.x > rt.position.x - rt.sizeDelta.x * 1.5f && Input.mousePosition.x < rt.position.x + rt.sizeDelta.x * 1.5f &&
		    Input.mousePosition.y > rt.position.y - rt.sizeDelta.y * 1.5f && Input.mousePosition.y < rt.position.y + rt.sizeDelta.y * 1.5f) {
			return true;
		} else {
			return false;
		}
	}

}
