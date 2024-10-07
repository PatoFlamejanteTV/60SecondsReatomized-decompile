using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster;
using UnityEngine;

namespace RG.Remaster.Survival;

public class EndGameStats : MonoBehaviour
{
	[SerializeField]
	private EndGameData _endGameData;

	[SerializeField]
	private SurvivalData _survivalData;

	[SerializeField]
	private GlobalIntVariable _survivalGamesSurvivedVariable;

	[SerializeField]
	private GlobalIntVariable _survivalGamesLostVariable;

	[SerializeField]
	private GlobalIntVariable _totalDaysSurvivedVariable;

	[SerializeField]
	private GlobalIntVariable _longestSurvivalVariable;

	[SerializeField]
	private GlobalFloatVariable _winRatioVariable;

	[SerializeField]
	private GlobalIntVariable _averageSurvivalTimeVariable;

	[SerializeField]
	private GlobalIntVariable _totalWinDayCount;

	[SerializeField]
	private CurrentChallengeData _currentChallengeData;

	[SerializeField]
	private GlobalBoolVariable _isTutorial;

	[SerializeField]
	private GlobalBoolVariable _isScavengeOnly;

	private void Start()
	{
		if (_endGameData.RuntimeData.ShouldEndGame && !_isTutorial.Value && _currentChallengeData.RuntimeData.Challenge == null && !_isScavengeOnly.Value)
		{
			if (_longestSurvivalVariable.Value < _survivalData.CurrentDay)
			{
				_longestSurvivalVariable.SetValue(_survivalData.CurrentDay);
			}
			_totalDaysSurvivedVariable.SetValue(_totalDaysSurvivedVariable.Value + _survivalData.CurrentDay);
			if (_endGameData.RuntimeData.IsGameWon)
			{
				_survivalGamesSurvivedVariable.SetValue(_survivalGamesSurvivedVariable.Value + 1);
				_totalWinDayCount.SetValue(_totalWinDayCount.Value + _survivalData.CurrentDay);
				_averageSurvivalTimeVariable.SetValue(_totalWinDayCount.Value / _survivalGamesSurvivedVariable.Value);
			}
			else
			{
				_survivalGamesLostVariable.SetValue(_survivalGamesLostVariable.Value + 1);
			}
			if (_survivalGamesSurvivedVariable.Value + _survivalGamesLostVariable.Value > 0)
			{
				_winRatioVariable.SetValue(_survivalGamesSurvivedVariable.Value * 100 / (_survivalGamesSurvivedVariable.Value + _survivalGamesLostVariable.Value));
			}
			else
			{
				_winRatioVariable.SetValue(0f);
			}
		}
	}
}
