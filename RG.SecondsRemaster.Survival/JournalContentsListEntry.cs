using System;
using System.Collections.Generic;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public abstract class JournalContentsListEntry<TJournalContent, TContentDisplayer> where TJournalContent : JournalContent where TContentDisplayer : JournalContentDisplayer<TJournalContent>
{
	[SerializeField]
	private List<TJournalContent> _journalContents = new List<TJournalContent>();

	public List<TJournalContent> JournalContents => _journalContents;

	public void SetContentData(TJournalContent data, TContentDisplayer displayer)
	{
		displayer.SetContentData(data);
	}

	public void AddContentToList(TJournalContent journalContent)
	{
		if (_journalContents == null)
		{
			_journalContents = new List<TJournalContent>();
		}
		_journalContents.Add(journalContent);
	}

	public void ClearJournalContents()
	{
		if (_journalContents == null)
		{
			_journalContents = new List<TJournalContent>();
		}
		else
		{
			_journalContents.Clear();
		}
	}
}
