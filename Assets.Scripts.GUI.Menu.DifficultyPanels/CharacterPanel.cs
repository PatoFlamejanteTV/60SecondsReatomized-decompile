using UnityEngine;

namespace Assets.Scripts.GUI.Menu.DifficultyPanels;

[RequireComponent(typeof(dfControl))]
internal class CharacterPanel : MonoBehaviour
{
	[SerializeField]
	private dfControl control;

	[SerializeField]
	private dfButton nextButton;

	[SerializeField]
	private dfButton prevButton;

	[SerializeField]
	private dfSprite charIcon;

	public bool IsVisible
	{
		get
		{
			return control.IsVisible;
		}
		set
		{
			control.IsVisible = value;
		}
	}

	public void SetCharacter(ECharacter character)
	{
		switch (character)
		{
		case ECharacter.DAD:
			charIcon.SpriteName = "icon_dad";
			break;
		case ECharacter.MOM:
			charIcon.SpriteName = "icon_mom";
			break;
		}
	}
}
