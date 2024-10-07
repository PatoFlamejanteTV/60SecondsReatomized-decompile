using System;
using System.Collections.Generic;
using UnityEngine;

namespace DunGen;

[Serializable]
public sealed class TilePlacementData
{
	public List<Doorway> UsedDoorways = new List<Doorway>();

	public List<Doorway> UnusedDoorways = new List<Doorway>();

	public List<Doorway> AllDoorways = new List<Doorway>();

	[SerializeField]
	private int pathDepth;

	[SerializeField]
	private float normalizedPathDepth;

	[SerializeField]
	private int branchDepth;

	[SerializeField]
	private float normalizedBranchDepth;

	[SerializeField]
	private bool isOnMainPath;

	[SerializeField]
	private Bounds bounds;

	[SerializeField]
	private GameObject root;

	[SerializeField]
	private Tile tile;

	public GameObject Root => root;

	public Tile Tile => tile;

	public int PathDepth
	{
		get
		{
			return pathDepth;
		}
		internal set
		{
			pathDepth = value;
		}
	}

	public float NormalizedPathDepth
	{
		get
		{
			return normalizedPathDepth;
		}
		internal set
		{
			normalizedPathDepth = value;
		}
	}

	public int BranchDepth
	{
		get
		{
			return branchDepth;
		}
		internal set
		{
			branchDepth = value;
		}
	}

	public float NormalizedBranchDepth
	{
		get
		{
			return normalizedBranchDepth;
		}
		internal set
		{
			normalizedBranchDepth = value;
		}
	}

	public bool IsOnMainPath
	{
		get
		{
			return isOnMainPath;
		}
		internal set
		{
			isOnMainPath = value;
		}
	}

	public Bounds Bounds
	{
		get
		{
			return bounds;
		}
		internal set
		{
			bounds = value;
		}
	}

	public int Depth
	{
		get
		{
			if (!isOnMainPath)
			{
				return branchDepth;
			}
			return pathDepth;
		}
	}

	public float NormalizedDepth
	{
		get
		{
			if (!isOnMainPath)
			{
				return normalizedBranchDepth;
			}
			return normalizedPathDepth;
		}
	}

	internal TilePlacementData(PreProcessTileData preProcessData, bool isOnMainPath, DungeonArchetype archetype, TileSet tileSet)
	{
		root = UnityEngine.Object.Instantiate(preProcessData.Prefab);
		Bounds = preProcessData.Proxy.GetComponent<Collider>().bounds;
		IsOnMainPath = isOnMainPath;
		tile = Root.GetComponent<Tile>();
		if (tile == null)
		{
			tile = Root.AddComponent<Tile>();
		}
		tile.Placement = this;
		tile.Archetype = archetype;
		tile.TileSet = tileSet;
		Doorway[] componentsInChildren = Root.GetComponentsInChildren<Doorway>();
		foreach (Doorway doorway in componentsInChildren)
		{
			doorway.Tile = tile;
			AllDoorways.Add(doorway);
		}
		UnusedDoorways.AddRange(AllDoorways);
		root.SetActive(value: false);
	}

	public void ProcessDoorways()
	{
		foreach (Doorway usedDoorway in UsedDoorways)
		{
			foreach (GameObject item in usedDoorway.AddWhenNotInUse)
			{
				UnityEngine.Object.DestroyImmediate(item);
			}
		}
		foreach (Doorway unusedDoorway in UnusedDoorways)
		{
			foreach (GameObject item2 in unusedDoorway.AddWhenInUse)
			{
				UnityEngine.Object.DestroyImmediate(item2);
			}
		}
		foreach (Doorway allDoorway in AllDoorways)
		{
			allDoorway.placedByGenerator = true;
		}
	}

	public void RecalculateBounds(bool ignoreSpriteRenderers)
	{
		Bounds = UnityUtil.CalculateObjectBounds(Root, includeInactive: true, ignoreSpriteRenderers);
	}

	public Doorway PickRandomDoorway(System.Random randomStream, bool mustBeAvailable)
	{
		int num = PickRandomDoorwayIndex(randomStream, mustBeAvailable);
		if (num != -1)
		{
			return AllDoorways[num];
		}
		return null;
	}

	public int PickRandomDoorwayIndex(System.Random randomStream, bool mustBeAvailable)
	{
		List<Doorway> list = (mustBeAvailable ? UnusedDoorways : AllDoorways);
		if (list.Count == 0)
		{
			return -1;
		}
		return AllDoorways.IndexOf(list[randomStream.Next(0, list.Count)]);
	}
}
