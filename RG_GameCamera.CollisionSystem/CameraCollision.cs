using RG_GameCamera.Config;
using UnityEngine;

namespace RG_GameCamera.CollisionSystem;

[RequireComponent(typeof(CollisionConfig))]
public class CameraCollision : MonoBehaviour
{
	private Camera unityCamera;

	private RG_GameCamera.Config.Config config;

	private TargetCollision targetCollision;

	private SimpleCollision simpleCollision;

	private VolumetricCollision volumetricCollision;

	private SphericalCollision sphericalCollision;

	public static CameraCollision Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		unityCamera = CameraManager.Instance.UnityCamera;
		config = GetComponent<CollisionConfig>();
		unityCamera.nearClipPlane = 0.01f;
		targetCollision = new TargetCollision(config);
		simpleCollision = new SimpleCollision(config);
		sphericalCollision = new SphericalCollision(config);
		volumetricCollision = new VolumetricCollision(config);
	}

	private ViewCollision GetCollisionAlgorithm(string algorithm)
	{
		return algorithm switch
		{
			"Simple" => simpleCollision, 
			"Spherical" => sphericalCollision, 
			"Volumetric" => volumetricCollision, 
			_ => null, 
		};
	}

	public void ProcessCollision(Vector3 cameraTarget, Vector3 targetHead, Vector3 dir, float distance, out float collisionTarget, out float collisionDistance)
	{
		collisionTarget = targetCollision.CalculateTarget(targetHead, cameraTarget);
		ViewCollision collisionAlgorithm = GetCollisionAlgorithm(config.GetSelection("CollisionAlgorithm"));
		Vector3 cameraTarget2 = cameraTarget * collisionTarget + targetHead * (1f - collisionTarget);
		collisionDistance = collisionAlgorithm.Process(cameraTarget2, dir, distance);
	}

	public float GetRaycastTolerance()
	{
		return config.GetFloat("RaycastTolerance");
	}

	public float GetClipSpeed()
	{
		return config.GetFloat("ClipSpeed");
	}

	public float GetTargetClipSpeed()
	{
		return config.GetFloat("TargetClipSpeed");
	}

	public float GetReturnSpeed()
	{
		return config.GetFloat("ReturnSpeed");
	}

	public float GetReturnTargetSpeed()
	{
		return config.GetFloat("ReturnTargetSpeed");
	}

	public float GetHeadOffset()
	{
		return config.GetFloat("HeadOffset");
	}

	public ViewCollision.CollisionClass GetCollisionClass(Collider coll)
	{
		string @string = config.GetString("IgnoreCollisionTag");
		string string2 = config.GetString("TransparentCollisionTag");
		return ViewCollision.GetCollisionClass(coll, @string, string2);
	}
}
