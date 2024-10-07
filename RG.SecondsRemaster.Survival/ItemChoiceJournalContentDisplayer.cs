using System.Collections.Generic;
using RG.Parsecs.Survival;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class ItemChoiceJournalContentDisplayer : JournalContentDisplayer<ItemChoiceJournalContent>
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
	private ActionChoiceTooltipContent[] _actionChoiceTooltipContents;

	public override int LinesAmount => 1;

	public override void SetContentData(JournalContent content)
	{
		if (content.Type != EJournalContentType.ITEM_CHOICE)
		{
			return;
		}
		ItemChoiceJournalContent obj = (ItemChoiceJournalContent)content;
		_survivalData.DailyEventResolved = true;
		List<IItem> items = obj.Items;
		_choiceCardsController.SetItemCards(items[0], items[1], items[2], items[3], useCardAsNoChoice: false);
		int num = items.Count - 1;
		int num2 = 0;
		while (num >= 0)
		{
			_checkmarks[num].sprite = _backgrounds[num].sprite;
			if (!(items[num] == null))
			{
				_actionChoiceTooltipContents[num2].SetItemContent(items[num]);
				num2++;
			}
			num--;
		}
		_switchButtonController.SetSelectable(SecondsEventManager.GetReferenceToJournalButtonNext());
		SecondsEventManager.SetCurrentChoiceCardsController(_choiceCardsController);
	}
}
