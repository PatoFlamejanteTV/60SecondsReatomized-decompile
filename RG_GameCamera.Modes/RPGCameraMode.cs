using System;
using RG_GameCamera.Config;
using RG_GameCamera.Input;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Modes;

[RequireComponent(typeof(RPGConfig))]
public class RPGCameraMode : CameraMode
{
	public bool dbgRing;

	private float rotX;

	private float rotY;

	private float targetZoom;

	private Vector3 targetPos;

	private PositionFilter targetFilter;

	private Vector3 springVelocity;

	private GameObject debugRing;

	private float transitDistance;

	private float activateTimeout;

	public override Type Type => Type.RPG;

	public override void Init()
	{
		base.Init();
		UnityCamera.transform.LookAt(cameraTarget);
		config = GetComponent<RPGConfig>();
		RG_GameCamera.Utils.DebugDraw.Enabled = true;
		targetFilter = new PositionFilter(10, 1f);
		targetFilter.Reset(Target.position);
		debugRing = RingPrimitive.Create(3f, 3f, 0.1f, 50, Color.red);
		debugRing.GetComponent<MeshRenderer>().castShadows = false;
		RG_GameCamera.Utils.Debug.SetActive(debugRing, dbgRing);
		config.TransitCallback = OnTransitMode;
		config.TransitionStartCallback = OnTransitStartMode;
	}

	public override void OnActivate()
	{
		base.OnActivate();
		config.SetCameraMode("Default");
		targetDistance = config.GetFloat("Distance");
		cameraTarget = Target.position;
		targetFilter.Reset(Target.position);
		targetPos = Target.position;
		UpdateYAngle();
		UpdateXAngle(force: true);
		UpdateDir();
		activateTimeout = 2f;
	}

	private void OnTransitMode(string newMode, float t)
	{
		float @float = config.GetFloat("Distance");
		targetDistance = Mathf.Lerp(transitDistance, @float, t);
	}

	private void OnTransitStartMode(string oldMode, string newMode)
	{
		transitDistance = targetDistance;
	}

	public override void SetCameraTarget(Transform target)
	{
		base.SetCameraTarget(target);
		if ((bool)target)
		{
			cameraTarget = target.position;
		}
	}

	private void RotateCamera(Vector2 mousePosition)
	{
		if (config.GetBool("EnableRotation") && mousePosition.sqrMagnitude > Mathf.Epsilon)
		{
			rotX += config.GetFloat("RotationSpeed") * mousePosition.x * 0.01f;
		}
	}

	private void UpdateFOV()
	{
		UnityCamera.fieldOfView = config.GetFloat("FOV");
	}

	private void UpdateYAngle()
	{
		RG_GameCamera.Utils.Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);
		float num = (targetDistance - config.GetFloat("DistanceMin")) / (config.GetFloat("DistanceMax") - config.GetFloat("DistanceMin"));
		float num2 = config.GetFloat("AngleZoomMin") * (1f - num) + config.GetFloat("AngleY") * num;
		rotY = Mathf.Lerp(rotY, num2 * -1f * ((float)System.Math.PI / 180f), Time.deltaTime * 50f);
	}

	private void UpdateXAngle(bool force)
	{
		if (!config.GetBool("EnableRotation") || force || activateTimeout > 0f)
		{
			rotX = config.GetFloat("DefaultAngleX") * (-(float)System.Math.PI / 180f);
		}
	}

	private void UpdateDir()
	{
		RG_GameCamera.Utils.Math.ToCartesian(rotX, rotY, out var dir);
		UnityCamera.transform.forward = dir;
		UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
	}

	private void UpdateConfig()
	{
	}

	private void UpdateCollision()
	{
		if ((bool)collision)
		{
			collision.ProcessCollision(cameraTarget, cameraTarget, UnityCamera.transform.forward, targetDistance, out var _, out var _);
		}
	}

	private void UpdateZoom()
	{
		RG_GameCamera.Utils.Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);
		if (Mathf.Abs(targetZoom) > Mathf.Epsilon)
		{
			float num = targetZoom * 20f * Time.deltaTime;
			if (Mathf.Abs(num) > Mathf.Abs(targetZoom))
			{
				num = targetZoom;
			}
			Zoom(num);
			targetZoom -= num;
		}
	}

	public void Zoom(float amount)
	{
		if (!config.GetBool("EnableZoom"))
		{
			return;
		}
		float num = amount * config.GetFloat("ZoomSpeed");
		if (!(Mathf.Abs(num) > Mathf.Epsilon))
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
		targetDistance = Mathf.Clamp(targetDistance, config.GetFloat("DistanceMin"), config.GetFloat("DistanceMax"));
	}

	private Vector3 GetOffsetPos()
	{
		Vector3 vector = config.GetVector3("TargetOffset");
		Vector3 vector2 = RG_GameCamera.Utils.Math.VectorXZ(UnityCamera.transform.forward);
		Vector3 vector3 = RG_GameCamera.Utils.Math.VectorXZ(UnityCamera.transform.right);
		Vector3 up = Vector3.up;
		return vector2 * vector.z + vector3 * vector.x + up * vector.y;
	}

	private void UpdateDistance()
	{
		cameraTarget = targetPos + GetOffsetPos();
	}

	public override void PostUpdate()
	{
		if (disableInput)
		{
			return;
		}
		if ((bool)InputManager)
		{
			UpdateConfig();
			UpdateFOV();
			if (InputManager.GetInput(InputType.Zoom).Valid)
			{
				targetZoom = (float)InputManager.GetInput(InputType.Zoom).Value;
			}
			UpdateZoom();
			UpdateYAngle();
			UpdateXAngle(force: false);
			if (InputManager.GetInput(InputType.Rotate).Valid)
			{
				RotateCamera((Vector2)InputManager.GetInput(InputType.Rotate).Value);
			}
			UpdateDistance();
			UpdateDir();
		}
		activateTimeout -= Time.deltaTime;
	}

	public override void FixedStepUpdate()
	{
		targetFilter.AddSample(Target.position);
		Vector2 vector = config.GetVector2("DeadZone");
		if (vector.sqrMagnitude > Mathf.Epsilon)
		{
			RingPrimitive.Generate(debugRing, vector.x, vector.y, 0.1f, 50);
			debugRing.transform.position = targetPos + Vector3.up * 2f;
			debugRing.transform.forward = RG_GameCamera.Utils.Math.VectorXZ(UnityCamera.transform.forward);
			RG_GameCamera.Utils.Debug.SetActive(debugRing, dbgRing);
			Vector3 vector2 = targetFilter.GetValue() - targetPos;
			float magnitude = vector2.magnitude;
			vector2 /= magnitude;
			if (magnitude > vector.x || magnitude > vector.y)
			{
				Vector3 vector3 = UnityCamera.transform.InverseTransformDirection(vector2);
				float f = Mathf.Atan2(vector3.x, vector3.z);
				Vector3 vector4 = new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f));
				float magnitude2 = new Vector3(vector4.x * vector.x, 0f, vector4.z * vector.y).magnitude;
				if (magnitude > magnitude2)
				{
					Vector3 target = targetPos + vector2 * (magnitude - magnitude2);
					targetPos = Vector3.SmoothDamp(targetPos, target, ref springVelocity, config.GetFloat("Spring"));
				}
			}
		}
		else
		{
			targetPos = Vector3.SmoothDamp(targetPos, targetFilter.GetValue(), ref springVelocity, config.GetFloat("Spring"));
			targetPos.y = targetFilter.GetValue().y;
		}
		UpdateCollision();
		UpdateTargetDummy();
	}
}
