using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Chunk {
	public Dictionary<BlockPos, Block> blocks;
	public bool needRender; // if == true, then the chunk will be renered
	public int chunkX;
	public int chunkZ;
	public bool changed;  // 如果有改变了的话，Serialization 才会存
	public bool rendered;

	//public World world;  // 这个其实没有必要， world 就只有一个，就是现在游戏中的 world
	//public Water water;

	public Chunk(int chunkX, int chunkZ) {
		blocks = new Dictionary<BlockPos, Block> ();
		this.chunkX = chunkX;
		this.chunkZ = chunkZ;

		needRender = false;
		changed = false;
		rendered = false; 
	}

	public void addBlock(Vector3 pos, Block block) {
		blocks.Add (new BlockPos(pos), block);
		block.chunk = this;
		changed = true; 
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

	// TODO: move this to terrain generation in the future.
	public void generateHeightMap(World world) {
		// public int heightScale = 20;
		// public float detailScale = 25.0f;

		// mountain
		// public int heightScale = 40; // 40+
		// public float detailScale = 30.0f;

		// plain 1
		//public int heightScale = 40; // 40+
		//public float detailScale = 60.0f;

		// plain 2
		int heightScale = 20; // 40+
		float detailScale = 70.0f;

		int width = Generate_Landscape.chunkWidth; 
		int depth = Generate_Landscape.chunkDepth;

		int startX = width * chunkX;
		int startZ = depth * chunkZ;

		int seed = world.worldData.seed;

		for (int z = 0; z < depth; z++) {
			for (int x = 0; x < width; x++) {
				int y; 

				if (seed == 0) {
					y = 10;
				} else {
					y = (int)(Mathf.PerlinNoise ((x+seed+startX)/detailScale, (z+seed+startZ)/detailScale) * heightScale); 
				}

				Vector3 blockPos = new Vector3 (x+startX, y, z+startZ);
				createBlock (blockPos, true);
				while (y > 0) {
					y--; 
					blockPos = new Vector3 (x+startX, y, z+startZ);
					createBlock (blockPos, false);
				}
			}
		}

		changed = false; // this chunk hasn't been changed by player...
	}

	void createBlock(Vector3 blockPos, bool isTop) {
		int y = (int)blockPos.y;
		Block block;

		if (y > 38) {
			block = new Snow ();
		} else if (isTop && y > 5) {
			block = new Grass ();
			if (UnityEngine.Random.Range (0, 10) <= 2) {
				addBlock (blockPos + new Vector3 (0, 1, 0), new Fern ());
			} else if (UnityEngine.Random.Range (0, 50) <= 2) {
				addBlock (blockPos + new Vector3 (0, 1, 0), new Rose ());
			}
			/*		 
			if (this.rnd.Next(0, 10) <= 2) { // FERN
				Vector3 pos = blockPos + new Vector3 (0, 1, 0);
				Block fern = new Fern(true, pos);
				chunk.Add(pos, fern);
			} else if (this.rnd.Next(0, 50) <= 5) {
				Vector3 pos = blockPos + new Vector3 (0, 1, 0);
				Block rose = new Rose(true, pos);
				chunk.Add(pos, rose);
			}
			*/
		} else if (y > 5) {
			block = new Dirt ();
		} else {
			block = new Sand ();
		}

		addBlock(blockPos, block);
	}

	public Vector3 ToWorldCoordinate() {
		return new Vector3 (chunkX * Generate_Landscape.chunkWidth, 0, chunkZ * Generate_Landscape.chunkDepth);
	}
}
