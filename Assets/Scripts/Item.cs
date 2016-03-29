using UnityEngine;
using System.Collections;

public enum ItemType {CUBE_BLOCK, TOOL, PLANT, AIR};

[System.Serializable]
public abstract class Item {
	public string name;
	public int maxStack;
	public ItemType itemType;
	public int stack;

	public Item(string itemName, ItemType itemType, int maxStack = 64) {
		this.name = itemName;
		this.itemType = itemType;
		this.maxStack = maxStack;
		stack = 1;
	}

	public void addStack(int s) {
		stack += s;
	}

	public void decrementStack(int s) {
		stack -= s;
	}

	public void setStack(int s) {
		stack = s;
	}

	public abstract Texture2D getTexture ();
}