using I2.Loc;
using RG.Parsecs.Survival;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class TextIconJournalContentDisplayer : JournalContentDisplayer<TextIconJournalContent>
{
	[SerializeField]
	private TextMeshProUGUI _textField;

	private int _currentAmount;

	private LocalizedString _iconTerm;

	private EventContentData.ETextIconContentType _contentType;

	public override int LinesAmount => 1;

	public override void SetContentData(JournalContent content)
	{
		if (content.Type == EJournalContentType.TEXT_ICON)
		{
			TextIconJournalContent textIconJournalContent = (TextIconJournalContent)content;
			_iconTerm = textIconJournalContent.IconTerm;
			_currentAmount = textIconJournalContent.Amount;
			_contentType = textIconJournalContent.IconType;
			UpdateDisplayedText();
		}
	}

	public void UpdateDisplayedText()
	{
		string arg = string.Empty;
		switch (_contentType)
		{
		case EventContentData.ETextIconContentType.NONE:
			arg = "";
			break;
		case EventContentData.ETextIconContentType.ADDITION:
			arg = "+";
			break;
		case EventContentData.ETextIconContentType.SUBTRACTION:
			arg = "-";
			break;
		}
		_textField.text = string.Format("{0}{1} {2} ", arg, (Mathf.Abs(_currentAmount) > 0) ? _currentAmount.ToString() : "", _iconTerm);
	}

	public bool TryToJoinNextTextIconContent(TextIconJournalContent iconContent)
	{
		if (iconContent.IconType == EventContentData.ETextIconContentType.NONE || iconContent.IconType != _contentType || !_iconTerm.ToString().Equals(iconContent.IconTerm.ToString()))
		{
			return false;
		}
		_currentAmount += iconContent.Amount;
		UpdateDisplayedText();
		return true;
	}
}
