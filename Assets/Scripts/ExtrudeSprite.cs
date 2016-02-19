using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ExtrudeSprite : MonoBehaviour
{
	private enum Edge {top, left, bottom, right};

	// public Material spriteMat;
	// private Material spriteMat;
	public int alphaTheshold = 0;
	public float depth = 0.0625f;
	private Color32[] m_Colors;
	private int m_Width;
	private int m_Height;

	private List<Vector3> m_Vertices = new List<Vector3>();
	private List<Vector3> m_Normals = new List<Vector3>();
	private List<Vector2> m_TexCoords = new List<Vector2>();

	private bool HasPixel(int aX, int aY) {
		return m_Colors[aX + aY*m_Width].a > alphaTheshold;
	}
	void AddQuad(Vector3 aFirstEdgeP1, Vector3 aFirstEdgeP2,Vector3 aSecondRelative, Vector3 aNormal, Vector2 aUV1, Vector2 aUV2, bool aFlipUVs) {
		m_Vertices.Add(aFirstEdgeP1);
		m_Vertices.Add(aFirstEdgeP2);
		m_Vertices.Add(aFirstEdgeP2 + aSecondRelative);
		m_Vertices.Add(aFirstEdgeP1 + aSecondRelative);
		m_Normals.Add(aNormal);
		m_Normals.Add(aNormal);
		m_Normals.Add(aNormal);
		m_Normals.Add(aNormal);
		if (aFlipUVs)
		{
			m_TexCoords.Add(new Vector2(aUV1.x,aUV1.y));
			m_TexCoords.Add(new Vector2(aUV2.x,aUV1.y));
			m_TexCoords.Add(new Vector2(aUV2.x,aUV2.y));
			m_TexCoords.Add(new Vector2(aUV1.x,aUV2.y));
		}
		else
		{
			m_TexCoords.Add(new Vector2(aUV1.x,aUV1.y));
			m_TexCoords.Add(new Vector2(aUV1.x,aUV2.y));
			m_TexCoords.Add(new Vector2(aUV2.x,aUV2.y));
			m_TexCoords.Add(new Vector2(aUV2.x,aUV1.y));
		}

	}

	void AddEdge(int aX, int aY, Edge aEdge) {
		Vector2 size = new Vector2(1.0f/m_Width, 1.0f/m_Height);
		Vector2 uv = new Vector3(aX*size.x, aY*size.y);
		Vector2 P = uv - Vector2.one*0.5f;
		uv += size*0.5f;
		Vector2 P2 = P;
		Vector3 normal;
		if (aEdge == Edge.top)
		{
			P += size;
			P2.y += size.y;
			normal =  Vector3.up;
		}
		else if(aEdge == Edge.left)
		{
			P.y += size.y;
			normal =  Vector3.left;
		}
		else if(aEdge == Edge.bottom)
		{
			P2.x += size.x;
			normal =  Vector3.down;
		}
		else
		{
			P2 += size;
			P.x += size.x;
			normal =  Vector3.right;
		}
		AddQuad(P, P2, Vector3.forward*depth, normal, uv,uv,false);
	}

	public void GenerateMesh(Texture2D tex) {
		int x, y;

		/*
		tex = new Texture2D (2, 2, TextureFormat.ARGB32, false);// Resources.Load(Application.dataPath + "/Textures/PIXIE_1/assets/minecraft/textures/items/iron_sword.png") as Texture2D;
		byte[] data = File.ReadAllBytes(Application.dataPath + "/Textures/items.png");
		tex.LoadImage (data);
		tex.filterMode = FilterMode.Point;
		tex.wrapMode = TextureWrapMode.Clamp;

		// 妈了个屯，这个 crop 的左下角是 (0, 0), 而不是左上角
		Color[] colors = tex.GetPixels (0, 512 - 16, 16, 16);
		Texture2D itemTex = new Texture2D (16, 16, TextureFormat.RGBA32, false);
		itemTex.SetPixels (colors);
		itemTex.Apply ();
		itemTex.filterMode = FilterMode.Point;
		itemTex.wrapMode = TextureWrapMode.Clamp;
		tex = itemTex;
		*/

		m_Colors = tex.GetPixels32();
		m_Width = tex.width;
		m_Height = tex.height;

		/*
		Texture2D test = new Texture2D (2, 2, TextureFormat.ARGB32, false);// Resources.Load(Application.dataPath + "/Textures/PIXIE_1/assets/minecraft/textures/items/iron_sword.png") as Texture2D;
		byte[] data = File.ReadAllBytes(Application.dataPath + "/Textures/PIXIE_1/assets/minecraft/textures/items/iron_sword.png");
		test.LoadImage (data);
		test.filterMode = FilterMode.Point;
		drawTexture = true;
		finish = test;
		*/

		// GetComponent<Renderer> ().sharedMaterial.mainTexture = itemTexture;
		Material material = GetComponent<Renderer>().material as Material;
		material.mainTexture = tex; 
		Shader shader = Shader.Find ("Standard");
		material.shader = shader;
		material.SetFloat ("_Mode", 1); // set Rendering Mode to cutout


		// the code below is very necessary to update material at runtime
		// see: http://answers.unity3d.com/questions/999594/change-material-rendering-mode-but-dont-update-mat.html
		material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
		material.SetInt("_ZWrite", 1);
		material.EnableKeyword("_ALPHATEST_ON");
		material.DisableKeyword("_ALPHABLEND_ON");
		material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = 2450;
		 
		//      first point                     , second point                    , relative 3. P, normal,          lower UV,     Upper UV,    flipUV
		AddQuad(new Vector3(-0.5f, -0.5f, 0    ), new Vector3(-0.5f,  0.5f, 0    ), Vector3.right, Vector3.back,    Vector2.zero, Vector2.one, false);
		AddQuad(new Vector3(-0.5f, -0.5f, depth), new Vector3( 0.5f, -0.5f, depth), Vector3.up,    Vector3.forward, Vector2.zero, Vector2.one, true);

		for (y = 0; y < m_Height; y++) // bottom to top
		{
			for (x = 0; x < m_Width; x++) // left to right
			{
				if (HasPixel(x,y))
				{
					if(x==0 || !HasPixel(x-1,y))
						AddEdge(x,y,Edge.left);

					if(x==m_Width-1 || !HasPixel(x+1,y))
						AddEdge(x,y,Edge.right);

					if(y==0 || !HasPixel(x,y-1))
						AddEdge(x,y,Edge.bottom);

					if(y==m_Height-1 || !HasPixel(x,y+1))
						AddEdge(x,y,Edge.top);
				}
			}
		}
		var mesh = new Mesh();
		mesh.vertices = m_Vertices.ToArray();
		mesh.normals = m_Normals.ToArray();
		mesh.uv = m_TexCoords.ToArray();
		int[] quads = new int[m_Vertices.Count];
		for (int i = 0; i < quads.Length; i++)
			quads[i] = i;
		mesh.SetIndices(quads,MeshTopology.Quads,0);
		GetComponent<MeshFilter>().sharedMesh = mesh;
	}

	void Start() {
		// spriteMat = GetComponent<Renderer> ().material;
		// GenerateMesh();
	}
}