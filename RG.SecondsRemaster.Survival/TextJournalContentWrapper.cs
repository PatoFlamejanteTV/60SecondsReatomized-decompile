using System;
using System.Collections.Generic;
using I2.Loc;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public class TextJournalContentWrapper : JournalContentWrapper
{
	public string PureText;

	public LocalizedString Term;

	public List<string> Characters;

	public string ExpeditionCharacter;

	public List<string> Items;

	public List<int> LocalVariablesInts;

	public List<LocalizedString> Terms;
}
