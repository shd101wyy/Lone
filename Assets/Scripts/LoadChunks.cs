using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadChunks : MonoBehaviour {
	public World world;
	public List<Vector2> buildList = new List<Vector2>();
	public List<Chunk> renderList = new List<Chunk>(); // Chunk need to be rendered.

	public static int halfPlaneSize = 4;

	int timer = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		DeleteChunks ();
		FindChunksToLoad ();
		LoadAndRenderChunks ();

	}

	void FindChunksToLoad() {
		int playerChunkX = (int)(transform.position.x / Generate_Landscape.chunkWidth);
		int playerChunkZ = (int)(transform.position.z / Generate_Landscape.chunkDepth);

		if (buildList.Count == 0) {
			for (int x = -halfPlaneSize; x <= halfPlaneSize; x++) {
				for (int z = -halfPlaneSize; z <= halfPlaneSize; z++) {
					Vector3 chunkPos = new Vector2 (playerChunkX + x, playerChunkZ + z);

					Chunk newChunk = world.getChunk (chunkPos);

					if (newChunk != null && (newChunk.rendered || renderList.Contains (newChunk)))
						continue;
					else
						buildList.Add (chunkPos); // TODO add something

					return;
				}
			}
		}
	}

	void BuildChunk(Vector2 chunkPos) {

	}

	void LoadAndRenderChunks() {
		for (int i = 0; i < 4; i++) {
			if (buildList.Count != 0) {
				BuildChunk (buildList [0]);
				buildList.RemoveAt (0);
			}
		}

		for (int i = 0; i < renderList.Count; i++) {
			Chunk chunk = renderList [0];
			chunk.needRender = true;
			renderList.RemoveAt (0);
		}
	}

	void DeleteChunks() {

	}
}
