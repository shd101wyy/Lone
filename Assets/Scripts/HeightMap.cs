using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Chunk {
	public Dictionary<Vector3, Block> blocks;
	public bool needRender;
	public int chunkX;
	public int chunkZ;
	public World world;
	public Water water;

	public Chunk(int chunkX, int chunkZ, World world) {
		blocks = new Dictionary<Vector3, Block> ();
		needRender = false;
		this.chunkX = chunkX;
		this.chunkZ = chunkZ;
		this.world = world;
	}

	public void addBlock(Vector3 pos, Block block) {
		blocks.Add (pos, block);
		block.chunk = this;
		block.pos = pos;
		block.world = world;

		if (block is Water) {
			this.water = block as Water;
		}
	}

	public Block getBlock(Vector3 pos) {
		if (!blocks.ContainsKey (pos))
			return null;
		return blocks [pos];
	}

	public void removeBlock(Vector3 pos) {
		blocks.Remove (pos);
	}

	public bool hasBlockAtPosition(Vector3 pos) {
		return blocks.ContainsKey (pos);
	}
}

public class HeightMap {
	public int chunkX;
	public int chunkZ;

	// public int heightScale = 20;
	// public float detailScale = 25.0f;

	// mountain
	public int heightScale = 40; // 40+
	public float detailScale = 30.0f;

	// plain 1
	//public int heightScale = 40; // 40+
	//public float detailScale = 60.0f;

	// plain 2
	// public int heightScale = 20; // 40+
	// public float detailScale = 70.0f;

	public Chunk chunk;

	public int seed;

	private System.Random rnd;

	public HeightMap (int chunkX, int chunkZ, int seed, World world) {
		this.chunkX = chunkX;
		this.chunkZ = chunkZ;
		this.seed = seed;

		rnd = new System.Random ();

		chunk = new Chunk(chunkX, chunkZ, world);
	}

	public void generateHeightMap() {
		int width = Generate_Landscape.chunkWidth; 
		int depth = Generate_Landscape.chunkDepth;

		int startX = width * chunkX;
		int startZ = depth * chunkZ;

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
	}

	void createBlock(Vector3 blockPos, bool isTop) {
		int y = (int)blockPos.y;
		Block block;

		if (y > 38) {
			block = new Snow ();
		} else if (isTop && y > 5) {
			block = new Grass ();
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

		chunk.addBlock(blockPos, block);
	}
}

