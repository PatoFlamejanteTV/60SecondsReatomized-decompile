using System;
using System.Collections.Generic;
using I2.Loc;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public class CharacterChoiceJournalContentWrapper : JournalContentWrapper
{
	public List<string> Characters;

	public LocalizedString CallToActionTerm;
}
