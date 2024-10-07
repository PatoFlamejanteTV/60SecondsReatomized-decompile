using RG.Parsecs.Common;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class SendExpeditionCharacterToggle : ToggleController
{
	[SerializeField]
	private Character _character;

	[SerializeField]
	private SendExpeditionPageController _sendExpeditionPageController;

	[SerializeField]
	private OnUIClickedSoundPlayer _onUiClickedSoundPlayer;

	[SerializeField]
	private RemasterItemsSlotsController _remasterItemsSlotsController;

	public Character Character => _character;

	public override void OnToggleValueChangedAction(bool toggleValue)
	{
		_sendExpeditionPageController.SetExpedtionCharacter(toggleValue ? _character : null);
		_onUiClickedSoundPlayer.PlaySound();
		_remasterItemsSlotsController.SetHandsInteractable();
	}

	public void SetToggleInteractable(bool interactable)
	{
		base.Toggle.interactable = interactable;
	}
}
