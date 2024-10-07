using System;
using System.Collections;
using RG_GameCamera.Config;
using UnityEngine;

namespace RG_GameCamera.CollisionSystem;

public class SphericalCollision : ViewCollision
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

	public SphericalCollision(RG_GameCamera.Config.Config config)
		: base(config)
	{
		rayHitComparer = new RayHitComparer();
	}

	public override float Process(Vector3 cameraTarget, Vector3 dir, float distance)
	{
		float value = distance;
		float @float = config.GetFloat("MinDistance");
		float float2 = config.GetFloat("SphereCastTolerance");
		float float3 = config.GetFloat("SphereCastRadius");
		ray.origin = cameraTarget + dir * float3;
		ray.direction = -dir;
		Collider[] array = Physics.OverlapSphere(ray.origin, float3);
		bool flag = false;
		string @string = config.GetString("IgnoreCollisionTag");
		string string2 = config.GetString("TransparentCollisionTag");
		for (int i = 0; i < array.Length; i++)
		{
			if (ViewCollision.GetCollisionClass(array[i], @string, string2) == CollisionClass.Collision)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			ray.origin += dir * float3;
			hits = Physics.RaycastAll(ray, distance - float3 + float2);
		}
		else
		{
			hits = Physics.SphereCastAll(ray, float3, distance + float3);
		}
		Array.Sort(hits, rayHitComparer);
		float num = float.PositiveInfinity;
		RaycastHit[] array2 = hits;
		for (int j = 0; j < array2.Length; j++)
		{
			RaycastHit raycastHit = array2[j];
			CollisionClass collisionClass = ViewCollision.GetCollisionClass(raycastHit.collider, @string, string2);
			if (raycastHit.distance < num && collisionClass == CollisionClass.Collision)
			{
				num = raycastHit.distance;
				value = raycastHit.distance - float2;
			}
			if (collisionClass == CollisionClass.IgnoreTransparent)
			{
				UpdateTransparency(raycastHit.collider);
			}
		}
		return Mathf.Clamp(value, @float, distance);
	}
}
