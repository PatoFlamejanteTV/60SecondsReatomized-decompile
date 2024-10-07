using RG.Core.Base;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Scavenge;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/Scavenge/Scavenge Item")]
public class ScavengeItem : RGScriptableObject
{
	[SerializeField]
	private IItem _item;

	[SerializeField]
	private Character _character;

	[SerializeField]
	private int _weight = 1;

	[SerializeField]
	private Sprite _icon;

	[SerializeField]
	private Sprite _menuIcon;

	private int _amount;

	private int _amountHolded;

	public IItem Item => _item;

	public Character Character => _character;

	public int Weight => _weight;

	public Sprite Icon => _icon;

	public Sprite MenuIcon => _menuIcon;

	public int Amount => _amount;

	public bool WasTaken => _amount > 0;

	public int AmountHolded => _amountHolded;

	private void OnEnable()
	{
		ResetItem();
	}

	public void SetItemAmount(int amount)
	{
		_amount = amount;
	}

	public void AddItem(int amount = 1)
	{
		_amount += amount;
	}

	public void AddHeldItem(int amount = 1)
	{
		_amountHolded += amount;
	}

	public void TransferHoldedItems()
	{
		AddItem(_amountHolded);
		_amountHolded = 0;
	}

	public void ResetItem(bool onlyHoldedAmount = false, int amountToLeave = 0)
	{
		if (!onlyHoldedAmount)
		{
			_amount = amountToLeave;
		}
		_amountHolded = 0;
	}
}
