using RG.Parsecs.Common;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class RationingToggle : ToggleController
{
	[SerializeField]
	private RationingManager _rationingManager;

	[SerializeField]
	private RationingData _rationingData;

	[SerializeField]
	private IItem _item;

	[SerializeField]
	private OnUIClickedSoundPlayer _onUiClickedSoundPlayer;

	[SerializeField]
	private GameObject _scratch;

	[SerializeField]
	private CharacterList _characterList;

	private int _characterIndex;

	public void SetCharacterIndex(int characterIndex)
	{
		_characterIndex = characterIndex;
	}

	public void SetScratchVisibility()
	{
		if (_scratch != null)
		{
			_scratch.SetActive(!_item.IsLockable() || !_item.BaseRuntimeData.IsAvailable);
		}
	}

	private bool IsItemRationedForAnyCharacter()
	{
		if (_scratch == null || _characterList == null || !(_item is Remedium))
		{
			return false;
		}
		bool flag = false;
		for (int i = 0; i < _characterList.CharactersInGame.Count; i++)
		{
			Character character = _characterList.CharactersInGame[i];
			flag |= _rationingManager.IsRemediumRationedToCharacter(character, (Remedium)_item);
		}
		return flag;
	}

	public void UpdateRemediumScratchVisibility()
	{
		if (_scratch == null)
		{
			return;
		}
		bool flag = false;
		if (_item.BaseRuntimeData.IsAvailable)
		{
			if (_item.IsLockable())
			{
				flag = true;
			}
			else if (IsItemRationedForAnyCharacter())
			{
				flag = true;
			}
		}
		_scratch.SetActive(!flag);
		base.Toggle.interactable = flag;
	}

	public override void OnToggleValueChangedAction(bool toggleValue)
	{
		if (_rationingManager.IsItemAvailableForRationing(_item) || _rationingData.IsItemRationedForCharacter(_characterIndex, _item.BaseStaticData.ItemId))
		{
			_rationingManager.CharacterRationed(_characterIndex, _item);
		}
		else
		{
			SetToggleWithoutInvokingValueChange(value: false);
		}
		_onUiClickedSoundPlayer.PlaySound();
	}
}
