using I2.Loc;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.Survival;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class SecondsEventManager : EventManager
{
	[SerializeField]
	private EndOfDayListenerList _endOfDayListener;

	[SerializeField]
	private JournalController _journalController;

	[SerializeField]
	private JournalContentsList _reportContents;

	[SerializeField]
	private JournalContentsList _actionContents;

	[SerializeField]
	private Button _journalButtonNext;

	private static SecondsEventManager _instance;

	private ChoiceCardsController _choiceCardsController;

	public Button JournalButtonNext => _journalButtonNext;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			EventManager.eventManager = this;
		}
		else
		{
			Object.Destroy(this);
		}
		_endOfDayListener.RegisterOnEndOfDay(SetJournalController, "PrepareSystem", 2, this, forceOrder: true);
		_endOfDayListener.RegisterOnEndOfDay(RenderPages, "Visuals", 7, this, forceOrder: true);
		_endOfDayListener.RegisterOnDayStarted(JournalOnDayStart, "ChangeFinished", 1);
	}

	private void OnDestroy()
	{
		_endOfDayListener.UnregisterOnEndOfDay(SetJournalController, "PrepareSystem");
		_endOfDayListener.UnregisterOnDayStarted(JournalOnDayStart, "ChangeFinished");
		_endOfDayListener.UnregisterOnEndOfDay(RenderPages, "Visuals");
	}

	private void SetJournalController()
	{
		_journalController.SetJournal();
	}

	private void RenderPages()
	{
		_journalController.RenderPages();
	}

	private void JournalOnDayStart()
	{
		_journalController.JournalOnDayStart();
	}

	public static void UnlockChoice()
	{
		ChoiceCardController playerChoice = EventManager.GetPlayerChoice();
		if (playerChoice != null)
		{
			if (playerChoice.Character != null)
			{
				playerChoice.Character.Unlock();
			}
			if (playerChoice.Item != null)
			{
				playerChoice.Item.Unlock();
			}
			if (playerChoice.ActionCondition != null)
			{
				playerChoice.ActionCondition.OnDeselect();
			}
		}
	}

	public static Button GetReferenceToJournalButtonNext()
	{
		return _instance._journalButtonNext;
	}

	public static void SetCurrentChoiceCardsController(ChoiceCardsController choiceCardsController)
	{
		_instance._choiceCardsController = choiceCardsController;
		RefreshChoiceCards();
	}

	public static void RefreshChoiceCards()
	{
		if (_instance._choiceCardsController != null)
		{
			_instance._choiceCardsController.RefreshCardsController();
		}
	}

	public static void AddJournalContent(NodeCanvas canvas, JournalContent content)
	{
		EParsecsEventPhase canvasPhase = GetCanvasPhase(canvas);
		TextJournalContent textJournalContent = content as TextJournalContent;
		switch (canvasPhase)
		{
		case EParsecsEventPhase.ACTION:
			if (textJournalContent != null)
			{
				textJournalContent.EventPhase = EParsecsEventPhase.ACTION;
			}
			_instance._actionContents.AddJournalContent(content);
			break;
		case EParsecsEventPhase.REPORT:
			if (textJournalContent != null)
			{
				textJournalContent.EventPhase = EParsecsEventPhase.REPORT;
			}
			_instance._reportContents.AddJournalContent(content);
			break;
		}
	}

	private static EParsecsEventPhase GetCanvasPhase(NodeCanvas parentCanvas)
	{
		if (parentCanvas is ParsecsEvent)
		{
			return (parentCanvas as ParsecsEvent).CurrentPhase;
		}
		if (parentCanvas is NodeFunction)
		{
			return (parentCanvas as NodeFunction).GetOriginalCanvasAs<ParsecsEvent>().CurrentPhase;
		}
		if (parentCanvas is Goal)
		{
			return EParsecsEventPhase.REPORT;
		}
		throw new UnityException("This canvas type can only be executed from canvases that derive from ParsecsEvent or NodeFunctions");
	}

	protected override ChoiceCardController GetChoice()
	{
		if (_choiceCardsController == null)
		{
			Debug.LogError("Choice Cards Controller jest nullem");
			return null;
		}
		return _choiceCardsController.GetPlayerChoice();
	}

	protected override void ShowGoalsUpdate(LocalizedString term, LocalizedString goalName)
	{
	}
}
