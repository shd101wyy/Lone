using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Chunk {
	public Dictionary<BlockPos, Block> blocks;
	public bool needRender;
	public int chunkX;
	public int chunkZ;
	public bool changed;  // 如果有改变了的话，Serialization 才会存

	//public World world;  // 这个其实没有必要， world 就只有一个，就是现在游戏中的 world
	//public Water water;

	public Chunk(int chunkX, int chunkZ, World world) {
		blocks = new Dictionary<BlockPos, Block> ();
		needRender = false;
		changed = false;
		this.chunkX = chunkX;
		this.chunkZ = chunkZ;
	}

	public void addBlock(Vector3 pos, Block block) {
		blocks.Add (new BlockPos(pos), block);
		block.chunk = this;
		changed = true; 
		// block.world = world;

		//if (block is Water) {
		//	this.water = block as Water;
		//}
	}

	public Block getBlock(Vector3 pos) {
		if (!blocks.ContainsKey (new BlockPos(pos)))
			return null;
		return blocks [new BlockPos(pos)];
	}

	public void removeBlock(Vector3 pos) {
		blocks.Remove (new BlockPos(pos));
		changed = true;
	}

	public bool hasBlockAtPosition(Vector3 pos) {
		return blocks.ContainsKey (new BlockPos(pos));
	}
}
