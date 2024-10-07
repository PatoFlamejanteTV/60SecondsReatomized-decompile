using System.Collections.Generic;
using UnityEngine;

namespace HighlightingSystem;

[RequireComponent(typeof(Camera))]
public class HighlightingBlitter : MonoBehaviour
{
	protected List<HighlightingBase> renderers = new List<HighlightingBase>();

	protected virtual void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		bool flag = true;
		for (int i = 0; i < renderers.Count; i++)
		{
			HighlightingBase highlightingBase = renderers[i];
			if (flag)
			{
				highlightingBase.Blit(src, dst);
			}
			else
			{
				highlightingBase.Blit(dst, src);
			}
			flag = !flag;
		}
		if (flag)
		{
			Graphics.Blit(src, dst);
		}
	}

	public virtual void Register(HighlightingBase renderer)
	{
		if (!renderers.Contains(renderer))
		{
			renderers.Add(renderer);
		}
		base.enabled = renderers.Count > 0;
	}

	public virtual void Unregister(HighlightingBase renderer)
	{
		int num = renderers.IndexOf(renderer);
		if (num != -1)
		{
			renderers.RemoveAt(num);
		}
		base.enabled = renderers.Count > 0;
	}
}
