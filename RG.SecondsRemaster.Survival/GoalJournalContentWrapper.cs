using System;
using I2.Loc;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public class GoalJournalContentWrapper : JournalContentWrapper
{
	public LocalizedString Term;

	public bool IsAchieved;

	public bool IsFailed;

	public int CheckmarkIndex;
}
