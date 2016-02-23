using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic;

public static class Serialization {
	public static string saveFolder = "saves";

	public static string SaveLocation (World world) {
		// assume world name is "test"
		string worldName = world.name;
		string saveLocation = saveFolder + "/" + worldName + "/";

		if (!Directory.Exists (saveLocation)) {
			Directory.CreateDirectory (saveLocation);
		}

		return saveLocation;
	}

	public static string FileName(Chunk chunk) {
		string fileName = chunk.chunkX + "," + chunk.chunkZ + ".bin";
		return fileName;
	}

	public static string FileName(Vector2 chunkPos) {
		string fileName = chunkPos.x + "," + chunkPos.y + ".bin";
		return fileName;
	}

	public static void SaveChunk(Chunk chunk, World world) {
		string saveFile = SaveLocation (world);
		saveFile = saveFile + FileName (chunk);

		IFormatter formatter = new BinaryFormatter ();
		Stream stream = new FileStream (saveFile, FileMode.Create, FileAccess.Write, FileShare.None);
		formatter.Serialize (stream, chunk.blocks);
		stream.Close ();
	}
		
	public static Chunk LoadChunk(Vector2 chunkPos, World world) {
		string saveFile = SaveLocation (world);
		saveFile = saveFile + FileName (chunkPos);

		if (!File.Exists (saveFile))
			return null; 

		IFormatter formatter = new BinaryFormatter ();
		FileStream stream = new FileStream (saveFile, FileMode.Open);

		Chunk chunk = new Chunk ((int)chunkPos.x, (int)chunkPos.y);
		chunk.blocks = formatter.Deserialize (stream) as Dictionary<BlockPos, Block>;
		chunk.needRender = true;
		stream.Close ();
		return chunk;
	}

	public static void SaveWorldData(World world) {
		string saveFile = SaveLocation (world);
		saveFile = saveFile + "wd.bin";

		IFormatter formatter = new BinaryFormatter ();
		Stream stream = new FileStream (saveFile, FileMode.Create, FileAccess.Write, FileShare.None);
		formatter.Serialize (stream, world.worldData);
		stream.Close ();
	}

	public static WorldData LoadWorldData(World world) {
		string saveFile = SaveLocation (world);
		saveFile = saveFile + "wd.bin";

		if (!File.Exists (saveFile))
			return null; 

		IFormatter formatter = new BinaryFormatter ();
		FileStream stream = new FileStream (saveFile, FileMode.Open);

		WorldData wd = formatter.Deserialize (stream) as WorldData;
		stream.Close ();
		return wd;
	}

}