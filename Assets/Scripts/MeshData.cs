using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshData{

	// mesh properties
	public List<Vector3> vertices; //Vector3[] vertices;
	public List<Vector3> normals;  //Vector3[] normals;
	public List<Vector2> uvs;      //Vector2[] uvs;
	public List<int> triangles;    //int[] triangles;

	// collision properties
	public List<int> colTriangles;
	public List<Vector3> colVertices;

	// boolean use collision?
	public bool useRenderDataForCollision;

	// constructor
	public MeshData(){
		this.vertices = new List<Vector3> ();
		this.normals = new List<Vector3>();
		this.uvs = new List<Vector2>();
		this.triangles = new List<int>();

		this.colTriangles = new List<int> ();
		this.colVertices = new List<Vector3>();

		useRenderDataForCollision = true; // use collision
	}

	public void FaceDataYPositive(CubeBlock block, Vector3 pos, bool dropItem = false){
		float half_block_size = block.blockSize / 2.0f;
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y + half_block_size, pos.z + half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y + half_block_size, pos.z + half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y + half_block_size, pos.z - half_block_size));
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y + half_block_size, pos.z - half_block_size));

		// add normals
		Vector3 up = Vector3.up;
		this.normals.Add (up);
		this.normals.Add (up);
		this.normals.Add (up);
		this.normals.Add (up);


		// add triangles
		this.AddQuadTriangles();


		// add textures
		BlockTile blockTile = dropItem ? block.getDropBlockTile() : block.getBlockTile();
		Vector2 _00 = blockTile.top_00;
		Vector2 _10 = blockTile.top_10;
		Vector2 _01 = blockTile.top_01;
		Vector2 _11 = blockTile.top_11;
		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
		this.uvs.Add (_10);
	}

	public void FaceDataYNegative(CubeBlock block, Vector3 pos, bool dropItem = false){
		float half_block_size = block.blockSize / 2.0f;
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y - half_block_size, pos.z - half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y - half_block_size, pos.z - half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y - half_block_size, pos.z + half_block_size));
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y - half_block_size, pos.z + half_block_size));

		// add normals
		Vector3 down = Vector3.down;
		this.normals.Add (down);
		this.normals.Add (down);
		this.normals.Add (down);
		this.normals.Add (down);

		// add triangles
		this.AddQuadTriangles();

		// add textures
		BlockTile blockTile = dropItem ? block.getDropBlockTile() : block.getBlockTile();
		Vector2 _00 = blockTile.bottom_00;
		Vector2 _10 = blockTile.bottom_10;
		Vector2 _01 = blockTile.bottom_01;
		Vector2 _11 = blockTile.bottom_11;
		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
		this.uvs.Add (_10);
	}

	public void FaceDataXPositive(CubeBlock block, Vector3 pos, bool dropItem = false){
		float half_block_size = block.blockSize / 2.0f;
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y - half_block_size, pos.z - half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y + half_block_size, pos.z - half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y + half_block_size, pos.z + half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y - half_block_size, pos.z + half_block_size));

		// add normals
		Vector3 right = Vector3.right;
		this.normals.Add (right);
		this.normals.Add (right);
		this.normals.Add (right);
		this.normals.Add (right);

		// add triangles
		this.AddQuadTriangles();

		// add textures
		BlockTile blockTile = dropItem ? block.getDropBlockTile() : block.getBlockTile();
		Vector2 _00 = blockTile.right_00;
		Vector2 _10 = blockTile.right_10;
		Vector2 _01 = blockTile.right_01;
		Vector2 _11 = blockTile.right_11;
		this.uvs.Add (_10);
		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
	}

	public void FaceDataXNegative(CubeBlock block, Vector3 pos, bool dropItem = false){
		float half_block_size = block.blockSize / 2.0f;
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y - half_block_size, pos.z + half_block_size));
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y + half_block_size, pos.z + half_block_size));
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y + half_block_size, pos.z - half_block_size));
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y - half_block_size, pos.z - half_block_size));


		// add normals
		Vector3 left = Vector3.left;
		this.normals.Add (left);
		this.normals.Add (left);
		this.normals.Add (left);
		this.normals.Add (left);

		// add triangles
		this.AddQuadTriangles();

		// add textures
		BlockTile blockTile = dropItem ? block.getDropBlockTile() : block.getBlockTile();
		Vector2 _00 = blockTile.left_00;
		Vector2 _10 = blockTile.left_10;
		Vector2 _01 = blockTile.left_01;
		Vector2 _11 = blockTile.left_11;
		this.uvs.Add (_10);
		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
	}

	public void FaceDataZPositive(CubeBlock block, Vector3 pos, bool dropItem = false){
		float half_block_size = block.blockSize / 2.0f;
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y - half_block_size, pos.z - half_block_size));
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y + half_block_size, pos.z - half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y + half_block_size, pos.z - half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y - half_block_size, pos.z - half_block_size));

		// add normals
		Vector3 front = Vector3.forward;
		this.normals.Add (front);
		this.normals.Add (front);
		this.normals.Add (front);
		this.normals.Add (front);

		// add triangles
		this.AddQuadTriangles();

		// add textures
		BlockTile blockTile = dropItem ? block.getDropBlockTile() : block.getBlockTile();
		Vector2 _00 = blockTile.front_00;
		Vector2 _10 = blockTile.front_10;
		Vector2 _01 = blockTile.front_01;
		Vector2 _11 = blockTile.front_11;
		this.uvs.Add (_10);
		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
	}

	public void FaceDataZNegative(CubeBlock block, Vector3 pos, bool dropItem = false){
		float half_block_size = block.blockSize / 2.0f;
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y - half_block_size, pos.z + half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y + half_block_size, pos.z + half_block_size));
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y + half_block_size, pos.z + half_block_size));
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y - half_block_size, pos.z + half_block_size));

		// add normals
		Vector3 back = Vector3.back;
		this.normals.Add (back);
		this.normals.Add (back);
		this.normals.Add (back);
		this.normals.Add (back);

		// add triangles
		this.AddQuadTriangles();

		// add textures
		BlockTile blockTile = dropItem ? block.getDropBlockTile() : block.getBlockTile();
		Vector2 _00 = blockTile.back_00;
		Vector2 _10 = blockTile.back_10;
		Vector2 _01 = blockTile.back_01;
		Vector2 _11 = blockTile.back_11;
		this.uvs.Add (_10);
		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
	}

	/*
	 *      /
	 */ 
	public void FaceDataDiag1(Block block, Vector3 pos) {
		float half_block_size = 0.5f;
		
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y + half_block_size, pos.z + half_block_size));
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y + half_block_size, pos.z - half_block_size));
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y - half_block_size, pos.z - half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y - half_block_size, pos.z + half_block_size));

		this.AddQuadTriangles();

		
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y + half_block_size, pos.z - half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y + half_block_size, pos.z + half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y - half_block_size, pos.z + half_block_size));
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y - half_block_size, pos.z - half_block_size));

		this.AddQuadTriangles();

		// add normals 
		Vector3 n1 = new Vector3(-1f, 0, 1f);
		this.normals.Add (n1);
		this.normals.Add (n1);
		this.normals.Add (n1);
		this.normals.Add (n1);
		
		Vector3 n2 = new Vector3(1f, 0, -1f);
		this.normals.Add (n2);
		this.normals.Add (n2);
		this.normals.Add (n2);
		this.normals.Add (n2);

		SpriteData spriteData = ((Plant)block).getSpriteDate();
		Vector2 _00 = spriteData._00;
		Vector2 _10 = spriteData._10;
		Vector2 _01 = spriteData._01;
		Vector2 _11 = spriteData._11;

		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
		this.uvs.Add (_10);

		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
		this.uvs.Add (_10);
	}

	/*
	 *      \
	*/ 
	public void FaceDataDiag2(Block block, Vector3 pos) {
		float half_block_size = 0.5f;
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y + half_block_size, pos.z + half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y + half_block_size, pos.z - half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y - half_block_size, pos.z - half_block_size));
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y - half_block_size, pos.z + half_block_size));

		this.AddQuadTriangles();

		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y + half_block_size, pos.z - half_block_size));
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y + half_block_size, pos.z + half_block_size));
		this.AddVertex(new Vector3(pos.x - half_block_size, pos.y - half_block_size, pos.z + half_block_size));
		this.AddVertex(new Vector3(pos.x + half_block_size, pos.y - half_block_size, pos.z - half_block_size));

		this.AddQuadTriangles();

		// add normals 
		Vector3 n1 = new Vector3(-1f, 0, -1f);
		this.normals.Add (n1);
		this.normals.Add (n1);
		this.normals.Add (n1);
		this.normals.Add (n1);

		Vector3 n2 = new Vector3(1f, 0, 1f);
		this.normals.Add (n2);
		this.normals.Add (n2);
		this.normals.Add (n2);
		this.normals.Add (n2);

		SpriteData spriteData = ((Plant)block).getSpriteDate();
		Vector2 _00 = spriteData._00;
		Vector2 _10 = spriteData._10;
		Vector2 _01 = spriteData._01;
		Vector2 _11 = spriteData._11;
		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
		this.uvs.Add (_10);

		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
		this.uvs.Add (_10);
	}

	public void AddVertex(Vector3 vertex){
		this.vertices.Add (vertex);
		if (this.useRenderDataForCollision) {
			this.colVertices.Add(vertex);
		}
	}


	// add triangles
	public void AddQuadTriangles(){
		triangles.Add(vertices.Count - 4);
		triangles.Add(vertices.Count - 3);
		triangles.Add(vertices.Count - 2);

		triangles.Add(vertices.Count - 4);
		triangles.Add(vertices.Count - 2);
		triangles.Add(vertices.Count - 1);

		if (this.useRenderDataForCollision) {
			colTriangles.Add(colVertices.Count - 4);
			colTriangles.Add(colVertices.Count - 3);
			colTriangles.Add(colVertices.Count - 2);
			colTriangles.Add(colVertices.Count - 4);
			colTriangles.Add(colVertices.Count - 2);
			colTriangles.Add(colVertices.Count - 1);
		}
	}
}
