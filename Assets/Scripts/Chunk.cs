using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using SimplexNoise;


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

	public static int GetNoise(int x, int y, int z, float scale, int max) {
		return Mathf.FloorToInt( (Noise.Generate(x * scale, y * scale, z * scale) + 1f) * (max/2f));
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
				generateColumn (x + startX, z + startZ);
			}
		}

		changed = false; // this chunk hasn't been changed by player...
	}

	void generateColumn(int x, int z) {
		float stoneBaseHeight = 32;
		float stoneBaseNoise = 0.05f;
		float stoneBaseNoiseHeight = 4;

		float stoneMountainHeight = 48;
		float stoneMountainFrequency = 0.008f;
		float stoneMinHeight = -12;

		float dirtBaseHeight = 1;
		float dirtNoise = 0.04f;
		float dirtNoiseHeight = 3;

		int stoneHeight = Mathf.FloorToInt(stoneBaseHeight);
		stoneHeight += GetNoise(x, 32, z, stoneMountainFrequency, Mathf.FloorToInt(stoneMountainHeight));

		if (stoneHeight < stoneMinHeight)
			stoneHeight = Mathf.FloorToInt(stoneMinHeight);

		stoneHeight += GetNoise(x, 32, z, stoneBaseNoise, Mathf.FloorToInt(stoneBaseNoiseHeight));

		int dirtHeight = stoneHeight + Mathf.FloorToInt(dirtBaseHeight);
		dirtHeight += GetNoise(x, 64, z, dirtNoise, Mathf.FloorToInt(dirtNoiseHeight));


		Block topBlock = null;
		Vector3 topBlockPos = Vector3.zero;
		for (int y = 0; y < Generate_Landscape.chunkHeight; y++) {
			Vector3 blockPos = new Vector3 (x, y, z);
			if (y == 0) {
				// addBlock(blockPos, new BedRock());
				topBlock = new Grass();
				topBlockPos = blockPos;
				addBlock (blockPos, topBlock);
			} else if (y <= stoneHeight) {
				topBlock = new Stone ();
				addBlock (blockPos, topBlock);
			} else if (y <= dirtHeight) {
				if (y == dirtHeight) {
					topBlock = new Grass ();
					topBlockPos = blockPos;
					addBlock (blockPos, topBlock);
				} else {
					topBlock = new Dirt ();
					addBlock (blockPos, topBlock);
				}
			} else {
			}
		}

		if (topBlock is Grass) {
			if (UnityEngine.Random.Range (0, 10) <= 2) {
				addBlock (topBlockPos + new Vector3 (0, 1, 0), new Fern ());
			} else if (UnityEngine.Random.Range (0, 50) <= 2) {
				addBlock (topBlockPos + new Vector3 (0, 1, 0), new Rose ());
			}
		}
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
