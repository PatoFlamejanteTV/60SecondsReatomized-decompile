using DunGen.Graph;
using UnityEngine;

namespace DunGen;

public sealed class DungeonArchetypeValidator
{
	public DungeonFlow Flow { get; private set; }

	public DungeonArchetypeValidator(DungeonFlow flow)
	{
		Flow = flow;
	}

	public bool IsValid()
	{
		if (Flow == null)
		{
			LogError("No Dungeon Flow is assigned");
			return false;
		}
		DungeonArchetype[] usedArchetypes = Flow.GetUsedArchetypes();
		TileSet[] usedTileSets = Flow.GetUsedTileSets();
		foreach (GraphLine line in Flow.Lines)
		{
			if (line.DungeonArchetypes.Count == 0)
			{
				LogError("One or more line segments in your dungeon flow graph have no archetype applied. Each line segment must have at least one archetype assigned to it.");
				return false;
			}
			foreach (DungeonArchetype dungeonArchetype in line.DungeonArchetypes)
			{
				if (dungeonArchetype == null)
				{
					LogError("One or more of the archetypes in your dungeon flow graph have an unset archetype value.");
					return false;
				}
			}
		}
		foreach (GraphNode node in Flow.Nodes)
		{
			if (node.TileSets.Count == 0)
			{
				LogError("The \"{0}\" node in your dungeon flow graph have no tile sets applied. Each node must have at least one tile set assigned to it.", node.Label);
				return false;
			}
		}
		DungeonArchetype[] array = usedArchetypes;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == null)
			{
				LogError("An Archetype in the Dungeon Flow has not been assigned a value");
				return false;
			}
		}
		TileSet[] array2 = usedTileSets;
		foreach (TileSet tileSet in array2)
		{
			if (tileSet == null)
			{
				LogError("A TileSet in the Dungeon Flow has not been assigned a value");
				return false;
			}
			if (tileSet.TileWeights.Weights.Count == 0)
			{
				LogError("TileSet \"{0}\" contains no Tiles", tileSet.name);
				return false;
			}
			foreach (GameObjectChance weight in tileSet.TileWeights.Weights)
			{
				if (weight.Value == null)
				{
					LogWarning("TileSet \"{0}\" contains an entry with no Tile", tileSet.name);
				}
				if (weight.MainPathWeight <= 0f && weight.BranchPathWeight <= 0f)
				{
					LogWarning("TileSet \"{0}\" contains an entry with an invalid weight. Both weights are below zero, resulting in no chance for this tile to spawn in the dungeon. Either MainPathWeight or BranchPathWeight can be zero, not both.", tileSet.name);
				}
			}
		}
		return true;
	}

	private void LogError(string format, params object[] args)
	{
		Debug.LogError(string.Format("[ArchetypeValidator] Error: " + format, args));
	}

	private void LogWarning(string format, params object[] args)
	{
		Debug.LogWarning(string.Format("[ArchetypeValidator] Warning: " + format, args));
	}
}
