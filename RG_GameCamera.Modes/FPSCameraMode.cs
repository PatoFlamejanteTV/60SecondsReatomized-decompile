using System;
using RG_GameCamera.Config;
using RG_GameCamera.Input;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Modes;

[RequireComponent(typeof(FPSConfig))]
public class FPSCameraMode : CameraMode
{
	private float rotX;

	private float rotY;

	private bool targetHide;

	private float activateTimeout;

	public override Type Type => Type.FPS;

	public override void OnActivate()
	{
		base.OnActivate();
		if ((bool)Target)
		{
			cameraTarget = Target.position;
			UnityCamera.transform.position = GetEyePos();
			UnityCamera.transform.LookAt(GetEyePos() + Target.forward);
			RotateCamera(Vector2.zero);
			targetHide = false;
			activateTimeout = 1f;
		}
	}

	public override void OnDeactivate()
	{
		ShowTarget(show: true);
	}

	private Vector3 GetEyePos()
	{
		if (config.IsVector3("TargetOffset"))
		{
			return Target.transform.position + config.GetVector3("TargetOffset");
		}
		return Target.transform.position;
	}

	private void UpdateFOV()
	{
		UnityCamera.fieldOfView = config.GetFloat("FOV");
	}

	public override void SetCameraTarget(Transform target)
	{
		base.SetCameraTarget(target);
		if ((bool)target)
		{
			cameraTarget = Target.position;
			UnityCamera.transform.position = GetEyePos();
			UnityCamera.transform.LookAt(GetEyePos() + Target.forward);
			RotateCamera(Vector2.zero);
		}
	}

	public override void Init()
	{
		base.Init();
		config = GetComponent<FPSConfig>();
		cameraTarget = Target.position;
		UnityCamera.transform.position = GetEyePos();
		RotateCamera(Vector2.zero);
	}

	public void RotateCamera(Vector2 mousePosition)
	{
		RG_GameCamera.Utils.Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);
		rotY += config.GetFloat("RotationSpeedY") * mousePosition.y * 0.01f;
		rotX += config.GetFloat("RotationSpeedX") * mousePosition.x * 0.01f;
		float num = (0f - rotY) * 57.29578f;
		float @float = config.GetFloat("RotationYMax");
		float float2 = config.GetFloat("RotationYMin");
		if (num > @float)
		{
			rotY = (0f - @float) * ((float)System.Math.PI / 180f);
		}
		if (num < float2)
		{
			rotY = (0f - float2) * ((float)System.Math.PI / 180f);
		}
	}

	private void UpdateDir()
	{
		RG_GameCamera.Utils.Math.ToCartesian(rotX, rotY, out var dir);
		UnityCamera.transform.forward = dir;
		UnityCamera.transform.position = GetEyePos();
		activateTimeout -= Time.deltaTime;
		if (activateTimeout > 0f)
		{
			UnityCamera.transform.LookAt(GetEyePos() + Target.forward);
		}
	}

	private void UpdateTargetVisibility()
	{
		bool @bool = config.GetBool("HideTarget");
		if (@bool != targetHide)
		{
			targetHide = @bool;
			ShowTarget(!targetHide);
		}
	}

	private void ShowTarget(bool show)
	{
		RG_GameCamera.Utils.Debug.SetVisible(Target.gameObject, show, includeInactive: true);
	}

	public override void PostUpdate()
	{
		if (!disableInput && (bool)InputManager)
		{
			if (activateTimeout < 0f)
			{
				UpdateTargetVisibility();
			}
			UpdateFOV();
			if (InputManager.GetInput(InputType.Rotate).Valid)
			{
				RotateCamera((Vector2)InputManager.GetInput(InputType.Rotate).Value);
			}
			UpdateDir();
		}
	}
}
