using System;
using System.Collections.Generic;
using RG.Core.SaveSystem;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public sealed class ItemChoiceJournalContent : JournalContent
{
	[SerializeField]
	private List<IItem> _items;

	public List<IItem> Items => _items;

	public ItemChoiceJournalContent(List<IItem> items)
		: base(int.MinValue)
	{
		type = EJournalContentType.ITEM_CHOICE;
		_items = items;
	}

	public ItemChoiceJournalContent(string serializedData, SaveEvent saveEvent)
		: base(saveEvent)
	{
		Deserialize(serializedData, saveEvent);
	}

	public override string Serialize()
	{
		ItemChoiceJournalContentWrapper itemChoiceJournalContentWrapper = new ItemChoiceJournalContentWrapper
		{
			DisplayOrder = displayOrder,
			DisplayPriority = displayPriority,
			GroupId = ((groupId != null) ? groupId.Guid : string.Empty),
			Type = type,
			Items = new List<string>()
		};
		if (_items != null)
		{
			for (int i = 0; i < _items.Count; i++)
			{
				if (_items[i] != null)
				{
					itemChoiceJournalContentWrapper.Items.Add(_items[i].Guid);
				}
			}
		}
		return JsonUtility.ToJson(itemChoiceJournalContentWrapper);
	}

	public override void Deserialize(string data, SaveEvent saveEvent)
	{
		ItemChoiceJournalContentWrapper itemChoiceJournalContentWrapper = JsonUtility.FromJson<ItemChoiceJournalContentWrapper>(data);
		DeserializeBaseWrapper(itemChoiceJournalContentWrapper, saveEvent);
		for (int i = 0; i < itemChoiceJournalContentWrapper.Items.Count; i++)
		{
			if (!string.IsNullOrEmpty(itemChoiceJournalContentWrapper.Items[i]))
			{
				_items.Add((IItem)saveEvent.GetReferenceObjectByID(itemChoiceJournalContentWrapper.Items[i]));
			}
		}
	}
}
