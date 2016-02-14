using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


public enum Type {GRASS, SAND, SNOW, DIRT, PLANT_FERN};

public abstract class Block {
	public Type type;
	public bool visible;
	public Vector3 pos;
	public Dictionary<Vector3, Block> chunk;

	protected GameObject clone;

	public float blockSize = 1f;
	public BlockTile blockTile;

	public Block(Type type, bool visible, Vector3 pos, Dictionary<Vector3, Block> chunk) {
		this.type = type;
		this.visible = visible;
		this.pos = pos;
		this.chunk = chunk;
	}

	public abstract void setClone (GameObject clone);

	public abstract void generateMesh (MeshData meshData);
}

public class CubeBlock: Block {
	public CubeBlock(Type type, bool visible, Vector3 pos, Dictionary<Vector3, Block> chunk) : base(type, visible, pos, chunk) {
	}

	public GameObject getClone() {
		return this.clone;
	}

	public override void setClone(GameObject clone) {
		this.clone = clone;
	}

	public override void generateMesh(MeshData meshData) {

		Vector3 frontDir = new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z - 1);
		Vector3 backDir = new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z + 1);
		Vector3 leftDir = new Vector3 ((int)pos.x - 1, (int)pos.y, (int)pos.z);
		Vector3 rightDir = new Vector3 ((int)pos.x + 1, (int)pos.y, (int)pos.z);
		Vector3 topDir = new Vector3 ((int)pos.x, (int)pos.y + 1, (int)pos.z);
		Vector3 bottomDir = new Vector3 ((int)pos.x, (int)pos.y - 1, (int)pos.z);

		if (!(chunk.ContainsKey (frontDir) &&
			chunk[frontDir] is CubeBlock)) {
			meshData.FaceDataZPositive (this);
		}
		if (!(chunk.ContainsKey (backDir)  &&
			chunk[backDir] is CubeBlock)) {
			meshData.FaceDataZNegative (this);
		}
		if (!(chunk.ContainsKey (rightDir) && 
			chunk[rightDir] is CubeBlock)) {
			meshData.FaceDataXPositive (this);
		}
		if (!(chunk.ContainsKey (leftDir) &&
			chunk[leftDir] is CubeBlock)) {
			meshData.FaceDataXNegative (this);
		}
		if (!(chunk.ContainsKey (bottomDir) && 
			chunk[bottomDir] is CubeBlock)) {
			meshData.FaceDataYNegative (this);
		}
		if (!(chunk.ContainsKey (topDir) && 
			chunk[topDir] is CubeBlock)) {
			meshData.FaceDataYPositive (this);
		}
	}
}

public class Fern: Block {
	public Fern(bool visible, Vector3 pos, Dictionary<Vector3, Block> chunk) : base(Type.PLANT_FERN, visible, pos, chunk) {
	}

	public override void setClone(GameObject clone) {
		this.clone = clone;
	}

	public override void generateMesh (MeshData meshData) {
		// nothing happened
		return;
	}
}

public class Rose: Block {
	public Rose(bool visible, Vector3 pos, Dictionary<Vector3, Block> chunk) : base(Type.PLANT_FERN, visible, pos, chunk) {
	}

	public override void setClone(GameObject clone) {
		this.clone = clone;
	}

	public override void generateMesh (MeshData meshData) {
		// nothing happened
		return;
	}
}

public class Grass: CubeBlock {
	public Grass(bool visible, Vector3 pos, Dictionary<Vector3, Block> chunk) : base(Type.GRASS, visible, pos, chunk) {
		this.blockTile = new BlockTile (3, 0, 3, 0, 0, 0, 2, 0, 3, 0, 3, 0);
	}
}

public class Dirt: CubeBlock {
	public Dirt(bool visible, Vector3 pos, Dictionary<Vector3, Block> chunk) : base(Type.DIRT, visible, pos, chunk) {
		this.blockTile = new BlockTile (2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0);
	}
}

public class Snow: CubeBlock {
	public Snow(bool visible, Vector3 pos, Dictionary<Vector3, Block> chunk) : base(Type.SNOW, visible, pos, chunk) {
		this.blockTile = new BlockTile (2, 4, 2, 4, 2, 4, 2, 4, 2, 4, 2, 4);
	}
}

public class Sand: CubeBlock {
	public Sand(bool visible, Vector3 pos, Dictionary<Vector3, Block> chunk) : base(Type.SAND, visible, pos, chunk) {
		this.blockTile = new BlockTile (2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1);
	}
}
