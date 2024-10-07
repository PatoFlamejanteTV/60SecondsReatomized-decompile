using System.Collections;
using RG.Parsecs.EventEditor;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class TextJournalContentDisplayer : JournalContentDisplayer<TextJournalContent>
{
	[SerializeField]
	private TextMeshProUGUI _textField;

	[SerializeField]
	private LinesDescription _linesDescription;

	private TextJournalContentDisplayer _previousDisplayer;

	public TextMeshProUGUI TextField => _textField;

	public override int LinesAmount => _textField.textInfo.lineCount;

	public override void SetContentData(JournalContent content)
	{
		if (content.Type == EJournalContentType.TEXT)
		{
			TextJournalContent textJournalContent = (TextJournalContent)content;
			if (string.IsNullOrEmpty(textJournalContent.Term.mTerm) || (string)textJournalContent.Term == null)
			{
				_textField.text = textJournalContent.PureText;
			}
			else
			{
				textJournalContent.RegisterManagers();
				_textField.text = textJournalContent.Term;
				textJournalContent.UnregisterManagers();
			}
			SetLinesData(textJournalContent);
		}
	}

	private void ForceTextMeshProUpdate()
	{
		Canvas.ForceUpdateCanvases();
		_textField.ForceMeshUpdate();
	}

	private void SetLinesData(TextJournalContent content)
	{
		LinesDescription.LineDescription linesDescriptionForCurrentLanguage = _linesDescription.GetLinesDescriptionForCurrentLanguage();
		if (content.EventPhase == EParsecsEventPhase.ACTION)
		{
			_textField.fontSize = linesDescriptionForCurrentLanguage.ActionPageTextSize;
		}
		else if (content.EventPhase == EParsecsEventPhase.REPORT)
		{
			_textField.fontSize = linesDescriptionForCurrentLanguage.ReportPageTextSize;
		}
		ForceTextMeshProUpdate();
		base.LayoutElement.preferredHeight = linesDescriptionForCurrentLanguage.FirstLineHeight + linesDescriptionForCurrentLanguage.LineHeight * (LinesAmount - 1);
		_textField.lineSpacing = linesDescriptionForCurrentLanguage.LineSpacing;
	}

	public void AttachPreviousTextMeshPro(TextMeshProUGUI previousTmp, TextJournalContentDisplayer previousDisplayer)
	{
		_previousDisplayer = previousDisplayer;
		previousTmp.overflowMode = TextOverflowModes.Linked;
		previousTmp.linkedTextComponent = _textField;
		_textField.lineSpacing = previousTmp.lineSpacing;
		Canvas.ForceUpdateCanvases();
		_textField.ForceMeshUpdate();
		previousTmp.ForceMeshUpdate();
		Canvas.ForceUpdateCanvases();
	}

	public void TryToFixLinkedText()
	{
		if (_textField != null && _textField.linkedTextComponent != null)
		{
			StartCoroutine(EnableAndDisablePreviousPage());
		}
	}

	private IEnumerator EnableAndDisablePreviousPage()
	{
		Transform previousPage = null;
		int siblingIndex = base.transform.parent.GetSiblingIndex();
		if (siblingIndex > 0)
		{
			previousPage = base.transform.parent.parent.GetChild(siblingIndex - 1);
		}
		if (!(previousPage != null))
		{
			yield break;
		}
		CanvasGroup pageCanvasGroup = previousPage.GetComponent<CanvasGroup>();
		if (pageCanvasGroup != null)
		{
			pageCanvasGroup.alpha = 0f;
			previousPage.gameObject.SetActive(value: true);
			Canvas.ForceUpdateCanvases();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			if (previousPage != null)
			{
				previousPage.gameObject.SetActive(value: false);
			}
			if (pageCanvasGroup != null)
			{
				pageCanvasGroup.alpha = 1f;
			}
			Canvas.ForceUpdateCanvases();
		}
	}

	public void AppendText(TextJournalContent content, string separator = "\n")
	{
		TextMeshProUGUI textField = GetParentDisplayer().TextField;
		if (string.IsNullOrEmpty(content.Term.mTerm) || (string)content.Term == null)
		{
			textField.text = textField.text + separator + content.PureText;
		}
		else
		{
			content.RegisterManagers();
			textField.text = textField.text + separator + content.Term;
			content.UnregisterManagers();
		}
		SetLinesData(content);
	}

	public TextJournalContentDisplayer GetParentDisplayer()
	{
		if (_previousDisplayer == null)
		{
			return this;
		}
		return _previousDisplayer.GetParentDisplayer();
	}
}
