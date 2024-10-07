using RG.Parsecs.Common;
using RG.Parsecs.Survival;
using RG.SecondsRemaster;
using UnityEngine;

namespace RG.Remaster.Common;

public class RemasterStatsManager : StatsManager
{
	[SerializeField]
	private CurrentChallengeData _challengeData;

	protected override bool CanAddExpeditionEntryToVariables()
	{
		if (_isTutorial != null && _challengeData != null && _expeditionsMadeVariable != null && _expeditionsSuccessfulVariable != null && !_isTutorial.Value)
		{
			return _challengeData.RuntimeData.Challenge == null;
		}
		return false;
	}

	protected override bool CanAddToGlobalStat()
	{
		Mission currentMission = MissionManager.Instance.GetCurrentMission();
		if (!_isTutorial.Value)
		{
			if (!(currentMission == null) && !(currentMission.ID == "ms_SurvivalMission"))
			{
				return currentMission.ID == "ms_ScavengeMission";
			}
			return true;
		}
		return false;
	}
}
