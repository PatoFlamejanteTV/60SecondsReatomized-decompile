using System.Collections.Generic;
using RG_GameCamera.Config;
using RG_GameCamera.Effects;
using RG_GameCamera.Modes;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Events;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(BoxCollider))]
public class CameraEvent : MonoBehaviour
{
	private abstract class ITween
	{
		public string mode;

		public string key;

		public float time;

		public float timeout;

		public abstract void Interpolate(float t);
	}

	private class FloatTween : ITween
	{
		public float t0;

		public float t1;

		public override void Interpolate(float t)
		{
			float inputValue = Interpolation.LerpS(t0, t1, t);
			CameraManager.Instance.GetCameraMode().Configuration.SetFloat(mode, key, inputValue);
		}
	}

	private class Vector2Tween : ITween
	{
		public Vector2 t0;

		public Vector2 t1;

		public override void Interpolate(float t)
		{
			Vector2 inputValue = Interpolation.LerpS(t0, t1, t);
			CameraManager.Instance.GetCameraMode().Configuration.SetVector2(mode, key, inputValue);
		}
	}

	private class Vector3Tween : ITween
	{
		public Vector3 t0;

		public Vector3 t1;

		public override void Interpolate(float t)
		{
			Vector3 inputValue = Interpolation.LerpS(t0, t1, t);
			CameraManager.Instance.GetCameraMode().Configuration.SetVector3(mode, key, inputValue);
		}
	}

	private List<ITween> tweens;

	public EventType Type;

	public RG_GameCamera.Modes.Type CameraMode;

	public string StringParam0;

	public string StringParam1;

	public RG_GameCamera.Config.Config.ConfigValue ConfigParamValueType;

	public bool ConfigParamBool;

	public string ConfigParamString;

	public float ConfigParamFloat;

	public Vector2 ConfigParamVector2;

	public Vector3 ConfigParamVector3;

	public RG_GameCamera.Effects.Type CameraEffect;

	public GameObject CustomObject;

	public bool RestoreOnExit;

	public bool SmoothFloatParams;

	public float SmoothTimeout;

	public bool LookAtFrom;

	public bool LookAtTo;

	public Transform LookAtFromObject;

	public Transform LookAtToObject;

	public bool RestoreOnTimeout;

	public float RestoreTimeout;

	private Collider cameraTrigger;

	private object oldParam0;

	private object oldParam1;

	private object oldParam2;

	private float restorationTimeout;

	private bool paramChanged;

	private void Awake()
	{
		tweens = new List<ITween>();
	}

	private void SmoothParam(string mode, string key, float t0, float t1, float time)
	{
		FloatTween item = new FloatTween
		{
			key = key,
			mode = mode,
			t0 = t0,
			t1 = t1,
			time = time,
			timeout = time
		};
		tweens.Add(item);
	}

	private void SmoothParam(string mode, string key, Vector2 t0, Vector2 t1, float time)
	{
		Vector2Tween item = new Vector2Tween
		{
			key = key,
			mode = mode,
			t0 = t0,
			t1 = t1,
			time = time,
			timeout = time
		};
		tweens.Add(item);
	}

	private void SmoothParam(string mode, string key, Vector3 t0, Vector3 t1, float time)
	{
		Vector3Tween item = new Vector3Tween
		{
			key = key,
			mode = mode,
			t0 = t0,
			t1 = t1,
			time = time,
			timeout = time
		};
		tweens.Add(item);
	}

	private void Update()
	{
		foreach (ITween tween in tweens)
		{
			tween.timeout -= Time.deltaTime;
			float t = 1f - Mathf.Clamp01(tween.timeout / tween.time);
			tween.Interpolate(t);
			if (tween.timeout < 0f)
			{
				tweens.Remove(tween);
				break;
			}
		}
		if (cameraTrigger != null && RestoreOnTimeout)
		{
			restorationTimeout -= Time.deltaTime;
			if (restorationTimeout < 0f)
			{
				Exit(onTimeout: true, cameraTrigger);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other || !other.gameObject)
		{
			return;
		}
		if ((bool)other.gameObject.GetComponent<CameraTrigger>())
		{
			if ((bool)cameraTrigger)
			{
				return;
			}
			cameraTrigger = other;
			switch (Type)
			{
			case EventType.ConfigMode:
			{
				RG_GameCamera.Config.Config configuration2 = CameraManager.Instance.GetCameraMode().Configuration;
				if ((bool)configuration2 && !string.IsNullOrEmpty(StringParam0))
				{
					oldParam0 = configuration2.GetCurrentMode();
					if ((string)oldParam0 != StringParam0)
					{
						paramChanged = configuration2.SetCameraMode(StringParam0);
					}
				}
				break;
			}
			case EventType.ConfigParam:
			{
				RG_GameCamera.Config.Config configuration = CameraManager.Instance.GetCameraMode().Configuration;
				string mode = (string)(oldParam2 = configuration.GetCurrentMode());
				if (!configuration || string.IsNullOrEmpty(StringParam0))
				{
					break;
				}
				oldParam0 = StringParam0;
				switch (ConfigParamValueType)
				{
				case RG_GameCamera.Config.Config.ConfigValue.Bool:
					oldParam1 = configuration.GetBool(mode, StringParam0);
					configuration.SetBool(mode, StringParam0, ConfigParamBool);
					break;
				case RG_GameCamera.Config.Config.ConfigValue.Range:
					oldParam1 = configuration.GetFloat(mode, StringParam0);
					if (SmoothFloatParams)
					{
						SmoothParam(mode, StringParam0, (float)oldParam1, ConfigParamFloat, SmoothTimeout);
					}
					else
					{
						configuration.SetFloat(mode, StringParam0, ConfigParamFloat);
					}
					break;
				case RG_GameCamera.Config.Config.ConfigValue.Selection:
					oldParam1 = configuration.GetSelection(mode, StringParam0);
					configuration.SetSelection(mode, StringParam0, StringParam1);
					break;
				case RG_GameCamera.Config.Config.ConfigValue.String:
					oldParam1 = configuration.GetString(mode, StringParam0);
					configuration.SetString(mode, StringParam0, StringParam1);
					break;
				case RG_GameCamera.Config.Config.ConfigValue.Vector2:
					oldParam1 = configuration.GetVector2(mode, StringParam0);
					if (SmoothFloatParams)
					{
						SmoothParam(mode, StringParam0, (Vector2)oldParam1, ConfigParamVector2, SmoothTimeout);
					}
					else
					{
						configuration.SetVector2(mode, StringParam0, ConfigParamVector2);
					}
					break;
				case RG_GameCamera.Config.Config.ConfigValue.Vector3:
					oldParam1 = configuration.GetVector3(mode, StringParam0);
					if (SmoothFloatParams)
					{
						SmoothParam(mode, StringParam0, (Vector3)oldParam1, ConfigParamVector3, SmoothTimeout);
					}
					else
					{
						configuration.SetVector2(mode, StringParam0, ConfigParamVector2);
					}
					break;
				}
				break;
			}
			case EventType.Effect:
				EffectManager.Instance.Create(CameraEffect).Play();
				break;
			case EventType.CustomMessage:
				if ((bool)CustomObject && !string.IsNullOrEmpty(StringParam0))
				{
					CustomObject.SendMessage(StringParam0);
				}
				break;
			case EventType.LookAt:
			{
				if ((LookAtFrom && !LookAtFromObject) || (LookAtTo && !LookAtToObject) || (!LookAtTo && !LookAtFrom))
				{
					break;
				}
				oldParam0 = CameraManager.Instance.GetCameraMode().Type;
				LookAtCameraMode lookAtCameraMode = CameraManager.Instance.SetMode(RG_GameCamera.Modes.Type.LookAt) as LookAtCameraMode;
				if (LookAtFrom)
				{
					if (LookAtTo)
					{
						lookAtCameraMode.LookAt(LookAtFromObject.position, LookAtToObject.position, SmoothTimeout);
					}
					else
					{
						lookAtCameraMode.LookFrom(LookAtFromObject.position, SmoothTimeout);
					}
				}
				else
				{
					lookAtCameraMode.LookAt(LookAtToObject.position, SmoothTimeout);
				}
				break;
			}
			}
		}
		if (RestoreOnTimeout)
		{
			restorationTimeout = RestoreTimeout;
		}
	}

	private void Exit(bool onTimeout, Collider other)
	{
		bool flag = false;
		flag = ((!onTimeout) ? (RestoreOnExit && cameraTrigger == other) : RestoreOnTimeout);
		if (!RestoreOnExit && !RestoreOnTimeout)
		{
			cameraTrigger = null;
		}
		if (!flag)
		{
			return;
		}
		cameraTrigger = null;
		switch (Type)
		{
		case EventType.ConfigMode:
			if (paramChanged)
			{
				RG_GameCamera.Config.Config configuration = CameraManager.Instance.GetCameraMode().Configuration;
				if ((bool)configuration && !string.IsNullOrEmpty((string)oldParam0) && (string)oldParam0 != configuration.GetCurrentMode())
				{
					configuration.SetCameraMode((string)oldParam0);
				}
			}
			break;
		case EventType.ConfigParam:
		{
			RG_GameCamera.Config.Config configuration2 = CameraManager.Instance.GetCameraMode().Configuration;
			if (!configuration2 || string.IsNullOrEmpty((string)oldParam0) || oldParam1 == null || string.IsNullOrEmpty((string)oldParam2))
			{
				break;
			}
			switch (ConfigParamValueType)
			{
			case RG_GameCamera.Config.Config.ConfigValue.Bool:
				configuration2.SetBool((string)oldParam2, (string)oldParam0, (bool)oldParam1);
				break;
			case RG_GameCamera.Config.Config.ConfigValue.Range:
			{
				float @float = configuration2.GetFloat((string)oldParam2, (string)oldParam0);
				if (SmoothFloatParams)
				{
					SmoothParam((string)oldParam2, (string)oldParam0, @float, (float)oldParam1, SmoothTimeout);
				}
				else
				{
					configuration2.SetFloat((string)oldParam2, (string)oldParam0, (float)oldParam1);
				}
				break;
			}
			case RG_GameCamera.Config.Config.ConfigValue.Selection:
				configuration2.SetSelection((string)oldParam2, (string)oldParam0, (string)oldParam1);
				break;
			case RG_GameCamera.Config.Config.ConfigValue.String:
				configuration2.SetString((string)oldParam2, (string)oldParam0, (string)oldParam1);
				break;
			case RG_GameCamera.Config.Config.ConfigValue.Vector2:
			{
				Vector2 vector2 = configuration2.GetVector2((string)oldParam2, (string)oldParam0);
				if (SmoothFloatParams)
				{
					SmoothParam((string)oldParam2, (string)oldParam0, vector2, (Vector2)oldParam1, SmoothTimeout);
				}
				else
				{
					configuration2.SetVector2((string)oldParam2, (string)oldParam0, (Vector2)oldParam1);
				}
				break;
			}
			case RG_GameCamera.Config.Config.ConfigValue.Vector3:
			{
				Vector3 vector = configuration2.GetVector3((string)oldParam2, (string)oldParam0);
				if (SmoothFloatParams)
				{
					SmoothParam((string)oldParam2, (string)oldParam0, vector, (Vector3)oldParam1, SmoothTimeout);
				}
				else
				{
					configuration2.SetVector3((string)oldParam2, (string)oldParam0, (Vector3)oldParam1);
				}
				break;
			}
			}
			break;
		}
		case EventType.CustomMessage:
			if ((bool)CustomObject && !string.IsNullOrEmpty(StringParam1))
			{
				CustomObject.SendMessage(StringParam1);
			}
			break;
		case EventType.LookAt:
			if (oldParam0 is RG_GameCamera.Modes.Type)
			{
				CameraManager.Instance.SetMode((RG_GameCamera.Modes.Type)oldParam0);
			}
			break;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Exit(onTimeout: false, other);
	}
}
