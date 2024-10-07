using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResolutionHandler : MonoBehaviour
{
	[SerializeField]
	private float _nativeWidth = 1920f;

	[SerializeField]
	private float _nativeHeight = 1080f;

	private float _nativeAspectRatio = 1f;

	[SerializeField]
	private SResolutionAspectRatio[] _aspectRatios;

	private Resolution[] _availableResolutions;

	private SResolutionAspectRatio _selectedAspectRatio;

	private Vector2 _adjustedMousePositionFactor = Vector2.zero;

	private Camera _2dCamera;

	private Camera _3dCamera;

	public float ResizeRatio => _selectedAspectRatio.AspectRatio / _nativeAspectRatio;

	public Vector2 AdjustedMousePosition => new Vector2(Input.mousePosition.x * _2dCamera.orthographicSize - _adjustedMousePositionFactor.x, Input.mousePosition.y * _2dCamera.orthographicSize - _adjustedMousePositionFactor.y);

	public Resolution[] Resolutions => _availableResolutions;

	public SResolutionAspectRatio SelectedAspectRatio => _selectedAspectRatio;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		_nativeAspectRatio = _nativeWidth / _nativeHeight;
		LoadResolutions();
	}

	private void Start()
	{
	}

	private void UpdateCamera()
	{
		_2dCamera = null;
		_3dCamera = null;
		if (Camera.main == null)
		{
			return;
		}
		if (Camera.main.orthographic)
		{
			_2dCamera = Camera.main;
			return;
		}
		_3dCamera = Camera.main;
		GameObject gameObject = GameObject.FindGameObjectWithTag("UICamera");
		if (gameObject != null)
		{
			_2dCamera = gameObject.GetComponent<Camera>();
		}
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		if (scene.name.Contains("scavenge") || scene.name.Contains("challenge") || scene.name.Contains("tutorial"))
		{
			UpdateCamera();
			HandleResolution();
		}
	}

	private void LoadResolutions()
	{
		List<Resolution> list = new List<Resolution>();
		for (int i = 0; i < Screen.resolutions.Length; i++)
		{
			if (FindResolution(Screen.resolutions[i].width, Screen.resolutions[i].height))
			{
				list.Add(Screen.resolutions[i]);
			}
		}
		_availableResolutions = list.ToArray();
	}

	public void HandleResolution()
	{
		FindResolution(Settings.Data.ResX, Settings.Data.ResY, out _selectedAspectRatio);
		Camera component = _2dCamera;
		if (component == null || (component != null && !component.orthographic))
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("UICamera");
			if (gameObject != null)
			{
				component = gameObject.GetComponent<Camera>();
			}
		}
		if (component != null)
		{
			component.orthographicSize = _selectedAspectRatio.LocalResolutionFactor;
			float y = ((float)Settings.Data.ResY - (float)Settings.Data.ResX / _nativeWidth * _nativeHeight) / 2f * _selectedAspectRatio.CursorOffset.y;
			float x = ((float)Settings.Data.ResY - (float)Settings.Data.ResX / _nativeWidth * _nativeHeight) * _selectedAspectRatio.CursorOffset.x;
			_adjustedMousePositionFactor = new Vector2(x, y);
		}
	}

	public bool FindResolution(float width, float height)
	{
		if (_aspectRatios != null)
		{
			for (int i = 0; i < _aspectRatios.Length; i++)
			{
				if (_aspectRatios[i].Enabled && _aspectRatios[i].FindResolution(width, height))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool FindResolution(float width, float height, out SResolutionAspectRatio data)
	{
		if (_aspectRatios != null)
		{
			for (int i = 0; i < _aspectRatios.Length; i++)
			{
				if (_aspectRatios[i].Enabled && _aspectRatios[i].FindResolution(width, height))
				{
					data = _aspectRatios[i];
					return true;
				}
			}
		}
		data = SResolutionAspectRatio.EMPTY;
		return false;
	}

	public static bool Is54(float val)
	{
		return Mathf.Approximately(val, 1.25f);
	}

	public static bool Is1610(float val)
	{
		return Mathf.Approximately(val, 1.6f);
	}

	public static bool Is169(float val)
	{
		return Mathf.Approximately(val, 1.77f);
	}

	public static bool Is43(float val)
	{
		if (!Mathf.Approximately(val, 1.33f))
		{
			return Mathf.Approximately(val, 1.25f);
		}
		return true;
	}
}
