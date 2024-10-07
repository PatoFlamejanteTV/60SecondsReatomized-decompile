using System.Collections.Generic;
using RG.Core.Base;
using RG.Core.SaveSystem;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/Events Renderer/New Journal Contents List", fileName = "Journal_Contents_List")]
public class JournalContentsList : RGScriptableObject, ISaveable
{
	[SerializeField]
	private SaveEvent _saveEvent;

	[SerializeField]
	private TextJournalContentsListEntry _textJournalContents;

	[SerializeField]
	private TextIconJournalContentsListEntry _textIconJournalContents;

	[SerializeField]
	private SpriteJournalContentsListEntry _spriteIconJournalContents;

	[SerializeField]
	private YesNoChoiceJournalContentsListEntry _yesNoChoiceJournalContents;

	[SerializeField]
	private ItemChoiceJournalContentsListEntry _itemChoiceJournalContents;

	[SerializeField]
	private CharacterChoiceJournalContentsListEntry _characterChoiceJournalContents;

	[SerializeField]
	private SpriteChoiceJournalContentsListEntry _spriteChoiceJournalContents;

	[SerializeField]
	private GoalJournalContentsListEntry _goalJournalContents;

	private Dictionary<TextJournalGroupId, GroupData> _groupsData;

	private int _currentDisplayOrder;

	public string ID => Guid;

	private void OnEnable()
	{
		Register();
	}

	private void OnDestroy()
	{
		Unregister();
	}

	public void ClearJournalContentsList()
	{
		_currentDisplayOrder = 0;
		if (_textJournalContents == null)
		{
			_textJournalContents = new TextJournalContentsListEntry();
		}
		if (_textIconJournalContents == null)
		{
			_textIconJournalContents = new TextIconJournalContentsListEntry();
		}
		if (_spriteIconJournalContents == null)
		{
			_spriteIconJournalContents = new SpriteJournalContentsListEntry();
		}
		if (_yesNoChoiceJournalContents == null)
		{
			_yesNoChoiceJournalContents = new YesNoChoiceJournalContentsListEntry();
		}
		if (_itemChoiceJournalContents == null)
		{
			_itemChoiceJournalContents = new ItemChoiceJournalContentsListEntry();
		}
		if (_characterChoiceJournalContents == null)
		{
			_characterChoiceJournalContents = new CharacterChoiceJournalContentsListEntry();
		}
		if (_spriteChoiceJournalContents == null)
		{
			_spriteChoiceJournalContents = new SpriteChoiceJournalContentsListEntry();
		}
		if (_goalJournalContents == null)
		{
			_goalJournalContents = new GoalJournalContentsListEntry();
		}
		_textJournalContents.ClearJournalContents();
		_textIconJournalContents.ClearJournalContents();
		_spriteIconJournalContents.ClearJournalContents();
		_yesNoChoiceJournalContents.ClearJournalContents();
		_itemChoiceJournalContents.ClearJournalContents();
		_characterChoiceJournalContents.ClearJournalContents();
		_spriteChoiceJournalContents.ClearJournalContents();
		_goalJournalContents.ClearJournalContents();
	}

	public List<JournalContent> GetSortedJournalContents()
	{
		List<JournalContent> list = AggregateAllContentsToOneList();
		SetGroupsData(list);
		SortJournalContents(list);
		if (list.Count > 0 && list[0].Type != 0)
		{
			for (int i = 1; i < list.Count; i++)
			{
				if (list[i].Type == EJournalContentType.TEXT)
				{
					JournalContent item = list[i];
					list.RemoveAt(i);
					list.Insert(0, item);
					break;
				}
			}
		}
		return list;
	}

	private List<JournalContent> AggregateAllContentsToOneList()
	{
		List<JournalContent> list = new List<JournalContent>();
		for (int i = 0; i < _textJournalContents.JournalContents.Count; i++)
		{
			list.Add(_textJournalContents.JournalContents[i]);
		}
		for (int j = 0; j < _textIconJournalContents.JournalContents.Count; j++)
		{
			list.Add(_textIconJournalContents.JournalContents[j]);
		}
		for (int k = 0; k < _spriteIconJournalContents.JournalContents.Count; k++)
		{
			list.Add(_spriteIconJournalContents.JournalContents[k]);
		}
		for (int l = 0; l < _yesNoChoiceJournalContents.JournalContents.Count; l++)
		{
			list.Add(_yesNoChoiceJournalContents.JournalContents[l]);
		}
		for (int m = 0; m < _itemChoiceJournalContents.JournalContents.Count; m++)
		{
			list.Add(_itemChoiceJournalContents.JournalContents[m]);
		}
		for (int n = 0; n < _characterChoiceJournalContents.JournalContents.Count; n++)
		{
			list.Add(_characterChoiceJournalContents.JournalContents[n]);
		}
		for (int num = 0; num < _spriteChoiceJournalContents.JournalContents.Count; num++)
		{
			list.Add(_spriteChoiceJournalContents.JournalContents[num]);
		}
		for (int num2 = 0; num2 < _goalJournalContents.JournalContents.Count; num2++)
		{
			list.Add(_goalJournalContents.JournalContents[num2]);
		}
		return list;
	}

	private void SetGroupsData(List<JournalContent> contents)
	{
		if (_groupsData == null)
		{
			_groupsData = new Dictionary<TextJournalGroupId, GroupData>();
		}
		else
		{
			_groupsData.Clear();
		}
		for (int i = 0; i < contents.Count; i++)
		{
			JournalContent journalContent = contents[i];
			if (!(journalContent.GroupId == null))
			{
				if (!_groupsData.ContainsKey(journalContent.GroupId))
				{
					_groupsData.Add(journalContent.GroupId, new GroupData(journalContent.DisplayOrder, journalContent.DisplayPriority));
					continue;
				}
				GroupData groupData = _groupsData[journalContent.GroupId];
				groupData.Priority = ((journalContent.DisplayPriority > groupData.Priority) ? journalContent.DisplayPriority : groupData.Priority);
				groupData.Order = ((journalContent.DisplayOrder < groupData.Order) ? journalContent.DisplayOrder : groupData.Order);
			}
		}
	}

	private GroupData GetGroupsData(TextJournalGroupId groupId)
	{
		if (_groupsData.ContainsKey(groupId))
		{
			return _groupsData[groupId];
		}
		return null;
	}

	private void SortJournalContents(List<JournalContent> journalContents)
	{
		journalContents.Sort(ContentsPriorityComparer);
	}

	private int ContentsPriorityComparer(JournalContent x, JournalContent y)
	{
		if (x.GroupId != null && y.GroupId != null && x.GroupId == y.GroupId)
		{
			if (y.DisplayPriority == x.DisplayPriority)
			{
				return x.DisplayOrder.CompareTo(y.DisplayOrder);
			}
			return y.DisplayPriority.CompareTo(x.DisplayPriority);
		}
		int num = ((x.GroupId != null) ? GetGroupsData(x.GroupId).Priority : x.DisplayPriority);
		int num2 = ((y.GroupId != null) ? GetGroupsData(y.GroupId).Priority : y.DisplayPriority);
		if (num2 == num)
		{
			int num3 = ((x.GroupId != null) ? GetGroupsData(x.GroupId).Order : x.DisplayOrder);
			int value = ((y.GroupId != null) ? GetGroupsData(y.GroupId).Order : y.DisplayOrder);
			return num3.CompareTo(value);
		}
		return num2.CompareTo(num);
	}

	public void AddJournalContent(JournalContent journalContent)
	{
		journalContent.DisplayOrder = _currentDisplayOrder;
		_currentDisplayOrder++;
		switch (journalContent.Type)
		{
		case EJournalContentType.TEXT:
			_textJournalContents.AddContentToList((TextJournalContent)journalContent);
			break;
		case EJournalContentType.TEXT_ICON:
			_textIconJournalContents.AddContentToList((TextIconJournalContent)journalContent);
			break;
		case EJournalContentType.SPRITE:
			_spriteIconJournalContents.AddContentToList((SpriteJournalContent)journalContent);
			break;
		case EJournalContentType.YESNO_CHOICE:
			_yesNoChoiceJournalContents.AddContentToList((YesNoChoiceJournalContent)journalContent);
			break;
		case EJournalContentType.ITEM_CHOICE:
			_itemChoiceJournalContents.AddContentToList((ItemChoiceJournalContent)journalContent);
			break;
		case EJournalContentType.CHARACTER_CHOICE:
			_characterChoiceJournalContents.AddContentToList((CharacterChoiceJournalContent)journalContent);
			break;
		case EJournalContentType.CUSTOM_CHOICE:
			_spriteChoiceJournalContents.AddContentToList((SpriteChoiceJournalContent)journalContent);
			break;
		case EJournalContentType.GOAL:
			_goalJournalContents.AddContentToList((GoalJournalContent)journalContent);
			break;
		default:
			Debug.LogWarning("JournalContentsList: Unknown content.");
			break;
		}
	}

	public string Serialize()
	{
		JournalContentListWrapper journalContentListWrapper = default(JournalContentListWrapper);
		journalContentListWrapper.TextJournalContents = new List<string>();
		journalContentListWrapper.TextIconJournalContents = new List<string>();
		journalContentListWrapper.SpriteIconJournalContents = new List<string>();
		journalContentListWrapper.YesNoChoiceJournalContents = new List<string>();
		journalContentListWrapper.ItemChoiceJournalContents = new List<string>();
		journalContentListWrapper.CharacterChoiceJournalContents = new List<string>();
		journalContentListWrapper.SpriteChoiceJournalContents = new List<string>();
		journalContentListWrapper.GoalJournalContents = new List<string>();
		for (int i = 0; i < _textJournalContents.JournalContents.Count; i++)
		{
			journalContentListWrapper.TextJournalContents.Add(_textJournalContents.JournalContents[i].Serialize());
		}
		for (int j = 0; j < _textIconJournalContents.JournalContents.Count; j++)
		{
			journalContentListWrapper.TextIconJournalContents.Add(_textIconJournalContents.JournalContents[j].Serialize());
		}
		for (int k = 0; k < _spriteIconJournalContents.JournalContents.Count; k++)
		{
			journalContentListWrapper.SpriteIconJournalContents.Add(_spriteIconJournalContents.JournalContents[k].Serialize());
		}
		for (int l = 0; l < _yesNoChoiceJournalContents.JournalContents.Count; l++)
		{
			journalContentListWrapper.YesNoChoiceJournalContents.Add(_yesNoChoiceJournalContents.JournalContents[l].Serialize());
		}
		for (int m = 0; m < _itemChoiceJournalContents.JournalContents.Count; m++)
		{
			journalContentListWrapper.ItemChoiceJournalContents.Add(_itemChoiceJournalContents.JournalContents[m].Serialize());
		}
		for (int n = 0; n < _characterChoiceJournalContents.JournalContents.Count; n++)
		{
			journalContentListWrapper.CharacterChoiceJournalContents.Add(_characterChoiceJournalContents.JournalContents[n].Serialize());
		}
		for (int num = 0; num < _spriteChoiceJournalContents.JournalContents.Count; num++)
		{
			journalContentListWrapper.SpriteChoiceJournalContents.Add(_spriteChoiceJournalContents.JournalContents[num].Serialize());
		}
		for (int num2 = 0; num2 < _goalJournalContents.JournalContents.Count; num2++)
		{
			journalContentListWrapper.GoalJournalContents.Add(_goalJournalContents.JournalContents[num2].Serialize());
		}
		return JsonUtility.ToJson(journalContentListWrapper);
	}

	public void Deserialize(string jsonData)
	{
		JournalContentListWrapper journalContentListWrapper = JsonUtility.FromJson<JournalContentListWrapper>(jsonData);
		ClearJournalContentsList();
		for (int i = 0; i < journalContentListWrapper.TextJournalContents.Count; i++)
		{
			_textJournalContents.AddContentToList(new TextJournalContent(journalContentListWrapper.TextJournalContents[i], _saveEvent));
		}
		for (int j = 0; j < journalContentListWrapper.TextIconJournalContents.Count; j++)
		{
			_textIconJournalContents.AddContentToList(new TextIconJournalContent(journalContentListWrapper.TextIconJournalContents[j], _saveEvent));
		}
		for (int k = 0; k < journalContentListWrapper.SpriteIconJournalContents.Count; k++)
		{
			_spriteIconJournalContents.AddContentToList(new SpriteJournalContent(journalContentListWrapper.SpriteIconJournalContents[k], _saveEvent));
		}
		for (int l = 0; l < journalContentListWrapper.YesNoChoiceJournalContents.Count; l++)
		{
			_yesNoChoiceJournalContents.AddContentToList(new YesNoChoiceJournalContent(journalContentListWrapper.YesNoChoiceJournalContents[l], _saveEvent));
		}
		for (int m = 0; m < journalContentListWrapper.ItemChoiceJournalContents.Count; m++)
		{
			_itemChoiceJournalContents.AddContentToList(new ItemChoiceJournalContent(journalContentListWrapper.ItemChoiceJournalContents[m], _saveEvent));
		}
		for (int n = 0; n < journalContentListWrapper.CharacterChoiceJournalContents.Count; n++)
		{
			_characterChoiceJournalContents.AddContentToList(new CharacterChoiceJournalContent(journalContentListWrapper.CharacterChoiceJournalContents[n], _saveEvent));
		}
		for (int num = 0; num < journalContentListWrapper.SpriteChoiceJournalContents.Count; num++)
		{
			_spriteChoiceJournalContents.AddContentToList(new SpriteChoiceJournalContent(journalContentListWrapper.SpriteChoiceJournalContents[num], _saveEvent));
		}
		for (int num2 = 0; num2 < journalContentListWrapper.GoalJournalContents.Count; num2++)
		{
			_goalJournalContents.AddContentToList(new GoalJournalContent(journalContentListWrapper.GoalJournalContents[num2], _saveEvent));
		}
	}

	public void Register()
	{
		if (_saveEvent != null)
		{
			_saveEvent.RegisterListener(this);
		}
	}

	public void Unregister()
	{
		if (_saveEvent != null)
		{
			_saveEvent.UnregisterListener(this);
		}
	}

	public void ResetData()
	{
		ClearJournalContentsList();
	}
}
