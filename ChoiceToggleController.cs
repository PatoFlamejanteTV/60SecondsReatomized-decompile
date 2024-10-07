using RG.Parsecs.Common;
using RG.Parsecs.Survival;
using UnityEngine;

public class ChoiceToggleController : ToggleController
{
	[SerializeField]
	private ChoiceCardController _choiceCardController;

	[SerializeField]
	private ChoiceCardsController _choiceCardsController;

	[SerializeField]
	private OnUIClickedSoundPlayer _onUiClickedSoundPlayer;

	private void OnEnable()
	{
		RefreshToggle();
	}

	public override void OnToggleValueChangedAction(bool toggleValue)
	{
		_choiceCardsController.SetCurrentChoice(toggleValue ? _choiceCardController : null);
		_onUiClickedSoundPlayer.PlaySound();
	}

	public void RefreshToggle()
	{
		if (!base.Toggle.isOn)
		{
			_choiceCardController.RefreshCard();
		}
	}
}
