using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 
 * WorldData is used to store player coordinate
 * player items
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
		this.playerY = 60;
		this.playerZ = 0;

		playerRX = 0;
		playerRY = 0;
		playerRZ = 0;
	}

	public WorldData() {
		this.seed = (int)System.DateTime.Now.Millisecond * 1000;
		this.playerX = 0;
		this.playerY = 60;
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
	public Dictionary<Vector2, HeightMap> heightMaps;
	public string name;
	public WorldData worldData;

	public World(string worldName) {
		this.name = worldName;
		heightMaps = new Dictionary<Vector2, HeightMap> ();

		// load WorldData
		worldData = Serialization.LoadWorldData(this);
		if (worldData == null) {
			worldData = new WorldData ();
		}
	}

	public void addHeightMap(Vector2 chunkPos, HeightMap heightMap) {
		heightMaps.Add (chunkPos, heightMap);
	}

	// chunkPos like (0, 0), (0, 1), (1, 1)...
	public Chunk getChunk(Vector2 chunkPos) {
		return heightMaps [chunkPos].chunk;
	}

	public bool hasChunkAtPosition(Vector2 chunkPos) {
		return heightMaps.ContainsKey (chunkPos);
	}

	// get block
	public Block getBlock(Vector3 blockPos) {
		Vector2 chunkPos = new Vector2 (Mathf.Floor (blockPos.x / Generate_Landscape.chunkWidth),
			Mathf.Floor (blockPos.z / Generate_Landscape.chunkDepth));
		if (!heightMaps.ContainsKey (chunkPos))
			return null;
		Chunk chunk = heightMaps [chunkPos].chunk;
		return chunk.getBlock (blockPos);
	}

	// add/set block
	public void addBlock(Vector3 blockPos, Block block, bool needRender = false) {
		Vector2 chunkPos = new Vector2 (Mathf.Floor (blockPos.x / Generate_Landscape.chunkWidth),
			Mathf.Floor (blockPos.z / Generate_Landscape.chunkDepth));

		Chunk chunk = heightMaps [chunkPos].chunk;
		if (chunk.hasBlockAtPosition (blockPos))
			return;
		chunk.addBlock (blockPos, block);
		chunk.needRender = needRender;
	}

	// remove block
	public void removeBlock(Vector3 blockPos, bool needRender = false) {
		Vector2 chunkPos = new Vector2 (Mathf.Floor (blockPos.x / Generate_Landscape.chunkWidth),
			Mathf.Floor (blockPos.z / Generate_Landscape.chunkDepth));
		Chunk chunk = heightMaps [chunkPos].chunk;
		chunk.removeBlock (blockPos);
		chunk.needRender = needRender;

		int chunkWidth = Generate_Landscape.chunkWidth;
		int chunkDepth = Generate_Landscape.chunkDepth;

		//Debug.Log (blockPos);
		//Debug.Log (chunkPos);
		// render left chunk
		if (Mathf.Floor((blockPos.x-1)/chunkWidth) != chunkPos.x  && 
			heightMaps.ContainsKey(chunkPos + new Vector2(-1, 0)) && 
			this.getBlock(blockPos + new Vector3(-1, 0, 0)) != null) {

			//Debug.Log ("left");
			heightMaps [chunkPos + new Vector2 (-1, 0)].chunk.needRender = true;

		} else if (Mathf.Floor((blockPos.x+1)/chunkWidth) != chunkPos.x && 
			heightMaps.ContainsKey(chunkPos + new Vector2(1, 0)) && 
			this.getBlock(blockPos + new Vector3(1, 0, 0)) != null) {

			//Debug.Log ("right");
			heightMaps [chunkPos + new Vector2 (1, 0)].chunk.needRender = true;
		}

		if (Mathf.Floor((blockPos.z-1)/chunkDepth) != chunkPos.y && 
			heightMaps.ContainsKey(chunkPos + new Vector2(0, -1)) && 
			this.getBlock(blockPos + new Vector3(0, 0, -1)) != null) {

			//Debug.Log ("front");
			heightMaps [chunkPos + new Vector2 (0, -1)].chunk.needRender = true;

		} else if (Mathf.Floor((blockPos.z+1)/chunkDepth) != chunkPos.y && 
			heightMaps.ContainsKey(chunkPos + new Vector2(0, 1)) && 
			this.getBlock(blockPos + new Vector3(0, 0, 1)) != null) {

			//Debug.Log ("back");
			heightMaps [chunkPos + new Vector2 (0, 1)].chunk.needRender = true;
		}
	}

	// save world to disk 
	public void saveWorld() {
		Debug.Log ("Save");
		Serialization.SaveWorldData (this);

		foreach (var item in heightMaps) {
			HeightMap heightMap = item.Value;	
			Chunk chunk = heightMap.chunk;
			if (chunk.changed) {
				chunk.changed = false;
				Serialization.SaveChunk (chunk, this);
			}
		}
		Debug.Log ("Done");
	}

	//  load world from disk
	// public void loadWorld() {
	//
	// }
}