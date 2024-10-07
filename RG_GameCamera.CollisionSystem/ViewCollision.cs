using RG_GameCamera.Config;
using UnityEngine;

namespace RG_GameCamera.CollisionSystem;

public abstract class ViewCollision
{
	public enum CollisionClass
	{
		Collision,
		Trigger,
		Ignore,
		IgnoreTransparent
	}

	protected readonly RG_GameCamera.Config.Config config;

	protected ViewCollision(RG_GameCamera.Config.Config config)
	{
		this.config = config;
	}

	public abstract float Process(Vector3 cameraTarget, Vector3 cameraDir, float distance);

	public static CollisionClass GetCollisionClass(Collider collider, string ignoreTag, string transparentTag)
	{
		CollisionClass result = CollisionClass.Collision;
		if (collider.isTrigger)
		{
			result = CollisionClass.Trigger;
		}
		else if (collider.gameObject != null)
		{
			if (collider.gameObject.tag == ignoreTag || (bool)collider.gameObject.GetComponent<IgnoreCollision>())
			{
				result = CollisionClass.Ignore;
			}
			else if (collider.gameObject.tag == transparentTag || (bool)collider.gameObject.GetComponent<TransparentCollision>())
			{
				result = CollisionClass.IgnoreTransparent;
			}
		}
		return result;
	}

	protected void UpdateTransparency(Collider collider)
	{
		TransparencyManager.Instance.UpdateObject(collider.gameObject);
	}
}
