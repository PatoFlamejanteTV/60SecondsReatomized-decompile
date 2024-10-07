using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DunGen;

public sealed class PreProcessTileData
{
	public readonly List<DoorwaySocketType> DoorwaySockets = new List<DoorwaySocketType>();

	public readonly List<Doorway> Doorways = new List<Doorway>();

	public GameObject Prefab { get; private set; }

	public GameObject Proxy { get; private set; }

	public PreProcessTileData(GameObject prefab, bool ignoreSpriteRendererBounds)
	{
		Prefab = prefab;
		Proxy = new GameObject(prefab.name + "_PROXY");
		CalculateProxyBounds(ignoreSpriteRendererBounds);
		GetAllDoorways();
	}

	public bool ChooseRandomDoorway(System.Random random, DoorwaySocketType? socketGroupFilter, Vector3? allowedDirection, out int doorwayIndex, out Doorway doorway)
	{
		doorwayIndex = -1;
		doorway = null;
		IEnumerable<Doorway> source = Doorways;
		if (socketGroupFilter.HasValue)
		{
			source = source.Where((Doorway x) => DoorwaySocket.IsMatchingSocket(x.SocketGroup, socketGroupFilter.Value));
		}
		if (allowedDirection.HasValue)
		{
			source = source.Where(delegate(Doorway x)
			{
				Vector3 forward = x.transform.forward;
				Vector3? vector = allowedDirection;
				return forward == vector;
			});
		}
		if (source.Count() == 0)
		{
			return false;
		}
		doorway = source.ElementAt(random.Next(0, source.Count()));
		doorwayIndex = Doorways.IndexOf(doorway);
		return true;
	}

	private void CalculateProxyBounds(bool ignoreSpriteRendererBounds)
	{
		Bounds bounds = UnityUtil.CalculateObjectBounds(Prefab, includeInactive: true, ignoreSpriteRendererBounds);
		bounds.size *= 0.99f;
		BoxCollider boxCollider = Proxy.AddComponent<BoxCollider>();
		boxCollider.center = bounds.center;
		boxCollider.size = bounds.size;
	}

	private void GetAllDoorways()
	{
		DoorwaySockets.Clear();
		Doorway[] componentsInChildren = Prefab.GetComponentsInChildren<Doorway>(includeInactive: true);
		foreach (Doorway doorway in componentsInChildren)
		{
			Doorways.Add(doorway);
			if (!DoorwaySockets.Contains(doorway.SocketGroup))
			{
				DoorwaySockets.Add(doorway.SocketGroup);
			}
		}
	}
}
