using System.Collections.Generic;
using RG.Core.Base;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/Events Renderer/New Journal Contents Displayers List", fileName = "Journal_Contents_Displayers")]
public class JournalContentsDisplayerList : RGScriptableObject
{
	[SerializeField]
	private List<JournalContentsDisplayerListEntry> _contentsDisplayers;

	public JournalContentDisplayer GetContentDisplayer(EJournalContentType contentType)
	{
		for (int i = 0; i < _contentsDisplayers.Count; i++)
		{
			if (_contentsDisplayers[i].Type == contentType)
			{
				return _contentsDisplayers[i].Displayer;
			}
		}
		return null;
	}
}
