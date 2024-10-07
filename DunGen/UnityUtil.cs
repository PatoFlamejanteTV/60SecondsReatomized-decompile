using System.Collections.Generic;
using UnityEngine;

namespace DunGen;

public static class UnityUtil
{
	public static string GetUniqueName(string name, IEnumerable<string> usedNames)
	{
		if (string.IsNullOrEmpty(name))
		{
			return GetUniqueName("New", usedNames);
		}
		string text = name;
		int result = 0;
		bool flag = false;
		int num = name.LastIndexOf(' ');
		if (num > -1)
		{
			text = name.Substring(0, num);
			flag = int.TryParse(name.Substring(num + 1), out result);
			result++;
		}
		foreach (string usedName in usedNames)
		{
			if (usedName == name)
			{
				if (flag)
				{
					return GetUniqueName(text + " " + result, usedNames);
				}
				return GetUniqueName(name + " 2", usedNames);
			}
		}
		return name;
	}

	public static Bounds CalculateObjectBounds(GameObject obj, bool includeInactive, bool ignoreSpriteRenderers)
	{
		Bounds result = default(Bounds);
		bool flag = false;
		Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>(includeInactive);
		foreach (Renderer renderer in componentsInChildren)
		{
			if (renderer is MeshRenderer || (renderer is SpriteRenderer && !ignoreSpriteRenderers))
			{
				if (flag)
				{
					result.Encapsulate(renderer.bounds);
				}
				else
				{
					result = renderer.bounds;
				}
				flag = true;
			}
		}
		Collider[] componentsInChildren2 = obj.GetComponentsInChildren<Collider>(includeInactive);
		foreach (Collider collider in componentsInChildren2)
		{
			if (flag)
			{
				result.Encapsulate(collider.bounds);
			}
			else
			{
				result = collider.bounds;
			}
			flag = true;
		}
		return result;
	}

	public static IEnumerable<T> GetComponentsInParents<T>(GameObject obj, bool includeInactive = false) where T : Component
	{
		if (obj.activeSelf || includeInactive)
		{
			T[] components = obj.GetComponents<T>();
			for (int i = 0; i < components.Length; i++)
			{
				yield return components[i];
			}
		}
		if (!(obj.transform.parent != null))
		{
			yield break;
		}
		foreach (T componentsInParent in GetComponentsInParents<T>(obj.transform.parent.gameObject, includeInactive))
		{
			yield return componentsInParent;
		}
	}

	public static T GetComponentInParents<T>(GameObject obj, bool includeInactive = false) where T : Component
	{
		if (obj.activeSelf || includeInactive)
		{
			T[] components = obj.GetComponents<T>();
			int num = 0;
			if (num < components.Length)
			{
				return components[num];
			}
		}
		if (obj.transform.parent != null)
		{
			return GetComponentInParents<T>(obj.transform.parent.gameObject, includeInactive);
		}
		return null;
	}
}
