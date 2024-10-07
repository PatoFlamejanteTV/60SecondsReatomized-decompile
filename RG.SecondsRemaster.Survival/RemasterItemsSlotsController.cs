using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class RemasterItemsSlotsController : MonoBehaviour
{
	[SerializeField]
	private RemasterItemSlotController[] _remasterItemSlotControllers;

	[SerializeField]
	private ItemList _itemList;

	[SerializeField]
	private ExpeditionData _expeditionData;

	[SerializeField]
	private IItem _suitcase;

	[SerializeField]
	private SendExpeditionCharacterToggle[] _sendExpeditionCharacterToggles;

	[SerializeField]
	private CharacterStatus _specialCharacterStatus;

	public void SetHandButtonsDefaultState()
	{
		if (_remasterItemSlotControllers != null)
		{
			bool flag = false;
			for (int i = 0; i < _sendExpeditionCharacterToggles.Length; i++)
			{
				flag |= _sendExpeditionCharacterToggles[i].Toggle.isOn;
			}
			_remasterItemSlotControllers[0].SetButtonInteractable(flag);
			SetAdditionalItemSlotsInteractable(interactable: false);
			for (int j = 0; j < _remasterItemSlotControllers.Length; j++)
			{
				_remasterItemSlotControllers[j].SetCurrentItem(null);
			}
		}
	}

	public void RemoveItemFromExpeditionItems(IItem item)
	{
		_expeditionData.RuntimeData.PlannedExpeditionData.ExpeditionItems.Remove(item);
		if (item == _suitcase)
		{
			SetAdditionalItemSlotsInteractable(interactable: false);
		}
	}

	public void AddItemToExpeditionItems(IItem item)
	{
		_expeditionData.RuntimeData.PlannedExpeditionData.ExpeditionItems.Add(item);
		if (item == _suitcase)
		{
			SetAdditionalItemSlotsInteractable(interactable: true);
		}
	}

	private void SetAdditionalItemSlotsInteractable(bool interactable)
	{
		for (int i = 1; i < _remasterItemSlotControllers.Length; i++)
		{
			_remasterItemSlotControllers[i].SetButtonInteractable(interactable);
		}
	}

	public void SetHandsInteractable()
	{
		if (_remasterItemSlotControllers == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < _sendExpeditionCharacterToggles.Length; i++)
		{
			if (!_sendExpeditionCharacterToggles[i].Character.RuntimeData.CurrentStatuses.Contains(_specialCharacterStatus))
			{
				flag |= _sendExpeditionCharacterToggles[i].Toggle.isOn;
			}
		}
		_remasterItemSlotControllers[0].SetButtonInteractable(flag);
	}
}
