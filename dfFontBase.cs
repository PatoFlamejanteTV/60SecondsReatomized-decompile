using System;
using UnityEngine;

[Serializable]
public abstract class dfFontBase : MonoBehaviour
{
	public abstract Material Material { get; set; }

	public abstract Texture Texture { get; }

	public abstract bool IsValid { get; }

	public abstract int FontSize { get; set; }

	public abstract int LineHeight { get; set; }

	public abstract dfFontRendererBase ObtainRenderer();
}
