using System;
using UnityEngine;

namespace DunGen;

[Serializable]
public sealed class GameObjectChance
{
	public GameObject Value;

	public float MainPathWeight;

	public float BranchPathWeight;

	public bool UseDepthScale;

	public AnimationCurve DepthWeightScale = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	public GameObjectChance()
		: this(null, 1f, 1f)
	{
	}

	public GameObjectChance(GameObject value)
		: this(value, 1f, 1f)
	{
	}

	public GameObjectChance(GameObject value, float mainPathWeight, float branchPathWeight)
	{
		Value = value;
		MainPathWeight = mainPathWeight;
		BranchPathWeight = branchPathWeight;
	}

	public float GetWeight(bool isOnMainPath, float normalizedDepth)
	{
		if (!isOnMainPath)
		{
			return BranchPathWeight;
		}
		return MainPathWeight;
	}
}
