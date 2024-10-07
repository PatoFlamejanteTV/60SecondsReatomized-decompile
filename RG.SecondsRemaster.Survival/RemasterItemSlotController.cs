using System.Collections.Generic;
using Rewired;
using RG.Parsecs.Common;
using RG.Parsecs.Survival;
using RG.VirtualInput;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class RemasterItemSlotController : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	[SerializeField]
	private Button _button;

	[SerializeField]
	private Image _image;

	[SerializeField]
	private Sprite _handIcon;

	[SerializeField]
	private ItemList _itemList;

	[SerializeField]
	private ExpeditionData _expeditionData;

	[SerializeField]
	private RemasterItemsSlotsController _remasterItemsSlotsController;

	[SerializeField]
	private OnUIClickedSoundPlayer _onUiClickedSoundPlayer;

	private IItem _currentItem;

	private int _currentItemIndex;

	private const int NO_ITEM_INDEX = -1;

	private const string NEXT_ITEM_BUTTON = "NextItem";

	private const string PREVIOUS_ITEM_BUTTON = "PreviousItem";

	private Player _player;

	private VirtualInputButton _virtualInputButton;

	private RectTransform _rectTransform;

	[SerializeField]
	private Canvas _parentCanvas;

	public void Awake()
	{
		_player = ReInput.players.GetPlayer(0);
		_virtualInputButton = GetComponent<VirtualInputButton>();
		_rectTransform = GetComponent<RectTransform>();
	}

	public void Update()
	{
		if (_button.interactable)
		{
			if (_player.GetButtonDown("NextItem") && IsPointerOverThisItemSlot())
			{
				SetNextItem();
			}
			if (_player.GetButtonDown("PreviousItem") && IsPointerOverThisItemSlot())
			{
				SetPreviousItem();
			}
		}
	}

	public void SetButtonInteractable(bool interactable)
	{
		_button.interactable = interactable;
		if (!interactable && _currentItem != null)
		{
			SetCurrentItem(null);
		}
	}

	public void SetCurrentItem(IItem item)
	{
		UnlockCurrentItem();
		if (item == null)
		{
			_image.sprite = _handIcon;
			_currentItem = null;
			_currentItemIndex = -1;
			return;
		}
		_image.sprite = item.BaseStaticData.IconJournal;
		_currentItem = item;
		_currentItem.Lock();
		_remasterItemsSlotsController.AddItemToExpeditionItems(item);
		if (_itemList.Items.Contains(item))
		{
			_currentItemIndex = _itemList.Items.IndexOf(item);
		}
		else
		{
			_currentItemIndex = 0;
		}
	}

	public void SetNextItem()
	{
		HandHintController.ShouldShowHint = false;
		if (_onUiClickedSoundPlayer != null)
		{
			_onUiClickedSoundPlayer.PlaySound();
		}
		UnlockCurrentItem();
		List<IItem> items = _itemList.Items;
		int num = ((_currentItemIndex != -1) ? (_currentItemIndex + 1) : 0);
		for (int i = num; i < items.Count; i++)
		{
			if (!items[i].IsDamaged() && items[i].BaseRuntimeData.IsAvailable && !_expeditionData.RuntimeData.PlannedExpeditionData.ExpeditionItems.Contains(items[i]) && items[i].IsLockable())
			{
				SetCurrentItem(items[i]);
				return;
			}
		}
		if (_currentItem != null)
		{
			SetCurrentItem(null);
		}
	}

	private void SetPreviousItem()
	{
		HandHintController.ShouldShowHint = false;
		if (_onUiClickedSoundPlayer != null)
		{
			_onUiClickedSoundPlayer.PlaySound();
		}
		UnlockCurrentItem();
		List<IItem> items = _itemList.Items;
		int num = ((_currentItemIndex != -1) ? (_currentItemIndex - 1) : (items.Count - 1));
		for (int num2 = num; num2 >= 0; num2--)
		{
			if (!items[num2].IsDamaged() && items[num2].BaseRuntimeData.IsAvailable && !_expeditionData.RuntimeData.PlannedExpeditionData.ExpeditionItems.Contains(items[num2]) && items[num2].IsLockable())
			{
				SetCurrentItem(items[num2]);
				return;
			}
		}
		if (_currentItem != null)
		{
			SetCurrentItem(null);
		}
	}

	private void UnlockCurrentItem()
	{
		if (_currentItem != null)
		{
			_remasterItemsSlotsController.RemoveItemFromExpeditionItems(_currentItem);
			_currentItem.Unlock();
		}
	}

	private void OnMouseUpAsButton()
	{
		if (_button.interactable)
		{
			SetPreviousItem();
		}
	}

	public void GamepadNextItem()
	{
		if (_player.controllers.GetLastActiveController().type == ControllerType.Joystick)
		{
			SetNextItem();
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (_button.interactable)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				SetPreviousItem();
			}
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				SetNextItem();
			}
		}
	}

	private bool IsPointerOverThisItemSlot()
	{
		Vector2 vector = _rectTransform.sizeDelta * _parentCanvas.scaleFactor;
		Vector3 mousePosition = Singleton<VirtualInputManager>.Instance.GetMousePosition();
		if (Mathf.Abs(mousePosition.x - _rectTransform.position.x) < vector.x / 2f && Mathf.Abs(mousePosition.y - _rectTransform.position.y) < vector.y / 2f)
		{
			return true;
		}
		return false;
	}
}
