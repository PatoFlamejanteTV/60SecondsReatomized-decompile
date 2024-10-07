using System;
using System.Collections;
using RG_GameCamera.Config;
using UnityEngine;

namespace RG_GameCamera.CollisionSystem;

public class SimpleCollision : ViewCollision
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

	public SimpleCollision(RG_GameCamera.Config.Config config)
		: base(config)
	{
		rayHitComparer = new RayHitComparer();
	}

	public override float Process(Vector3 cameraTarget, Vector3 dir, float distance)
	{
		float value = distance;
		float @float = config.GetFloat("RaycastTolerance");
		float float2 = config.GetFloat("MinDistance");
		float num = float.PositiveInfinity;
		ray.origin = cameraTarget;
		ray.direction = -dir;
		hits = Physics.RaycastAll(ray, distance + @float);
		Array.Sort(hits, rayHitComparer);
		string @string = config.GetString("IgnoreCollisionTag");
		string string2 = config.GetString("TransparentCollisionTag");
		RaycastHit[] array = hits;
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			CollisionClass collisionClass = ViewCollision.GetCollisionClass(raycastHit.collider, @string, string2);
			if (raycastHit.distance < num && collisionClass == CollisionClass.Collision)
			{
				num = raycastHit.distance;
				value = raycastHit.distance - @float;
			}
			if (collisionClass == CollisionClass.IgnoreTransparent)
			{
				UpdateTransparency(raycastHit.collider);
			}
		}
		return Mathf.Clamp(value, float2, distance);
	}
}
