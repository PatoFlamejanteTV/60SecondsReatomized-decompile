using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class YesNoChoiceJournalContentDisplayer : JournalContentDisplayer<YesNoChoiceJournalContent>
{
	[SerializeField]
	private ChoiceCardsController _choiceCardsController;

	[SerializeField]
	private SwitchButtonController _switchButtonController;

	[SerializeField]
	private SurvivalData _survivalData;

	[SerializeField]
	private GlobalBoolVariable _attentionVariable;

	public override int LinesAmount => 1;

	public override void SetContentData(JournalContent content)
	{
		if (content.Type == EJournalContentType.YESNO_CHOICE)
		{
			_survivalData.DailyEventResolved = false;
			_choiceCardsController.SetYesNoCards(null, null);
			_switchButtonController.SetSelectable(SecondsEventManager.GetReferenceToJournalButtonNext());
			SecondsEventManager.SetCurrentChoiceCardsController(_choiceCardsController);
		}
	}

	public void SetAttentionVariableValue(bool value)
	{
		_attentionVariable.Value = value;
	}
}
