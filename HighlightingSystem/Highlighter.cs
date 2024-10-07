using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HighlightingSystem;

[DisallowMultipleComponent]
public class Highlighter : MonoBehaviour
{
	private enum Mode
	{
		None,
		Highlighter,
		Occluder,
		HighlighterSeeThrough,
		OccluderSeeThrough
	}

	public static readonly List<Type> types = new List<Type>
	{
		typeof(MeshRenderer),
		typeof(SkinnedMeshRenderer),
		typeof(SpriteRenderer),
		typeof(ParticleSystemRenderer)
	};

	private const float doublePI = (float)Math.PI * 2f;

	private readonly Color occluderColor = new Color(0f, 0f, 0f, 0f);

	private const int zTestLessEqual = 4;

	private const int zTestAlways = 8;

	private static readonly Mode[] renderingOrder = new Mode[4]
	{
		Mode.Highlighter,
		Mode.Occluder,
		Mode.HighlighterSeeThrough,
		Mode.OccluderSeeThrough
	};

	private static HashSet<Highlighter> highlighters = new HashSet<Highlighter>();

	private static int zWrite = -1;

	private static float offsetFactor = float.NaN;

	private static float offsetUnits = float.NaN;

	[HideInInspector]
	public bool seeThrough;

	[HideInInspector]
	public bool occluder;

	private Transform tr;

	private List<HighlighterRenderer> highlightableRenderers = new List<HighlighterRenderer>();

	private bool renderersDirty;

	private static List<Component> sComponents = new List<Component>(4);

	private Mode mode;

	private bool zTest;

	private bool stencilRef;

	private int _once = -1;

	private bool flashing;

	private Color currentColor;

	private float transitionValue;

	private float transitionTarget;

	private float transitionTime;

	private Color onceColor;

	private float flashingFreq;

	private Color flashingColorMin;

	private Color flashingColorMax;

	private Color constantColor;

	private static Shader _opaqueShader;

	private static Shader _transparentShader;

	private Material _opaqueMaterial;

	private bool once
	{
		get
		{
			return _once == Time.frameCount;
		}
		set
		{
			_once = (value ? Time.frameCount : (-1));
		}
	}

	public static Shader opaqueShader
	{
		get
		{
			if (_opaqueShader == null)
			{
				_opaqueShader = Shader.Find("Hidden/Highlighted/Opaque");
			}
			return _opaqueShader;
		}
	}

	public static Shader transparentShader
	{
		get
		{
			if (_transparentShader == null)
			{
				_transparentShader = Shader.Find("Hidden/Highlighted/Transparent");
			}
			return _transparentShader;
		}
	}

	private Material opaqueMaterial
	{
		get
		{
			if (_opaqueMaterial == null)
			{
				_opaqueMaterial = new Material(opaqueShader);
				ShaderPropertyID.Initialize();
				_opaqueMaterial.SetInt(ShaderPropertyID._ZTest, GetZTest(zTest));
				_opaqueMaterial.SetInt(ShaderPropertyID._StencilRef, GetStencilRef(stencilRef));
			}
			return _opaqueMaterial;
		}
	}

	public void ReinitMaterials()
	{
		renderersDirty = true;
	}

	public void OnParams(Color color)
	{
		onceColor = color;
	}

	public void On()
	{
		once = true;
	}

	public void On(Color color)
	{
		onceColor = color;
		once = true;
	}

	public void FlashingParams(Color color1, Color color2, float freq)
	{
		flashingColorMin = color1;
		flashingColorMax = color2;
		flashingFreq = freq;
	}

	public void FlashingOn()
	{
		flashing = true;
	}

	public void FlashingOn(Color color1, Color color2)
	{
		flashingColorMin = color1;
		flashingColorMax = color2;
		flashing = true;
	}

	public void FlashingOn(Color color1, Color color2, float freq)
	{
		flashingColorMin = color1;
		flashingColorMax = color2;
		flashingFreq = freq;
		flashing = true;
	}

	public void FlashingOn(float freq)
	{
		flashingFreq = freq;
		flashing = true;
	}

	public void FlashingOff()
	{
		flashing = false;
	}

	public void FlashingSwitch()
	{
		flashing = !flashing;
	}

	public void ConstantParams(Color color)
	{
		constantColor = color;
	}

	public void ConstantOn(float time = 0.25f)
	{
		transitionTime = ((time >= 0f) ? time : 0f);
		transitionTarget = 1f;
	}

	public void ConstantOn(Color color, float time = 0.25f)
	{
		constantColor = color;
		transitionTime = ((time >= 0f) ? time : 0f);
		transitionTarget = 1f;
	}

	public void ConstantOff(float time = 0.25f)
	{
		transitionTime = ((time >= 0f) ? time : 0f);
		transitionTarget = 0f;
	}

	public void ConstantSwitch(float time = 0.25f)
	{
		transitionTime = ((time >= 0f) ? time : 0f);
		transitionTarget = ((transitionTarget > 0f) ? 0f : 1f);
	}

	public void ConstantOnImmediate()
	{
		transitionValue = (transitionTarget = 1f);
	}

	public void ConstantOnImmediate(Color color)
	{
		constantColor = color;
		transitionValue = (transitionTarget = 1f);
	}

	public void ConstantOffImmediate()
	{
		transitionValue = (transitionTarget = 0f);
	}

	public void ConstantSwitchImmediate()
	{
		transitionValue = (transitionTarget = ((transitionTarget > 0f) ? 0f : 1f));
	}

	public void Off()
	{
		once = false;
		flashing = false;
		transitionValue = (transitionTarget = 0f);
	}

	public void Die()
	{
		UnityEngine.Object.Destroy(this);
	}

	public void SeeThrough(bool state)
	{
		seeThrough = state;
	}

	public void SeeThroughOn()
	{
		seeThrough = true;
	}

	public void SeeThroughOff()
	{
		seeThrough = false;
	}

	public void SeeThroughSwitch()
	{
		seeThrough = !seeThrough;
	}

	public void OccluderOn()
	{
		occluder = true;
	}

	public void OccluderOff()
	{
		occluder = false;
	}

	public void OccluderSwitch()
	{
		occluder = !occluder;
	}

	private void Awake()
	{
		ShaderPropertyID.Initialize();
		tr = GetComponent<Transform>();
		renderersDirty = true;
		seeThrough = (zTest = true);
		mode = Mode.None;
		stencilRef = true;
		once = false;
		flashing = false;
		occluder = false;
		transitionValue = (transitionTarget = 0f);
		onceColor = Color.red;
		flashingFreq = 2f;
		flashingColorMin = new Color(0f, 1f, 1f, 0f);
		flashingColorMax = new Color(0f, 1f, 1f, 1f);
		constantColor = Color.yellow;
	}

	private void OnEnable()
	{
		highlighters.Add(this);
	}

	private void OnDisable()
	{
		highlighters.Remove(this);
		ClearRenderers();
		renderersDirty = true;
		once = false;
		flashing = false;
		transitionValue = (transitionTarget = 0f);
	}

	private void Update()
	{
		UpdateTransition();
	}

	private void ClearRenderers()
	{
		for (int num = highlightableRenderers.Count - 1; num >= 0; num--)
		{
			highlightableRenderers[num].SetState(alive: false);
		}
		highlightableRenderers.Clear();
	}

	private void UpdateRenderers()
	{
		if (!renderersDirty)
		{
			return;
		}
		ClearRenderers();
		List<Renderer> list = new List<Renderer>();
		GrabRenderers(tr, list);
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			GameObject gameObject = list[i].gameObject;
			HighlighterRenderer highlighterRenderer = gameObject.GetComponent<HighlighterRenderer>();
			if (highlighterRenderer == null)
			{
				highlighterRenderer = gameObject.AddComponent<HighlighterRenderer>();
			}
			highlighterRenderer.SetState(alive: true);
			highlighterRenderer.Initialize(opaqueMaterial, transparentShader);
			highlightableRenderers.Add(highlighterRenderer);
		}
		renderersDirty = false;
	}

	private void GrabRenderers(Transform t, List<Renderer> renderers)
	{
		GameObject gameObject = t.gameObject;
		int i = 0;
		for (int count = types.Count; i < count; i++)
		{
			gameObject.GetComponents(types[i], sComponents);
			int j = 0;
			for (int count2 = sComponents.Count; j < count2; j++)
			{
				renderers.Add(sComponents[j] as Renderer);
			}
		}
		sComponents.Clear();
		int childCount = t.childCount;
		if (childCount == 0)
		{
			return;
		}
		for (int k = 0; k < childCount; k++)
		{
			Transform child = t.GetChild(k);
			if (!(child.GetComponent<Highlighter>() != null) && !(child.GetComponent<HighlighterBlocker>() != null))
			{
				GrabRenderers(child, renderers);
			}
		}
	}

	private void UpdateShaderParams(bool zt, bool sr)
	{
		if (zTest != zt)
		{
			zTest = zt;
			int num = GetZTest(zTest);
			opaqueMaterial.SetInt(ShaderPropertyID._ZTest, num);
			for (int i = 0; i < highlightableRenderers.Count; i++)
			{
				highlightableRenderers[i].SetZTestForTransparent(num);
			}
		}
		if (stencilRef != sr)
		{
			stencilRef = sr;
			int num2 = GetStencilRef(stencilRef);
			opaqueMaterial.SetInt(ShaderPropertyID._StencilRef, num2);
			for (int j = 0; j < highlightableRenderers.Count; j++)
			{
				highlightableRenderers[j].SetStencilRefForTransparent(num2);
			}
		}
	}

	private void UpdateColors()
	{
		if (once)
		{
			currentColor = onceColor;
		}
		else if (flashing)
		{
			currentColor = Color.Lerp(flashingColorMin, flashingColorMax, 0.5f * Mathf.Sin(Time.realtimeSinceStartup * flashingFreq * ((float)Math.PI * 2f)) + 0.5f);
		}
		else if (transitionValue > 0f)
		{
			currentColor = constantColor;
			currentColor.a *= transitionValue;
		}
		else
		{
			if (!occluder)
			{
				return;
			}
			currentColor = occluderColor;
		}
		opaqueMaterial.SetColor(ShaderPropertyID._Color, currentColor);
		for (int i = 0; i < highlightableRenderers.Count; i++)
		{
			highlightableRenderers[i].SetColorForTransparent(currentColor);
		}
	}

	private void UpdateTransition()
	{
		if (transitionValue != transitionTarget)
		{
			if (transitionTime <= 0f)
			{
				transitionValue = transitionTarget;
				return;
			}
			float num = ((transitionTarget > 0f) ? 1f : (-1f));
			transitionValue = Mathf.Clamp01(transitionValue + num * Time.unscaledDeltaTime / transitionTime);
		}
	}

	private void FillBufferInternal(CommandBuffer buffer, Mode m, bool depthAvailable)
	{
		UpdateRenderers();
		bool flag = once || flashing || transitionValue > 0f;
		bool flag2 = occluder && (seeThrough || !depthAvailable);
		mode = Mode.None;
		if (flag)
		{
			mode = ((!seeThrough) ? Mode.Highlighter : Mode.HighlighterSeeThrough);
		}
		else if (flag2)
		{
			mode = (seeThrough ? Mode.OccluderSeeThrough : Mode.Occluder);
		}
		if (mode == Mode.None || mode != m)
		{
			return;
		}
		if (flag)
		{
			UpdateShaderParams(seeThrough, sr: true);
		}
		else if (flag2)
		{
			UpdateShaderParams(zt: false, seeThrough);
		}
		UpdateColors();
		for (int num = highlightableRenderers.Count - 1; num >= 0; num--)
		{
			HighlighterRenderer highlighterRenderer = highlightableRenderers[num];
			if (highlighterRenderer == null)
			{
				highlightableRenderers.RemoveAt(num);
			}
			else if (!highlighterRenderer.FillBuffer(buffer))
			{
				highlightableRenderers.RemoveAt(num);
				highlighterRenderer.SetState(alive: false);
			}
		}
	}

	public static void FillBuffer(CommandBuffer buffer, bool depthAvailable)
	{
		for (int i = 0; i < renderingOrder.Length; i++)
		{
			Mode m = renderingOrder[i];
			HashSet<Highlighter>.Enumerator enumerator = highlighters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.FillBufferInternal(buffer, m, depthAvailable);
			}
		}
	}

	private static int GetZTest(bool enabled)
	{
		if (!enabled)
		{
			return 4;
		}
		return 8;
	}

	private static int GetStencilRef(bool enabled)
	{
		if (!enabled)
		{
			return 0;
		}
		return 1;
	}

	public static void SetZWrite(int value)
	{
		if (zWrite != value)
		{
			zWrite = value;
			Shader.SetGlobalInt(ShaderPropertyID._HighlightingZWrite, zWrite);
		}
	}

	public static void SetOffsetFactor(float value)
	{
		if (offsetFactor != value)
		{
			offsetFactor = value;
			Shader.SetGlobalFloat(ShaderPropertyID._HighlightingOffsetFactor, offsetFactor);
		}
	}

	public static void SetOffsetUnits(float value)
	{
		if (offsetUnits != value)
		{
			offsetUnits = value;
			Shader.SetGlobalFloat(ShaderPropertyID._HighlightingOffsetUnits, offsetUnits);
		}
	}
}
