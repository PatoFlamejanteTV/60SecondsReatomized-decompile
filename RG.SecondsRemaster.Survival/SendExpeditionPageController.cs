using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class SendExpeditionPageController : PageController
{
	[SerializeField]
	private GlobalBoolVariable _shouldDisplaySendExpeditionPageVariable;

	[SerializeField]
	private ExpeditionData _expeditionData;

	[SerializeField]
	private SurvivalData _survivalData;

	[SerializeField]
	private CharacterAvailabilityController[] _characterAvailabilityControllers;

	[SerializeField]
	private SendExpeditionCharacterToggle[] _sendExpeditionCharacterToggles;

	[SerializeField]
	private ExpeditionDestinationList _demoExpeditionDestinationList;

	[SerializeField]
	private ExpeditionDestinationList _expeditionDestinationList;

	[SerializeField]
	private RemasterItemsSlotsController _remasterItemsSlotsController;

	[SerializeField]
	private GlobalBoolVariable _isTutorialVariable;

	[SerializeField]
	private FunctionTextController _reassuringDescription;

	[SerializeField]
	private GlobalBoolVariable _attentionVariable;

	[SerializeField]
	private GlobalBoolVariable _isTutorialExpedition;

	[SerializeField]
	private GameObject _handsContainer;

	public override void SetPageData(bool visible)
	{
		if (visible && _attentionVariable != null && _attentionVariable.Value)
		{
			_attentionVariable.Value = false;
		}
		if (_isTutorialExpedition.Value)
		{
			_handsContainer.SetActive(value: false);
		}
		else
		{
			_handsContainer.SetActive(value: true);
		}
		if (CanRefreshPageToday())
		{
			SetPageNotRefreshableToday();
			_shouldDisplaySendExpeditionPageVariable.SetValue(value: false);
			for (int i = 0; i < _characterAvailabilityControllers.Length; i++)
			{
				_characterAvailabilityControllers[i].RefreshCharacterAvailability();
			}
			for (int j = 0; j < _sendExpeditionCharacterToggles.Length; j++)
			{
				_sendExpeditionCharacterToggles[j].SetToggleWithoutInvokingValueChange(value: false);
			}
			_remasterItemsSlotsController.SetHandButtonsDefaultState();
			_reassuringDescription.RefreshText();
		}
	}

	public void SetExpedtionCharacter(Character character)
	{
		_expeditionData.RuntimeData.PlannedExpeditionData.ExpeditionCharacter = character;
	}

	public void SetExpeditionItems(IItem[] items)
	{
		_expeditionData.RuntimeData.PlannedExpeditionData.ExpeditionItems.Clear();
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i] != null)
			{
				_expeditionData.RuntimeData.PlannedExpeditionData.ExpeditionItems.Add(items[i]);
			}
		}
	}

	public override void OnPageSwitched()
	{
		base.OnPageSwitched();
		_expeditionData.RuntimeData.IsPlanned = _expeditionData.RuntimeData.PlannedExpeditionData.ExpeditionCharacter != null;
		if (DemoManager.IS_DEMO_VERSION && _survivalData.CurrentDay < 11)
		{
			_expeditionData.RuntimeData.PlannedExpeditionData.ChosenDestination = (_expeditionData.RuntimeData.IsPlanned ? _demoExpeditionDestinationList.GetRandomDestination(_expeditionData.RuntimeData.PlannedExpeditionData.ExpeditionCharacter, _isTutorialVariable.Value) : null);
		}
		else
		{
			_expeditionData.RuntimeData.PlannedExpeditionData.ChosenDestination = (_expeditionData.RuntimeData.IsPlanned ? _expeditionDestinationList.GetRandomDestination(_expeditionData.RuntimeData.PlannedExpeditionData.ExpeditionCharacter, _isTutorialVariable.Value) : null);
		}
	}

	public override bool CanBeDisplayed()
	{
		if (base.CanBeDisplayed() && _shouldDisplaySendExpeditionPageVariable.Value && _expeditionData.RuntimeData.IsActive)
		{
			return !_expeditionData.RuntimeData.IsOngoing;
		}
		return false;
	}

	public override void InitializePage()
	{
		base.InitializePage();
		if (IsEnabled())
		{
			_attentionVariable.Value = true;
		}
	}
}
