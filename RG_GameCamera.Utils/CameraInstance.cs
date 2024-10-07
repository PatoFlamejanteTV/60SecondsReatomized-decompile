using UnityEngine;

namespace RG_GameCamera.Utils;

internal class CameraInstance
{
	public static string RootName = "GameCamera";

	private static GameObject cameraRoot;

	public static GameObject GetCameraRoot()
	{
		if (!cameraRoot)
		{
			cameraRoot = GameObject.Find(RootName);
			if (!cameraRoot)
			{
				cameraRoot = new GameObject(RootName);
			}
		}
		return cameraRoot;
	}

	public static T CreateInstance<T>(string name) where T : Component
	{
		GameObject gameObject = GetCameraRoot();
		T componentInChildren = gameObject.GetComponentInChildren<T>();
		if ((bool)componentInChildren)
		{
			return componentInChildren;
		}
		GameObject gameObject2 = new GameObject(name);
		gameObject2.transform.parent = gameObject.transform;
		return gameObject2.AddComponent<T>();
	}
}
