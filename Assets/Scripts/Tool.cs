using UnityEngine;
using System.Collections;

public class Tool: Item {
	public Tool(string toolName) : base(toolName, ItemType.TOOL, 1) {
	}

	public override Texture2D getTexture () {
		throw new System.NotImplementedException ();
	}
}

public class StoneAxe : Tool {
	public StoneAxe(): base("stone_axe") {
	}

	public override Texture2D getTexture () {
		return Game.textures ["stone_axe"].texture_2d;
	}

}

public class StoneSword: Tool {
	public StoneSword(): base("stone_sword") {
	}

	public override Texture2D getTexture () {
		return Game.textures ["stone_sword"].texture_2d;
	}
}