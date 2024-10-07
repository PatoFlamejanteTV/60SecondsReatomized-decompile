using System;
using RG_GameCamera.Config;
using RG_GameCamera.Input;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Modes;

[RequireComponent(typeof(ThirdPersonConfig))]
public class ThirdPersonCameraMode : CameraMode
{
	public bool dbgRing;

	private bool rotationInput;

	private float rotationInputTimeout;

	private float rotX;

	private float rotY;

	private float targetVelocity;

	private float collisionDistance;

	private float collisionZoomVelocity;

	private float currCollisionTargetDist;

	private float collisionTargetDist;

	private float collisionTargetVelocity;

	private Vector3 targetPos;

	private Vector3 lastTargetPos;

	private Vector3 springVelocity;

	private GameObject debugRing;

	private float activateTimeout;

	private float _fov = 60f;

	private PositionFilter targetFilter;

	private float _freeRotateTimeout = 0.75f;

	private float _freeRotateTimer;

	private bool _doFreeRotateTimeout = true;

	private float _angleJumpLimit = 5f;

	public override Type Type => Type.ThirdPerson;

	public override void Init()
	{
		base.Init();
		UnityCamera.transform.LookAt(cameraTarget);
		config = GetComponent<ThirdPersonConfig>();
		lastTargetPos = Target.position;
		targetVelocity = 0f;
		debugRing = RingPrimitive.Create(3f, 3f, 0.1f, 50, Color.red);
		debugRing.GetComponent<MeshRenderer>().castShadows = false;
		RG_GameCamera.Utils.Debug.SetActive(debugRing, dbgRing);
		targetFilter = new PositionFilter(10, 1f);
		targetFilter.Reset(Target.position);
		_fov = UnityEngine.Object.FindObjectOfType<ResolutionHandler>().SelectedAspectRatio.CamFov;
		RG_GameCamera.Utils.DebugDraw.Enabled = true;
	}

	public override void OnActivate()
	{
		base.OnActivate();
		config.SetCameraMode("Default");
		targetFilter.Reset(Target.position);
		lastTargetPos = Target.position;
		targetVelocity = 0f;
		activateTimeout = 1f;
	}

	private void RotateCamera(Vector2 mousePosition)
	{
		rotationInput = mousePosition.sqrMagnitude > Mathf.Epsilon;
		if (rotationInput)
		{
			rotationInputTimeout = 0f;
		}
		else
		{
			rotationInputTimeout += Time.deltaTime;
		}
		bool @bool = config.GetBool("FreeRotate");
		rotY += config.GetFloat("RotationSpeedY") * mousePosition.y * (@bool ? 0.01f : 1f);
		rotX += config.GetFloat("RotationSpeedX") * mousePosition.x * (@bool ? 0.01f : 1f);
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

	private void UpdateFollow()
	{
		Vector3 vector = targetPos - lastTargetPos;
		vector.y = 0f;
		float num = Mathf.Clamp(vector.magnitude, 0f, 5f);
		if (Time.deltaTime > Mathf.Epsilon)
		{
			targetVelocity = num / Time.deltaTime;
		}
		else
		{
			targetVelocity = 0f;
		}
		if (InputManager.GetInput(InputType.Move).Valid)
		{
			Vector2 vector2 = (Vector2)InputManager.GetInput(InputType.Move).Value;
			vector2.Normalize();
			float @float = config.GetFloat("FollowCoef");
			float num2 = Mathf.Sin(Mathf.Atan2(vector2.x, vector2.y));
			float num3 = Mathf.Clamp01(rotationInputTimeout);
			float num4 = num2 * Time.deltaTime * @float * targetVelocity * 0.2f * num3;
			rotX += num4;
		}
	}

	private void UpdateDistance()
	{
		Vector3 a = targetPos + GetOffsetPos();
		cameraTarget = Vector3.Lerp(a, GetTargetHeadPos(), 1f - currCollisionTargetDist);
	}

	private void UpdateFOV()
	{
		UnityCamera.fieldOfView = _fov;
	}

	private void UpdateDir()
	{
		activateTimeout -= Time.deltaTime;
		if (activateTimeout > 0f)
		{
			float @float = config.GetFloat("DefaultYRotation");
			rotY = (0f - @float) * ((float)System.Math.PI / 180f);
			rotX = Mathf.Atan2(Target.forward.x, Target.forward.z);
		}
		RG_GameCamera.Utils.Math.ToCartesian(rotX, rotY, out var dir);
		UnityCamera.transform.forward = dir;
		UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
		lastTargetPos = targetPos;
	}

	private Vector3 GetOffsetPos()
	{
		Vector3 vector = Vector3.zero;
		if (config.IsVector3("TargetOffset"))
		{
			vector = config.GetVector3("TargetOffset");
		}
		Vector3 vector2 = RG_GameCamera.Utils.Math.VectorXZ(UnityCamera.transform.forward);
		Vector3 vector3 = RG_GameCamera.Utils.Math.VectorXZ(UnityCamera.transform.right);
		Vector3 up = Vector3.up;
		return vector2 * vector.z + vector3 * vector.x + up * vector.y;
	}

	private void UpdateYRotation()
	{
		if (!rotationInput && targetVelocity > 0.1f)
		{
			float a = (0f - rotY) * 57.29578f;
			float @float = config.GetFloat("DefaultYRotation");
			float num = Mathf.Clamp01(rotationInputTimeout);
			float t = Mathf.Clamp01(targetVelocity * config.GetFloat("AutoYRotation") * Time.deltaTime) * num;
			a = Mathf.Lerp(a, @float, t);
			rotY = (0f - a) * ((float)System.Math.PI / 180f);
		}
	}

	public override void PostUpdate()
	{
		if (!disableInput && (bool)InputManager)
		{
			UpdateFOV();
			if (InputManager.GetInput(InputType.Rotate).Valid)
			{
				RotateCamera((Vector2)InputManager.GetInput(InputType.Rotate).Value);
			}
			UpdateFollow();
			UpdateDistance();
			UpdateYRotation();
			UpdateDir();
		}
	}

	private void UpdateCollision()
	{
		Vector3 vector = targetPos + GetOffsetPos();
		float @float = config.GetFloat("Distance");
		collision.ProcessCollision(vector, GetTargetHeadPos(), UnityCamera.transform.forward, @float, out collisionTargetDist, out collisionDistance);
		float num = collisionDistance / @float;
		if (collisionTargetDist > num)
		{
			collisionTargetDist = num;
		}
		targetDistance = Interpolation.Lerp(targetDistance, collisionDistance, (targetDistance > collisionDistance) ? collision.GetClipSpeed() : collision.GetReturnSpeed());
		currCollisionTargetDist = Mathf.SmoothDamp(currCollisionTargetDist, collisionTargetDist, ref collisionTargetVelocity, (currCollisionTargetDist > collisionTargetDist) ? collision.GetTargetClipSpeed() : collision.GetReturnTargetSpeed());
	}

	public override void GameUpdate()
	{
		base.GameUpdate();
		float @float = config.GetFloat("Spring");
		Vector2 vector = config.GetVector2("DeadZone");
		if (@float <= 0f && vector.sqrMagnitude <= Mathf.Epsilon)
		{
			targetPos = targetFilter.GetValue();
		}
		UpdateTargetDummy();
	}

	public override void FixedStepUpdate()
	{
		targetFilter.AddSample(Target.position);
		UpdateCollision();
		Vector2 vector = config.GetVector2("DeadZone");
		if (vector.sqrMagnitude > Mathf.Epsilon)
		{
			RingPrimitive.Generate(debugRing, vector.x, vector.y, 0.1f, 50);
			debugRing.transform.position = targetPos + Vector3.up * 2f;
			Vector3 forward = RG_GameCamera.Utils.Math.VectorXZ(UnityCamera.transform.forward);
			if (forward.sqrMagnitude < Mathf.Epsilon)
			{
				forward = Vector3.forward;
			}
			debugRing.transform.forward = forward;
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
		}
	}
}
