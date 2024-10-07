using System;
using System.Collections;
using RG_GameCamera.Config;
using UnityEngine;

namespace RG_GameCamera.CollisionSystem;

public class TargetCollision
{
	public class RayHitComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
		}
	}

	private Ray ray;

	private RaycastHit[] hits;

	private readonly RayHitComparer rayHitComparer;

	private readonly RG_GameCamera.Config.Config config;

	public TargetCollision(RG_GameCamera.Config.Config config)
	{
		rayHitComparer = new RayHitComparer();
		this.config = config;
	}

	public float CalculateTarget(Vector3 targetHead, Vector3 cameraTarget)
	{
		string @string = config.GetString("IgnoreCollisionTag");
		string string2 = config.GetString("TransparentCollisionTag");
		float @float = config.GetFloat("TargetSphereRadius");
		float num = 1f;
		Vector3 normalized = (cameraTarget - targetHead).normalized;
		Ray ray = new Ray(targetHead, normalized);
		hits = Physics.RaycastAll(ray, normalized.magnitude + @float);
		Array.Sort(hits, rayHitComparer);
		float num2 = float.PositiveInfinity;
		bool flag = false;
		RaycastHit[] array = hits;
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			ViewCollision.CollisionClass collisionClass = ViewCollision.GetCollisionClass(raycastHit.collider, @string, string2);
			if (raycastHit.distance < num2 && collisionClass == ViewCollision.CollisionClass.Collision)
			{
				num2 = raycastHit.distance;
				num = raycastHit.distance - @float;
				flag = true;
				Debug.DrawLine(targetHead, raycastHit.point, Color.yellow);
			}
		}
		if (flag)
		{
			return Mathf.Clamp01(num / (targetHead - cameraTarget).magnitude);
		}
		return 1f;
	}
}
