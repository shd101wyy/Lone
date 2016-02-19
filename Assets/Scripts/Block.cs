using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;


public enum Type {GRASS, SAND, SNOW, DIRT, PLANT_FERN, WATER, LOG_JUNGLE};

[Serializable]
public abstract class Block {
	public Type type;
	public Vector3 pos;
	public Chunk chunk;
	public World world;


	public float blockSize = 1f;
	public BlockTile blockTile;

	public Block(Type type) {
		this.type = type;
		this.pos = Vector3.zero;
		this.chunk = null;
	}

	public abstract void generateMesh (MeshData meshData, bool collidable = true);
}

public class CubeBlock: Block {
	public CubeBlock(Type type) : base(type) {
	}
		
	public override void generateMesh(MeshData meshData, bool collidable = true) {

		bool col = meshData.useRenderDataForCollision;
		meshData.useRenderDataForCollision = collidable;

		Vector3 frontDir = new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z - 1);
		Vector3 backDir = new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z + 1);
		Vector3 leftDir = new Vector3 ((int)pos.x - 1, (int)pos.y, (int)pos.z);
		Vector3 rightDir = new Vector3 ((int)pos.x + 1, (int)pos.y, (int)pos.z);
		Vector3 topDir = new Vector3 ((int)pos.x, (int)pos.y + 1, (int)pos.z);
		Vector3 bottomDir = new Vector3 ((int)pos.x, (int)pos.y - 1, (int)pos.z);

		Block front = world.getBlock (frontDir);
		Block back = world.getBlock (backDir);
		Block left = world.getBlock (leftDir);
		Block right = world.getBlock (rightDir);
		Block top = world.getBlock (topDir);
		Block bottom = world.getBlock (bottomDir);

		if (!(front != null && front is CubeBlock)) {
			meshData.FaceDataZPositive (this);
		}
		if (!(back != null && back is CubeBlock)) {
			meshData.FaceDataZNegative (this);
		}
		if (!(right != null && right is CubeBlock)) {
			meshData.FaceDataXPositive (this);
		}
		if (!(left != null && left is CubeBlock)) {
			meshData.FaceDataXNegative (this);
		}
		if (!(bottom != null && bottom is CubeBlock)) {
			meshData.FaceDataYNegative (this);
		}
		if (!(top != null && top is CubeBlock)) {
			meshData.FaceDataYPositive (this);
		}

		meshData.useRenderDataForCollision = col;
	}
}

public class Fern: Block {
	public Fern() : base(Type.PLANT_FERN) {
	}
		
	public override void generateMesh (MeshData meshData, bool collidable = false) {
		// nothing happened
		return;
	}
}

public class Rose: Block {
	public Rose() : base(Type.PLANT_FERN) {
	}

	public override void generateMesh (MeshData meshData, bool collidable = false) {
		// nothing happened
		return;
	}
}

public class Grass: CubeBlock {
	public Grass() : base(Type.GRASS) {
		SpriteData top = Game.textures ["grass_top"];
		SpriteData side = Game.textures ["grass_side"];
		SpriteData bottom = Game.textures ["dirt"];
		this.blockTile = new BlockTile (side, side, top, bottom, side, side); 
	}
}

public class Dirt: CubeBlock {
	public Dirt() : base(Type.DIRT) {
		SpriteData side = Game.textures ["dirt"];
		this.blockTile = new BlockTile (side, side, side, side, side, side);
	}
}

public class Snow: CubeBlock {
	public Snow() : base(Type.SNOW) {
		SpriteData side = Game.textures ["snow"];
		this.blockTile = new BlockTile (side, side, side, side, side, side);
	}
}

public class Sand: CubeBlock {
	public Sand() : base(Type.SAND) {
		SpriteData side = Game.textures ["sand"];
		this.blockTile = new BlockTile (side, side, side, side, side, side);
	}
}

public class LogJungle: CubeBlock {
	public LogJungle() : base(Type.LOG_JUNGLE) {
		SpriteData side = Game.textures ["log_jungle"];
		SpriteData top = Game.textures ["log_jungle_top"];
		this.blockTile = new BlockTile (side, side, top, top, side, side);
	}
}

public class Water: Block {
	public int energy;
	public Water(int energy = 5) : base(Type.WATER) {
		SpriteData side = Game.textures ["dirt"]; // TODO: change later
		this.blockTile = new BlockTile (side, side, side, side, side, side);
		this.energy = energy;

		// Debug.Log ("energy : " + energy);
	}
	public override void generateMesh(MeshData meshData, bool collidable = false) {
		collidable = false;
		bool col = meshData.useRenderDataForCollision;
		meshData.useRenderDataForCollision = collidable;

		Vector3 frontDir = new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z - 1);
		Vector3 backDir = new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z + 1);
		Vector3 leftDir = new Vector3 ((int)pos.x - 1, (int)pos.y, (int)pos.z);
		Vector3 rightDir = new Vector3 ((int)pos.x + 1, (int)pos.y, (int)pos.z);
		Vector3 topDir = new Vector3 ((int)pos.x, (int)pos.y + 1, (int)pos.z);
		Vector3 bottomDir = new Vector3 ((int)pos.x, (int)pos.y - 1, (int)pos.z);

		Block front = world.getBlock (frontDir);
		Block back = world.getBlock (backDir);
		Block left = world.getBlock (leftDir);
		Block right = world.getBlock (rightDir);
		Block top = world.getBlock (topDir);
		Block bottom = world.getBlock (bottomDir);

		if (!(front != null && front is CubeBlock)) {
			meshData.FaceDataZPositive (this);
		}
		if (!(back != null && back is CubeBlock)) {
			meshData.FaceDataZNegative (this);
		}
		if (!(right != null && right is CubeBlock)) {
			meshData.FaceDataXPositive (this);
		}
		if (!(left != null && left is CubeBlock)) {
			meshData.FaceDataXNegative (this);
		}
		if (!(bottom != null && bottom is CubeBlock)) {
			meshData.FaceDataYNegative (this);
		}
		if (!(top != null && top is CubeBlock)) {
			meshData.FaceDataYPositive (this);
		}

		meshData.useRenderDataForCollision = col;
	}
}
