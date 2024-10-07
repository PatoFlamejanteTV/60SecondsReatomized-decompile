namespace RG.SecondsRemaster.Survival;

public class GoalsPageDisplayController : BasePagesDisplayController
{
	public override void SetPagesInitialVisibility()
	{
		bool flag = false;
		for (int i = 0; i < _displayablePages.Count; i++)
		{
			if (!flag)
			{
				flag = true;
				_currentPageIndex = i;
				_currentPage = _displayablePages[i];
				_journalButtonNext.SetActive(value: false);
				_journalButtonPrevious.SetActive(value: false);
			}
			_displayablePages[i].Hide();
		}
		BasePagesDisplayController.BlockSwitchingPages = false;
		_visible = false;
	}

	public override void ShowNextPage()
	{
		base.ShowNextPage();
		if (!BasePagesDisplayController.BlockSwitchingPages)
		{
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
				BasePagesDisplayController.BlockSwitchingPages = false;
			}
			else
			{
				ShowPage(pageController);
			}
		}
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

	public override void OnPageChange()
	{
		UpdateArrowsVisibility();
	}

	private void UpdateArrowsVisibility()
	{
		_journalButtonNext.SetActive(_currentPageIndex != _displayablePages.Count - 1);
		_journalButtonPrevious.SetActive(_currentPageIndex > 0);
	}
}
