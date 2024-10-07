using System;
using System.Collections.Generic;
using UnityEngine;

namespace DunGen;

[AddComponentMenu("DunGen/Random Props/Local Prop Set")]
public class LocalPropSet : RandomProp
{
	public GameObjectChanceTable Props = new GameObjectChanceTable();

	public IntRange PropCount = new IntRange(1, 1);

	public override void Process(System.Random randomStream, Tile tile)
	{
		GameObjectChanceTable gameObjectChanceTable = Props.Clone();
		int random = PropCount.GetRandom(randomStream);
		random = Mathf.Clamp(random, 0, Props.Weights.Count);
		List<GameObject> list = new List<GameObject>(random);
		for (int i = 0; i < random; i++)
		{
			list.Add(gameObjectChanceTable.GetRandom(randomStream, tile.Placement.IsOnMainPath, tile.Placement.NormalizedDepth, removeFromTable: true));
		}
		foreach (GameObjectChance weight in Props.Weights)
		{
			if (!list.Contains(weight.Value))
			{
				UnityEngine.Object.DestroyImmediate(weight.Value);
			}
		}
	}
}
