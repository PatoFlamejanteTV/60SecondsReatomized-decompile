using System;
using RG_GameCamera.Config;
using RG_GameCamera.Input;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Modes;

[RequireComponent(typeof(OrbitConfig))]
public class OrbitCameraMode : CameraMode
{
	private Vector2 lastPanPosition;

	private bool panValid;

	private Vector3 newTarget;

	private float interpolateTime;

	public override Type Type => Type.Orbit;

	public override void OnActivate()
	{
		base.OnActivate();
		if ((bool)Target)
		{
			cameraTarget = Target.position;
			newTarget = Target.position;
			interpolateTime = 0.1f;
			UnityCamera.transform.LookAt(cameraTarget);
			targetDistance = (UnityCamera.transform.position - cameraTarget).magnitude;
			panValid = false;
			RotateCamera(Vector2.zero * 0.01f);
			lastPanPosition = Vector2.zero;
			return;
		}
		Ray ray = new Ray(UnityCamera.transform.position, UnityCamera.transform.forward);
		Vector3 vector = ray.origin + ray.direction * 100f;
		if (Physics.Raycast(ray, out var hitInfo, float.MaxValue))
		{
			vector = hitInfo.point;
		}
		newTarget = vector;
		cameraTarget = newTarget;
		targetDistance = (UnityCamera.transform.position - cameraTarget).magnitude;
		RotateCamera(Vector2.zero * 0.01f);
		lastPanPosition = Vector2.zero;
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
			newTarget = Target.position;
			interpolateTime = 0.1f;
			UnityCamera.transform.LookAt(cameraTarget);
			targetDistance = (UnityCamera.transform.position - cameraTarget).magnitude;
			panValid = false;
			RotateCamera(Vector2.zero * 0.01f);
			lastPanPosition = Vector2.zero;
		}
	}

	public override void Init()
	{
		base.Init();
		UnityCamera.transform.LookAt(cameraTarget);
		config = GetComponent<OrbitConfig>();
	}

	public void ZoomCamera(float amount)
	{
		if (config.GetBool("DisableZoom"))
		{
			return;
		}
		float num = amount * config.GetFloat("ZoomSpeed");
		if (!(System.Math.Abs(num) > Mathf.Epsilon))
		{
			return;
		}
		if (UnityCamera.orthographic)
		{
			float zoomFactor = GetZoomFactor();
			num *= zoomFactor;
			UnityCamera.orthographicSize -= num;
			if (UnityCamera.orthographicSize < 0.01f)
			{
				UnityCamera.orthographicSize = 0.01f;
			}
		}
		else
		{
			float num2 = GetZoomFactor();
			if (num2 < 0.01f)
			{
				num2 = 0.01f;
			}
			num *= num2;
			Vector3 vector = UnityCamera.transform.forward * num;
			Vector3 vector2 = UnityCamera.transform.position + vector;
			if (!new Plane(UnityCamera.transform.forward, cameraTarget).GetSide(vector2))
			{
				UnityCamera.transform.position = vector2;
			}
		}
		targetDistance = (UnityCamera.transform.position - cameraTarget).magnitude;
	}

	public void PanCamera(Vector2 mousePosition)
	{
		if (!config.GetBool("DisablePan"))
		{
			if (panValid)
			{
				Vector2 vector = mousePosition - lastPanPosition;
				lastPanPosition = mousePosition;
				vector *= 0.01f * config.GetFloat("PanSpeed") * GetZoomFactor();
				Vector3 vector2 = -UnityCamera.transform.up * vector.y + -UnityCamera.transform.right * vector.x;
				UnityCamera.transform.position += vector2;
				cameraTarget += vector2;
			}
			else
			{
				lastPanPosition = mousePosition;
				panValid = true;
			}
		}
	}

	public void PanCameraWithMove(Vector2 move)
	{
		if (!(move.sqrMagnitude <= Mathf.Epsilon) && !config.GetBool("DisablePan"))
		{
			move *= 0.1f * config.GetFloat("PanSpeed") * GetZoomFactor();
			Vector3 vector = UnityCamera.transform.up * move.y + UnityCamera.transform.right * move.x;
			UnityCamera.transform.position += vector;
			cameraTarget += vector;
		}
	}

	public void RotateCamera(Vector2 mousePosition)
	{
		if (config.GetBool("DisableRotation") || panValid)
		{
			return;
		}
		Vector3 right = UnityCamera.transform.right;
		UnityCamera.transform.RotateAround(cameraTarget, right, config.GetFloat("RotationSpeedY") * (0f - mousePosition.y));
		float @float = config.GetFloat("RotationYMax");
		float float2 = config.GetFloat("RotationYMin");
		float floatMax = config.GetFloatMax("RotationYMax");
		float floatMin = config.GetFloatMin("RotationYMin");
		if (@float < floatMax || float2 > floatMin)
		{
			RG_GameCamera.Utils.Math.ToSpherical(UnityCamera.transform.forward, out var rotX, out var rotZ);
			float num = (0f - rotZ) * 57.29578f;
			bool flag = false;
			float rotZ2 = 0f;
			if (@float < floatMax && num > @float)
			{
				rotZ2 = (0f - @float) * ((float)System.Math.PI / 180f);
				flag = true;
			}
			if (float2 > floatMin && num < float2)
			{
				rotZ2 = (0f - float2) * ((float)System.Math.PI / 180f);
				flag = true;
			}
			if (flag)
			{
				RG_GameCamera.Utils.Math.ToCartesian(rotX, rotZ2, out var dir);
				UnityCamera.transform.forward = dir;
				UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
			}
		}
		Vector3 up = Vector3.up;
		UnityCamera.transform.RotateAround(cameraTarget, up, config.GetFloat("RotationSpeedX") * mousePosition.x);
		Vector3 eulerAngles = UnityCamera.transform.eulerAngles;
		UnityCamera.transform.rotation = Quaternion.Euler(eulerAngles);
		UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
	}

	public void ResetCamera(Vector2 position)
	{
		Ray ray = UnityCamera.ScreenPointToRay(position);
		Vector3 vector = ray.origin + ray.direction * 100f;
		if (Physics.Raycast(ray, out var hitInfo, float.MaxValue))
		{
			vector = hitInfo.point;
		}
		newTarget = vector;
		interpolateTime = config.GetFloat("TargetInterpolation");
	}

	private void InterpolateTarget()
	{
		interpolateTime -= Time.deltaTime;
		cameraTarget = Vector3.Lerp(cameraTarget, newTarget, Time.deltaTime * 10f);
		UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
		targetDistance = (UnityCamera.transform.position - cameraTarget).magnitude;
	}

	public override void PostUpdate()
	{
		if (disableInput)
		{
			return;
		}
		if (interpolateTime >= 0f)
		{
			InterpolateTarget();
		}
		else
		{
			if (!InputManager)
			{
				return;
			}
			UpdateFOV();
			if (InputManager.GetInput(InputType.Pan).Valid)
			{
				PanCamera((Vector2)InputManager.GetInput(InputType.Pan).Value);
			}
			else
			{
				if (InputManager.GetInput(InputType.Move).Valid)
				{
					PanCameraWithMove((Vector2)InputManager.GetInput(InputType.Move).Value);
				}
				panValid = false;
			}
			if (InputManager.GetInput(InputType.Zoom).Valid)
			{
				ZoomCamera((float)InputManager.GetInput(InputType.Zoom).Value);
			}
			if (InputManager.GetInput(InputType.Rotate).Valid)
			{
				RotateCamera((Vector2)InputManager.GetInput(InputType.Rotate).Value);
			}
			RG_GameCamera.Input.Input input = InputManager.GetInput(InputType.Reset);
			if (input.Valid && (bool)input.Value)
			{
				ResetCamera(UnityEngine.Input.mousePosition);
			}
		}
	}
}
