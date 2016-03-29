using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 
 * WorldData is used to store player coordinate
 * player items
 * Sun/Moon quaternion
 * etc
 */ 
[Serializable]
public class WorldData {
	public float playerX;
	public float playerY;
	public float playerZ;
	public float playerRX;
	public float playerRY;
	public float playerRZ;

	public int seed;

	public WorldData(int seed) {
		this.seed = seed;
		this.playerX = 0;
		this.playerY = 120;
		this.playerZ = 0;

		playerRX = 0;
		playerRY = 0;
		playerRZ = 0;
	}

	public WorldData() {
		this.seed = (int)System.DateTime.Now.Millisecond * 1000;
		this.playerX = 0;
		this.playerY = 120;
		this.playerZ = 0;

		playerRX = 0;
		playerRY = 0;
		playerRZ = 0;
	}

	public void updatePlayerTransformationData(GameObject player) {
		Vector3 playerPos = player.transform.position;
		Vector3 playerR = player.transform.localEulerAngles;

		playerX = playerPos.x;
		playerY = playerPos.y;
		playerZ = playerPos.z;

		playerRX = playerR.x;
		playerRY = playerR.y;
		playerRZ = playerR.z;
	}
}

public class World {
	public Dictionary<Vector3, Chunk> chunks;
	public string name;
	public WorldData worldData;

	public World(string worldName) {
		this.name = worldName;
		chunks = new Dictionary<Vector3, Chunk> ();

		// load WorldData
		worldData = Serialization.LoadWorldData(this);
		if (worldData == null) {
			worldData = new WorldData ();
		}
	}

	public World(string worldName, int seed) {
		this.name = worldName;
		chunks = new Dictionary<Vector3, Chunk> ();

		// load WorldData
		worldData = Serialization.LoadWorldData(this);
		if (worldData == null) {
			worldData = new WorldData (seed);
		}
	}

	public void addChunk(Vector3 chunkPos, Chunk chunk) {
		chunks.Add (chunkPos, chunk);
	}

	// chunkPos like (0, 0, 0), (0, 0, 1), (1, 0, 1)...
	public Chunk getChunk(Vector3 chunkPos) {
		if (chunks.ContainsKey (chunkPos))
			return chunks [chunkPos];
		else
			return null;
	}

	public bool hasChunkAtPosition(Vector3 chunkPos) {
		return chunks.ContainsKey (chunkPos);
	}

	// get block
	public Block getBlock(Vector3 blockPos) {
		Vector3 chunkPos = new Vector3 (Mathf.Floor (blockPos.x / Chunk.width),
			Mathf.Floor (blockPos.y / Chunk.height),
			Mathf.Floor (blockPos.z / Chunk.depth));
		if (!chunks.ContainsKey (chunkPos))
			return null;
		return chunks[chunkPos].getBlock (blockPos);
	}

	// add/set block
	public void addBlock(Vector3 blockPos, Block block, bool needRender = false) {
		Vector3 chunkPos = new Vector3 (Mathf.Floor (blockPos.x / Chunk.width),
			Mathf.Floor (blockPos.y / Chunk.height),
			Mathf.Floor (blockPos.z / Chunk.depth));

		Chunk chunk = chunks [chunkPos];
		if (chunk.getBlock (blockPos) is Air) {
			chunk.addBlock (blockPos, block);
			chunk.needRender = needRender;
		}
	}

	// remove block
	public void removeBlock(Vector3 blockPos, bool needRender = false) {
		Vector3 chunkPos = new Vector3 (Mathf.Floor (blockPos.x / Chunk.width),
			Mathf.Floor (blockPos.y / Chunk.height),
			Mathf.Floor (blockPos.z / Chunk.depth));
		
		Chunk chunk = chunks [chunkPos];
		chunk.removeBlock (blockPos);
		chunk.needRender = needRender;

		int chunkWidth = Chunk.width;
		int chunkDepth = Chunk.depth;
		int chunkHeight = Chunk.height;

		//Debug.Log (blockPos);
		//Debug.Log (chunkPos);
		// render left chunk
		if (Mathf.Floor((blockPos.x-1)/chunkWidth) != chunkPos.x  && 
			chunks.ContainsKey(chunkPos + new Vector3(-1, 0, 0)) && 
			this.getBlock(blockPos + new Vector3(-1, 0, 0)) != null) {

			//Debug.Log ("left");
			chunks [chunkPos + new Vector3 (-1, 0, 0)].needRender = true;

		} else if (Mathf.Floor((blockPos.x+1)/chunkWidth) != chunkPos.x && 
			chunks.ContainsKey(chunkPos + new Vector3(1, 0, 0)) && 
			this.getBlock(blockPos + new Vector3(1, 0, 0)) != null) {

			//Debug.Log ("right");
			chunks [chunkPos + new Vector3 (1, 0, 0)].needRender = true;
		}

		if (Mathf.Floor((blockPos.z-1)/chunkDepth) != chunkPos.z && 
			chunks.ContainsKey(chunkPos + new Vector3(0, 0, -1)) && 
			this.getBlock(blockPos + new Vector3(0, 0, -1)) != null) {

			//Debug.Log ("front");
			chunks [chunkPos + new Vector3 (0, 0, -1)].needRender = true;

		} else if (Mathf.Floor((blockPos.z+1)/chunkDepth) != chunkPos.z && 
			chunks.ContainsKey(chunkPos + new Vector3(0, 0, 1)) && 
			this.getBlock(blockPos + new Vector3(0, 0, 1)) != null) {

			//Debug.Log ("back");
			chunks [chunkPos + new Vector3 (0, 0, 1)].needRender = true;
		}

		if (Mathf.Floor((blockPos.y-1)/chunkHeight) != chunkPos.y && 
			chunks.ContainsKey(chunkPos + new Vector3(0, -1, 0)) && 
			this.getBlock(blockPos + new Vector3(0, -1, 0)) != null) {

			//Debug.Log ("bottom");
			chunks [chunkPos + new Vector3 (0, -1, 0)].needRender = true;

		} else if (Mathf.Floor((blockPos.y+1)/chunkHeight) != chunkPos.y && 
			chunks.ContainsKey(chunkPos + new Vector3(0, 1, 0)) && 
			this.getBlock(blockPos + new Vector3(0, 1, 0)) != null) {

			//Debug.Log ("back");
			chunks [chunkPos + new Vector3 (0, 1, 0)].needRender = true;
		}
	}

	// save world to disk 
	public void saveWorld() {
		Debug.Log ("Save");
		Serialization.SaveWorldData (this);

		foreach (var item in chunks) {
			Chunk chunk = item.Value;	
			if (chunk.changed) {
				chunk.changed = false;
				Serialization.SaveChunk (chunk, this);
			}
		}
		Debug.Log ("Done");
	}

	public void saveChunk(Vector3 chunkPos) {
		Chunk chunk = chunks [chunkPos];
		if (chunk.changed) {
			Debug.Log ("Save chunk");
			chunk.changed = false;
			Serialization.SaveChunk (chunk, this);
			Debug.Log ("Done");
		}
	}

	//  load world from disk
	// public void loadWorld() {
	//
	// }

	public Chunk CreateChunk(Vector3 chunkPos) {
		if (hasChunkAtPosition (chunkPos) == false) {
			Chunk chunk = Serialization.LoadChunk (chunkPos, this);
			if (chunk != null) { // loaded chunk from disk 
				// chunk.changed = false; // 存的时候应该就是 false
				// do nothing
			} else {
				chunk = new Chunk ((int)chunkPos.x, (int)chunkPos.y, (int)chunkPos.z);
				chunk.generateHeightMap (this);
			}
			addChunk (chunkPos, chunk);
			return chunk; 
		} else {
			return chunks [chunkPos];
		}
	}

	public void RemoveChunk(Vector3 chunkPos) {
		chunks.Remove (chunkPos);
	}
}