using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class ReportSubPageController : SubPageController
{
	[SerializeField]
	private GameObject _doodlesHolder;

	public void SetDoodlesHolder(GameObject doodlesHolder)
	{
		_doodlesHolder = doodlesHolder;
	}

	public override void Show()
	{
		base.Show();
		if (_doodlesHolder != null)
		{
			_doodlesHolder.SetActive(value: true);
		}
	}

	public override void Hide()
	{
		base.Hide();
		if (_doodlesHolder != null)
		{
			_doodlesHolder.SetActive(value: false);
		}
	}

	public override bool CanBeDisplayed()
	{
		return true;
	}

	private void FixLinkedTextsInDisplayer()
	{
		TextJournalContentDisplayer[] componentsInChildren = GetComponentsInChildren<TextJournalContentDisplayer>();
		foreach (TextJournalContentDisplayer textJournalContentDisplayer in componentsInChildren)
		{
			if (textJournalContentDisplayer != null)
			{
				textJournalContentDisplayer.TryToFixLinkedText();
			}
		}
	}
}
