using System;
using RG_GameCamera.Config;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Modes;

[RequireComponent(typeof(LookAtConfig))]
public class LookAtCameraMode : CameraMode
{
	public delegate void OnLookAtFinished();

	private Vector3 newTarget;

	private Vector3 newPos;

	private Vector3 oldPos;

	private Vector3 oldTarget;

	private Quaternion oldRot;

	private Quaternion newRot;

	private float targetTimeoutMax;

	private float targetTimeout;

	private OnLookAtFinished finishedCallback;

	public override Type Type => Type.LookAt;

	public override void Init()
	{
		base.Init();
		UnityCamera.transform.LookAt(cameraTarget);
		config = GetComponent<LookAtConfig>();
		targetTimeout = -1f;
		targetTimeoutMax = 1f;
	}

	public override void OnActivate()
	{
		ApplyCurrentCamera();
	}

	public void RegisterFinishCallback(OnLookAtFinished callback)
	{
		finishedCallback = (OnLookAtFinished)Delegate.Combine(finishedCallback, callback);
	}

	public void UnregisterFinishCallback(OnLookAtFinished callback)
	{
		finishedCallback = (OnLookAtFinished)Delegate.Remove(finishedCallback, callback);
	}

	public void ApplyCurrentCamera()
	{
		Ray ray = new Ray(UnityCamera.transform.position, UnityCamera.transform.forward);
		Vector3 vector = ray.origin + ray.direction * 100f;
		if (Physics.Raycast(ray, out var hitInfo, float.MaxValue))
		{
			vector = hitInfo.point;
		}
		cameraTarget = vector;
		targetDistance = (UnityCamera.transform.position - cameraTarget).magnitude;
		UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
	}

	public void LookAt(Vector3 point, float timeout)
	{
		LookAt(UnityCamera.transform.position, point, timeout);
	}

	public void LookAt(Vector3 from, Vector3 point, float timeout)
	{
		oldPos = UnityCamera.transform.position;
		oldTarget = cameraTarget;
		oldRot = UnityCamera.transform.rotation;
		newPos = from;
		newTarget = point;
		if (timeout < 0f)
		{
			timeout = 0f;
		}
		newRot = Quaternion.LookRotation(point - from);
		targetTimeoutMax = timeout;
		targetTimeout = timeout;
	}

	public void LookFrom(Vector3 from, float timeout)
	{
		LookAt(from, cameraTarget, timeout);
	}

	private void UpdateLookAt()
	{
		if (targetTimeout >= 0f)
		{
			targetTimeout -= Time.deltaTime;
			float t = ((!(targetTimeoutMax < Mathf.Epsilon)) ? (1f - targetTimeout / targetTimeoutMax) : 1f);
			Vector3 position = Interpolation.LerpS(oldPos, newPos, t);
			UnityCamera.transform.position = position;
			if (config.GetBool("InterpolateTarget"))
			{
				cameraTarget = Interpolation.LerpS(oldTarget, newTarget, t);
				UnityCamera.transform.LookAt(cameraTarget);
			}
			else
			{
				Quaternion rotation = Quaternion.Slerp(oldRot, newRot, Interpolation.LerpS(0f, 1f, t));
				UnityCamera.transform.rotation = rotation;
			}
			if (targetTimeout < 0f && finishedCallback != null)
			{
				finishedCallback();
			}
		}
	}

	private void UpdateFOV()
	{
		UnityCamera.fieldOfView = config.GetFloat("FOV");
	}

	public override void PostUpdate()
	{
		UpdateFOV();
		UpdateLookAt();
	}
}
