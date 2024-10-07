using System;
using System.Collections.Generic;

namespace DunGen.Graph;

[Serializable]
public class GraphLine
{
	public DungeonFlow Graph;

	public List<DungeonArchetype> DungeonArchetypes = new List<DungeonArchetype>();

	public float Position;

	public float Length;

	public List<KeyLockPlacement> Keys = new List<KeyLockPlacement>();

	public List<KeyLockPlacement> Locks = new List<KeyLockPlacement>();

	public GraphLine(DungeonFlow graph)
	{
		Graph = graph;
	}
}
