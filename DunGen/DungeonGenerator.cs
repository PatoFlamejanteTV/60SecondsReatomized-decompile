using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DunGen.Analysis;
using DunGen.Graph;
using UnityEngine;

namespace DunGen;

[Serializable]
public class DungeonGenerator
{
	private struct UsedTileSet
	{
		public DungeonArchetype DungeonArchetypeRes;

		public List<TileSet> TileSets;

		public UsedTileSet(DungeonArchetype archetype)
		{
			DungeonArchetypeRes = archetype;
			TileSets = new List<TileSet>();
		}
	}

	public int Seed;

	public bool ShouldRandomizeSeed = true;

	public int MaxAttemptCount = 20;

	public bool IgnoreSpriteBounds = true;

	public Vector3 UpVector = Vector3.up;

	public GameObject Root;

	public DungeonFlow DungeonFlow;

	protected int retryCount;

	protected Dungeon currentDungeon;

	protected readonly List<PreProcessTileData> preProcessData = new List<PreProcessTileData>();

	protected readonly List<GameObject> useableTiles = new List<GameObject>();

	protected int targetLength;

	private int nextNodeIndex;

	private DungeonArchetype currentArchetype;

	private GraphLine previousLineSegment;

	private bool isAnalysis;

	public System.Random RandomStream { get; protected set; }

	public GenerationStatus Status { get; private set; }

	public GenerationStats GenerationStats { get; private set; }

	public int ChosenSeed { get; protected set; }

	public Dungeon CurrentDungeon => currentDungeon;

	public event GenerationStatusDelegate OnGenerationStatusChanged;

	public DungeonGenerator()
	{
		GenerationStats = new GenerationStats();
	}

	public DungeonGenerator(GameObject root)
		: this()
	{
		Root = root;
	}

	public bool OuterGenerate(int? seed)
	{
		ShouldRandomizeSeed = !seed.HasValue;
		if (seed.HasValue)
		{
			Seed = seed.Value;
		}
		return Generate();
	}

	public bool Generate()
	{
		isAnalysis = false;
		return OuterGenerate();
	}

	protected virtual bool OuterGenerate()
	{
		Status = GenerationStatus.NotStarted;
		if (!new DungeonArchetypeValidator(DungeonFlow).IsValid())
		{
			ChangeStatus(GenerationStatus.Failed);
			return false;
		}
		ChosenSeed = (ShouldRandomizeSeed ? new System.Random().Next() : Seed);
		RandomStream = new System.Random(ChosenSeed);
		if (Root == null)
		{
			Root = new GameObject("Dungeon");
		}
		bool num = InnerGenerate(isRetry: false);
		if (!num)
		{
			Clear();
		}
		return num;
	}

	public GenerationAnalysis RunAnalysis(int iterations, float maximumAnalysisTime)
	{
		DungeonArchetypeValidator dungeonArchetypeValidator = new DungeonArchetypeValidator(DungeonFlow);
		if (Application.isEditor && !dungeonArchetypeValidator.IsValid())
		{
			ChangeStatus(GenerationStatus.Failed);
			return null;
		}
		bool shouldRandomizeSeed = ShouldRandomizeSeed;
		isAnalysis = true;
		ShouldRandomizeSeed = true;
		GenerationAnalysis generationAnalysis = new GenerationAnalysis(iterations);
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < iterations; i++)
		{
			if (maximumAnalysisTime > 0f && stopwatch.Elapsed.TotalMilliseconds >= (double)maximumAnalysisTime)
			{
				break;
			}
			if (OuterGenerate())
			{
				generationAnalysis.IncrementSuccessCount();
				generationAnalysis.Add(GenerationStats);
			}
		}
		Clear();
		generationAnalysis.Analyze();
		ShouldRandomizeSeed = shouldRandomizeSeed;
		return generationAnalysis;
	}

	public void RandomizeSeed()
	{
		Seed = new System.Random().Next();
	}

	protected virtual bool InnerGenerate(bool isRetry)
	{
		if (isRetry)
		{
			if (retryCount >= MaxAttemptCount)
			{
				UnityEngine.Debug.LogError($"Failed to generate the dungeon {MaxAttemptCount} times. This could indicate a problem with the way the tiles are set up. Try to make sure most rooms have more than one doorway and that all doorways are easily accessible.");
				ChangeStatus(GenerationStatus.Failed);
				return false;
			}
			retryCount++;
			GenerationStats.IncrementRetryCount();
		}
		else
		{
			retryCount = 0;
			GenerationStats.Clear();
		}
		currentDungeon = Root.GetComponent<Dungeon>();
		if (currentDungeon == null)
		{
			currentDungeon = Root.AddComponent<Dungeon>();
		}
		currentDungeon.PreGenerateDungeon(this);
		Clear();
		GenerationStats.BeginTime(GenerationStatus.PreProcessing);
		PreProcess();
		GenerationStats.BeginTime(GenerationStatus.MainPath);
		if (!GenerateMainPath())
		{
			ChosenSeed = RandomStream.Next();
			return InnerGenerate(isRetry: true);
		}
		GenerationStats.BeginTime(GenerationStatus.Branching);
		GenerateBranchPaths();
		GenerationStats.BeginTime(GenerationStatus.PostProcessing);
		PostProcess();
		GenerationStats.EndTime();
		ChangeStatus(GenerationStatus.Complete);
		return true;
	}

	protected virtual void Clear()
	{
		currentDungeon.Clear();
		foreach (PreProcessTileData preProcessDatum in preProcessData)
		{
			UnityEngine.Object.DestroyImmediate(preProcessDatum.Proxy);
		}
		useableTiles.Clear();
		preProcessData.Clear();
	}

	private void ChangeStatus(GenerationStatus status)
	{
		GenerationStatus status2 = Status;
		Status = status;
		if (status2 != status && this.OnGenerationStatusChanged != null)
		{
			this.OnGenerationStatusChanged(this, status);
		}
	}

	protected virtual void PreProcess()
	{
		if (preProcessData.Count > 0)
		{
			return;
		}
		ChangeStatus(GenerationStatus.PreProcessing);
		TileSet[] usedTileSets = DungeonFlow.GetUsedTileSets();
		for (int i = 0; i < usedTileSets.Length; i++)
		{
			foreach (GameObjectChance weight in usedTileSets[i].TileWeights.Weights)
			{
				if (weight.Value != null)
				{
					useableTiles.Add(weight.Value);
				}
			}
		}
		foreach (GameObject useableTile in useableTiles)
		{
			preProcessData.Add(new PreProcessTileData(useableTile, IgnoreSpriteBounds));
		}
	}

	protected virtual bool GenerateMainPath()
	{
		ChangeStatus(GenerationStatus.MainPath);
		targetLength = DungeonFlow.Length.GetRandom(RandomStream);
		nextNodeIndex = 0;
		List<GraphNode> list = new List<GraphNode>(DungeonFlow.Nodes.Count);
		bool flag = false;
		int num = 0;
		List<List<TileSet>> list2 = new List<List<TileSet>>(targetLength);
		List<DungeonArchetype> list3 = new List<DungeonArchetype>(targetLength);
		List<GraphNode> list4 = new List<GraphNode>(targetLength);
		List<GraphLine> list5 = new List<GraphLine>(targetLength);
		while (!flag)
		{
			float num2 = Mathf.Clamp((float)num / (float)(targetLength - 1), 0f, 1f);
			GraphLine lineAtDepth = DungeonFlow.GetLineAtDepth(num2);
			if (lineAtDepth == null)
			{
				return false;
			}
			if (lineAtDepth != previousLineSegment)
			{
				currentArchetype = lineAtDepth.DungeonArchetypes[RandomStream.Next(0, lineAtDepth.DungeonArchetypes.Count)];
				previousLineSegment = lineAtDepth;
			}
			List<TileSet> list6 = null;
			GraphNode graphNode = null;
			GraphNode[] array = DungeonFlow.Nodes.OrderBy((GraphNode x) => x.Position).ToArray();
			GraphNode[] array2 = array;
			foreach (GraphNode graphNode2 in array2)
			{
				if (num2 >= graphNode2.Position && !list.Contains(graphNode2))
				{
					graphNode = graphNode2;
					list.Add(graphNode2);
					break;
				}
			}
			if (graphNode != null)
			{
				list6 = graphNode.TileSets;
				nextNodeIndex = ((nextNodeIndex >= array.Length - 1) ? (-1) : (nextNodeIndex + 1));
				list3.Add(currentArchetype);
				list5.Add(null);
				list4.Add(graphNode);
				if (graphNode == array[^1])
				{
					flag = true;
				}
			}
			else
			{
				list6 = currentArchetype.TileSets;
				list3.Add(currentArchetype);
				list5.Add(lineAtDepth);
				list4.Add(null);
			}
			list2.Add(list6);
			num++;
		}
		for (int j = 0; j < list2.Count; j++)
		{
			Tile tile = AddTile((j == 0) ? null : currentDungeon.MainPathTiles[j - 1], list2[j], (float)j / (float)(list2.Count - 1), list3[j], tryAllDoors: true);
			if (tile == null)
			{
				return false;
			}
			tile.Node = list4[j];
			tile.Line = list5[j];
		}
		return true;
	}

	protected virtual void GenerateBranchPaths(bool recoverOnTileGenerationFail = true)
	{
		ChangeStatus(GenerationStatus.Branching);
		Dictionary<DungeonArchetype, List<TileSet>> dictionary = new Dictionary<DungeonArchetype, List<TileSet>>();
		TileSet[] array = new TileSet[currentDungeon.DungeonFlow.GetUsedArchetypes()[0].TileSets.Count];
		currentDungeon.DungeonFlow.GetUsedArchetypes()[0].TileSets.CopyTo(array);
		for (int i = 0; i < 5; i++)
		{
			foreach (Tile mainPathTile in currentDungeon.MainPathTiles)
			{
				if (mainPathTile.Archetype == null)
				{
					continue;
				}
				if (!dictionary.ContainsKey(mainPathTile.Archetype))
				{
					dictionary.Add(mainPathTile.Archetype, new List<TileSet>());
				}
				int random = mainPathTile.Archetype.BranchCount.GetRandom(RandomStream);
				if (random == 0)
				{
					continue;
				}
				Tile tile = mainPathTile;
				for (int j = 0; j < random && mainPathTile.Archetype.TileSets.Count != 0; j++)
				{
					Tile tile2 = AddTile(tile, mainPathTile.Archetype.TileSets, (float)j / (float)(random - 1), mainPathTile.Archetype, tryAllDoors: true);
					if (tile2 == null && recoverOnTileGenerationFail)
					{
						tile2 = AddTile(tile, mainPathTile.Archetype.TileSets, (float)j / (float)(random - 1), mainPathTile.Archetype, tryAllDoors: true);
						if (tile2 == null)
						{
							continue;
						}
					}
					mainPathTile.Archetype.TileSets.Remove(tile2.TileSet);
					dictionary[mainPathTile.Archetype].Add(tile2.TileSet);
					tile2.Placement.BranchDepth = j;
					tile2.Placement.NormalizedBranchDepth = (float)j / (float)(random - 1);
					tile2.Node = tile.Node;
					tile2.Line = tile.Line;
					tile = ((tile2.Placement.UnusedDoorways.Count > 1) ? tile2 : mainPathTile);
				}
			}
			foreach (DungeonArchetype key in dictionary.Keys)
			{
				for (int num = key.TileSets.Count - 1; num >= 0; num--)
				{
					for (int k = 0; k < currentDungeon.BranchPathTiles.Count; k++)
					{
						int random2 = key.BranchCount.GetRandom(RandomStream);
						Tile tile3 = AddTile(currentDungeon.BranchPathTiles[k], key.TileSets, 1f, key, tryAllDoors: true);
						if (tile3 != null)
						{
							key.TileSets.Remove(tile3.TileSet);
							dictionary[key].Add(tile3.TileSet);
							tile3.Placement.BranchDepth = num;
							tile3.Placement.NormalizedBranchDepth = (float)num / (float)(random2 - 1);
							tile3.Node = currentDungeon.BranchPathTiles[k].Node;
							tile3.Line = currentDungeon.BranchPathTiles[k].Line;
							break;
						}
					}
				}
			}
			bool flag = false;
			foreach (DungeonArchetype key2 in dictionary.Keys)
			{
				flag |= key2.TileSets.Count == 0;
			}
			if (flag)
			{
				break;
			}
		}
		DungeonArchetype obj = currentDungeon.DungeonFlow.GetUsedArchetypes()[0];
		obj.TileSets.Clear();
		obj.TileSets.AddRange(array);
		dictionary.Clear();
		array = null;
	}

	protected virtual Tile AddTile(Tile attachTo, IList<TileSet> useableTileSets, float normalizedDepth, DungeonArchetype archetype, bool tryAllDoors = false)
	{
		Tile tile = null;
		TileSet tileSet = useableTileSets[RandomStream.Next(0, useableTileSets.Count)];
		Doorway doorway = null;
		if (tryAllDoors && attachTo != null)
		{
			int num = attachTo.Placement.UnusedDoorways.Count;
			int num2 = UnityEngine.Random.Range(0, num);
			while (num > 0)
			{
				doorway = attachTo.Placement.UnusedDoorways[num2];
				tile = AddTile(attachTo, tileSet, doorway, normalizedDepth, archetype);
				if (tile != null)
				{
					break;
				}
				num--;
				num2++;
				if (num2 >= attachTo.Placement.UnusedDoorways.Count)
				{
					num2 = 0;
				}
			}
		}
		else
		{
			tile = AddTile(attachTo, tileSet, (attachTo == null) ? null : attachTo.Placement.PickRandomDoorway(RandomStream, mustBeAvailable: true), normalizedDepth, archetype);
		}
		return tile;
	}

	protected virtual Tile AddTile(Tile attachTo, TileSet tileSet, Doorway fromDoorway, float normalizedDepth, DungeonArchetype archetype)
	{
		if (attachTo != null && fromDoorway == null)
		{
			return null;
		}
		GameObjectChanceTable gameObjectChanceTable = tileSet.TileWeights.Clone();
		if (attachTo != null)
		{
			for (int num = gameObjectChanceTable.Weights.Count - 1; num >= 0; num--)
			{
				GameObjectChance c = gameObjectChanceTable.Weights[num];
				PreProcessTileData preProcessTileData = preProcessData.Where((PreProcessTileData x) => x.Prefab == c.Value).FirstOrDefault();
				if (preProcessTileData == null || !preProcessTileData.DoorwaySockets.Contains(fromDoorway.SocketGroup))
				{
					gameObjectChanceTable.Weights.RemoveAt(num);
				}
			}
		}
		if (gameObjectChanceTable.Weights.Count == 0)
		{
			return null;
		}
		GameObject tilePrefab = tileSet.TileWeights.GetRandom(RandomStream, Status == GenerationStatus.MainPath, normalizedDepth);
		PreProcessTileData preProcessTileData2 = preProcessData.Where((PreProcessTileData x) => x.Prefab == tilePrefab).FirstOrDefault();
		if (preProcessTileData2 == null)
		{
			return null;
		}
		int doorwayIndex = 0;
		Doorway doorway = null;
		if (fromDoorway != null)
		{
			Tile component = preProcessTileData2.Prefab.GetComponent<Tile>();
			if (!preProcessTileData2.ChooseRandomDoorway(allowedDirection: (!(component == null) && !component.AllowRotation) ? new Vector3?(-fromDoorway.transform.forward) : null, random: RandomStream, socketGroupFilter: fromDoorway.SocketGroup, doorwayIndex: out doorwayIndex, doorway: out doorway))
			{
				return null;
			}
			MoveIntoPosition(preProcessTileData2.Proxy, fromDoorway, doorway);
			if (IsCollidingWithAnyTile(preProcessTileData2.Proxy))
			{
				return null;
			}
		}
		TilePlacementData tilePlacementData = new TilePlacementData(preProcessTileData2, Status == GenerationStatus.MainPath, archetype, tileSet);
		if (tilePlacementData == null)
		{
			return null;
		}
		if (tilePlacementData.IsOnMainPath)
		{
			if (attachTo != null)
			{
				tilePlacementData.PathDepth = attachTo.Placement.PathDepth + 1;
			}
		}
		else
		{
			tilePlacementData.PathDepth = attachTo.Placement.PathDepth;
			tilePlacementData.BranchDepth = ((!attachTo.Placement.IsOnMainPath) ? (attachTo.Placement.BranchDepth + 1) : 0);
		}
		if (fromDoorway != null)
		{
			if (!Application.isPlaying)
			{
				tilePlacementData.Root.SetActive(value: false);
			}
			tilePlacementData.Root.transform.parent = Root.transform;
			doorway = tilePlacementData.AllDoorways[doorwayIndex];
			MoveIntoPosition(tilePlacementData.Root, fromDoorway, doorway);
			if (!Application.isPlaying)
			{
				tilePlacementData.Root.SetActive(value: true);
			}
			currentDungeon.MakeConnection(fromDoorway, doorway, RandomStream);
		}
		else
		{
			tilePlacementData.Root.transform.parent = Root.transform;
		}
		if (tilePlacementData != null)
		{
			currentDungeon.AddTile(tilePlacementData.Tile);
		}
		tilePlacementData.RecalculateBounds(IgnoreSpriteBounds);
		return tilePlacementData.Tile;
	}

	protected PreProcessTileData PickRandomTemplate(DoorwaySocketType? socketGroupFilter)
	{
		IEnumerable<PreProcessTileData> enumerable2;
		if (!socketGroupFilter.HasValue)
		{
			IEnumerable<PreProcessTileData> enumerable = preProcessData;
			enumerable2 = enumerable;
		}
		else
		{
			enumerable2 = preProcessData.Where((PreProcessTileData x) => x.DoorwaySockets.Contains(socketGroupFilter.Value));
		}
		IEnumerable<PreProcessTileData> source = enumerable2;
		return source.ElementAt(RandomStream.Next(0, source.Count()));
	}

	protected int NormalizedDepthToIndex(float normalizedDepth)
	{
		return Mathf.RoundToInt(normalizedDepth * (float)(targetLength - 1));
	}

	protected float IndexToNormalizedDepth(int index)
	{
		return (float)index / (float)targetLength;
	}

	protected void MoveIntoPosition(GameObject obj, Doorway fromDoorway, Doorway toDoorway)
	{
		obj.transform.position = -toDoorway.transform.position + fromDoorway.transform.position;
		obj.transform.rotation = Quaternion.identity;
		Vector3 forward = fromDoorway.transform.forward;
		Vector3 rhs = -toDoorway.transform.forward;
		float num = Vector3.Dot(forward, rhs);
		float num2 = 0f;
		num2 = ((num >= 0.99999f) ? 0f : ((!(num <= -0.99999f)) ? Mathf.Acos(num) : ((float)Math.PI)));
		if (float.IsNaN(num2))
		{
			UnityEngine.Debug.LogError("[FloorGenerator] Offset angle is NaN. This should never happen. | Dot: " + num + ", From: " + fromDoorway.transform.forward.ToString() + ", To: " + toDoorway.transform.forward.ToString());
		}
		Vector3 rhs2 = Vector3.Cross(forward, rhs);
		if (Vector3.Dot(UpVector, rhs2) > 0f)
		{
			num2 *= -1f;
		}
		obj.transform.RotateAround(fromDoorway.transform.position, UpVector, num2 * 57.29578f);
	}

	protected bool IsCollidingWithAnyTile(GameObject proxy)
	{
		foreach (Tile allTile in currentDungeon.AllTiles)
		{
			if (allTile.Placement.Bounds.Intersects(proxy.GetComponent<Collider>().bounds))
			{
				return true;
			}
		}
		return false;
	}

	protected void ClearPreProcessData()
	{
		foreach (PreProcessTileData preProcessDatum in preProcessData)
		{
			UnityEngine.Object.DestroyImmediate(preProcessDatum.Proxy);
		}
		preProcessData.Clear();
	}

	protected virtual void ConnectOverlappingDoorways(float percentageChance)
	{
		if (percentageChance <= 0f)
		{
			return;
		}
		Doorway[] componentsInChildren = Root.GetComponentsInChildren<Doorway>();
		List<Doorway> list = new List<Doorway>(componentsInChildren.Length);
		Doorway[] array = componentsInChildren;
		foreach (Doorway doorway in array)
		{
			Doorway[] array2 = componentsInChildren;
			foreach (Doorway doorway2 in array2)
			{
				if (!(doorway == doorway2) && !(doorway.Tile == doorway2.Tile) && !list.Contains(doorway2) && (doorway.transform.position - doorway2.transform.position).sqrMagnitude < 1E-05f && RandomStream.NextDouble() < (double)percentageChance)
				{
					currentDungeon.MakeConnection(doorway, doorway2, RandomStream);
				}
			}
			list.Add(doorway);
		}
	}

	protected virtual void PostProcess()
	{
		ChangeStatus(GenerationStatus.PostProcessing);
		foreach (Tile allTile in currentDungeon.AllTiles)
		{
			allTile.gameObject.SetActive(value: true);
		}
		int count = currentDungeon.MainPathTiles.Count;
		int maxBranchDepth = (from x in currentDungeon.BranchPathTiles
			orderby x.Placement.BranchDepth descending
			select x.Placement.BranchDepth).FirstOrDefault();
		if (!isAnalysis)
		{
			ConnectOverlappingDoorways(DungeonFlow.DoorwayConnectionChance);
			foreach (Tile allTile2 in currentDungeon.AllTiles)
			{
				allTile2.Placement.NormalizedPathDepth = (float)allTile2.Placement.PathDepth / (float)(count - 1);
				allTile2.Placement.ProcessDoorways();
			}
			currentDungeon.PostGenerateDungeon(this);
			PlaceLocksAndKeys();
			foreach (Tile allTile3 in currentDungeon.AllTiles)
			{
				RandomProp[] componentsInChildren = allTile3.GetComponentsInChildren<RandomProp>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].Process(RandomStream, allTile3);
				}
			}
			ProcessGlobalProps();
		}
		GenerationStats.SetRoomStatistics(currentDungeon.MainPathTiles.Count, currentDungeon.BranchPathTiles.Count, maxBranchDepth);
		ClearPreProcessData();
	}

	protected virtual void ProcessGlobalProps()
	{
		Dictionary<int, GameObjectChanceTable> dictionary = new Dictionary<int, GameObjectChanceTable>();
		foreach (Tile allTile in currentDungeon.AllTiles)
		{
			GlobalProp[] componentsInChildren = allTile.GetComponentsInChildren<GlobalProp>();
			foreach (GlobalProp globalProp in componentsInChildren)
			{
				GameObjectChanceTable value = null;
				if (!dictionary.TryGetValue(globalProp.PropGroupID, out value))
				{
					value = new GameObjectChanceTable();
					dictionary[globalProp.PropGroupID] = value;
				}
				float num = (allTile.Placement.IsOnMainPath ? globalProp.MainPathWeight : globalProp.BranchPathWeight);
				num *= globalProp.DepthWeightScale.Evaluate(allTile.Placement.NormalizedDepth);
				value.Weights.Add(new GameObjectChance(globalProp.gameObject, num, 0f));
			}
		}
		foreach (GameObject item in dictionary.SelectMany((KeyValuePair<int, GameObjectChanceTable> x) => x.Value.Weights.Select((GameObjectChance y) => y.Value)))
		{
			item.SetActive(value: false);
		}
		List<int> list = new List<int>(dictionary.Count);
		foreach (KeyValuePair<int, GameObjectChanceTable> item2 in dictionary)
		{
			if (list.Contains(item2.Key))
			{
				UnityEngine.Debug.LogWarning("Dungeon Flow contains multiple entries for the global prop group ID: " + item2.Key + ". Only the first entry will be used.");
				continue;
			}
			int num2 = DungeonFlow.GlobalPropGroupIDs.IndexOf(item2.Key);
			if (num2 == -1)
			{
				continue;
			}
			IntRange intRange = DungeonFlow.GlobalPropRanges[num2];
			GameObjectChanceTable gameObjectChanceTable = item2.Value.Clone();
			int random = intRange.GetRandom(RandomStream);
			random = Mathf.Clamp(random, 0, gameObjectChanceTable.Weights.Count);
			for (int j = 0; j < random; j++)
			{
				GameObject random2 = gameObjectChanceTable.GetRandom(RandomStream, isOnMainPath: true, 0f, removeFromTable: true);
				if (random2 != null)
				{
					random2.SetActive(value: true);
				}
			}
			list.Add(item2.Key);
		}
	}

	protected virtual void PlaceLocksAndKeys()
	{
		GraphNode[] array = (from x in currentDungeon.ConnectionGraph.Nodes
			select x.Tile.Node into x
			where x != null
			select x).Distinct().ToArray();
		GraphLine[] array2 = (from x in currentDungeon.ConnectionGraph.Nodes
			select x.Tile.Line into x
			where x != null
			select x).Distinct().ToArray();
		Dictionary<Doorway, Key> dictionary = new Dictionary<Doorway, Key>();
		GraphNode[] array3 = array;
		foreach (GraphNode node in array3)
		{
			foreach (KeyLockPlacement @lock in node.Locks)
			{
				Tile tile = currentDungeon.AllTiles.Where((Tile x) => x.Node == node).FirstOrDefault();
				List<DungeonGraphConnection> connections = currentDungeon.ConnectionGraph.Nodes.Where((DungeonGraphNode x) => x.Tile == tile).FirstOrDefault().Connections;
				Doorway doorway = null;
				Doorway doorway2 = null;
				foreach (DungeonGraphConnection item in connections)
				{
					if (item.DoorwayA.Tile == tile)
					{
						doorway2 = item.DoorwayA;
					}
					else if (item.DoorwayB.Tile == tile)
					{
						doorway = item.DoorwayB;
					}
				}
				if (doorway != null && (node.LockPlacement & NodeLockPlacement.Entrance) == NodeLockPlacement.Entrance)
				{
					Key keyByID = node.Graph.KeyManager.GetKeyByID(@lock.ID);
					dictionary[doorway] = keyByID;
				}
				if (doorway2 != null && (node.LockPlacement & NodeLockPlacement.Exit) == NodeLockPlacement.Exit)
				{
					Key keyByID2 = node.Graph.KeyManager.GetKeyByID(@lock.ID);
					dictionary[doorway] = keyByID2;
				}
			}
		}
		GraphLine[] array4 = array2;
		foreach (GraphLine line in array4)
		{
			List<Doorway> list = (from x in currentDungeon.ConnectionGraph.Connections
				where x.DoorwayA.Tile.Line == line && x.DoorwayB.Tile.Line == line
				select x.DoorwayA).ToList();
			foreach (KeyLockPlacement lock2 in line.Locks)
			{
				Mathf.Clamp(lock2.Range.GetRandom(RandomStream), 0, list.Count);
				Doorway doorway3 = list[RandomStream.Next(0, list.Count)];
				list.Remove(doorway3);
				Key keyByID3 = line.Graph.KeyManager.GetKeyByID(lock2.ID);
				dictionary.Add(doorway3, keyByID3);
			}
		}
		List<Doorway> list2 = new List<Doorway>();
		foreach (KeyValuePair<Doorway, Key> item2 in dictionary)
		{
			Doorway key2 = item2.Key;
			Key key = item2.Value;
			List<Tile> list3 = new List<Tile>();
			foreach (Tile allTile in currentDungeon.AllTiles)
			{
				if (!(allTile.Placement.NormalizedPathDepth >= key2.Tile.Placement.NormalizedPathDepth))
				{
					bool flag = false;
					if (allTile.Node != null && allTile.Node.Keys.Where((KeyLockPlacement x) => x.ID == key.ID).Count() > 0)
					{
						flag = true;
					}
					else if (allTile.Line != null && allTile.Line.Keys.Where((KeyLockPlacement x) => x.ID == key.ID).Count() > 0)
					{
						flag = true;
					}
					if (flag && (key2.Tile.Placement.IsOnMainPath || !(allTile.Placement.NormalizedBranchDepth >= key2.Tile.Placement.NormalizedBranchDepth)))
					{
						list3.Add(allTile);
					}
				}
			}
			IEnumerable<IKeySpawnable> source = list3.SelectMany((Tile x) => x.GetComponentsInChildren<Component>().OfType<IKeySpawnable>());
			if (source.Count() == 0)
			{
				list2.Add(key2);
				continue;
			}
			IKeySpawnable keySpawnable = source.ElementAt(RandomStream.Next(0, source.Count()));
			keySpawnable.SpawnKey(key, DungeonFlow.KeyManager);
			foreach (IKeyLock item3 in (keySpawnable as Component).GetComponentsInChildren<Component>().OfType<IKeyLock>())
			{
				item3.OnKeyAssigned(key, DungeonFlow.KeyManager);
			}
		}
		foreach (Doorway item4 in list2)
		{
			dictionary.Remove(item4);
		}
		foreach (KeyValuePair<Doorway, Key> item5 in dictionary)
		{
			item5.Key.RemoveUsedPrefab();
			LockDoorway(item5.Key, item5.Value, DungeonFlow.KeyManager);
		}
	}

	protected virtual void LockDoorway(Doorway doorway, Key key, KeyManager keyManager)
	{
		TilePlacementData placement = doorway.Tile.Placement;
		GameObjectChanceTable[] array = (from x in doorway.Tile.TileSet.LockPrefabs
			where x.SocketGroup == doorway.SocketGroup
			select x.LockPrefabs).ToArray();
		GameObject gameObject = UnityEngine.Object.Instantiate(array[RandomStream.Next(0, array.Length)].GetRandom(RandomStream, placement.IsOnMainPath, placement.NormalizedDepth));
		gameObject.transform.parent = doorway.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		foreach (IKeyLock item in doorway.GetComponentsInChildren<Component>().OfType<IKeyLock>())
		{
			item.OnKeyAssigned(key, keyManager);
		}
	}
}
