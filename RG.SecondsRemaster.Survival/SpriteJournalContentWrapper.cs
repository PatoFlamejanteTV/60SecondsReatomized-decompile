using System;
using RG.Parsecs.Survival;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public class SpriteJournalContentWrapper : JournalContentWrapper
{
	public string Sprite;

	public EventContentData.ESpriteAlign Align;
}
