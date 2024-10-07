using RG.Parsecs.Survival;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class RationAllController : MonoBehaviour
{
	[SerializeField]
	private RationingManager _rationingManager;

	[SerializeField]
	private TextMeshProUGUI _moreTextField;

	[SerializeField]
	private Image[] _icons;

	[SerializeField]
	private ConsumableRemedium _consumable;

	[SerializeField]
	private RationingToggle[] _consumableToggles;

	[SerializeField]
	private Button _rationAllButton;

	private const float CONSUMABLE_MIN_AMOUNT = 0.001f;

	private void SetCurrentFill(float amount)
	{
		float num = Mathf.Clamp(amount, 0f, _icons.Length);
		for (int i = 0; i < _icons.Length; i++)
		{
			if (num >= 1f)
			{
				_icons[i].fillAmount = 1f;
			}
			else
			{
				_icons[i].fillAmount = num;
			}
			num -= 1f;
		}
	}

	private void SetMoreTextField(float amount)
	{
		float num = amount - (float)_icons.Length;
		_moreTextField.text = ((num > 0f) ? $"+{num:G}" : " ");
	}

	private void SetRationingInteractable(bool interactable)
	{
		_rationAllButton.interactable = interactable;
		for (int i = 0; i < _consumableToggles.Length; i++)
		{
			_consumableToggles[i].Toggle.interactable = interactable;
		}
	}

	public void UpdateFill()
	{
		SetRationingInteractable(_consumable.RuntimeData.Amount > 0.001f);
		float num = _consumable.RuntimeData.Amount - _consumable.RuntimeData.PlannedConsumption;
		SetCurrentFill(num);
		SetMoreTextField(num);
	}

	public void RationAll()
	{
		bool toggleWithoutInvokingValueChange = false;
		if (!_rationingManager.AreAllEligibleCharactersRationed(_consumable) && _rationingManager.IsThereEnoughRations(_consumable))
		{
			_rationingManager.RationAllCharacters(_consumable);
			toggleWithoutInvokingValueChange = true;
		}
		else
		{
			_rationingManager.UnrationAllCharacters(_consumable);
		}
		for (int i = 0; i < _consumableToggles.Length; i++)
		{
			_consumableToggles[i].SetToggleWithoutInvokingValueChange(toggleWithoutInvokingValueChange);
		}
		UpdateFill();
	}
}
