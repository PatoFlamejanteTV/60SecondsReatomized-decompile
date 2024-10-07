using UnityEngine;

namespace HighlightingSystem;

public static class ShaderPropertyID
{
	private static bool initialized;

	public static int _MainTex { get; private set; }

	public static int _Color { get; private set; }

	public static int _Cutoff { get; private set; }

	public static int _Intensity { get; private set; }

	public static int _ZTest { get; private set; }

	public static int _StencilRef { get; private set; }

	public static int _Cull { get; private set; }

	public static int _HighlightingBlur1 { get; private set; }

	public static int _HighlightingBlur2 { get; private set; }

	public static int _HighlightingBuffer { get; private set; }

	public static int _HighlightingBufferTexelSize { get; private set; }

	public static int _HighlightingBlurred { get; private set; }

	public static int _HighlightingBlurOffset { get; private set; }

	public static int _HighlightingZWrite { get; private set; }

	public static int _HighlightingOffsetFactor { get; private set; }

	public static int _HighlightingOffsetUnits { get; private set; }

	public static void Initialize()
	{
		if (!initialized)
		{
			_MainTex = Shader.PropertyToID("_MainTex");
			_Color = Shader.PropertyToID("_Color");
			_Cutoff = Shader.PropertyToID("_Cutoff");
			_Intensity = Shader.PropertyToID("_Intensity");
			_ZTest = Shader.PropertyToID("_ZTest");
			_StencilRef = Shader.PropertyToID("_StencilRef");
			_Cull = Shader.PropertyToID("_Cull");
			_HighlightingBlur1 = Shader.PropertyToID("_HighlightingBlur1");
			_HighlightingBlur2 = Shader.PropertyToID("_HighlightingBlur2");
			_HighlightingBuffer = Shader.PropertyToID("_HighlightingBuffer");
			_HighlightingBufferTexelSize = Shader.PropertyToID("_HighlightingBufferTexelSize");
			_HighlightingBlurred = Shader.PropertyToID("_HighlightingBlurred");
			_HighlightingBlurOffset = Shader.PropertyToID("_HighlightingBlurOffset");
			_HighlightingZWrite = Shader.PropertyToID("_HighlightingZWrite");
			_HighlightingOffsetFactor = Shader.PropertyToID("_HighlightingOffsetFactor");
			_HighlightingOffsetUnits = Shader.PropertyToID("_HighlightingOffsetUnits");
			initialized = true;
		}
	}
}
