using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DunGen.Graph;

[Serializable]
public class DungeonFlow : ScriptableObject
{
	public IntRange Length = new IntRange(5, 10);

	public List<int> GlobalPropGroupIDs = new List<int>();

	public List<IntRange> GlobalPropRanges = new List<IntRange>();

	public KeyManager KeyManager;

	public float DoorwayConnectionChance;

	public List<GraphNode> Nodes = new List<GraphNode>();

	public List<GraphLine> Lines = new List<GraphLine>();

	public void Reset()
	{
		Nodes.Clear();
		Lines.Clear();
		GraphNode graphNode = new GraphNode(this);
		GraphLine graphLine = new GraphLine(this);
		GraphNode graphNode2 = new GraphNode(this);
		graphNode.NodeType = NodeType.Start;
		graphNode.Label = "Start";
		graphLine.Length = 1f;
		graphNode2.NodeType = NodeType.Goal;
		graphNode2.Label = "Goal";
		graphNode2.Position = 1f;
		Nodes.Add(graphNode);
		Nodes.Add(graphNode2);
		Lines.Add(graphLine);
	}

	public GraphLine GetLineAtDepth(float normalizedDepth)
	{
		normalizedDepth = Mathf.Clamp(normalizedDepth, 0f, 1f);
		if (normalizedDepth == 0f)
		{
			return Lines[0];
		}
		if (normalizedDepth == 1f)
		{
			return Lines[Lines.Count - 1];
		}
		foreach (GraphLine line in Lines)
		{
			if (normalizedDepth >= line.Position && normalizedDepth < line.Position + line.Length)
			{
				return line;
			}
		}
		Debug.LogError("GetLineAtDepth was unable to find a line at depth " + normalizedDepth + ". This shouldn't happen.");
		return null;
	}

	public DungeonArchetype[] GetUsedArchetypes()
	{
		return Lines.SelectMany((GraphLine x) => x.DungeonArchetypes).ToArray();
	}

	public TileSet[] GetUsedTileSets()
	{
		return Nodes.SelectMany((GraphNode x) => x.TileSets).Concat(Lines.SelectMany((GraphLine x) => x.DungeonArchetypes).SelectMany((DungeonArchetype y) => y.TileSets)).ToArray();
	}
}
