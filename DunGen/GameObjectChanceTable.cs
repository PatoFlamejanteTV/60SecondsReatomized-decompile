using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DunGen;

[Serializable]
public class GameObjectChanceTable
{
	public List<GameObjectChance> Weights = new List<GameObjectChance>();

	public GameObjectChanceTable Clone()
	{
		GameObjectChanceTable gameObjectChanceTable = new GameObjectChanceTable();
		foreach (GameObjectChance weight in Weights)
		{
			gameObjectChanceTable.Weights.Add(new GameObjectChance(weight.Value, weight.MainPathWeight, weight.BranchPathWeight)
			{
				UseDepthScale = weight.UseDepthScale,
				DepthWeightScale = weight.DepthWeightScale
			});
		}
		return gameObjectChanceTable;
	}

	public GameObject GetRandom(System.Random random, bool isOnMainPath, float normalizedDepth, bool removeFromTable = false)
	{
		float num = Weights.Select((GameObjectChance x) => x.GetWeight(isOnMainPath, normalizedDepth)).Sum();
		float num2 = (float)(random.NextDouble() * (double)num);
		foreach (GameObjectChance weight2 in Weights)
		{
			float weight = weight2.GetWeight(isOnMainPath, normalizedDepth);
			if (num2 < weight)
			{
				if (removeFromTable)
				{
					Weights.Remove(weight2);
				}
				return weight2.Value;
			}
			num2 -= weight;
		}
		return null;
	}

	public static GameObject GetCombinedRandom(System.Random random, bool isOnMainPath, float normalizedDepth, params GameObjectChanceTable[] tables)
	{
		float num = tables.SelectMany((GameObjectChanceTable x) => x.Weights.Select((GameObjectChance y) => y.GetWeight(isOnMainPath, normalizedDepth))).Sum();
		float num2 = (float)(random.NextDouble() * (double)num);
		foreach (GameObjectChance item in tables.SelectMany((GameObjectChanceTable x) => x.Weights))
		{
			float weight = item.GetWeight(isOnMainPath, normalizedDepth);
			if (num2 < weight)
			{
				return item.Value;
			}
			num2 -= weight;
		}
		return null;
	}
}
