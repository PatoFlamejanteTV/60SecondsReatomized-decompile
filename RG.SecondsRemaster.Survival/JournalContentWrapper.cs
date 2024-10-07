using System;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public class JournalContentWrapper
{
	public EJournalContentType Type;

	public int DisplayPriority;

	public int DisplayOrder;

	public string GroupId;
}
