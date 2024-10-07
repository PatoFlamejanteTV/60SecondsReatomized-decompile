using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace HighlightingSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class HighlightingBase : MonoBehaviour
{
	protected static readonly Color colorClear = new Color(0f, 0f, 0f, 0f);

	protected static readonly string renderBufferName = "HighlightingSystem";

	protected static readonly Matrix4x4 identityMatrix = Matrix4x4.identity;

	protected const CameraEvent queue = CameraEvent.BeforeImageEffectsOpaque;

	protected static RenderTargetIdentifier cameraTargetID;

	protected static Mesh quad;

	protected static GraphicsDeviceType device = GraphicsDeviceType.Null;

	public float offsetFactor;

	public float offsetUnits;

	protected CommandBuffer renderBuffer;

	protected int cachedWidth = -1;

	protected int cachedHeight = -1;

	protected int cachedAA = -1;

	[FormerlySerializedAs("downsampleFactor")]
	[SerializeField]
	protected int _downsampleFactor = 4;

	[FormerlySerializedAs("iterations")]
	[SerializeField]
	protected int _iterations = 2;

	[FormerlySerializedAs("blurMinSpread")]
	[SerializeField]
	protected float _blurMinSpread = 0.65f;

	[FormerlySerializedAs("blurSpread")]
	[SerializeField]
	protected float _blurSpread = 0.25f;

	[SerializeField]
	protected float _blurIntensity = 0.3f;

	[SerializeField]
	protected HighlightingBlitter _blitter;

	protected RenderTargetIdentifier highlightingBufferID;

	protected RenderTexture highlightingBuffer;

	protected Camera cam;

	protected bool isDepthAvailable = true;

	protected const int BLUR = 0;

	protected const int CUT = 1;

	protected const int COMP = 2;

	protected static readonly string[] shaderPaths = new string[3] { "Hidden/Highlighted/Blur", "Hidden/Highlighted/Cut", "Hidden/Highlighted/Composite" };

	protected static Shader[] shaders;

	protected static Material[] materials;

	protected Material blurMaterial;

	protected Material cutMaterial;

	protected Material compMaterial;

	protected static bool initialized = false;

	protected static HashSet<Camera> cameras = new HashSet<Camera>();

	protected static bool uvStartsAtTop
	{
		get
		{
			if (device != GraphicsDeviceType.Direct3D9 && device != GraphicsDeviceType.Xbox360 && device != GraphicsDeviceType.PlayStation3 && device != GraphicsDeviceType.Direct3D11 && device != GraphicsDeviceType.PlayStationVita && device != GraphicsDeviceType.PlayStation4)
			{
				return device == GraphicsDeviceType.Metal;
			}
			return true;
		}
	}

	public bool isSupported => CheckSupported(verbose: false);

	public int downsampleFactor
	{
		get
		{
			return _downsampleFactor;
		}
		set
		{
			if (_downsampleFactor != value)
			{
				if (value != 0 && (value & (value - 1)) == 0)
				{
					_downsampleFactor = value;
				}
				else
				{
					Debug.LogWarning("HighlightingSystem : Prevented attempt to set incorrect downsample factor value.");
				}
			}
		}
	}

	public int iterations
	{
		get
		{
			return _iterations;
		}
		set
		{
			if (_iterations != value)
			{
				_iterations = value;
			}
		}
	}

	public float blurMinSpread
	{
		get
		{
			return _blurMinSpread;
		}
		set
		{
			if (_blurMinSpread != value)
			{
				_blurMinSpread = value;
			}
		}
	}

	public float blurSpread
	{
		get
		{
			return _blurSpread;
		}
		set
		{
			if (_blurSpread != value)
			{
				_blurSpread = value;
			}
		}
	}

	public float blurIntensity
	{
		get
		{
			return _blurIntensity;
		}
		set
		{
			if (_blurIntensity != value)
			{
				_blurIntensity = value;
				if (Application.isPlaying)
				{
					blurMaterial.SetFloat(ShaderPropertyID._Intensity, _blurIntensity);
				}
			}
		}
	}

	public HighlightingBlitter blitter
	{
		get
		{
			return _blitter;
		}
		set
		{
			if (_blitter != null)
			{
				_blitter.Unregister(this);
			}
			_blitter = value;
			if (_blitter != null)
			{
				_blitter.Register(this);
			}
		}
	}

	protected virtual void OnEnable()
	{
		Initialize();
		if (!CheckSupported(verbose: true))
		{
			base.enabled = false;
			Debug.LogError("HighlightingSystem : Highlighting System has been disabled due to unsupported Unity features on the current platform!");
			return;
		}
		blurMaterial = new Material(materials[0]);
		cutMaterial = new Material(materials[1]);
		compMaterial = new Material(materials[2]);
		blurMaterial.SetFloat(ShaderPropertyID._Intensity, _blurIntensity);
		renderBuffer = new CommandBuffer();
		renderBuffer.name = renderBufferName;
		cam = GetComponent<Camera>();
		cameras.Add(cam);
		cam.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, renderBuffer);
		if (_blitter != null)
		{
			_blitter.Register(this);
		}
	}

	protected virtual void OnDisable()
	{
		cameras.Remove(cam);
		if (renderBuffer != null)
		{
			cam.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, renderBuffer);
			renderBuffer = null;
		}
		if (highlightingBuffer != null && highlightingBuffer.IsCreated())
		{
			highlightingBuffer.Release();
			highlightingBuffer = null;
		}
		if (_blitter != null)
		{
			_blitter.Unregister(this);
		}
	}

	protected virtual void OnPreRender()
	{
		bool flag = false;
		int aA = GetAA();
		bool flag2 = aA == 1;
		if (cam.actualRenderingPath == RenderingPath.Forward || cam.actualRenderingPath == RenderingPath.VertexLit)
		{
			if (aA > 1)
			{
				flag2 = false;
			}
			if (cam.clearFlags == CameraClearFlags.Depth || cam.clearFlags == CameraClearFlags.Nothing)
			{
				flag2 = false;
			}
		}
		if (isDepthAvailable != flag2)
		{
			flag = true;
			isDepthAvailable = flag2;
			Highlighter.SetZWrite((!isDepthAvailable) ? 1 : 0);
			if (isDepthAvailable)
			{
				Debug.LogWarning("HighlightingSystem : Framebuffer depth data is available back again. Depth occlusion enabled, highlighting occluders disabled. (This message is harmless)");
			}
			else
			{
				Debug.LogWarning("HighlightingSystem : Framebuffer depth data is not available. Depth occlusion disabled, highlighting occluders enabled. (This message is harmless)");
			}
		}
		if (flag | (highlightingBuffer == null || cam.pixelWidth != cachedWidth || cam.pixelHeight != cachedHeight || aA != cachedAA))
		{
			if (highlightingBuffer != null && highlightingBuffer.IsCreated())
			{
				highlightingBuffer.Release();
			}
			cachedWidth = cam.pixelWidth;
			cachedHeight = cam.pixelHeight;
			cachedAA = aA;
			highlightingBuffer = new RenderTexture(cachedWidth, cachedHeight, (!isDepthAvailable) ? 24 : 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
			highlightingBuffer.antiAliasing = cachedAA;
			highlightingBuffer.filterMode = FilterMode.Point;
			highlightingBuffer.useMipMap = false;
			highlightingBuffer.wrapMode = TextureWrapMode.Clamp;
			if (!highlightingBuffer.Create())
			{
				Debug.LogError("HighlightingSystem : UpdateHighlightingBuffer() : Failed to create highlightingBuffer RenderTexture!");
			}
			highlightingBufferID = new RenderTargetIdentifier(highlightingBuffer);
			cutMaterial.SetTexture(ShaderPropertyID._HighlightingBuffer, highlightingBuffer);
			compMaterial.SetTexture(ShaderPropertyID._HighlightingBuffer, highlightingBuffer);
			Vector4 value = new Vector4(1f / (float)highlightingBuffer.width, 1f / (float)highlightingBuffer.height, 0f, 0f);
			cutMaterial.SetVector(ShaderPropertyID._HighlightingBufferTexelSize, value);
		}
		Highlighter.SetOffsetFactor(offsetFactor);
		Highlighter.SetOffsetUnits(offsetUnits);
		RebuildCommandBuffer();
	}

	protected virtual void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (blitter == null)
		{
			Blit(src, dst);
		}
		else
		{
			Graphics.Blit(src, dst);
		}
	}

	public static bool IsHighlightingCamera(Camera cam)
	{
		return cameras.Contains(cam);
	}

	protected static void Initialize()
	{
		if (!initialized)
		{
			device = SystemInfo.graphicsDeviceType;
			ShaderPropertyID.Initialize();
			int num = shaderPaths.Length;
			shaders = new Shader[num];
			materials = new Material[num];
			for (int i = 0; i < num; i++)
			{
				Shader shader = Shader.Find(shaderPaths[i]);
				shaders[i] = shader;
				Material material = new Material(shader);
				materials[i] = material;
			}
			cameraTargetID = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);
			CreateQuad();
			initialized = true;
		}
	}

	protected static void CreateQuad()
	{
		if (quad == null)
		{
			quad = new Mesh();
		}
		else
		{
			quad.Clear();
		}
		float y = -1f;
		float y2 = 1f;
		if (uvStartsAtTop)
		{
			y = 1f;
			y2 = -1f;
		}
		quad.vertices = new Vector3[4]
		{
			new Vector3(-1f, y, 0f),
			new Vector3(-1f, y2, 0f),
			new Vector3(1f, y2, 0f),
			new Vector3(1f, y, 0f)
		};
		quad.uv = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f)
		};
		quad.colors = new Color[4] { colorClear, colorClear, colorClear, colorClear };
		quad.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
	}

	protected virtual int GetAA()
	{
		int num = QualitySettings.antiAliasing;
		if (num == 0)
		{
			num = 1;
		}
		if (cam.actualRenderingPath == RenderingPath.DeferredLighting || cam.actualRenderingPath == RenderingPath.DeferredShading)
		{
			num = 1;
		}
		return num;
	}

	protected virtual bool CheckSupported(bool verbose)
	{
		bool result = true;
		if (!SystemInfo.supportsImageEffects)
		{
			if (verbose)
			{
				Debug.LogError("HighlightingSystem : Image effects is not supported on this platform!");
			}
			result = false;
		}
		if (!SystemInfo.supportsRenderTextures)
		{
			if (verbose)
			{
				Debug.LogError("HighlightingSystem : RenderTextures is not supported on this platform!");
			}
			result = false;
		}
		if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32))
		{
			if (verbose)
			{
				Debug.LogError("HighlightingSystem : RenderTextureFormat.ARGB32 is not supported on this platform!");
			}
			result = false;
		}
		if (SystemInfo.supportsStencil < 1)
		{
			if (verbose)
			{
				Debug.LogError("HighlightingSystem : Stencil buffer is not supported on this platform!");
			}
			result = false;
		}
		if (!Highlighter.opaqueShader.isSupported)
		{
			if (verbose)
			{
				Debug.LogError("HighlightingSystem : HighlightingOpaque shader is not supported on this platform!");
			}
			result = false;
		}
		if (!Highlighter.transparentShader.isSupported)
		{
			if (verbose)
			{
				Debug.LogError("HighlightingSystem : HighlightingTransparent shader is not supported on this platform!");
			}
			result = false;
		}
		for (int i = 0; i < shaders.Length; i++)
		{
			Shader shader = shaders[i];
			if (!shader.isSupported)
			{
				if (verbose)
				{
					Debug.LogError("HighlightingSystem : Shader '" + shader.name + "' is not supported on this platform!");
				}
				result = false;
			}
		}
		return result;
	}

	protected virtual void RebuildCommandBuffer()
	{
		renderBuffer.Clear();
		RenderTargetIdentifier depth = (isDepthAvailable ? cameraTargetID : highlightingBufferID);
		renderBuffer.SetRenderTarget(highlightingBufferID, depth);
		renderBuffer.ClearRenderTarget(!isDepthAvailable, clearColor: true, colorClear);
		Highlighter.FillBuffer(renderBuffer, isDepthAvailable);
		RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(ShaderPropertyID._HighlightingBlur1);
		RenderTargetIdentifier renderTargetIdentifier2 = new RenderTargetIdentifier(ShaderPropertyID._HighlightingBlur2);
		int width = highlightingBuffer.width / _downsampleFactor;
		int height = highlightingBuffer.height / _downsampleFactor;
		renderBuffer.GetTemporaryRT(ShaderPropertyID._HighlightingBlur1, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
		renderBuffer.GetTemporaryRT(ShaderPropertyID._HighlightingBlur2, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
		renderBuffer.Blit(highlightingBufferID, renderTargetIdentifier);
		bool flag = true;
		for (int i = 0; i < _iterations; i++)
		{
			float value = _blurMinSpread + _blurSpread * (float)i;
			renderBuffer.SetGlobalFloat(ShaderPropertyID._HighlightingBlurOffset, value);
			if (flag)
			{
				renderBuffer.Blit(renderTargetIdentifier, renderTargetIdentifier2, blurMaterial);
			}
			else
			{
				renderBuffer.Blit(renderTargetIdentifier2, renderTargetIdentifier, blurMaterial);
			}
			flag = !flag;
		}
		renderBuffer.SetGlobalTexture(ShaderPropertyID._HighlightingBlurred, flag ? renderTargetIdentifier : renderTargetIdentifier2);
		renderBuffer.SetRenderTarget(highlightingBufferID, depth);
		renderBuffer.DrawMesh(quad, identityMatrix, cutMaterial);
		renderBuffer.ReleaseTemporaryRT(ShaderPropertyID._HighlightingBlur1);
		renderBuffer.ReleaseTemporaryRT(ShaderPropertyID._HighlightingBlur2);
	}

	public virtual void Blit(RenderTexture src, RenderTexture dst)
	{
		Graphics.Blit(src, dst, compMaterial);
	}
}
