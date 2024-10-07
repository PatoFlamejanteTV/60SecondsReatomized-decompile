using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class PlanExpeditionToggleController : ToggleController
{
	[SerializeField]
	private GlobalBoolVariable _shouldDisplaySendExpeditionPage;

	[SerializeField]
	private OnUIClickedSoundPlayer _ouClickedSoundPlayer;

	public override void OnToggleValueChangedAction(bool toggleValue)
	{
		_shouldDisplaySendExpeditionPage.SetValue(toggleValue);
		_ouClickedSoundPlayer.PlaySound();
	}
}
