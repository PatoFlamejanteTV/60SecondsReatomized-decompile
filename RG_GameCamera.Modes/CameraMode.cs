using RG_GameCamera.CollisionSystem;
using RG_GameCamera.Config;
using RG_GameCamera.Input;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Modes;

public abstract class CameraMode : MonoBehaviour
{
	public Transform Target;

	public bool ShowTargetDummy;

	public bool EnableLiveGUI;

	protected CameraCollision collision;

	protected InputManager InputManager;

	protected Camera UnityCamera;

	protected RG_GameCamera.Config.Config config;

	protected Vector3 cameraTarget;

	protected float targetDistance;

	protected bool disableInput;

	private GameObject targetDummy;

	public abstract Type Type { get; }

	public RG_GameCamera.Config.Config Configuration => config;

	protected virtual void Awake()
	{
		CameraManager.Instance.RegisterMode(this);
	}

	public virtual void Init()
	{
		CameraManager instance = CameraManager.Instance;
		UnityCamera = instance.UnityCamera;
		InputManager = InputManager.Instance;
		if (!Target)
		{
			Target = instance.CameraTarget;
		}
		if ((bool)Target)
		{
			cameraTarget = Target.position;
			targetDistance = (UnityCamera.transform.position - Target.position).magnitude;
		}
		CreateTargetDummy();
		collision = CameraCollision.Instance;
	}

	public virtual void OnActivate()
	{
	}

	public virtual void OnDeactivate()
	{
	}

	public virtual void SetCameraTarget(Transform target)
	{
		Target = target;
	}

	public virtual void SetCameraConfigMode(string modeName)
	{
		config.SetCameraMode(modeName);
	}

	public void EnableOrthoCamera(bool status)
	{
		if (status != UnityCamera.orthographic)
		{
			if (status)
			{
				UnityCamera.orthographic = true;
				UnityCamera.orthographicSize = (UnityCamera.transform.position - cameraTarget).magnitude / 2f;
			}
			else
			{
				UnityCamera.orthographic = false;
				UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * UnityCamera.orthographicSize * 2f;
			}
		}
	}

	public bool IsOrthoCamera()
	{
		return UnityCamera.orthographic;
	}

	public void CreateTargetDummy()
	{
		targetDummy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		targetDummy.name = "TargetDummy";
		targetDummy.transform.parent = base.gameObject.transform;
		SphereCollider component = targetDummy.GetComponent<SphereCollider>();
		if ((bool)component)
		{
			Object.Destroy(component);
		}
		Material material = new Material(Shader.Find("Diffuse"));
		material.color = Color.magenta;
		targetDummy.GetComponent<MeshRenderer>().sharedMaterial = material;
		targetDummy.transform.position = cameraTarget;
		targetDummy.SetActive(ShowTargetDummy);
	}

	protected Vector3 GetTargetHeadPos()
	{
		float num = collision.GetHeadOffset();
		RG_GameCamera.Input.Input input = InputManager.GetInput(InputType.Crouch);
		if (input.Valid && (bool)input.Value)
		{
			num = 1.2f;
		}
		if ((bool)Target)
		{
			return Target.position + Vector3.up * num;
		}
		return cameraTarget + Vector3.up * num;
	}

	protected void UpdateTargetDummy()
	{
		RG_GameCamera.Utils.Debug.SetActive(targetDummy, ShowTargetDummy);
		if ((bool)targetDummy)
		{
			float num = (UnityCamera.transform.position - targetDummy.transform.position).magnitude;
			if (num > 70f)
			{
				num = 70f;
			}
			float num2 = num / 70f;
			targetDummy.transform.localScale = new Vector3(num2, num2, num2);
			targetDummy.transform.position = cameraTarget;
		}
	}

	public virtual void GameUpdate()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.O))
		{
			EnableOrthoCamera(!UnityCamera.orthographic);
		}
		UpdateTargetDummy();
		config.EnableLiveGUI(EnableLiveGUI);
		if (config.IsBool("Orthographic"))
		{
			EnableOrthoCamera(config.GetBool("Orthographic"));
		}
	}

	public virtual void FixedStepUpdate()
	{
	}

	public virtual void PostUpdate()
	{
	}

	protected float GetZoomFactor()
	{
		float num = 1f;
		num = ((!UnityCamera.orthographic) ? (UnityCamera.transform.position - cameraTarget).magnitude : UnityCamera.orthographicSize);
		if (num > 1f)
		{
			return num / (1f + Mathf.Log(num));
		}
		return num;
	}

	protected void DebugDraw()
	{
		UnityEngine.Debug.DrawLine(UnityCamera.transform.position, cameraTarget, Color.red, 1f);
		UnityEngine.Debug.DrawRay(cameraTarget, UnityCamera.transform.up, Color.green, 1f);
		UnityEngine.Debug.DrawRay(cameraTarget, UnityCamera.transform.right, Color.yellow, 1f);
	}

	private void OnGUI()
	{
		string[] results = Profiler.GetResults();
		int num = 10;
		int num2 = Screen.width - 300;
		string[] array = results;
		foreach (string text in array)
		{
			GUI.Label(new Rect(num2, num, 500f, 30f), text);
			num += 20;
		}
	}
}
