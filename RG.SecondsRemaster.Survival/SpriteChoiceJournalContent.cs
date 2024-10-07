using System;
using System.Collections.Generic;
using RG.Core.SaveSystem;
using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public sealed class SpriteChoiceJournalContent : JournalContent
{
	[SerializeField]
	private List<BaseActionCondition> _actionConditions;

	public List<BaseActionCondition> ActionConditions => _actionConditions;

	public SpriteChoiceJournalContent(List<BaseActionCondition> actionConditions)
		: base(int.MinValue)
	{
		type = EJournalContentType.CUSTOM_CHOICE;
		_actionConditions = actionConditions;
	}

	public SpriteChoiceJournalContent(string serializedData, SaveEvent saveEvent)
		: base(saveEvent)
	{
		Deserialize(serializedData, saveEvent);
	}

	public override string Serialize()
	{
		SpriteChoiceContentWrapper spriteChoiceContentWrapper = new SpriteChoiceContentWrapper
		{
			DisplayOrder = displayOrder,
			DisplayPriority = displayPriority,
			GroupId = ((groupId != null) ? groupId.Guid : string.Empty),
			Type = type,
			ActionConditions = new List<string>()
		};
		if (_actionConditions != null)
		{
			for (int i = 0; i < _actionConditions.Count; i++)
			{
				if (_actionConditions[i] != null)
				{
					spriteChoiceContentWrapper.ActionConditions.Add(_actionConditions[i].Guid);
				}
			}
		}
		return JsonUtility.ToJson(spriteChoiceContentWrapper);
	}

	public override void Deserialize(string data, SaveEvent saveEvent)
	{
		SpriteChoiceContentWrapper spriteChoiceContentWrapper = JsonUtility.FromJson<SpriteChoiceContentWrapper>(data);
		DeserializeBaseWrapper(spriteChoiceContentWrapper, saveEvent);
		_actionConditions = new List<BaseActionCondition>();
		for (int i = 0; i < spriteChoiceContentWrapper.ActionConditions.Count; i++)
		{
			if (!string.IsNullOrEmpty(spriteChoiceContentWrapper.ActionConditions[i]))
			{
				_actionConditions.Add((BaseActionCondition)saveEvent.GetReferenceObjectByID(spriteChoiceContentWrapper.ActionConditions[i]));
			}
		}
	}
}
