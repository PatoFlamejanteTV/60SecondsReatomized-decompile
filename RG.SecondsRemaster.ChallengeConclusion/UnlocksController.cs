using System;
using System.Collections.Generic;
using RG.Parsecs.Common;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.ChallengeConclusion;

public class UnlocksController : MonoBehaviour
{
	[Serializable]
	public struct RewardSceneRepresentation
	{
		public GameObject UnlockObject;

		public Image Image;
	}

	[Serializable]
	public struct ChallengeAchievementPair
	{
		public Challenge ChallengeToComplete;

		public Achievement AchievementToUnlock;
	}

	[SerializeField]
	private CurrentChallengeData _challengeRewards;

	[SerializeField]
	private Achievement _challengerAchievement;

	[SerializeField]
	private Achievement _rogueOneAchievement;

	[SerializeField]
	private List<ChallengeAchievementPair> _challengeAchievements;

	[SerializeField]
	private List<RewardSceneRepresentation> _challengeRewardRepresentations;

	public void Start()
	{
		if (IsSetupValid())
		{
			for (int i = 0; i < _challengeRewards.RuntimeData.Challenge.Rewards.Count; i++)
			{
				_challengeRewardRepresentations[i].Image.sprite = _challengeRewards.RuntimeData.Challenge.Rewards[i].ConclusionIcon;
				_challengeRewardRepresentations[i].UnlockObject.SetActive(value: true);
			}
		}
		if (IsAchievementSetupValid())
		{
			for (int j = 0; j < _challengeAchievements.Count; j++)
			{
				if (_challengeAchievements[j].ChallengeToComplete != null && _challengeAchievements[j].AchievementToUnlock != null && _challengeRewards.RuntimeData.Challenge.Equals(_challengeAchievements[j].ChallengeToComplete))
				{
					AchievementsSystem.UnlockAchievement(_challengeAchievements[j].AchievementToUnlock);
				}
			}
		}
		if (_challengerAchievement != null && !_challengerAchievement.IsAchieved && _challengeRewards.RuntimeData.Challenge.ChallengeType == Challenge.EChallengeType.SCAVENGE)
		{
			AchievementsSystem.UnlockAchievement(_challengerAchievement);
		}
		if (_rogueOneAchievement != null && !_rogueOneAchievement.IsAchieved && _challengeRewards.RuntimeData.Challenge.ChallengeType == Challenge.EChallengeType.SURVIVAL)
		{
			AchievementsSystem.UnlockAchievement(_rogueOneAchievement);
		}
	}

	private bool IsAchievementSetupValid()
	{
		if (_challengeRewardRepresentations != null)
		{
			return _challengeRewardRepresentations.Count > 0;
		}
		return false;
	}

	private bool IsSetupValid()
	{
		if (_challengeRewards != null && _challengeRewardRepresentations != null && _challengeRewards.RuntimeData.Challenge.Rewards != null)
		{
			return _challengeRewards.RuntimeData.Challenge.Rewards.Count <= _challengeRewardRepresentations.Count;
		}
		return false;
	}
}
