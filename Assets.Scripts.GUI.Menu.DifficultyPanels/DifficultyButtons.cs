using UnityEngine;

namespace Assets.Scripts.GUI.Menu.DifficultyPanels;

[RequireComponent(typeof(dfControl))]
internal class DifficultyButtons : ControlPanel
{
	[SerializeField]
	private dfButton easy;

	[SerializeField]
	private dfButton normal;

	[SerializeField]
	private dfButton hard;

	public dfButton Easy => easy;

	public dfButton Normal => normal;

	public dfButton Hard => hard;

	public override string SetDifficulty(EGameDifficulty difficulty)
	{
		string result = string.Empty;
		switch (difficulty)
		{
		case EGameDifficulty.EASY:
			Easy.TextColor = Color.red;
			Normal.TextColor = Color.black;
			Hard.TextColor = Color.black;
			result = "easy";
			break;
		case EGameDifficulty.NORMAL:
			Normal.TextColor = Color.red;
			Easy.TextColor = Color.black;
			Hard.TextColor = Color.black;
			result = "normal";
			break;
		case EGameDifficulty.HARD:
			Hard.TextColor = Color.red;
			Normal.TextColor = Color.black;
			Easy.TextColor = Color.black;
			result = "hard";
			break;
		}
		Easy.HoverTextColor = Easy.TextColor;
		Normal.HoverTextColor = Normal.TextColor;
		Hard.HoverTextColor = Hard.TextColor;
		return result;
	}
}
