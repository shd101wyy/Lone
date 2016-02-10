using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


public enum Type {GRASS, SAND, SNOW, DIRT, PLANT_FERN};

public abstract class Block {
	public Type type;
	public bool visible;
	public Vector3 pos;

	public GameObject meshObject;

	protected GameObject clone;

	public Block(Type type, bool visible, Vector3 pos) {
		this.type = type;
		this.visible = visible;
		this.pos = pos;
	}

	public abstract void setClone (GameObject clone);

	public abstract void disableInvisibleFaces (Dictionary<Vector3, Block> chunk);
}

public class CubeBlock: Block {
	protected GameObject front;
	protected GameObject back;
	protected GameObject left;
	protected GameObject right;
	protected GameObject top;
	protected GameObject bottom;

	public CubeBlock(Type type, bool visible, Vector3 pos) : base(type, visible, pos) {
		front = null;
		back = null;
		left = null;
		right = null;
		top = null;
		bottom = null;
	}

	public override void setClone(GameObject clone) {
		this.clone = clone;

		GameObject top = (GameObject)clone.transform.FindChild ("Top").gameObject;
		GameObject bottom = (GameObject)clone.transform.FindChild ("Bottom").gameObject;
		GameObject left = (GameObject)clone.transform.FindChild ("Left").gameObject;
		GameObject right = (GameObject)clone.transform.FindChild ("Right").gameObject;
		GameObject front = (GameObject)clone.transform.FindChild ("Front").gameObject;
		GameObject back = (GameObject)clone.transform.FindChild ("Back").gameObject;

		addFaces (front, back, left, right, top, bottom);
	}

	public void addFaces( GameObject front, 
		GameObject back,
		GameObject left,
		GameObject right,
		GameObject top,
		GameObject bottom) {

		this.front = front;
		this.back = back;
		this.left = left;
		this.right = right;
		this.top = top;
		this.bottom = bottom;
	}

	public override void disableInvisibleFaces(Dictionary<Vector3, Block> chunk) {
		front.SetActive (true);
		back.SetActive (true);
		left.SetActive (true);
		right.SetActive (true);
		top.SetActive (true);
		bottom.SetActive (true);


		Vector3 frontDir = new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z - 1);
		Vector3 backDir = new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z + 1);
		Vector3 leftDir = new Vector3 ((int)pos.x - 1, (int)pos.y, (int)pos.z);
		Vector3 rightDir = new Vector3 ((int)pos.x + 1, (int)pos.y, (int)pos.z);
		Vector3 bottomDir = new Vector3 ((int)pos.x, (int)pos.y - 1, (int)pos.z);

		if (chunk.ContainsKey (frontDir) &&
			chunk[frontDir] is CubeBlock) {
			front.SetActive(false);
		}
		if (chunk.ContainsKey (backDir)  &&
			chunk[backDir] is CubeBlock) {
			back.SetActive(false);
		}
		if (chunk.ContainsKey (rightDir) && 
			chunk[rightDir] is CubeBlock) {
			right.SetActive(false);
		}
		if (chunk.ContainsKey (leftDir) &&
			chunk[leftDir] is CubeBlock) {
			left.SetActive(false);
		}
		if (chunk.ContainsKey (bottomDir) && 
			chunk[bottomDir] is CubeBlock) {
			bottom.SetActive(false);
		}
	}
}

public class Fern: Block {
	public Fern(bool visible, Vector3 pos) : base(Type.PLANT_FERN, visible, pos) {
		this.meshObject = Generate_Landscape.prefab_Fern;
	}

	public override void setClone(GameObject clone) {
		this.clone = clone;
	}

	public override void disableInvisibleFaces (Dictionary<Vector3, Block> chunk) {
		// nothing happened
	}
}

public class Rose: Block {
	public Rose(bool visible, Vector3 pos) : base(Type.PLANT_FERN, visible, pos) {
		this.meshObject = Generate_Landscape.prefab_Rose;
	}

	public override void setClone(GameObject clone) {
		this.clone = clone;
	}

	public override void disableInvisibleFaces (Dictionary<Vector3, Block> chunk) {
		// nothing happened
	}
}

public class Grass: CubeBlock {
	public Grass(bool visible, Vector3 pos) : base(Type.GRASS, visible, pos) {
		this.meshObject = Generate_Landscape.prefab_Grass;
	}
}

public class Dirt: CubeBlock {
	public Dirt(bool visible, Vector3 pos) : base(Type.DIRT, visible, pos) {
		this.meshObject = Generate_Landscape.prefab_Dirt;
	}
}

public class Snow: CubeBlock {
	public Snow(bool visible, Vector3 pos) : base(Type.SNOW, visible, pos) {
		this.meshObject = Generate_Landscape.prefab_Snow;
	}
}

public class Sand: CubeBlock {
	public Sand(bool visible, Vector3 pos) : base(Type.SAND, visible, pos) {
		this.meshObject = Generate_Landscape.prefab_Sand;
	}
}
