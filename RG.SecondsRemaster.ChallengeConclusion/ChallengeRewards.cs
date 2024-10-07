using System;
using System.Collections.Generic;
using RG.Core.Base;
using RG.SecondsRemaster.Menu;
using UnityEngine;

namespace RG.SecondsRemaster.ChallengeConclusion;

[Serializable]
[CreateAssetMenu(menuName = "60 Seconds Remaster!/New Challenge Rewards", fileName = "New Challenge Rewards")]
public class ChallengeRewards : RGScriptableObject
{
	[SerializeField]
	private List<RewardItem> _rewards;

	public List<RewardItem> Rewards => _rewards;
}
