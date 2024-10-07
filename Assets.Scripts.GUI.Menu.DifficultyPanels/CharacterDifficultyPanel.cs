using UnityEngine;

namespace Assets.Scripts.GUI.Menu.DifficultyPanels;

internal class CharacterDifficultyPanel : DifficultyPanel
{
	[SerializeField]
	private DifficultyArrows _arrowsDifficulty;

	[SerializeField]
	private CharacterPanel _characterPanel;

	[SerializeField]
	private dfPanel _difficultyInfoPanelFull;

	[SerializeField]
	private dfPanel _difficultyInfoPanelScavenge;

	public override string SetDifficulty(EGameDifficulty difficulty)
	{
		return _arrowsDifficulty.SetDifficulty(difficulty);
	}

	public override void SetGameType(EGameType type)
	{
		_difficultyInfoPanelFull.IsVisible = type == EGameType.FULL;
		_difficultyInfoPanelScavenge.IsVisible = type == EGameType.SCAVENGE;
	}

	public void SetCharacter(ECharacter character)
	{
		_characterPanel.SetCharacter(character);
	}
}
