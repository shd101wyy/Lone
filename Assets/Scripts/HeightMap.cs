using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HeightMap {
	public int chunkX;
	public int chunkZ;

	public int width;
	public int depth;

	// public int heightScale = 20;
	// public float detailScale = 25.0f;

	// mountain
	// public int heightScale = 40; // 40+
	// public float detailScale = 30.0f;

	// plain 1
	//public int heightScale = 40; // 40+
	//public float detailScale = 60.0f;

	// plain 2
	public int heightScale = 20; // 40+
	public float detailScale = 70.0f;

	public Dictionary<Vector3, Block> chunk;
	public ArrayList visibleBlocks;

	public int seed;

	private System.Random rnd;

	public HeightMap (int chunkX, int chunkZ, int chunkWidth, int chunkDepth, int seed) {
		this.width = chunkWidth;
		this.depth = chunkDepth;
		this.chunkX = chunkX;
		this.chunkZ = chunkZ;
		this.seed = seed;

		rnd = new System.Random ();

		// int seed = (int)Network.time * 10;
		chunk = new Dictionary<Vector3, Block>();
		visibleBlocks = new ArrayList ();
	}

	public void generateHeightMap() {
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

				visibleBlocks.Add(blockPos);
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

		if (y > 15) {
			block = new Snow (false, blockPos, chunk);
		} else if (isTop && y > 5) {
			block = new Grass (false, blockPos, chunk);
			/*		
			if (this.rnd.Next(0, 10) <= 2) { // FERN
				Vector3 pos = blockPos + new Vector3 (0, 1, 0);
				Block fern = new Fern(true, pos);
				chunk.Add(pos, fern);
				visibleBlocks.Add (pos);
			} else if (this.rnd.Next(0, 50) <= 5) {
				Vector3 pos = blockPos + new Vector3 (0, 1, 0);
				Block rose = new Rose(true, pos);
				chunk.Add(pos, rose);
				visibleBlocks.Add (pos);
			}
			*/
		} else if (y > 5) {
			block = new Dirt (false, blockPos, chunk);
		} else {
			block = new Sand (false, blockPos, chunk);
		}

		chunk.Add(blockPos, block);
	}
}

