using RG.Parsecs.Common;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public abstract class JournalContentDisplayer<TJournalContent> : JournalContentDisplayer where TJournalContent : JournalContent
{
}
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(LayoutElement))]
public abstract class JournalContentDisplayer : MonoBehaviour
{
	[SerializeField]
	private TooltipController[] _tooltipControllers;

	public const int NO_LINES_VALUE = -1;

	private RectTransform _rectTransform;

	private LayoutElement _layoutElement;

	public virtual int LinesAmount => -1;

	public RectTransform RectTransform
	{
		get
		{
			if (_rectTransform == null)
			{
				_rectTransform = GetComponent<RectTransform>();
			}
			return _rectTransform;
		}
	}

	public LayoutElement LayoutElement
	{
		get
		{
			if (_layoutElement == null)
			{
				_layoutElement = GetComponent<LayoutElement>();
			}
			return _layoutElement;
		}
	}

	public abstract void SetContentData(JournalContent content);

	public void SetTooltipController(RectTransform tooltipTransform)
	{
		if (_tooltipControllers != null && !(tooltipTransform == null))
		{
			for (int i = 0; i < _tooltipControllers.Length; i++)
			{
				_tooltipControllers[i].SetTooltipRect(tooltipTransform);
			}
		}
	}
}
