using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public enum BlockType {GRASS, SAND, SNOW, DIRT, PLANT_FERN, WATER, LOG_JUNGLE, PLANKS_JUNGLE, LOG_OAK, PLANKS_OAK, TNT, STONE, BEDROCK};

/*
 * Only Block type object can be put on chunk 
 */
[Serializable]
public abstract class Block : Item {
	public Chunk chunk;

	public Block(string blockName, ItemType itemType, int maxStack=64) : base(blockName, itemType, maxStack) {
		this.chunk = null;
	}
		
	public override Texture2D getTexture () {
		throw new NotImplementedException ();
	}
}

[Serializable]
public abstract class CubeBlock: Block {
	public BlockType blockType;
	public float blockSize = 1f;

	public CubeBlock(BlockType blockType, string blockName) : base(blockName, ItemType.CUBE_BLOCK, 64) {
		this.blockType = blockType;
	}
		
	public abstract BlockTile getBlockTile ();

	public void generateMesh(MeshData meshData, Vector3 pos, World world, bool collidable = true, bool dropItem = false) {

		bool col = meshData.useRenderDataForCollision;
		meshData.useRenderDataForCollision = collidable;

		Vector3 frontDir = new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z - 1);
		Vector3 backDir = new Vector3 ((int)pos.x, (int)pos.y, (int)pos.z + 1);
		Vector3 leftDir = new Vector3 ((int)pos.x - 1, (int)pos.y, (int)pos.z);
		Vector3 rightDir = new Vector3 ((int)pos.x + 1, (int)pos.y, (int)pos.z);
		Vector3 topDir = new Vector3 ((int)pos.x, (int)pos.y + 1, (int)pos.z);
		Vector3 bottomDir = new Vector3 ((int)pos.x, (int)pos.y - 1, (int)pos.z);

		Block front = world == null ? null : world.getBlock (frontDir);
		Block back = world == null ? null :  world.getBlock (backDir);
		Block left = world == null ? null :  world.getBlock (leftDir);
		Block right = world == null ? null :  world.getBlock (rightDir);
		Block top = world == null ? null :  world.getBlock (topDir);
		Block bottom = world == null ? null :  world.getBlock (bottomDir);

		if (!(front != null && front is CubeBlock)) {
			meshData.FaceDataZPositive (this, pos, dropItem);
		}
		if (!(back != null && back is CubeBlock)) {
			meshData.FaceDataZNegative (this, pos, dropItem);
		}
		if (!(right != null && right is CubeBlock)) {
			meshData.FaceDataXPositive (this, pos, dropItem);
		}
		if (!(left != null && left is CubeBlock)) {
			meshData.FaceDataXNegative (this, pos, dropItem);
		}
		if (!(bottom != null && bottom is CubeBlock)) {
			meshData.FaceDataYNegative (this, pos, dropItem);
		}
		if (!(top != null && top is CubeBlock)) {
			meshData.FaceDataYPositive (this, pos, dropItem);
		}

		meshData.useRenderDataForCollision = col;
	}

	public override Texture2D getTexture () {
		throw new NotImplementedException ();
	}

	public virtual BlockTile getDropBlockTile () {  // DropItem BlockTile
		return getBlockTile ();
	}
}

[Serializable]
public class Grass: CubeBlock {
	public Grass() : base(BlockType.GRASS, "grass") {
	}

	public override Texture2D getTexture () {
		return Game.textures ["dirt"].texture_2d;
	}

	public override BlockTile getBlockTile () {
		SpriteData top = Game.textures ["grass_top"];
		SpriteData side = Game.textures ["grass_side"];
		SpriteData bottom = Game.textures ["dirt"];
		return new BlockTile (side, side, top, bottom, side, side);
	}

	public override BlockTile getDropBlockTile () {
		SpriteData side = Game.textures ["dirt"];
		return new BlockTile (side, side, side, side, side, side);
	}
}

[Serializable]
public class Dirt: CubeBlock {
	public Dirt() : base(BlockType.DIRT,  "dirt") {
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
	public Snow() : base(BlockType.SNOW, "snow") {
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
	public Sand() : base(BlockType.SAND, "sand") {
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
	public LogJungle() : base(BlockType.LOG_JUNGLE, "log_jungle") {
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
	public PlanksJungle() : base(BlockType.PLANKS_JUNGLE, "planks_jungle") {
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
	public LogOak() : base(BlockType.LOG_OAK, "log_oak") {
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
	public PlanksOak() : base(BlockType.PLANKS_OAK, "planks_oak") {
	}

	public override Texture2D getTexture () {
		return Game.textures ["planks_oak"].texture_2d;
	}

	public override BlockTile getBlockTile () {
		SpriteData side = Game.textures ["planks_oak"];
		return new BlockTile (side, side, side, side, side, side);
	}
}

[Serializable]
public class TNT: CubeBlock {
	public TNT() : base(BlockType.TNT, "tnt") {
	}

	public override Texture2D getTexture () {
		return Game.textures ["tnt_side"].texture_2d;
	}

	public override BlockTile getBlockTile () {
		SpriteData side = Game.textures ["tnt_side"];
		SpriteData top = Game.textures ["tnt_top"];
		SpriteData bottom = Game.textures ["tnt_bottom"];
		return new BlockTile (side, side, top, bottom, side, side);
	}
}

[Serializable]
public class Stone: CubeBlock {
	public Stone() : base(BlockType.STONE, "stone") {
	}

	public override Texture2D getTexture () {
		return Game.textures ["stone"].texture_2d;
	}

	public override BlockTile getBlockTile () {
		SpriteData side = Game.textures ["stone"];
		return new BlockTile (side, side, side, side, side, side);
	}
}

[Serializable]
public class BedRock: CubeBlock {
	public BedRock() : base(BlockType.BEDROCK, "bedrock") {
	}

	public override Texture2D getTexture () {
		return Game.textures ["bedrock"].texture_2d;
	}

	public override BlockTile getBlockTile () {
		SpriteData side = Game.textures ["bedrock"];
		return new BlockTile (side, side, side, side, side, side);
	}
}

// TODO: the water below is super bad
/*
public class Water: Block {
	public int energy;
	public Water(int energy = 5) : base(BlockType.WATER, "water") {
		this.energy = energy;

		// Debug.Log ("energy : " + energy);
	}
	public override void generateMesh(MeshData meshData, Vector3 pos, World world, bool collidable = false, bool dropItem = false) {
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

	public override BlockTile getDropBlockTile ()
	{
		throw new NotImplementedException ();
	}
}
*/