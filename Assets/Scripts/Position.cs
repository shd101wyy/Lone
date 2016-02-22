using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class BlockPos: System.Object {
	public int x;
	public int y;
	public int z;

	public BlockPos(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public BlockPos(Vector3 pos) {
		this.x = (int)pos.x;
		this.y = (int)pos.y;
		this.z = (int)pos.z;
	}

	public override bool Equals(System.Object obj) {
		if (obj == null || GetType () != obj.GetType ())
			return false;

		BlockPos p = (BlockPos)obj;
		return (this.x == p.x) && (this.y == p.y) && (this.z == p.z);
	}

	public override int GetHashCode ()
	{
		return x ^ y ^ z;
	}

	public Vector3 ToVector3() {
		return new Vector3 (x, y, z);
	}

	public override string ToString () {
		return string.Format ("(" + x + ", " + y + ", " + z + ")");
	}

	public static bool operator ==(BlockPos a, BlockPos b) {
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(a, b))
		{
			return true;
		}

		// If one is null, but not both, return false.
		if (((object)a == null) || ((object)b == null))
		{
			return false;
		}

		// Return true if the fields match:
		return a.x == b.x && a.y == b.y && a.z == b.z;
	}

	public static bool operator !=(BlockPos a, BlockPos b) {
		return !(a == b);
	}
}

[Serializable]
public class ChunkPos: System.Object {
	public int x;
	public int z;

	public ChunkPos(int x, int z) {
		this.x = x;
		this.z = z;
	}

	public ChunkPos(Vector3 pos) {
		this.x = (int)pos.x;
		this.z = (int)pos.z;
	}

	public ChunkPos(Vector2 pos) {
		this.x = (int)pos.x;
		this.z = (int)pos.y;
	}

	public override bool Equals(System.Object obj) {
		if (obj == null || GetType () != obj.GetType ())
			return false;

		BlockPos p = (BlockPos)obj;
		return (this.x == p.x) && (this.z == p.z);
	}

	public override int GetHashCode ()
	{
		return x ^ z;
	}

	public Vector3 ToVector2() {
		return new Vector2 (x, z);
	}

	public override string ToString () {
		return string.Format ("(" + x +  ", " + z + ")");
	}

	public static bool operator ==(ChunkPos a, ChunkPos b) {
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(a, b)) {
			return true;
		}

		// If one is null, but not both, return false.
		if (((object)a == null) || ((object)b == null)) {
			return false;
		}

		// Return true if the fields match:
		return a.x == b.x && a.z == b.z;
	}

	public static bool operator !=(ChunkPos a, ChunkPos b) {
		return !(a == b);
	}
}

