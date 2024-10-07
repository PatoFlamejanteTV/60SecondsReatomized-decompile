using System;
using Cinemachine;
using RG.Parsecs.EventEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace RG.Remaster.Common;

[ExecuteInEditMode]
public class PerspectiveCameraAspect : MonoBehaviour
{
	[SerializeField]
	[FormerlySerializedAs("horizontalFoV")]
	private float _horizontalFoV = 90f;

	[SerializeField]
	private GlobalIntVariable _screenHeight;

	[SerializeField]
	private GlobalIntVariable _screenWidth;

	[SerializeField]
	private CinemachineBrain _cinemachineBrain;

	[SerializeField]
	private CinemachineVirtualCamera _forcedCamera;

	private void Update()
	{
		float f = Mathf.Tan(0.5f * _horizontalFoV * ((float)Math.PI / 180f)) * (float)_screenHeight.Value / (float)_screenWidth.Value;
		float num = 2f * Mathf.Atan(f) * 57.29578f;
		if (CinemachineBrain.SoloCamera != _forcedCamera && _forcedCamera != null)
		{
			CinemachineBrain.SoloCamera = _forcedCamera;
		}
		CinemachineVirtualCamera cinemachineVirtualCamera = _cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
		if (cinemachineVirtualCamera != null && cinemachineVirtualCamera.m_Lens.FieldOfView != num)
		{
			cinemachineVirtualCamera.m_Lens.FieldOfView = num;
		}
	}
}
