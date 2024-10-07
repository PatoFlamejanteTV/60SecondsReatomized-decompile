using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using RG.Parsecs.Common;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public abstract class BasePagesDisplayController : MonoBehaviour
{
	[SerializeField]
	protected CanvasGroupLerp _canvasGroupLerp;

	[SerializeField]
	protected PagesListController _rootPageList;

	[SerializeField]
	protected PageController[] _staticPages;

	[SerializeField]
	protected JournalController _journalController;

	[SerializeField]
	protected JournalTabsController _tabsController;

	[EventRef]
	[SerializeField]
	protected string _changePageSound;

	[SerializeField]
	protected GameObject _journalButtonNext;

	[SerializeField]
	protected GameObject _journalButtonPrevious;

	[SerializeField]
	protected GameObject _journalButtonNextEndDay;

	[SerializeField]
	private BasePagesDisplayController[] _otherPagesDisplayControllers;

	[SerializeField]
	private GameObject _doodlesOnAction;

	[SerializeField]
	private PageController _actionPage;

	protected static bool BlockSwitchingPages;

	protected int _currentPageIndex;

	protected PageController _currentPage;

	protected List<PageController> _displayablePages;

	protected bool _visible;

	private const int PAGE_NOT_AVAILABLE_IN_ARRAY = -1;

	public int CurrentPageIndex => _currentPageIndex;

	public bool Visible => _visible;

	public void ResetPages()
	{
		for (int i = 0; i < _rootPageList.Pages.Count; i++)
		{
			_rootPageList.Pages[i].ResetPage();
		}
		for (int j = 0; j < _staticPages.Length; j++)
		{
			_staticPages[j].ResetPage();
		}
	}

	public PageController GetCurrentPage()
	{
		return _currentPage;
	}

	public void InitializePages()
	{
		for (int i = 0; i < _rootPageList.Pages.Count; i++)
		{
			_rootPageList.Pages[i].InitializePage();
		}
		for (int j = 0; j < _staticPages.Length; j++)
		{
			_staticPages[j].InitializePage();
		}
		if (_doodlesOnAction != null)
		{
			_doodlesOnAction.SetActive(value: false);
		}
	}

	public void RefreshTabs()
	{
		_tabsController.RefreshAllTabs();
	}

	public void GetAllPages()
	{
		if (_displayablePages == null)
		{
			_displayablePages = new List<PageController>();
		}
		else
		{
			for (int i = 0; i < _displayablePages.Count; i++)
			{
				_displayablePages[i].Hide();
				_displayablePages[i].SetEnabled(value: false);
			}
			_displayablePages.Clear();
		}
		GetAllPagesRecursive(_displayablePages, _rootPageList);
	}

	protected void GetAllPagesRecursive(List<PageController> pages, PagesListController pagesList)
	{
		for (int i = 0; i < pagesList.Pages.Count; i++)
		{
			if (!pagesList.Pages[i].CanBeDisplayed())
			{
				pagesList.Pages[i].SetEnabled(value: false);
				continue;
			}
			PagesListController subpagesList = pagesList.Pages[i].GetSubpagesList();
			pagesList.Pages[i].SetEnabled(value: true);
			if (subpagesList != null)
			{
				GetAllPagesRecursive(pages, subpagesList);
			}
			else
			{
				_displayablePages.Add(pagesList.Pages[i]);
			}
		}
	}

	public void ShowSpecificPage(PageController page)
	{
		if (BlockSwitchingPages)
		{
			return;
		}
		BlockSwitchingPages = true;
		InvokePageSwitched();
		for (int i = 0; i < _otherPagesDisplayControllers.Length; i++)
		{
			if (_otherPagesDisplayControllers[i].Visible)
			{
				_otherPagesDisplayControllers[i].InvokePageSwitched();
			}
		}
		if (_displayablePages.Contains(page))
		{
			for (int j = 0; j < _displayablePages.Count; j++)
			{
				if (_displayablePages[j] == page)
				{
					_currentPageIndex = j;
				}
			}
		}
		else
		{
			_currentPageIndex = -1;
		}
		ShowPage(page);
	}

	public abstract void SetPagesInitialVisibility();

	public virtual void ShowNextPage()
	{
		if (_currentPageIndex < 0)
		{
			Debug.LogError("You're trying to show next page when current page is not in _displayablePages array");
		}
	}

	public virtual void ShowPreviousPage()
	{
		if (_currentPageIndex < 0)
		{
			Debug.LogError("You're trying to show next page when current page is not in _displayablePages array");
		}
	}

	public virtual void OnPageChange()
	{
	}

	public void ShowPage(PageController page)
	{
		StartCoroutine(ShowPageCoroutine(page));
		OnPageChange();
	}

	private IEnumerator ShowPageCoroutine(PageController page)
	{
		BlockSwitchingPages = true;
		if (_visible)
		{
			yield return _canvasGroupLerp.HideCanvasGroup();
		}
		else
		{
			for (int i = 0; i < _otherPagesDisplayControllers.Length; i++)
			{
				if (_otherPagesDisplayControllers[i].Visible)
				{
					yield return _otherPagesDisplayControllers[i].HidePages();
				}
			}
		}
		if (_currentPage != null && _currentPage != page)
		{
			_currentPage.Hide();
		}
		OnPageChange();
		_currentPage = page;
		if (_actionPage != null && _doodlesOnAction != null && _doodlesOnAction.activeSelf && page.RootPage != _actionPage)
		{
			_doodlesOnAction.SetActive(value: false);
		}
		page.Show();
		AudioManager.PlaySound(_changePageSound);
		yield return _canvasGroupLerp.ShowCanvasGroup();
		BlockSwitchingPages = false;
		_visible = true;
	}

	public IEnumerator HidePages()
	{
		yield return _canvasGroupLerp.HideCanvasGroup();
		if (_currentPage != null)
		{
			_currentPage.Hide();
		}
		if (_journalButtonNext != null)
		{
			_journalButtonNext.SetActive(value: false);
		}
		if (_journalButtonPrevious != null)
		{
			_journalButtonPrevious.SetActive(value: false);
		}
		if (_journalButtonNextEndDay != null)
		{
			_journalButtonNextEndDay.SetActive(value: false);
		}
		_visible = false;
	}

	protected void InvokePageSwitched()
	{
		if (_currentPage != null)
		{
			_currentPage.OnPageSwitched();
		}
	}
}
