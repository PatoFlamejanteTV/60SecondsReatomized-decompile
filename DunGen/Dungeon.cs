using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DunGen.Graph;
using UnityEngine;

namespace DunGen;

public class Dungeon : MonoBehaviour
{
	private readonly List<Tile> allTiles = new List<Tile>();

	private readonly List<Tile> mainPathTiles = new List<Tile>();

	private readonly List<Tile> branchPathTiles = new List<Tile>();

	private readonly List<DoorwayConnection> connections = new List<DoorwayConnection>();

	public DungeonFlow DungeonFlow { get; protected set; }

	public ReadOnlyCollection<Tile> AllTiles { get; private set; }

	public ReadOnlyCollection<Tile> MainPathTiles { get; private set; }

	public ReadOnlyCollection<Tile> BranchPathTiles { get; private set; }

	public ReadOnlyCollection<DoorwayConnection> Connections { get; private set; }

	public DungeonGraph ConnectionGraph { get; private set; }

	internal void PreGenerateDungeon(DungeonGenerator dungeonGenerator)
	{
		DungeonFlow = dungeonGenerator.DungeonFlow;
		AllTiles = new ReadOnlyCollection<Tile>(new Tile[0]);
		MainPathTiles = new ReadOnlyCollection<Tile>(new Tile[0]);
		BranchPathTiles = new ReadOnlyCollection<Tile>(new Tile[0]);
		Connections = new ReadOnlyCollection<DoorwayConnection>(new DoorwayConnection[0]);
	}

	internal void PostGenerateDungeon(DungeonGenerator dungeonGenerator)
	{
		ConnectionGraph = new DungeonGraph(this);
	}

	public void Clear()
	{
		foreach (Tile allTile in allTiles)
		{
			UnityEngine.Object.DestroyImmediate(allTile.gameObject);
		}
		allTiles.Clear();
		mainPathTiles.Clear();
		branchPathTiles.Clear();
		connections.Clear();
		ExposeRoomProperties();
	}

	internal void MakeConnection(Doorway a, Doorway b, System.Random randomStream)
	{
		DoorwayConnection item = new DoorwayConnection(a, b);
		a.Tile.Placement.UnusedDoorways.Remove(a);
		a.Tile.Placement.UsedDoorways.Add(a);
		b.Tile.Placement.UnusedDoorways.Remove(b);
		b.Tile.Placement.UsedDoorways.Add(b);
		connections.Add(item);
		List<GameObject> list = ((a.DoorPrefabs.Count > 0) ? a.DoorPrefabs : b.DoorPrefabs);
		if (list.Count > 0)
		{
			GameObject gameObject = list[randomStream.Next(0, list.Count)];
			if (gameObject != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
				gameObject2.transform.position = a.transform.position;
				gameObject2.transform.rotation = a.transform.rotation;
				gameObject2.transform.localScale = a.transform.localScale;
				gameObject2.transform.parent = a.transform;
				a.SetUsedPrefab(gameObject2);
				b.SetUsedPrefab(gameObject2);
			}
		}
	}

	internal void AddTile(Tile tile)
	{
		allTiles.Add(tile);
		if (tile.Placement.IsOnMainPath)
		{
			mainPathTiles.Add(tile);
		}
		else
		{
			branchPathTiles.Add(tile);
		}
	}

	internal void ExposeRoomProperties()
	{
		AllTiles = new ReadOnlyCollection<Tile>(allTiles);
		MainPathTiles = new ReadOnlyCollection<Tile>(mainPathTiles);
		BranchPathTiles = new ReadOnlyCollection<Tile>(branchPathTiles);
		Connections = new ReadOnlyCollection<DoorwayConnection>(connections);
	}
}
