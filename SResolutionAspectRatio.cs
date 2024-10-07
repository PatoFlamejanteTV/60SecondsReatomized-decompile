using System;
using UnityEngine;

[Serializable]
public struct SResolutionAspectRatio
{
	public static SResolutionAspectRatio EMPTY;

	[SerializeField]
	private bool _enabled;

	[SerializeField]
	private float _aspectRatio;

	[SerializeField]
	private float _localResolutionFactor;

	[SerializeField]
	private Vector2 _cursorOffset;

	[SerializeField]
	private float _camFov;

	[SerializeField]
	private Vector2[] _supportedResolutions;

	public bool Enabled => _enabled;

	public float CamFov => _camFov;

	public float AspectRatio => _aspectRatio;

	public Vector2 CursorOffset => _cursorOffset;

	public float LocalResolutionFactor => _localResolutionFactor;

	public Vector2[] SupportedResolutions => _supportedResolutions;

	public bool FindResolution(float width, float height)
	{
		if (_supportedResolutions != null)
		{
			for (int i = 0; i < _supportedResolutions.Length; i++)
			{
				if (Mathf.Approximately(width, _supportedResolutions[i].x) && Mathf.Approximately(height, _supportedResolutions[i].y))
				{
					return true;
				}
			}
		}
		return false;
	}
}
