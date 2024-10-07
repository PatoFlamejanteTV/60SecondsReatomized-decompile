using UnityEngine;

namespace Assets.Scripts.GUI.Menu.DifficultyPanels;

internal class DifficultyArrows : ControlPanel
{
	[SerializeField]
	private dfLabel easy;

	[SerializeField]
	private dfLabel normal;

	[SerializeField]
	private dfLabel hard;

	[SerializeField]
	private dfButton leftButton;

	[SerializeField]
	private dfButton rightButton;

	public dfLabel Easy => easy;

	public dfLabel Normal => normal;

	public dfLabel Hard => hard;

	public override string SetDifficulty(EGameDifficulty difficulty)
	{
		string result = string.Empty;
		switch (difficulty)
		{
		case EGameDifficulty.EASY:
		{
			Easy.IsVisible = true;
			dfLabel dfLabel3 = Normal;
			bool isVisible = (Hard.IsVisible = false);
			dfLabel3.IsVisible = isVisible;
			result = "easy";
			break;
		}
		case EGameDifficulty.NORMAL:
		{
			Normal.IsVisible = true;
			dfLabel dfLabel2 = Easy;
			bool isVisible = (Hard.IsVisible = false);
			dfLabel2.IsVisible = isVisible;
			result = "normal";
			break;
		}
		case EGameDifficulty.HARD:
		{
			Hard.IsVisible = true;
			dfLabel dfLabel = Easy;
			bool isVisible = (Normal.IsVisible = false);
			dfLabel.IsVisible = isVisible;
			result = "hard";
			break;
		}
		}
		return result;
	}
}
