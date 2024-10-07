using System.Collections.Generic;
using RG.Parsecs.Common;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Scavenge;

public class SurvivalTransferManager : MonoBehaviour
{
	[SerializeField]
	private CharacterList _characterList;

	[SerializeField]
	private ScavengeItemList _itemList;

	[SerializeField]
	private ScavengeItem _soup;

	[SerializeField]
	private ScavengeItem _water;

	private List<ScavengeItem> _currentItems;

	public ScavengeItemList ItemList => _itemList;

	private void Awake()
	{
		ResetItems();
	}

	public void ResetItems(List<ScavengeItem> excludeItems = null, int[] excludeAmount = null)
	{
		for (int i = 0; i < _itemList.Items.Count; i++)
		{
			if (excludeItems != null && excludeItems.Contains(_itemList.Items[i]))
			{
				if (excludeAmount == null)
				{
					_itemList.Items[i].ResetItem(onlyHoldedAmount: true);
					continue;
				}
				int num = excludeItems.IndexOf(_itemList.Items[i]);
				if (excludeAmount[num] != 0)
				{
					_itemList.Items[i].ResetItem(onlyHoldedAmount: false, excludeAmount[num]);
					excludeAmount[num] = 0;
				}
			}
			else
			{
				_itemList.Items[i].ResetItem();
			}
		}
	}

	public void TransferScavengedItems()
	{
		StatsManager instance = StatsManager.Instance;
		for (int i = 0; i < _itemList.Items.Count; i++)
		{
			ScavengeItem scavengeItem = _itemList.Items[i];
			if (scavengeItem == null || !scavengeItem.WasTaken)
			{
				continue;
			}
			if (scavengeItem.Character != null)
			{
				if (!_characterList.CharactersInGame.Contains(scavengeItem.Character))
				{
					_characterList.AddCharToList(scavengeItem.Character);
				}
			}
			else
			{
				if (!(scavengeItem.Item != null))
				{
					continue;
				}
				ConsumableRemedium consumableRemedium = scavengeItem.Item as ConsumableRemedium;
				instance.AddGlobalData("TotalItemsCollected", scavengeItem.Amount);
				if (consumableRemedium != null)
				{
					if (consumableRemedium.StaticData.ItemId.Equals("item_water"))
					{
						instance.AddGlobalData("ScavengedWater", scavengeItem.Amount);
					}
					else
					{
						instance.AddGlobalData("ScavengedSoups", scavengeItem.Amount);
					}
					consumableRemedium.RuntimeData.Amount = scavengeItem.Amount;
				}
				else
				{
					instance.AddGlobalDataToList("ScavengedItemsIDs", scavengeItem.Item.Guid);
					scavengeItem.Item.BaseRuntimeData.IsAvailable = true;
				}
			}
		}
	}

	public void TransferHeldItems()
	{
		for (int i = 0; i < _itemList.Items.Count; i++)
		{
			if (_itemList.Items[i].AmountHolded > 0)
			{
				_itemList.Items[i].TransferHoldedItems();
			}
		}
	}

	public List<ScavengeItem> GetCurrentInventory()
	{
		if (_currentItems == null)
		{
			_currentItems = new List<ScavengeItem>();
		}
		for (int i = 0; i < _itemList.Items.Count; i++)
		{
			ScavengeItem scavengeItem = _itemList.Items[i];
			if (scavengeItem.WasTaken && !_currentItems.Contains(scavengeItem))
			{
				_currentItems.Add(scavengeItem);
			}
		}
		return _currentItems;
	}

	public int GetCurrentItemsCount()
	{
		int num = 0;
		for (int i = 0; i < _itemList.Items.Count; i++)
		{
			ScavengeItem scavengeItem = _itemList.Items[i];
			if (scavengeItem.WasTaken)
			{
				num += scavengeItem.Amount;
			}
		}
		return num;
	}
}
