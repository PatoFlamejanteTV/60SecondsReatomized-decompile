using System;
using System.Collections.Generic;
using UnityEngine;

internal class dfMaterialCache
{
	private class Cache
	{
		private static List<Cache> cacheInstances = new List<Cache>();

		private Material baseMaterial;

		private List<Material> instances = new List<Material>(10);

		private int currentIndex;

		private Cache()
		{
			throw new NotImplementedException();
		}

		public Cache(Material BaseMaterial)
		{
			baseMaterial = BaseMaterial;
			instances.Add(BaseMaterial);
			cacheInstances.Add(this);
		}

		public static void ClearAll()
		{
			for (int i = 0; i < cacheInstances.Count; i++)
			{
				cacheInstances[i].Clear();
			}
			cacheInstances.Clear();
		}

		public static void ResetAll()
		{
			for (int i = 0; i < cacheInstances.Count; i++)
			{
				cacheInstances[i].Reset();
			}
		}

		public Material Obtain()
		{
			if (currentIndex < instances.Count)
			{
				return instances[currentIndex++];
			}
			currentIndex++;
			Material material = new Material(baseMaterial)
			{
				hideFlags = (HideFlags.DontSave | HideFlags.HideInInspector),
				name = $"{baseMaterial.name} (Copy {currentIndex})"
			};
			instances.Add(material);
			return material;
		}

		public void Reset()
		{
			currentIndex = 0;
		}

		public void Clear()
		{
			currentIndex = 0;
			for (int i = 1; i < instances.Count; i++)
			{
				Material material = instances[i];
				if (material != null)
				{
					if (Application.isPlaying)
					{
						UnityEngine.Object.Destroy(material);
					}
					else
					{
						UnityEngine.Object.DestroyImmediate(material);
					}
				}
			}
			instances.Clear();
		}
	}

	private static Dictionary<Material, Cache> caches = new Dictionary<Material, Cache>();

	public static Material Lookup(Material BaseMaterial)
	{
		if (BaseMaterial == null)
		{
			Debug.LogError("Cache lookup on null material");
			return null;
		}
		Cache value = null;
		if (!caches.TryGetValue(BaseMaterial, out value))
		{
			Cache cache2 = (caches[BaseMaterial] = new Cache(BaseMaterial));
			value = cache2;
		}
		return value.Obtain();
	}

	public static void Reset()
	{
		Cache.ResetAll();
	}

	public static void Clear()
	{
		Cache.ClearAll();
		caches.Clear();
	}
}
