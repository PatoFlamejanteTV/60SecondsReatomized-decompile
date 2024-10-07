using System;
using System.Collections.Generic;
using UnityEngine;

namespace DunGen;

[Serializable]
public sealed class DungeonArchetype : ScriptableObject
{
	public List<TileSet> TileSets = new List<TileSet>();

	public IntRange BranchingDepth = new IntRange(2, 4);

	public IntRange BranchCount = new IntRange(0, 2);
}
