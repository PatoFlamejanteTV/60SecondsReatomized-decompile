using I2.Loc;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using RG.Remaster.Common;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class JournalDayController : MonoBehaviour
{
	[SerializeField]
	private LocalizedString _dayTerm;

	[SerializeField]
	private LocalizedString _endGameTerm;

	[SerializeField]
	private TextMeshProUGUI _dayTextField;

	[SerializeField]
	private SurvivalData _survivalData;

	[SerializeField]
	private EndGameData _endGameData;

	[SerializeField]
	private GlobalBoolVariable _survivalChallenge;

	private void Awake()
	{
		ShowCurrentDay();
	}

	private void ShowCurrentDay()
	{
		if ((string)_dayTerm != null && !string.IsNullOrEmpty(_dayTerm.mTerm))
		{
			_dayTextField.text = string.Format(_dayTerm, _survivalData.DisplayDay);
			RG.Remaster.Common.RichPresenceManager richPresenceManager = Singleton<PlatformManager>.Instance.RichPresenceManager;
			if ((_survivalChallenge != null && _survivalChallenge.Value) || richPresenceManager.CurrentRichPresenceStatus == ERichPresenceStatus.CHALLENGE_SURVIVAL_INTRO)
			{
				richPresenceManager.SetParametrizedRichPresence(ERichPresenceStatus.CHALLENGE_SURVIVAL, ERichPresenceParameter.SURVIVAL_DAY, _survivalData.DisplayDay.ToString());
			}
			else
			{
				richPresenceManager.SetParametrizedRichPresence(ERichPresenceStatus.SURVIVAL, ERichPresenceParameter.SURVIVAL_DAY, _survivalData.DisplayDay.ToString());
			}
		}
	}

	public void SetCurrentDayText()
	{
		if (_endGameData.RuntimeData.ShouldEndGame && (string)_dayTerm != null && !string.IsNullOrEmpty(_endGameTerm))
		{
			_dayTextField.text = _endGameTerm;
		}
		else
		{
			ShowCurrentDay();
		}
	}
}
