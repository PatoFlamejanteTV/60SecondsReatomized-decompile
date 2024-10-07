using System;
using System.Collections.Generic;
using UnityEngine;

namespace DunGen;

[Serializable]
public sealed class TileSet : ScriptableObject
{
	public GameObjectChanceTable TileWeights = new GameObjectChanceTable();

	public List<LockedDoorwayAssociation> LockPrefabs = new List<LockedDoorwayAssociation>();
}
