using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;


public enum Type {GRASS, SAND, SNOW, DIRT, PLANT_FERN, WATER, LOG_JUNGLE, PLANKS_JUNGLE, LOG_OAK, PLANKS_OAK};

[Serializable]
public abstract class Block {
	public Type type;
	public Chunk chunk;

	public float blockSize = 1f;

	public Block(Type type) {
		this.type = type;
		this.chunk = null;
	}

	public abstract void generateMesh (MeshData meshData, Vector3 pos, World world, bool collidable = true);

	public abstract Texture2D getTexture ();

	public abstract BlockTile getBlockTile ();
}

[Serializable]
public class CubeBlock: Block {
	public CubeBlock(Type type) : base(type) {
	}

	public override void generateMesh(MeshData meshData, Vector3 pos, World world, bool collidable = true) {

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
			meshData.FaceDataZPositive (this, pos);
		}
		if (!(back != null && back is CubeBlock)) {
			meshData.FaceDataZNegative (this, pos);
		}
		if (!(right != null && right is CubeBlock)) {
			meshData.FaceDataXPositive (this, pos);
		}
		if (!(left != null && left is CubeBlock)) {
			meshData.FaceDataXNegative (this, pos);
		}
		if (!(bottom != null && bottom is CubeBlock)) {
			meshData.FaceDataYNegative (this, pos);
		}
		if (!(top != null && top is CubeBlock)) {
			meshData.FaceDataYPositive (this, pos);
		}

		meshData.useRenderDataForCollision = col;
	}

	public override Texture2D getTexture () {
		throw new NotImplementedException ();
	}

	public override BlockTile getBlockTile () {
		throw new NotImplementedException ();
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

[Serializable]
public class Grass: CubeBlock {
	public Grass() : base(Type.GRASS) {
	}

	public override Texture2D getTexture () {
		return Game.textures ["dirt"].texture_2d;
	}

	public override BlockTile getBlockTile (){
		SpriteData top = Game.textures ["grass_top"];
		SpriteData side = Game.textures ["grass_side"];
		SpriteData bottom = Game.textures ["dirt"];
		return new BlockTile (side, side, top, bottom, side, side);
	}
}

[Serializable]
public class Dirt: CubeBlock {
	public Dirt() : base(Type.DIRT) {
	}

	public override Texture2D getTexture () {
		return Game.textures ["dirt"].texture_2d;
	}

	public override BlockTile getBlockTile (){
		SpriteData side = Game.textures ["dirt"];
		return new BlockTile (side, side, side, side, side, side);
	}
}

[Serializable]
public class Snow: CubeBlock {
	public Snow() : base(Type.SNOW) {
	}

	public override Texture2D getTexture () {
		return Game.textures["snow"].texture_2d;
	}

	public override BlockTile getBlockTile () {
		SpriteData side = Game.textures ["snow"];
		return new BlockTile (side, side, side, side, side, side);
	}
}

[Serializable]
public class Sand: CubeBlock {
	public Sand() : base(Type.SAND) {
	}

	public override Texture2D getTexture () {
		return Game.textures ["sand"].texture_2d;
	}

	public override BlockTile getBlockTile () {
		SpriteData side = Game.textures ["sand"];
		return new BlockTile (side, side, side, side, side, side);
	}
}

[Serializable]
public class LogJungle: CubeBlock {
	public LogJungle() : base(Type.LOG_JUNGLE) {
	}

	public override Texture2D getTexture () {
		return Game.textures ["log_jungle"].texture_2d;
	}

	public override BlockTile getBlockTile () {
		SpriteData side = Game.textures ["log_jungle"];
		SpriteData top = Game.textures ["log_jungle_top"];
		return new BlockTile (side, side, top, top, side, side);
	}
}

[Serializable]
public class PlanksJungle: CubeBlock {
	public PlanksJungle() : base(Type.PLANKS_JUNGLE) {
	}

	public override Texture2D getTexture () {
		return Game.textures ["planks_jungle"].texture_2d;
	}

	public override BlockTile getBlockTile () {
		SpriteData side = Game.textures ["planks_jungle"];
		return new BlockTile (side, side, side, side, side, side);
	}
}

[Serializable]
public class LogOak: CubeBlock {
	public LogOak() : base(Type.LOG_OAK) {
	}

	public override Texture2D getTexture () {
		return Game.textures ["log_oak"].texture_2d;
	}

	public override BlockTile getBlockTile () {
		SpriteData side = Game.textures ["log_oak"];
		SpriteData top = Game.textures ["log_oak_top"];
		return new BlockTile (side, side, top, top, side, side);
	}
}

[Serializable]
public class PlanksOak: CubeBlock {
	public PlanksOak() : base(Type.PLANKS_OAK) {
	}

	public override Texture2D getTexture () {
		return Game.textures ["planks_oak"].texture_2d;
	}

	public override BlockTile getBlockTile () {
		SpriteData side = Game.textures ["planks_oak"];
		return new BlockTile (side, side, side, side, side, side);
	}
}

// TODO: the water below is super bad
public class Water: Block {
	public int energy;
	public Water(int energy = 5) : base(Type.WATER) {
		this.energy = energy;

		// Debug.Log ("energy : " + energy);
	}
	public override void generateMesh(MeshData meshData, Vector3 pos, World world, bool collidable = false) {
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
			meshData.FaceDataZPositive (this, pos);
		}
		if (!(back != null && back is CubeBlock)) {
			meshData.FaceDataZNegative (this, pos);
		}
		if (!(right != null && right is CubeBlock)) {
			meshData.FaceDataXPositive (this, pos);
		}
		if (!(left != null && left is CubeBlock)) {
			meshData.FaceDataXNegative (this, pos);
		}
		if (!(bottom != null && bottom is CubeBlock)) {
			meshData.FaceDataYNegative (this, pos);
		}
		if (!(top != null && top is CubeBlock)) {
			meshData.FaceDataYPositive (this, pos);
		}

		meshData.useRenderDataForCollision = col;
	}

	public override Texture2D getTexture () {
		return Game.textures ["dirt"].texture_2d;
	}

	public override BlockTile getBlockTile ()
	{
		SpriteData side = Game.textures ["dirt"]; // TODO: change later
		return new BlockTile (side, side, side, side, side, side);
	}
}
