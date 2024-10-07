using System.Collections.Generic;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.CollisionSystem;

public class TransparencyManager : MonoBehaviour
{
	private class TransObject
	{
		public float originalAlpha;

		public bool fadeIn;

		public float fadeoutTimer;
	}

	private static TransparencyManager instance;

	public float TransparencyMax = 0.5f;

	public float TransparencyFadeOut = 0.2f;

	public float TransparencyFadeIn = 0.1f;

	private float fadeVelocity;

	private const float fadeoutTimerMax = 0.1f;

	private Dictionary<GameObject, TransObject> objects;

	public static TransparencyManager Instance
	{
		get
		{
			if (!instance)
			{
				instance = CameraInstance.CreateInstance<TransparencyManager>("TransparencyManager");
			}
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		objects = new Dictionary<GameObject, TransObject>();
	}

	private void Update()
	{
		foreach (KeyValuePair<GameObject, TransObject> @object in objects)
		{
			@object.Value.fadeoutTimer += Time.deltaTime;
			if (@object.Value.fadeoutTimer > 0.1f)
			{
				@object.Value.fadeIn = false;
			}
			float alpha = GetAlpha(@object.Key);
			bool flag = false;
			if (@object.Value.fadeIn)
			{
				alpha = Mathf.SmoothDamp(alpha, TransparencyMax, ref fadeVelocity, TransparencyFadeIn);
			}
			else
			{
				alpha = Mathf.SmoothDamp(alpha, @object.Value.originalAlpha, ref fadeVelocity, TransparencyFadeOut);
				if (Mathf.Abs(alpha - @object.Value.originalAlpha) < Mathf.Epsilon)
				{
					flag = true;
					alpha = @object.Value.originalAlpha;
				}
			}
			SetAlpha(@object.Key, alpha);
			if (flag)
			{
				objects.Remove(@object.Key);
				break;
			}
		}
	}

	public void UpdateObject(GameObject obj)
	{
		TransObject value = null;
		if (objects.TryGetValue(obj, out value))
		{
			value.fadeIn = true;
			value.fadeoutTimer = 0f;
			return;
		}
		objects.Add(obj, new TransObject
		{
			originalAlpha = GetAlpha(obj),
			fadeIn = true,
			fadeoutTimer = 0f
		});
	}

	private static void SetAlpha(GameObject obj, float alpha)
	{
		MeshRenderer component = obj.GetComponent<MeshRenderer>();
		if ((bool)component)
		{
			Material sharedMaterial = component.sharedMaterial;
			if ((bool)sharedMaterial)
			{
				Color color = sharedMaterial.color;
				color.a = alpha;
				sharedMaterial.color = color;
			}
		}
	}

	private static float GetAlpha(GameObject obj)
	{
		MeshRenderer component = obj.GetComponent<MeshRenderer>();
		if ((bool)component)
		{
			Material sharedMaterial = component.sharedMaterial;
			if ((bool)sharedMaterial)
			{
				return sharedMaterial.color.a;
			}
		}
		return 1f;
	}

	private void OnApplicationQuit()
	{
		foreach (KeyValuePair<GameObject, TransObject> @object in objects)
		{
			SetAlpha(@object.Key, @object.Value.originalAlpha);
		}
	}
}
