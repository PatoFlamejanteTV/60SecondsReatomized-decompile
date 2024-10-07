using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class PageController : MonoBehaviour
{
	[SerializeField]
	private bool _hasSubpages;

	[SerializeField]
	private PagesListController _subPagesList;

	[SerializeField]
	protected EndGameData _endGameData;

	[SerializeField]
	private JournalTabController _associatedTab;

	[SerializeField]
	private bool _enabled;

	private bool _initialized;

	private int _lastDayWhenDataSet;

	public virtual PageController ParentPage
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public PageController RootPage
	{
		get
		{
			if (ParentPage == null)
			{
				return this;
			}
			return ParentPage.RootPage;
		}
	}

	public bool HasSubpages => _hasSubpages;

	public bool Initialized
	{
		get
		{
			return _initialized;
		}
		set
		{
			_initialized = value;
		}
	}

	protected void SetPageNotRefreshableToday()
	{
		_lastDayWhenDataSet = EndOfDayManager.Instance.AcutalDay;
	}

	protected bool CanRefreshPageToday()
	{
		return _lastDayWhenDataSet != EndOfDayManager.Instance.AcutalDay;
	}

	public virtual void SetPageData(bool visible)
	{
	}

	public virtual bool CanBeDisplayed()
	{
		return !_endGameData.RuntimeData.ShouldEndGame;
	}

	public virtual void InitializePage()
	{
		if (!_initialized && ParentPage != null)
		{
			ParentPage.InitializePage();
		}
	}

	public void SetEnabled(bool value)
	{
		_enabled = value;
	}

	public bool IsEnabled()
	{
		return _enabled;
	}

	public virtual bool IsSubpage()
	{
		return false;
	}

	public virtual void Show()
	{
		if (_associatedTab != null)
		{
			_associatedTab.ActivateTab();
		}
		SetPageData(visible: true);
		base.gameObject.SetActive(value: true);
	}

	public virtual void Show(bool doNotSetPageData)
	{
		if (_associatedTab != null)
		{
			_associatedTab.ActivateTab(!doNotSetPageData);
		}
		if (!doNotSetPageData)
		{
			SetPageData(visible: true);
		}
		base.gameObject.SetActive(value: true);
	}

	public PageController GetFirstSubpage()
	{
		if (!_hasSubpages)
		{
			return null;
		}
		return _subPagesList.Pages[0];
	}

	public virtual void Hide()
	{
		SetPageData(visible: false);
		base.gameObject.SetActive(value: false);
	}

	public virtual void OnPageSwitched()
	{
		if (_associatedTab != null)
		{
			_associatedTab.SetExclamationMarksVisibility();
		}
		if (ParentPage != null)
		{
			ParentPage.OnPageSwitched();
		}
	}

	public PagesListController GetSubpagesList()
	{
		if (!_hasSubpages)
		{
			return null;
		}
		return _subPagesList;
	}

	public void AddNewSubPage(PageController subPage)
	{
		_subPagesList.Pages.Add(subPage);
		if (subPage is SubPageController)
		{
			((SubPageController)subPage).ParentPage = this;
		}
	}

	public void ClearSubpages()
	{
		if (_subPagesList != null)
		{
			_subPagesList.ClearPages();
		}
	}

	public void ResetPage()
	{
		_initialized = false;
	}
}
