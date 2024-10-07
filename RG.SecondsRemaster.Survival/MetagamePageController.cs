using RG.Parsecs.EventEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class MetagamePageController : PageController
{
	[SerializeField]
	private Button _nextPageButton;

	[SerializeField]
	private Button _previousPageButton;

	[SerializeField]
	private Button _nextPageButtonEndDay;

	[SerializeField]
	private GlobalBoolVariable _attentionVariable;

	private bool _endDayButtonStateWhenHidden;

	private bool _nextButtonStateWhenHidden;

	private bool _previousButtonStateWhenHidden;

	public override void SetPageData(bool visible)
	{
		if (visible && _attentionVariable != null && _attentionVariable.Value)
		{
			_attentionVariable.Value = false;
		}
		if (visible)
		{
			_endDayButtonStateWhenHidden = _nextPageButtonEndDay.interactable;
			_nextButtonStateWhenHidden = _nextPageButton.interactable;
			_previousButtonStateWhenHidden = _previousPageButton.interactable;
			_nextPageButton.interactable = false;
			_nextPageButtonEndDay.interactable = false;
			_previousPageButton.interactable = false;
		}
		else
		{
			_nextPageButton.interactable = _nextButtonStateWhenHidden;
			_nextPageButtonEndDay.interactable = _endDayButtonStateWhenHidden;
			_previousPageButton.interactable = _previousButtonStateWhenHidden;
		}
	}

	public override bool CanBeDisplayed()
	{
		return true;
	}
}
