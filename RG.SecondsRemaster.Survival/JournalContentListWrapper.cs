using System;
using System.Collections.Generic;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public struct JournalContentListWrapper
{
	public List<string> TextJournalContents;

	public List<string> TextIconJournalContents;

	public List<string> SpriteIconJournalContents;

	public List<string> YesNoChoiceJournalContents;

	public List<string> ItemChoiceJournalContents;

	public List<string> CharacterChoiceJournalContents;

	public List<string> SpriteChoiceJournalContents;

	public List<string> GoalJournalContents;
}
