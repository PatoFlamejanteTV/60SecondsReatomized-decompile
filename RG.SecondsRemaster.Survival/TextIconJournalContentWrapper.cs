using System;
using I2.Loc;
using RG.Parsecs.Survival;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public class TextIconJournalContentWrapper : JournalContentWrapper
{
	public string Sign;

	public LocalizedString IconTerm;

	public EventContentData.ETextIconContentType IconType;

	public int Amount;
}
