using UnityEngine;

namespace Assets.Scripts.GUI.Menu.DifficultyPanels;

internal class MainDifficultyPanel : DifficultyPanel
{
	[SerializeField]
	private dfPanel _difficultyInfoPanelSurvival;

	[SerializeField]
	private dfPanel _challengeDataPanelScavenge;

	[SerializeField]
	private dfPanel _challengeDataPanelSurvival;

	[SerializeField]
	private DifficultyButtons _difficultyButtons;

	[SerializeField]
	private DifficultyArrows _arrowsDifficulty;

	public override string SetDifficulty(EGameDifficulty difficulty)
	{
		if (_arrowsDifficulty != null)
		{
			return _arrowsDifficulty.SetDifficulty(difficulty);
		}
		if (_difficultyButtons != null)
		{
			return _difficultyButtons.SetDifficulty(difficulty);
		}
		return null;
	}

	public override void SetGameType(EGameType type)
	{
		bool flag2 = (_difficultyInfoPanelSurvival.IsVisible = type == EGameType.SURVIVAL);
		bool isVisible = flag2;
		if (_arrowsDifficulty != null)
		{
			_arrowsDifficulty.IsVisible = isVisible;
		}
		else if (_difficultyButtons != null)
		{
			_difficultyButtons.IsVisible = isVisible;
		}
		_challengeDataPanelScavenge.IsVisible = type == EGameType.CHALLENGE_SCAVENGE;
		_challengeDataPanelSurvival.IsVisible = type == EGameType.CHALLENGE_SURVIVAL;
	}

	public void LoadDifficulties()
	{
		if (_arrowsDifficulty != null)
		{
			_arrowsDifficulty.Easy.Tag = EGameDifficulty.EASY;
			_arrowsDifficulty.Normal.Tag = EGameDifficulty.NORMAL;
			_arrowsDifficulty.Hard.Tag = EGameDifficulty.HARD;
		}
		else if (_difficultyButtons != null)
		{
			_difficultyButtons.Easy.Tag = EGameDifficulty.EASY;
			_difficultyButtons.Normal.Tag = EGameDifficulty.NORMAL;
			_difficultyButtons.Hard.Tag = EGameDifficulty.HARD;
		}
	}

	public void SetButtonsTextScale(float diffButtonScale)
	{
		dfButton easy = _difficultyButtons.Easy;
		dfButton normal = _difficultyButtons.Normal;
		float num2 = (_difficultyButtons.Hard.TextScale = diffButtonScale);
		float textScale = (normal.TextScale = num2);
		easy.TextScale = textScale;
	}
}
