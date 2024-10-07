using System;
using RG.Core.SaveSystem;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public sealed class YesNoChoiceJournalContent : JournalContent
{
	public YesNoChoiceJournalContent()
		: base(int.MinValue)
	{
		type = EJournalContentType.YESNO_CHOICE;
	}

	public YesNoChoiceJournalContent(string serializedData, SaveEvent saveEvent)
		: base(saveEvent)
	{
		Deserialize(serializedData, saveEvent);
	}
}
