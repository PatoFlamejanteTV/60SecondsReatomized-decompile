using System;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class EventsRendererController : MonoBehaviour
{
	[SerializeField]
	private JournalContentsList _contents;

	[SerializeField]
	private JournalContentsDisplayerList _contentsDisplayers;

	[SerializeField]
	private PageController _pageController;

	[SerializeField]
	private SubPageController _subPagePrefab;

	[SerializeField]
	private RectTransform _textIconGroupPrefab;

	[SerializeField]
	private float _maxPageHeight;

	[SerializeField]
	private int _minimumLinesAmountEligibleForBreaking = 5;

	[SerializeField]
	private RectTransform _pageHolder;

	[SerializeField]
	private LinesDescription _linesDescriptions;

	[SerializeField]
	private RectTransform _actionChoiceTooltipTransform;

	private RectTransform _currentContentHolder;

	private float _currentPageHeight;

	private float _previousTextIconGroupHeight;

	private RectTransform _currentTextIconGroup;

	private List<List<RectTransform>> _rectTransforms;

	private int _currentPageIndex;

	private bool _choiceWasRendered;

	public bool CanShowDoodleOnLastPage(float maxSpaceTakenByText = 0.5f)
	{
		float num = 0f;
		num = ((!_choiceWasRendered) ? CalculateCurrentPageHeight() : CalculateCurrentPageHeightWithoutLastElement());
		return num < _maxPageHeight * maxSpaceTakenByText;
	}

	private float CalculateCurrentPageHeight()
	{
		if (_rectTransforms == null || _rectTransforms.Count == 0 || _rectTransforms[_currentPageIndex] == null)
		{
			return 0f;
		}
		Canvas.ForceUpdateCanvases();
		float num = 0f;
		for (int i = 0; i < _rectTransforms[_currentPageIndex].Count; i++)
		{
			num += _rectTransforms[_currentPageIndex][i].sizeDelta.y;
		}
		return num;
	}

	private float CalculateCurrentPageHeightWithoutLastElement()
	{
		if (_rectTransforms == null || _rectTransforms.Count == 0 || _rectTransforms[_currentPageIndex] == null)
		{
			return 0f;
		}
		Canvas.ForceUpdateCanvases();
		float num = 0f;
		for (int i = 0; i < _rectTransforms[_currentPageIndex].Count - 1; i++)
		{
			num += _rectTransforms[_currentPageIndex][i].sizeDelta.y;
		}
		return num;
	}

	private JournalContentDisplayer GroupTextIconContents(List<JournalContent> journalContents, int index)
	{
		if (journalContents[index].Type == EJournalContentType.TEXT_ICON && index - 1 >= 0)
		{
			int num = index - 1;
			while (num >= 0 && journalContents[num].Type == EJournalContentType.TEXT_ICON)
			{
				if (journalContents[num] is TextIconJournalContent textIconJournalContent)
				{
					TextIconJournalContentDisplayer textIconJournalContentDisplayer = textIconJournalContent.Displayer as TextIconJournalContentDisplayer;
					if (textIconJournalContentDisplayer != null && journalContents[index] is TextIconJournalContent iconContent && textIconJournalContentDisplayer.TryToJoinNextTextIconContent(iconContent))
					{
						return textIconJournalContentDisplayer;
					}
				}
				num--;
			}
		}
		return null;
	}

	private JournalContentDisplayer GroupTextContents(List<JournalContent> journalContents, int index)
	{
		if (index - 1 >= 0 && journalContents[index].GroupId != null && journalContents[index - 1].GroupId != null && journalContents[index].GroupId == journalContents[index - 1].GroupId && journalContents[index].Type == EJournalContentType.TEXT && journalContents[index - 1].Type == EJournalContentType.TEXT && journalContents[index - 1] is TextJournalContent textJournalContent)
		{
			TextJournalContentDisplayer textJournalContentDisplayer = textJournalContent.Displayer as TextJournalContentDisplayer;
			if (textJournalContentDisplayer != null && journalContents[index] is TextJournalContent textJournalContent2)
			{
				textJournalContent2.Displayer = textJournalContent.Displayer;
				textJournalContentDisplayer.AppendText(textJournalContent2, Environment.NewLine);
				textJournalContentDisplayer.GetComponent<LayoutElement>().CalculateLayoutInputVertical();
				textJournalContentDisplayer.GetComponent<LayoutElement>().preferredHeight += _linesDescriptions.GetLinesDescriptionForCurrentLanguage().TextMargin;
				return textJournalContentDisplayer;
			}
		}
		return null;
	}

	private JournalContentDisplayer SpawnNewContentDisplayer(List<JournalContent> journalContents, int index)
	{
		JournalContentDisplayer journalContentDisplayer = SpawnContent(journalContents[index]);
		journalContents[index].Displayer = journalContentDisplayer;
		Canvas.ForceUpdateCanvases();
		if (journalContents[index].Type == EJournalContentType.TEXT_ICON)
		{
			if (_currentTextIconGroup == null)
			{
				_currentTextIconGroup = UnityEngine.Object.Instantiate(_textIconGroupPrefab, _currentContentHolder);
			}
			journalContentDisplayer.RectTransform.SetParent(_currentTextIconGroup);
			_currentTextIconGroup.GetComponent<LayoutElement>().CalculateLayoutInputVertical();
			_currentTextIconGroup.GetComponent<LayoutElement>().preferredHeight += _linesDescriptions.GetLinesDescriptionForCurrentLanguage().TextMargin;
		}
		else if (journalContents[index].Type == EJournalContentType.YESNO_CHOICE || journalContents[index].Type == EJournalContentType.CHARACTER_CHOICE || journalContents[index].Type == EJournalContentType.CUSTOM_CHOICE || journalContents[index].Type == EJournalContentType.ITEM_CHOICE)
		{
			journalContents[index].Displayer.LayoutElement.preferredHeight = _maxPageHeight - _currentPageHeight;
			journalContents[index].Displayer.SetTooltipController(_actionChoiceTooltipTransform);
			_choiceWasRendered = true;
		}
		else if (journalContents[index].Type == EJournalContentType.TEXT)
		{
			TextJournalContentDisplayer textJournalContentDisplayer = journalContentDisplayer as TextJournalContentDisplayer;
			if (textJournalContentDisplayer != null)
			{
				if (index == 0)
				{
					textJournalContentDisplayer.TextField.margin = new Vector4(textJournalContentDisplayer.TextField.margin.x, 0f, textJournalContentDisplayer.TextField.margin.z, textJournalContentDisplayer.TextField.margin.w);
				}
				else
				{
					textJournalContentDisplayer.TextField.margin = new Vector4(textJournalContentDisplayer.TextField.margin.x, _linesDescriptions.GetLinesDescriptionForCurrentLanguage().TextMargin, textJournalContentDisplayer.TextField.margin.z, textJournalContentDisplayer.TextField.margin.w);
					textJournalContentDisplayer.GetComponent<LayoutElement>().CalculateLayoutInputVertical();
					textJournalContentDisplayer.GetComponent<LayoutElement>().preferredHeight += _linesDescriptions.GetLinesDescriptionForCurrentLanguage().TextMargin;
				}
			}
		}
		if (journalContents[index].Type != EJournalContentType.TEXT_ICON)
		{
			_currentTextIconGroup = null;
		}
		return journalContentDisplayer;
	}

	private JournalContentDisplayer MoveContentToNextPage(List<JournalContent> journalContents, int index, JournalContentDisplayer currentDisplayer)
	{
		JournalContentDisplayer journalContentDisplayer = null;
		float y = currentDisplayer.RectTransform.sizeDelta.y;
		float num = _currentPageHeight - _maxPageHeight;
		float currentPageHeight = _currentPageHeight;
		if (journalContents[index].Type == EJournalContentType.TEXT)
		{
			SpawnNewSubPage();
			if (currentDisplayer.LinesAmount < _minimumLinesAmountEligibleForBreaking)
			{
				currentDisplayer.RectTransform.SetParent(_currentContentHolder);
				journalContentDisplayer = currentDisplayer;
			}
			else
			{
				TextJournalContentDisplayer textJournalContentDisplayer = (TextJournalContentDisplayer)UnityEngine.Object.Instantiate(_contentsDisplayers.GetContentDisplayer(EJournalContentType.TEXT), _currentContentHolder);
				TextJournalContentDisplayer textJournalContentDisplayer2 = (TextJournalContentDisplayer)currentDisplayer;
				textJournalContentDisplayer.AttachPreviousTextMeshPro(textJournalContentDisplayer2.TextField, textJournalContentDisplayer2);
				textJournalContentDisplayer.GetComponent<Localize>().LocalizeEvent.AddListener(delegate
				{
					WordwrapLanguageSetup.StaticAsianWordWrap();
				});
				LinesDescription.LineDescription linesDescriptionForCurrentLanguage = _linesDescriptions.GetLinesDescriptionForCurrentLanguage();
				float num2 = Mathf.Ceil(_maxPageHeight - (currentPageHeight - y));
				if (num < (float)linesDescriptionForCurrentLanguage.LineHeight)
				{
					num2 -= (float)linesDescriptionForCurrentLanguage.LineHeight - linesDescriptionForCurrentLanguage.TextMargin;
					num = linesDescriptionForCurrentLanguage.FirstLineHeight;
				}
				currentDisplayer.LayoutElement.preferredHeight = num2;
				textJournalContentDisplayer.LayoutElement.preferredHeight = num;
				textJournalContentDisplayer.TextField.margin = new Vector4(textJournalContentDisplayer.TextField.margin.x, 0f, textJournalContentDisplayer.TextField.margin.z, textJournalContentDisplayer.TextField.margin.w);
				journalContentDisplayer = textJournalContentDisplayer;
				journalContents[index].Displayer = journalContentDisplayer;
				UpdateTMPMeshes(textJournalContentDisplayer2.TextField, textJournalContentDisplayer.TextField);
				int num3 = linesDescriptionForCurrentLanguage.FirstLineHeight + linesDescriptionForCurrentLanguage.LineHeight * (textJournalContentDisplayer.LinesAmount - 1);
				textJournalContentDisplayer.LayoutElement.preferredHeight = num3;
				UpdateTMPMeshes(textJournalContentDisplayer2.TextField, textJournalContentDisplayer.TextField);
			}
		}
		else if (journalContents[index].Type == EJournalContentType.TEXT_ICON)
		{
			SpawnNewSubPage();
			_currentTextIconGroup = UnityEngine.Object.Instantiate(_textIconGroupPrefab, _currentContentHolder);
			currentDisplayer.RectTransform.SetParent(_currentTextIconGroup);
			journalContentDisplayer = currentDisplayer;
		}
		else
		{
			SpawnNewSubPage();
			currentDisplayer.RectTransform.SetParent(_currentContentHolder);
			journalContentDisplayer = currentDisplayer;
		}
		return journalContentDisplayer;
	}

	public void RenderContents()
	{
		_choiceWasRendered = false;
		ClearRenderedPages();
		SpawnNewSubPage(firstPage: true);
		List<JournalContent> sortedJournalContents = _contents.GetSortedJournalContents();
		for (int i = 0; i < sortedJournalContents.Count; i++)
		{
			JournalContentDisplayer journalContentDisplayer = null;
			_currentPageHeight = CalculateCurrentPageHeight();
			if (sortedJournalContents[i].Type == EJournalContentType.TEXT_ICON)
			{
				journalContentDisplayer = GroupTextIconContents(sortedJournalContents, i);
			}
			else if (sortedJournalContents[i].Type == EJournalContentType.TEXT)
			{
				journalContentDisplayer = GroupTextContents(sortedJournalContents, i);
			}
			if (journalContentDisplayer == null)
			{
				journalContentDisplayer = SpawnNewContentDisplayer(sortedJournalContents, i);
			}
			AddContentDisplayerToPageTransformsList(journalContentDisplayer);
			for (_currentPageHeight = CalculateCurrentPageHeight(); _currentPageHeight > _maxPageHeight; _currentPageHeight = CalculateCurrentPageHeight())
			{
				journalContentDisplayer = MoveContentToNextPage(sortedJournalContents, i, journalContentDisplayer);
				AddContentDisplayerToPageTransformsList(journalContentDisplayer);
			}
		}
		RefreshPages();
		_contents.ClearJournalContentsList();
	}

	private void AddContentDisplayerToPageTransformsList(JournalContentDisplayer displayer)
	{
		if (_rectTransforms != null)
		{
			if (_rectTransforms.Count <= _currentPageIndex)
			{
				_rectTransforms.Add(new List<RectTransform>());
			}
			RectTransform item = ((!(displayer is TextIconJournalContentDisplayer) || !(_currentTextIconGroup != null)) ? displayer.RectTransform : _currentTextIconGroup);
			if (!_rectTransforms[_currentPageIndex].Contains(item))
			{
				_rectTransforms[_currentPageIndex].Add(item);
			}
		}
	}

	private void ClearRenderedPages()
	{
		_pageController.ClearSubpages();
		if (_rectTransforms == null)
		{
			_rectTransforms = new List<List<RectTransform>>();
		}
		else
		{
			_rectTransforms.Clear();
		}
		_currentPageIndex = 0;
	}

	private void UpdateTMPMeshes(TextMeshProUGUI firstTmp, TextMeshProUGUI secondTmp)
	{
		firstTmp.enabled = false;
		Canvas.ForceUpdateCanvases();
		firstTmp.enabled = true;
		Canvas.ForceUpdateCanvases();
		firstTmp.ForceMeshUpdate();
		secondTmp.ForceMeshUpdate();
		Canvas.ForceUpdateCanvases();
	}

	private void SpawnNewSubPage(bool firstPage = false)
	{
		SubPageController subPageController = UnityEngine.Object.Instantiate(_subPagePrefab, _pageHolder);
		_currentContentHolder = subPageController.GetComponent<RectTransform>();
		_pageController.AddNewSubPage(subPageController);
		_currentPageHeight = 0f;
		if (!firstPage)
		{
			_currentPageIndex++;
		}
	}

	private JournalContentDisplayer SpawnContent(JournalContent content)
	{
		JournalContentDisplayer journalContentDisplayer = UnityEngine.Object.Instantiate(_contentsDisplayers.GetContentDisplayer(content.Type), _currentContentHolder);
		journalContentDisplayer.SetContentData(content);
		return journalContentDisplayer;
	}

	private void RefreshPages()
	{
		List<PageController> pages = _pageController.GetSubpagesList().Pages;
		for (int i = 0; i < pages.Count; i++)
		{
			pages[i].gameObject.SetActive(i == 0);
		}
	}
}
