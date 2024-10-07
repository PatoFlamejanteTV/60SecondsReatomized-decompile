using System;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using RG.VirtualInput;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RG.SecondsRemaster.Survival;

public class JournalController : MonoBehaviour
{
	public enum JournalState
	{
		HIDDEN,
		PARTIALLY_HIDDEN,
		VISIBLE,
		HIDE
	}

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private JournalDayController _dayController;

	[SerializeField]
	private JournalButtonController _journalButtonController;

	[SerializeField]
	private ReportPageController _reportPageController;

	[SerializeField]
	private ActionPageController _actionPageController;

	[SerializeField]
	private GoalPageController _goalPageController;

	[SerializeField]
	private EndOfDayManager _endOfDayManager;

	[SerializeField]
	private BasePagesDisplayController[] _pagesDisplayControllers;

	[SerializeField]
	private GlobalBoolVariable _shouldDisplaySendExpeditionPage;

	[SerializeField]
	private VisualsController _onActionDoodlesController;

	[SerializeField]
	private VirtualInputClosablePanel _journalGamepadCloseable;

	[SerializeField]
	private VirtualInputClosablePanel _hiddenJournalGamepadCloseable;

	[SerializeField]
	private JournalTabsController _journalTabsController;

	[SerializeField]
	private float _endDayDelay = 1f;

	private JournalState _currentJournalState;

	private const string RESET_JOURNAL_TRIGGER_NAME = "ResetJournal";

	private const string JOURNAL_VISIBLE_VAR_NAME = "Visible";

	private const string JOURNAL_PARTIALLY_HIDDEN_VAR_NAME = "PartiallyHidden";

	public JournalState CurrentJournalState => _currentJournalState;

	public JournalTabsController Tabs => _journalTabsController;

	private void Awake()
	{
		SetHidden();
		_journalGamepadCloseable.Hide();
	}

	public void RenderPages()
	{
		_journalTabsController.ResetFirstEnabledPage();
		_actionPageController.Show(doNotSetPageData: true);
		_reportPageController.Show(doNotSetPageData: true);
		_goalPageController.Show(doNotSetPageData: true);
		_reportPageController.RenderPages();
		_actionPageController.RenderPages();
		_goalPageController.RenderPages();
		for (int i = 0; i < _pagesDisplayControllers.Length; i++)
		{
			_pagesDisplayControllers[i].ResetPages();
			_pagesDisplayControllers[i].GetAllPages();
			_pagesDisplayControllers[i].InitializePages();
			_pagesDisplayControllers[i].SetPagesInitialVisibility();
			_pagesDisplayControllers[i].RefreshTabs();
		}
	}

	public void Show()
	{
		SetJournalState(JournalState.VISIBLE);
		_hiddenJournalGamepadCloseable.Hide();
		_journalGamepadCloseable.Show();
	}

	public void Hide()
	{
		SetJournalState(JournalState.HIDE);
		_hiddenJournalGamepadCloseable.Hide();
		_journalGamepadCloseable.Hide();
	}

	public void PartiallyHide()
	{
		SetJournalState(JournalState.PARTIALLY_HIDDEN);
		_journalGamepadCloseable.Hide();
		_hiddenJournalGamepadCloseable.Show();
	}

	public void SetHidden()
	{
		SetJournalState(JournalState.HIDDEN);
	}

	private void SetJournalState(JournalState state)
	{
		switch (state)
		{
		case JournalState.HIDDEN:
			_animator.SetTrigger("ResetJournal");
			_animator.SetBool("Visible", value: false);
			_animator.SetBool("PartiallyHidden", value: false);
			break;
		case JournalState.PARTIALLY_HIDDEN:
			_animator.SetBool("PartiallyHidden", value: true);
			break;
		case JournalState.VISIBLE:
			_animator.SetBool("PartiallyHidden", value: false);
			_animator.SetBool("Visible", value: true);
			break;
		case JournalState.HIDE:
			_animator.SetBool("PartiallyHidden", value: false);
			_animator.SetBool("Visible", value: false);
			break;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
		_currentJournalState = state;
	}

	public void SetJournal()
	{
		_dayController.SetCurrentDayText();
	}

	public void JournalOnDayStart()
	{
		_journalButtonController.Show();
		HandHintController.ShouldShowHint = true;
	}

	public void TryToEndDay()
	{
		Singleton<VirtualInputManager>.Instance.IsGamepadInputBlocked = true;
		Hide();
		Singleton<GameManager>.Instance.RaycastCatcher.SetClickawayBlocked(block: true);
		SecondsEventManager.UnlockChoice();
		_onActionDoodlesController.ResetVisualsData();
		EventSystem.current.SetSelectedGameObject(null);
		Invoke("EndDay", _endDayDelay);
	}

	private void EndDay()
	{
		_endOfDayManager.EndOfDay();
	}

	public bool CanSwitchPage()
	{
		return _currentJournalState == JournalState.VISIBLE;
	}
}
