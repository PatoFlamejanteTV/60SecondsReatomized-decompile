using System.Collections.Generic;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class ReportPageController : PageController
{
	[SerializeField]
	private EventsRendererController _eventsRendererController;

	[SerializeField]
	private VisualsData _bigDoodlesData;

	[SerializeField]
	private VisualId[] _bigDoodlesIds;

	[SerializeField]
	private VisualsController _bigDoodlesVisualsController;

	[SerializeField]
	private VisualsData _topDoodlesData;

	[SerializeField]
	private VisualId[] _topDoodlesIds;

	[SerializeField]
	private VisualsController _topDoodlesVisualsController;

	[SerializeField]
	private int _chanceToShowDoodle = 20;

	[SerializeField]
	private NodeFunction _displayHistoryTextFunction;

	[SerializeField]
	private Button _previousPageButton;

	[SerializeField]
	private GlobalBoolVariable _attentionVariable;

	[SerializeField]
	private PagesDisplayController _pagesDisplayController;

	private List<PageController> _subPages;

	private void SetRandomReportDoodles()
	{
		PagesListController subpagesList = GetSubpagesList();
		if (!(subpagesList == null) && subpagesList.Pages != null)
		{
			_subPages = subpagesList.Pages;
			if (_subPages.Count > 1 && _eventsRendererController.CanShowDoodleOnLastPage() && Random.Range(0, 100) < _chanceToShowDoodle && ShowRandomDoodle(_bigDoodlesData, _bigDoodlesIds))
			{
				AssignDoodlesToLastPage(_bigDoodlesVisualsController.gameObject);
			}
			if (Random.Range(0, 100) < _chanceToShowDoodle && ShowRandomDoodle(_topDoodlesData, _topDoodlesIds))
			{
				AssignDoodlesToRandomPage(_topDoodlesVisualsController.gameObject);
			}
			if (_bigDoodlesVisualsController != null)
			{
				_bigDoodlesVisualsController.RefreshVisualsState();
			}
			if (_topDoodlesVisualsController != null)
			{
				_topDoodlesVisualsController.RefreshVisualsState();
			}
		}
	}

	private bool ShowRandomDoodle(VisualsData doodlesVisualsData, VisualId[] visualIds)
	{
		if (doodlesVisualsData == null || visualIds == null || visualIds.Length == 0)
		{
			return false;
		}
		int num = Random.Range(0, visualIds.Length);
		doodlesVisualsData.RuntimeData.VisualToDisplay = visualIds[num];
		return true;
	}

	private void AssignDoodlesToRandomPage(GameObject doodlesHolder)
	{
		if (_subPages.Count == 1)
		{
			AssignDoodlesToPage(_subPages[0] as ReportSubPageController, doodlesHolder);
			return;
		}
		int index = Random.Range(0, _subPages.Count - 1);
		AssignDoodlesToPage(_subPages[index] as ReportSubPageController, doodlesHolder);
	}

	private void AssignDoodlesToLastPage(GameObject doodlesHolder)
	{
		AssignDoodlesToPage(_subPages[_subPages.Count - 1] as ReportSubPageController, doodlesHolder);
	}

	private void AssignDoodlesToPage(ReportSubPageController subPageController, GameObject doodlesHolder)
	{
		if (!(subPageController == null))
		{
			subPageController.SetDoodlesHolder(doodlesHolder);
		}
	}

	public void RenderPages()
	{
		if (_endGameData.RuntimeData.ShouldEndGame)
		{
			_displayHistoryTextFunction.Execute(null);
		}
		_eventsRendererController.RenderContents();
		SetRandomReportDoodles();
		DisableDoodlesHolders();
	}

	private void DisableDoodlesHolders()
	{
		if (_bigDoodlesVisualsController != null)
		{
			_bigDoodlesVisualsController.gameObject.SetActive(value: false);
		}
		if (_topDoodlesVisualsController != null)
		{
			_topDoodlesVisualsController.gameObject.SetActive(value: false);
		}
	}

	public override bool CanBeDisplayed()
	{
		return true;
	}

	public override void SetPageData(bool visible)
	{
		_previousPageButton.interactable = _pagesDisplayController.CurrentPageIndex != 0;
		if (visible && _attentionVariable != null && _attentionVariable.Value)
		{
			_attentionVariable.Value = false;
		}
	}

	public override void InitializePage()
	{
		base.InitializePage();
		if (IsEnabled())
		{
			_attentionVariable.Value = true;
		}
	}
}
