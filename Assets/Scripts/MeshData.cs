﻿using UnityEngine;
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

	public void FaceDataYPositive(Block block){
		float half_block_size = block.blockSize / 2.0f;
		this.AddVertex(new Vector3(block.pos.x - half_block_size, block.pos.y + half_block_size, block.pos.z + half_block_size));
		this.AddVertex(new Vector3(block.pos.x + half_block_size, block.pos.y + half_block_size, block.pos.z + half_block_size));
		this.AddVertex(new Vector3(block.pos.x + half_block_size, block.pos.y + half_block_size, block.pos.z - half_block_size));
		this.AddVertex(new Vector3(block.pos.x - half_block_size, block.pos.y + half_block_size, block.pos.z - half_block_size));

		// add normals
		Vector3 up = Vector3.up;
		this.normals.Add (up);
		this.normals.Add (up);
		this.normals.Add (up);
		this.normals.Add (up);


		// add triangles
		this.AddQuadTriangles();


		// add textures
		Vector2 _00 = block.blockTile.top_00;
		Vector2 _10 = block.blockTile.top_10;
		Vector2 _01 = block.blockTile.top_01;
		Vector2 _11 = block.blockTile.top_11;
		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
		this.uvs.Add (_10);
	}

	public void FaceDataYNegative(Block block){
		float half_block_size = block.blockSize / 2.0f;
		this.AddVertex(new Vector3(block.pos.x - half_block_size, block.pos.y - half_block_size, block.pos.z - half_block_size));
		this.AddVertex(new Vector3(block.pos.x + half_block_size, block.pos.y - half_block_size, block.pos.z - half_block_size));
		this.AddVertex(new Vector3(block.pos.x + half_block_size, block.pos.y - half_block_size, block.pos.z + half_block_size));
		this.AddVertex(new Vector3(block.pos.x - half_block_size, block.pos.y - half_block_size, block.pos.z + half_block_size));

		// add normals
		Vector3 down = Vector3.down;
		this.normals.Add (down);
		this.normals.Add (down);
		this.normals.Add (down);
		this.normals.Add (down);

		// add triangles
		this.AddQuadTriangles();

		// add textures
		Vector2 _00 = block.blockTile.bottom_00;
		Vector2 _10 = block.blockTile.bottom_10;
		Vector2 _01 = block.blockTile.bottom_01;
		Vector2 _11 = block.blockTile.bottom_11;
		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
		this.uvs.Add (_10);
	}

	public void FaceDataXPositive(Block block){
		float half_block_size = block.blockSize / 2.0f;
		this.AddVertex(new Vector3(block.pos.x + half_block_size, block.pos.y - half_block_size, block.pos.z - half_block_size));
		this.AddVertex(new Vector3(block.pos.x + half_block_size, block.pos.y + half_block_size, block.pos.z - half_block_size));
		this.AddVertex(new Vector3(block.pos.x + half_block_size, block.pos.y + half_block_size, block.pos.z + half_block_size));
		this.AddVertex(new Vector3(block.pos.x + half_block_size, block.pos.y - half_block_size, block.pos.z + half_block_size));

		// add normals
		Vector3 right = Vector3.right;
		this.normals.Add (right);
		this.normals.Add (right);
		this.normals.Add (right);
		this.normals.Add (right);

		// add triangles
		this.AddQuadTriangles();

		// add textures
		Vector2 _00 = block.blockTile.right_00;
		Vector2 _10 = block.blockTile.right_10;
		Vector2 _01 = block.blockTile.right_01;
		Vector2 _11 = block.blockTile.right_11;
		this.uvs.Add (_10);
		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
	}

	public void FaceDataXNegative(Block block){
		float half_block_size = block.blockSize / 2.0f;
		this.AddVertex(new Vector3(block.pos.x - half_block_size, block.pos.y - half_block_size, block.pos.z + half_block_size));
		this.AddVertex(new Vector3(block.pos.x - half_block_size, block.pos.y + half_block_size, block.pos.z + half_block_size));
		this.AddVertex(new Vector3(block.pos.x - half_block_size, block.pos.y + half_block_size, block.pos.z - half_block_size));
		this.AddVertex(new Vector3(block.pos.x - half_block_size, block.pos.y - half_block_size, block.pos.z - half_block_size));


		// add normals
		Vector3 left = Vector3.left;
		this.normals.Add (left);
		this.normals.Add (left);
		this.normals.Add (left);
		this.normals.Add (left);

		// add triangles
		this.AddQuadTriangles();

		// add textures
		Vector2 _00 = block.blockTile.left_00;
		Vector2 _10 = block.blockTile.left_10;
		Vector2 _01 = block.blockTile.left_01;
		Vector2 _11 = block.blockTile.left_11;
		this.uvs.Add (_10);
		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
	}

	public void FaceDataZPositive(Block block){
		float half_block_size = block.blockSize / 2.0f;
		this.AddVertex(new Vector3(block.pos.x - half_block_size, block.pos.y - half_block_size, block.pos.z - half_block_size));
		this.AddVertex(new Vector3(block.pos.x - half_block_size, block.pos.y + half_block_size, block.pos.z - half_block_size));
		this.AddVertex(new Vector3(block.pos.x + half_block_size, block.pos.y + half_block_size, block.pos.z - half_block_size));
		this.AddVertex(new Vector3(block.pos.x + half_block_size, block.pos.y - half_block_size, block.pos.z - half_block_size));

		// add normals
		Vector3 front = Vector3.forward;
		this.normals.Add (front);
		this.normals.Add (front);
		this.normals.Add (front);
		this.normals.Add (front);

		// add triangles
		this.AddQuadTriangles();

		// add textures
		Vector2 _00 = block.blockTile.front_00;
		Vector2 _10 = block.blockTile.front_10;
		Vector2 _01 = block.blockTile.front_01;
		Vector2 _11 = block.blockTile.front_11;
		this.uvs.Add (_10);
		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
	}

	public void FaceDataZNegative(Block block){
		float half_block_size = block.blockSize / 2.0f;
		this.AddVertex(new Vector3(block.pos.x + half_block_size, block.pos.y - half_block_size, block.pos.z + half_block_size));
		this.AddVertex(new Vector3(block.pos.x + half_block_size, block.pos.y + half_block_size, block.pos.z + half_block_size));
		this.AddVertex(new Vector3(block.pos.x - half_block_size, block.pos.y + half_block_size, block.pos.z + half_block_size));
		this.AddVertex(new Vector3(block.pos.x - half_block_size, block.pos.y - half_block_size, block.pos.z + half_block_size));

		// add normals
		Vector3 back = Vector3.back;
		this.normals.Add (back);
		this.normals.Add (back);
		this.normals.Add (back);
		this.normals.Add (back);

		// add triangles
		this.AddQuadTriangles();

		// add textures
		Vector2 _00 = block.blockTile.back_00;
		Vector2 _10 = block.blockTile.back_10;
		Vector2 _01 = block.blockTile.back_01;
		Vector2 _11 = block.blockTile.back_11;
		this.uvs.Add (_10);
		this.uvs.Add (_11);
		this.uvs.Add (_01);
		this.uvs.Add (_00);
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