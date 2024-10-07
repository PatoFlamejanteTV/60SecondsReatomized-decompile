using System.Collections.Generic;
using RG.Parsecs.Common;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class PagesDisplayController : BasePagesDisplayController
{
	[SerializeField]
	private EndGameData _endGameData;

	[SerializeField]
	private float _timePageSwitchingIsBlockedWhenEndingDay = 2f;

	[SerializeField]
	private List<JournalPageButtonHelper> _pageButtonHelpers = new List<JournalPageButtonHelper>();

	private const string UNBLOCK_SWITCHING_PAGES_METHOD_NAME = "UnblockSwitchingPages";

	public override void SetPagesInitialVisibility()
	{
		bool flag = false;
		PageController pageController = null;
		for (int i = 0; i < _displayablePages.Count; i++)
		{
			if (!flag)
			{
				flag = true;
				_currentPageIndex = i;
				_currentPage = _displayablePages[i];
				SetButtonsVisibility(_currentPageIndex == _displayablePages.Count - 1);
				_displayablePages[i].Show();
				if (_displayablePages[i].IsSubpage())
				{
					pageController = _displayablePages[i].ParentPage;
				}
			}
			else if (!_displayablePages[i].IsSubpage() || _displayablePages[i].ParentPage != pageController)
			{
				_displayablePages[i].Hide();
			}
		}
		BasePagesDisplayController.BlockSwitchingPages = false;
		_visible = true;
	}

	public override void ShowNextPage()
	{
		base.ShowNextPage();
		if (!_journalController.CanSwitchPage() || BasePagesDisplayController.BlockSwitchingPages)
		{
			return;
		}
		BasePagesDisplayController.BlockSwitchingPages = true;
		_currentPage.OnPageSwitched();
		PageController pageController = null;
		int num = _currentPageIndex + 1;
		if (num < _displayablePages.Count)
		{
			pageController = _displayablePages[num];
			_currentPageIndex = num;
		}
		if (pageController == null)
		{
			if (_endGameData.RuntimeData.ShouldEndGame)
			{
				_journalController.Hide();
				Singleton<GameManager>.Instance.RaycastCatcher.SetClickawayBlocked(block: true);
				EndGameManager.Instance.LoadEndGameScene();
			}
			else
			{
				_journalController.TryToEndDay();
			}
			Invoke("UnblockSwitchingPages", _timePageSwitchingIsBlockedWhenEndingDay);
		}
		else
		{
			ShowPage(pageController);
		}
	}

	private void UnblockSwitchingPages()
	{
		BasePagesDisplayController.BlockSwitchingPages = false;
	}

	public override void ShowPreviousPage()
	{
		base.ShowPreviousPage();
		if (!BasePagesDisplayController.BlockSwitchingPages)
		{
			BasePagesDisplayController.BlockSwitchingPages = true;
			_currentPage.OnPageSwitched();
			PageController pageController = null;
			int num = _currentPageIndex - 1;
			if (num >= 0)
			{
				pageController = _displayablePages[num];
				_currentPageIndex = num;
			}
			if (pageController == null)
			{
				BasePagesDisplayController.BlockSwitchingPages = false;
			}
			else
			{
				ShowPage(pageController);
			}
		}
	}

	private void SetButtonsVisibility(bool endGameButtonVisible)
	{
		if (_journalButtonPrevious != null)
		{
			_journalButtonPrevious.SetActive(value: true);
		}
		if (_journalButtonNext != null)
		{
			_journalButtonNext.SetActive(!endGameButtonVisible);
		}
		if (_journalButtonNextEndDay != null)
		{
			_journalButtonNextEndDay.SetActive(endGameButtonVisible);
		}
	}

	public override void OnPageChange()
	{
		SetButtonsVisibility(_currentPageIndex == _displayablePages.Count - 1);
	}

	public void ResetPageButtonHelpers()
	{
		foreach (JournalPageButtonHelper pageButtonHelper in _pageButtonHelpers)
		{
			pageButtonHelper.ResetButton();
		}
	}
}
