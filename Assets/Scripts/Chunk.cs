using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using SimplexNoise;


[Serializable]
public class Chunk {
	// public Dictionary<BlockPos, Block> blocks;
	public Block[,,] blocks;
	public bool needRender; // if == true, then the chunk will be renered

	// eg: (0, 0, 0) (1, 0, 0)
	public int chunkX;
	public int chunkY;
	public int chunkZ;

	// in world coord
	public int x;
	public int y;
	public int z;

	public bool changed;  // 如果有改变了的话，Serialization 才会存
	public bool rendered;

	// sea level is at 0
	public static int width = 16;
	public static int height = 32;
	public static int depth = 16;

	public int airNum = 0;

	//public World world;  // 这个其实没有必要， world 就只有一个，就是现在游戏中的 world
	//public Water water;

	public Chunk(int chunkX, int chunkY, int chunkZ) {
		// blocks = new Dictionary<BlockPos, Block> ();
		blocks = new Block[Chunk.width, Chunk.height, Chunk.depth];
		for (int x = 0; x < Chunk.width; x++) {
			for (int y = 0; y < Chunk.height; y++) {
				for (int z = 0; z < Chunk.depth; z++) {
					blocks [x, y, z] = new Air ();
				}
			}
		}

		this.chunkX = chunkX;
		this.chunkY = chunkY;
		this.chunkZ = chunkZ;

		this.x = chunkX * Chunk.width;
		this.y = chunkY * Chunk.height;
		this.z = chunkZ * Chunk.depth;

		needRender = false;
		changed = false;
		rendered = false; 
	}
		
	public static int GetNoise(int x, int y, int z, float scale, int max) {
		return Mathf.FloorToInt( (Noise.Generate(x * scale, y * scale, z * scale) + 1f) * (max/2f));
	}

	public void addBlock(Vector3 pos, Block block) {
		blocks[(int)pos.x - x, (int)pos.y - y, (int)pos.z - z] = block;
		block.chunk = this;
		changed = true; 
		airNum -= 1;
	}

	public Block getBlock(Vector3 pos) {
		return blocks [(int)pos.x - x, (int)pos.y - y, (int)pos.z - z];
	}

	public void removeBlock(Vector3 pos) {
		// blocks.Remove (new BlockPos(pos));
		blocks[(int)pos.x - x, (int)pos.y - y, (int)pos.z - z] = new Air();
		airNum += 1;

		changed = true;
	}

	public bool isAllAir() {
		return airNum == Chunk.width * Chunk.height * Chunk.depth;
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

		int startX = width * chunkX;
		int startY = height * chunkY;
		int startZ = depth * chunkZ;

		/*
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
		*/

		for (int z = 0; z < depth; z++) {
			for (int x = 0; x < width; x++) {
				int y = 15;
				//Vector3 blockPos = new Vector3 (x+startX, y+startY, z+startZ);
				//createBlock (blockPos, false);
				while (y >= 0) {
					// y--;
					Vector3 blockPos = new Vector3 (x+startX, y+startY, z+startZ);
					createBlock (blockPos, false);
					y--;
				}
			}
		}
						
		changed = false; // this chunk hasn't been changed by player...
	}
		

	void createBlock(Vector3 blockPos, bool isTop) {
		int y = (int)blockPos.y;
		if (y < -128)
			return;
		
		Block block;

		if (y > 42) {
			block = new Air ();
			airNum += 1;
		} else if (y > 38) {
			block = new Snow ();
		} else if (isTop && y > 5) {
			block = new Grass ();
			/*
			if (UnityEngine.Random.Range (0, 10) <= 2) {
				addBlock (blockPos + new Vector3 (0, 1, 0), new Fern ());
			} else if (UnityEngine.Random.Range (0, 50) <= 2) {
				addBlock (blockPos + new Vector3 (0, 1, 0), new Rose ());
			}
			*/
		} else if (y > 5) {
			block = new Dirt ();
		} else if (y == -128) {
			block = new BedRock ();
		} else {
			block = new Sand ();
		}
			
		addBlock(blockPos, block);
	}
}
