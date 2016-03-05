using UnityEngine;
using System;
using System.Collections;

[Serializable]
public abstract class Plant: Block {
	public Plant(string plantName) : base(plantName, ItemType.PLANT, 64) {
	}

	public override Texture2D getTexture () {
		throw new System.NotImplementedException ();
	}

	public abstract GameObject getGameObject();
	public abstract SpriteData getSpriteDate();

	public void generateMesh (MeshData meshData, Vector3 pos, World world) {
		GameObject obj = getGameObject ();
		meshData.FaceDataDiag1 (this, pos);
		meshData.FaceDataDiag2 (this, pos);
	}
}

[Serializable]
public class Rose : Plant {
	public static GameObject obj = GameObject.Find("Rose");
	public Rose(): base("flower_rose") {
	}

	public override Texture2D getTexture () {
		return Game.textures ["flower_rose"].texture_2d;
	}

	public override SpriteData getSpriteDate () {
		return Game.textures ["flower_rose"];
	}

	public override GameObject getGameObject () {
		return obj;
	}
}


[Serializable]
public class Fern : Plant {
	public static GameObject obj = GameObject.Find("Fern");
	public Fern(): base("fern") {
	}
	public override Texture2D getTexture () {
		return Game.textures ["fern"].texture_2d;
	}
	public override SpriteData getSpriteDate () {
		return Game.textures ["fern"];
	}
		
	public override GameObject getGameObject () {
		return obj;
	}
}