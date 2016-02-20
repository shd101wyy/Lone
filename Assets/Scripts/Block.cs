using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;


public enum Type {GRASS, SAND, SNOW, DIRT, PLANT_FERN, WATER, LOG_JUNGLE, PLANKS_JUNGLE, LOG_OAK, PLANKS_OAK};

[Serializable]
public abstract class Block {
	public Type type;
	public Vector3 pos;
	public Chunk chunk;
	public World world;
	public Texture2D texture_2d;


	public float blockSize = 1f;
	public BlockTile blockTile;

	public Block(Type type, Texture2D texture_2d) {
		this.type = type;
		this.pos = Vector3.zero;
		this.chunk = null;
		this.texture_2d = texture_2d;
	}

	public abstract void generateMesh (MeshData meshData, bool collidable = true);
}

public class CubeBlock: Block {
	public CubeBlock(Type type, Texture2D texture_2d) : base(type, texture_2d) {
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

/*
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
*/

public class Grass: CubeBlock {
	public Grass() : base(Type.GRASS, Game.textures["dirt"].texture_2d) {
		SpriteData top = Game.textures ["grass_top"];
		SpriteData side = Game.textures ["grass_side"];
		SpriteData bottom = Game.textures ["dirt"];
		this.blockTile = new BlockTile (side, side, top, bottom, side, side); 
	}
}

public class Dirt: CubeBlock {
	public Dirt() : base(Type.DIRT, Game.textures["dirt"].texture_2d) {
		SpriteData side = Game.textures ["dirt"];
		this.blockTile = new BlockTile (side, side, side, side, side, side);
	}
}

public class Snow: CubeBlock {
	public Snow() : base(Type.SNOW, Game.textures["snow"].texture_2d) {
		SpriteData side = Game.textures ["snow"];
		this.blockTile = new BlockTile (side, side, side, side, side, side);
	}
}

public class Sand: CubeBlock {
	public Sand() : base(Type.SAND, Game.textures["sand"].texture_2d) {
		SpriteData side = Game.textures ["sand"];
		this.blockTile = new BlockTile (side, side, side, side, side, side);
	}
}

public class LogJungle: CubeBlock {
	public LogJungle() : base(Type.LOG_JUNGLE, Game.textures["log_jungle"].texture_2d) {
		SpriteData side = Game.textures ["log_jungle"];
		SpriteData top = Game.textures ["log_jungle_top"];
		this.blockTile = new BlockTile (side, side, top, top, side, side);
	}
}

public class PlanksJungle: CubeBlock {
	public PlanksJungle() : base(Type.PLANKS_JUNGLE, Game.textures["planks_jungle"].texture_2d) {
		SpriteData side = Game.textures ["planks_jungle"];
		this.blockTile = new BlockTile (side, side, side, side, side, side);
	}
}

public class LogOak: CubeBlock {
	public LogOak() : base(Type.LOG_OAK, Game.textures["log_oak"].texture_2d) {
		SpriteData side = Game.textures ["log_oak"];
		SpriteData top = Game.textures ["log_oak_top"];
		this.blockTile = new BlockTile (side, side, top, top, side, side);
	}
}

public class PlanksOak: CubeBlock {
	public PlanksOak() : base(Type.PLANKS_OAK, Game.textures["planks_oak"].texture_2d) {
		SpriteData side = Game.textures ["planks_oak"];
		this.blockTile = new BlockTile (side, side, side, side, side, side);
	}
}

// TODO: the water below is super bad
public class Water: Block {
	public int energy;
	public Water(int energy = 5) : base(Type.WATER, Game.textures["dirt"].texture_2d) {
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
