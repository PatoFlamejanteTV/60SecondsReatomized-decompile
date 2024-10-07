using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HighlightingSystem;

[DisallowMultipleComponent]
public class HighlighterRenderer : MonoBehaviour
{
	private struct Data
	{
		public Material material;

		public int submeshIndex;

		public bool transparent;
	}

	private static float transparentCutoff = 0.5f;

	private const HideFlags flags = HideFlags.HideInInspector | HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild;

	private const int cullOff = 0;

	private static readonly string sRenderType = "RenderType";

	private static readonly string sOpaque = "Opaque";

	private static readonly string sTransparent = "Transparent";

	private static readonly string sTransparentCutout = "TransparentCutout";

	private static readonly string sMainTex = "_MainTex";

	private Renderer r;

	private List<Data> data;

	private Camera lastCamera;

	private bool isAlive;

	private Coroutine endOfFrame;

	private void OnEnable()
	{
		endOfFrame = StartCoroutine(EndOfFrame());
	}

	private void OnDisable()
	{
		lastCamera = null;
		if (endOfFrame != null)
		{
			StopCoroutine(endOfFrame);
		}
	}

	private void OnWillRenderObject()
	{
		Camera current = Camera.current;
		if (HighlightingBase.IsHighlightingCamera(current))
		{
			lastCamera = current;
		}
	}

	private IEnumerator EndOfFrame()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			lastCamera = null;
			if (!isAlive)
			{
				Object.Destroy(this);
			}
		}
	}

	public void Initialize(Material sharedOpaqueMaterial, Shader transparentShader)
	{
		data = new List<Data>();
		r = GetComponent<Renderer>();
		base.hideFlags = HideFlags.HideInInspector | HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild;
		Material[] sharedMaterials = r.sharedMaterials;
		if (sharedMaterials == null)
		{
			return;
		}
		for (int i = 0; i < sharedMaterials.Length; i++)
		{
			Material material = sharedMaterials[i];
			if (material == null)
			{
				continue;
			}
			Data item = default(Data);
			string text = material.GetTag(sRenderType, searchFallbacks: true, sOpaque);
			if (text == sTransparent || text == sTransparentCutout)
			{
				Material material2 = new Material(transparentShader);
				if (r is SpriteRenderer)
				{
					material2.SetInt(ShaderPropertyID._Cull, 0);
				}
				if (material.HasProperty(ShaderPropertyID._MainTex))
				{
					material2.SetTexture(ShaderPropertyID._MainTex, material.mainTexture);
					material2.SetTextureOffset(sMainTex, material.mainTextureOffset);
					material2.SetTextureScale(sMainTex, material.mainTextureScale);
				}
				int cutoff = ShaderPropertyID._Cutoff;
				material2.SetFloat(cutoff, material.HasProperty(cutoff) ? material.GetFloat(cutoff) : transparentCutoff);
				item.material = material2;
				item.transparent = true;
			}
			else
			{
				item.material = sharedOpaqueMaterial;
				item.transparent = false;
			}
			item.submeshIndex = i;
			data.Add(item);
		}
	}

	public bool FillBuffer(CommandBuffer buffer)
	{
		if (r == null)
		{
			return false;
		}
		if (lastCamera == Camera.current)
		{
			int i = 0;
			for (int count = this.data.Count; i < count; i++)
			{
				Data data = this.data[i];
				buffer.DrawRenderer(r, data.material, data.submeshIndex);
			}
		}
		return true;
	}

	public void SetColorForTransparent(Color clr)
	{
		int i = 0;
		for (int count = this.data.Count; i < count; i++)
		{
			Data data = this.data[i];
			if (data.transparent)
			{
				data.material.SetColor(ShaderPropertyID._Color, clr);
			}
		}
	}

	public void SetZTestForTransparent(int zTest)
	{
		int i = 0;
		for (int count = this.data.Count; i < count; i++)
		{
			Data data = this.data[i];
			if (data.transparent)
			{
				data.material.SetInt(ShaderPropertyID._ZTest, zTest);
			}
		}
	}

	public void SetStencilRefForTransparent(int stencilRef)
	{
		int i = 0;
		for (int count = this.data.Count; i < count; i++)
		{
			Data data = this.data[i];
			if (data.transparent)
			{
				data.material.SetInt(ShaderPropertyID._StencilRef, stencilRef);
			}
		}
	}

	public void SetState(bool alive)
	{
		isAlive = alive;
	}
}
