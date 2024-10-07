using System;
using System.Collections.Generic;
using RG_GameCamera.Effects;
using RG_GameCamera.Input;
using RG_GameCamera.Modes;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera;

public class CameraManager : MonoBehaviour
{
	public delegate void OnTransitionFinished();

	private struct CameraTransform
	{
		private Vector3 pos;

		private Quaternion rot;

		private float fov;

		private Vector3 posVel;

		private Vector3 rotVel;

		private float fovVel;

		private float timeout;

		private float speedRatio;

		public CameraTransform(Camera cam)
		{
			pos = cam.transform.position;
			rot = cam.transform.rotation;
			fov = cam.fieldOfView;
			posVel = Vector3.zero;
			rotVel = Vector3.zero;
			fovVel = 0f;
			timeout = 0f;
			speedRatio = 1f;
		}

		public bool Interpolate(Camera cam, float speed, float timeMax)
		{
			float smoothTime = speed * speedRatio;
			pos = Vector3.SmoothDamp(pos, cam.transform.position, ref posVel, smoothTime);
			rot = Quaternion.Euler(Vector3.SmoothDamp(rot.eulerAngles, cam.transform.eulerAngles, ref rotVel, smoothTime));
			RG_GameCamera.Utils.Math.CorrectRotationUp(ref rot);
			fov = Mathf.SmoothDamp(fov, cam.fieldOfView, ref fovVel, 0.05f);
			bool num = (cam.transform.position - pos).sqrMagnitude < 0.001f && Quaternion.Angle(cam.transform.rotation, rot) < 0.001f && Mathf.Abs(fov - cam.fieldOfView) < 0.001f;
			timeout += Time.deltaTime;
			speedRatio = 1f - Mathf.Clamp01(timeout / timeMax);
			cam.transform.position = pos;
			cam.transform.rotation = rot;
			cam.fieldOfView = fov;
			return !num;
		}
	}

	public Camera UnityCamera;

	public float TransitionSpeed = 0.5f;

	public float TransitionTimeMax = 1f;

	public GUISkin GuiSkin;

	public RG_GameCamera.Modes.Type ActivateModeOnStart;

	public Transform CameraTarget;

	private static CameraManager instance;

	private static Queue<CameraMode> preRegistered;

	private Dictionary<RG_GameCamera.Modes.Type, CameraMode> cameraModes;

	private bool transition;

	private RG_GameCamera.Modes.Type currModeType;

	private bool initialized;

	private CameraTransform oldModeTransform;

	private OnTransitionFinished finishedCallbak;

	public static CameraManager Instance
	{
		get
		{
			if (!instance)
			{
				instance = CameraInstance.CreateInstance<CameraManager>("CameraManager");
			}
			return instance;
		}
	}

	public void RegisterMode(CameraMode cameraModeMode)
	{
		if (cameraModes == null)
		{
			if (preRegistered == null)
			{
				preRegistered = new Queue<CameraMode>();
			}
			preRegistered.Enqueue(cameraModeMode);
		}
		else
		{
			cameraModes.Add(cameraModeMode.Type, cameraModeMode);
			cameraModeMode.gameObject.SetActive(value: false);
		}
	}

	public CameraMode SetMode(RG_GameCamera.Modes.Type cameraMode)
	{
		Initialize();
		if (currModeType != cameraMode)
		{
			cameraModes[currModeType].OnDeactivate();
			RG_GameCamera.Utils.Debug.SetActive(cameraModes[currModeType].gameObject, status: false);
			oldModeTransform = new CameraTransform(UnityCamera);
			transition = true;
			currModeType = cameraMode;
			RG_GameCamera.Utils.Debug.SetActive(cameraModes[currModeType].gameObject, status: true);
			cameraModes[currModeType].SetCameraTarget(CameraTarget);
			cameraModes[currModeType].OnActivate();
		}
		return cameraModes[currModeType];
	}

	public void SetCameraTarget(Transform target)
	{
		CameraTarget = target;
		cameraModes[currModeType].SetCameraTarget(target);
	}

	public CameraMode GetCameraMode()
	{
		if (cameraModes != null && cameraModes.ContainsKey(currModeType))
		{
			return cameraModes[currModeType];
		}
		return null;
	}

	public void RegisterTransitionCallback(OnTransitionFinished callback)
	{
		finishedCallbak = (OnTransitionFinished)Delegate.Combine(finishedCallbak, callback);
	}

	public void UnregisterTransitionCallback(OnTransitionFinished callback)
	{
		finishedCallbak = (OnTransitionFinished)Delegate.Remove(finishedCallbak, callback);
	}

	private void Awake()
	{
		instance = this;
		cameraModes = new Dictionary<RG_GameCamera.Modes.Type, CameraMode>();
		currModeType = RG_GameCamera.Modes.Type.None;
		if (!UnityCamera)
		{
			UnityCamera = GetComponent<Camera>();
			if (!UnityCamera)
			{
				UnityCamera = Camera.main;
			}
		}
		if (preRegistered == null)
		{
			return;
		}
		foreach (CameraMode item in preRegistered)
		{
			RegisterMode(item);
		}
		preRegistered.Clear();
	}

	private void Initialize()
	{
		if (initialized)
		{
			return;
		}
		foreach (KeyValuePair<RG_GameCamera.Modes.Type, CameraMode> cameraMode in cameraModes)
		{
			cameraMode.Value.Init();
		}
		initialized = true;
	}

	private void Start()
	{
		Initialize();
		SetMode(ActivateModeOnStart);
	}

	private void Update()
	{
		InputManager.Instance.GameUpdate();
		cameraModes[currModeType].GameUpdate();
	}

	private void LateUpdate()
	{
		cameraModes[currModeType].PostUpdate();
		if (transition)
		{
			transition = oldModeTransform.Interpolate(UnityCamera, TransitionSpeed, TransitionTimeMax);
			if (!transition && finishedCallbak != null)
			{
				finishedCallbak();
			}
		}
		EffectManager.Instance.PostUpdate();
	}

	private void FixedUpdate()
	{
		cameraModes[currModeType].FixedStepUpdate();
	}
}
