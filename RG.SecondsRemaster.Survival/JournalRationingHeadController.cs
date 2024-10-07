using System.Collections.Generic;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Core;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class JournalRationingHeadController : MonoBehaviour
{
	[SerializeField]
	private RationingManager _rationingManager;

	[SerializeField]
	private Selectable _headButton;

	[SerializeField]
	private GameObject _scratch;

	[SerializeField]
	private GameObject _medkitHolder;

	[SerializeField]
	private GameObject _waterHolder;

	[SerializeField]
	private GameObject _soupHolder;

	[SerializeField]
	private RationingToggle _waterToggle;

	[SerializeField]
	private RationingToggle _soupToggle;

	[SerializeField]
	private RationingToggle _medkitToggle;

	[SerializeField]
	private SecondsCharacter _character;

	[SerializeField]
	private SecondsRemedium _medkit;

	private int _characterIndex;

	private List<Character> _characters;

	private const int NO_CHARACTER_INDEX = -1;

	private void SetCharacterIndex()
	{
		if (_characters == null)
		{
			_characters = CharacterManager.Instance.GetCharacterList().CharactersInGame;
		}
		_characterIndex = (_characters.Contains(_character) ? _characters.IndexOf(_character) : (-1));
		_soupToggle.SetCharacterIndex(_characterIndex);
		_medkitToggle.SetCharacterIndex(_characterIndex);
		_waterToggle.SetCharacterIndex(_characterIndex);
	}

	public void SetScratchVisibility()
	{
		_soupToggle.SetScratchVisibility();
		_medkitToggle.UpdateRemediumScratchVisibility();
		_waterToggle.SetScratchVisibility();
	}

	public void UpdateMedkitScratch()
	{
		_medkitToggle.UpdateRemediumScratchVisibility();
	}

	public void UpdateHeadVisibility()
	{
		SetCharacterIndex();
		_headButton.interactable = _characterIndex != -1 && _character.RuntimeData.IsAlive() && _character.RuntimeData.IsDrawnOnShip();
		_scratch.SetActive(!_headButton.interactable);
		_medkitHolder.SetActive(_headButton.interactable && _rationingManager.CanItemBeRationedToCharacter(_medkit, _characterIndex) && _rationingManager.IsStatusNeededToRationItemToCharacter(_medkit, _characterIndex));
		_medkitHolder.GetComponentInChildren<Toggle>().interactable = _medkit.RuntimeData.IsAvailable && !_medkit.SecondsRemediumRuntimeData.IsDamaged;
		_waterHolder.SetActive(_headButton.interactable);
		_soupHolder.SetActive(_headButton.interactable);
	}

	public void RationWaterAndSoup()
	{
		bool isOn = true;
		if (_soupToggle.Toggle.isOn == _waterToggle.Toggle.isOn)
		{
			isOn = !_soupToggle.Toggle.isOn;
		}
		_soupToggle.Toggle.isOn = isOn;
		_waterToggle.Toggle.isOn = isOn;
	}

	public void ResetHead()
	{
		_soupToggle.SetToggleWithoutInvokingValueChange(value: false);
		_waterToggle.SetToggleWithoutInvokingValueChange(value: false);
		_medkitToggle.SetToggleWithoutInvokingValueChange(value: false);
	}
}
