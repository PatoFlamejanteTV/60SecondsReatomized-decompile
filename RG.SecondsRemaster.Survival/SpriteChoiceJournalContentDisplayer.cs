using System.Collections.Generic;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class SpriteChoiceJournalContentDisplayer : JournalContentDisplayer<SpriteChoiceJournalContent>
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
	private ActionChoiceTooltipContent[] _actionChoiceTooltipContent;

	public override int LinesAmount => 1;

	public override void SetContentData(JournalContent content)
	{
		if (content.Type != EJournalContentType.CUSTOM_CHOICE)
		{
			return;
		}
		SpriteChoiceJournalContent obj = (SpriteChoiceJournalContent)content;
		_survivalData.DailyEventResolved = true;
		List<BaseActionCondition> actionConditions = obj.ActionConditions;
		_choiceCardsController.SetSpriteCards(actionConditions[0], actionConditions[1], actionConditions[2], actionConditions[3]);
		for (int i = 0; i < _checkmarks.Count; i++)
		{
			_checkmarks[i].sprite = _backgrounds[i].sprite;
			if (actionConditions[i] != null)
			{
				_actionChoiceTooltipContent[i].SetTermContent(actionConditions[i].SelectableTerm);
			}
		}
		_switchButtonController.SetSelectable(SecondsEventManager.GetReferenceToJournalButtonNext());
		SecondsEventManager.SetCurrentChoiceCardsController(_choiceCardsController);
	}
}
