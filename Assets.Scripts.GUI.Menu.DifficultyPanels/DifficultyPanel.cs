using UnityEngine;

namespace Assets.Scripts.GUI.Menu.DifficultyPanels;

internal abstract class DifficultyPanel : ControlPanel
{
	[SerializeField]
	private dfLabel _gameModeTitleLabel;

	[SerializeField]
	private dfSprite _gameModeImg;

	[SerializeField]
	private dfRichTextLabel _gameModeDescriptionLabel;

	public void SetTitleLabel(string text)
	{
		_gameModeTitleLabel.Text = text;
	}

	public void SetDescriptionLabel(string text)
	{
		_gameModeDescriptionLabel.Text = text;
	}

	public void SetImage(string sprite, dfAtlas atlas)
	{
		_gameModeImg.SpriteName = sprite;
		_gameModeImg.Atlas = atlas;
	}

	public abstract void SetGameType(EGameType type);
}
