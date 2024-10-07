using System;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public class JournalContentsDisplayerListEntry
{
	[SerializeField]
	private EJournalContentType _type;

	[SerializeField]
	private JournalContentDisplayer _displayer;

	public EJournalContentType Type => _type;

	public JournalContentDisplayer Displayer => _displayer;
}
