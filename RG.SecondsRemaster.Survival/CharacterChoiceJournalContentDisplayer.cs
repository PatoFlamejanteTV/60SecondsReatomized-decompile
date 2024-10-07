using System.Collections.Generic;
using RG.Parsecs.Survival;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class CharacterChoiceJournalContentDisplayer : JournalContentDisplayer<CharacterChoiceJournalContent>
{
	[SerializeField]
	private ChoiceCardsController _choiceCardsController;

	[SerializeField]
	private SwitchButtonController _switchButtonController;

	[SerializeField]
	private SurvivalData _survivalData;

	[SerializeField]
	private List<Image> _checkmarks;

	[SerializeField]
	private List<Image> _backgrounds;

	[SerializeField]
	private List<RectTransform> _rectTransforms;

	[SerializeField]
	private TextMeshProUGUI _callToAction;

	[SerializeField]
	private ActionChoiceTooltipContent[] _actionChoiceTooltipContents;

	public override int LinesAmount => 1;

	public override void SetContentData(JournalContent content)
	{
		if (content.Type != EJournalContentType.CHARACTER_CHOICE)
		{
			return;
		}
		CharacterChoiceJournalContent characterChoiceJournalContent = (CharacterChoiceJournalContent)content;
		_survivalData.DailyEventResolved = true;
		List<Character> characters = characterChoiceJournalContent.Characters;
		_choiceCardsController.SetCharacterCards(characters[0], characters[1], characters[2], characters[3]);
		int num = characters.Count - 1;
		int num2 = 0;
		while (num >= 0)
		{
			_checkmarks[num].sprite = _backgrounds[num].sprite;
			if (!(characters[num] == null))
			{
				_actionChoiceTooltipContents[num2].SetCharacterContent(characters[num]);
				num2++;
				SecondsCharacter secondsCharacter = characters[num] as SecondsCharacter;
				if (secondsCharacter != null && secondsCharacter.SizeDefinition != null)
				{
					_rectTransforms[num].sizeDelta = secondsCharacter.SizeDefinition.Size;
					Canvas.ForceUpdateCanvases();
				}
			}
			num--;
		}
		if ((string)characterChoiceJournalContent.CallToActionTerm == null || string.IsNullOrEmpty(characterChoiceJournalContent.CallToActionTerm.mTerm))
		{
			_callToAction.gameObject.SetActive(value: false);
		}
		else
		{
			_callToAction.gameObject.SetActive(value: true);
			_callToAction.text = characterChoiceJournalContent.CallToActionTerm;
		}
		_switchButtonController.SetSelectable(SecondsEventManager.GetReferenceToJournalButtonNext());
		SecondsEventManager.SetCurrentChoiceCardsController(_choiceCardsController);
	}
}
