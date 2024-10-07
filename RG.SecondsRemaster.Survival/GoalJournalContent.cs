using System;
using I2.Loc;
using RG.Core.SaveSystem;
using RG.Parsecs.GoalEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public sealed class GoalJournalContent : JournalContent
{
	[SerializeField]
	private LocalizedString _term;

	[SerializeField]
	private bool _isAchieved;

	[SerializeField]
	private bool _isFailed;

	[SerializeField]
	private int _checkmarkIndex;

	public LocalizedString Term => _term;

	public bool IsAchieved => _isAchieved;

	public bool IsFailed => _isFailed;

	public int CheckmarkIndex
	{
		get
		{
			return _checkmarkIndex;
		}
		set
		{
			_checkmarkIndex = value;
		}
	}

	public GoalJournalContent(string serializedData, SaveEvent saveEvent)
		: base(saveEvent)
	{
		Deserialize(serializedData, saveEvent);
	}

	public GoalJournalContent(Goal goal, bool isFailed, bool isAchieved, int displayPriority)
		: base(displayPriority)
	{
		type = EJournalContentType.GOAL;
		_term = goal.Name;
		_isAchieved = isAchieved;
		_isFailed = isFailed;
	}

	public override string Serialize()
	{
		return JsonUtility.ToJson(new GoalJournalContentWrapper
		{
			DisplayOrder = displayOrder,
			DisplayPriority = displayPriority,
			GroupId = ((groupId != null) ? groupId.Guid : string.Empty),
			Type = type,
			Term = _term,
			IsAchieved = _isAchieved,
			IsFailed = _isFailed,
			CheckmarkIndex = _checkmarkIndex
		});
	}

	public override void Deserialize(string data, SaveEvent saveEvent)
	{
		GoalJournalContentWrapper goalJournalContentWrapper = JsonUtility.FromJson<GoalJournalContentWrapper>(data);
		DeserializeBaseWrapper(goalJournalContentWrapper, saveEvent);
		_term = goalJournalContentWrapper.Term;
		_isAchieved = goalJournalContentWrapper.IsAchieved;
		_isFailed = goalJournalContentWrapper.IsFailed;
		_checkmarkIndex = goalJournalContentWrapper.CheckmarkIndex;
	}
}
