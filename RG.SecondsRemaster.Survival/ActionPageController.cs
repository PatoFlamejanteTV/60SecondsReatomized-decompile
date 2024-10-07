using System.Collections;
using System.Collections.Generic;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class ActionPageController : PageController
{
	[SerializeField]
	private EventsRendererController _eventsRendererController;

	[SerializeField]
	private VisualsController _doodlesVisualsController;

	[SerializeField]
	private Selectable _journalButtonNext;

	[SerializeField]
	private SurvivalData _survivalData;

	[SerializeField]
	private GlobalBoolVariable _shouldDisplaySendExpeditionPage;

	[SerializeField]
	private GlobalBoolVariable _attentionVariable;

	[SerializeField]
	private VisualsData _onActionDoodlesVisualData;

	[SerializeField]
	private DoodlePageHeightsDefinitions _doodlePageHeightsDefinitions;

	private void Awake()
	{
		_survivalData.DailyEventResolved = true;
	}

	private void OnEnable()
	{
		StartCoroutine(WaitFrameAndRefreshMappedTarget());
	}

	public void RenderPages()
	{
		_eventsRendererController.RenderContents();
		AssignDoodlesToLastPage();
		_doodlesVisualsController.RefreshVisualsState();
	}

	private void AssignDoodlesToLastPage()
	{
		List<PageController> pages = GetSubpagesList().Pages;
		ActionSubPageController actionSubPageController = pages[pages.Count - 1] as ActionSubPageController;
		VisualId visualToDisplay = _onActionDoodlesVisualData.RuntimeData.VisualToDisplay;
		if (visualToDisplay != null)
		{
			float pageHeight = _doodlePageHeightsDefinitions.GetPageHeight(visualToDisplay);
			if (actionSubPageController != null && _eventsRendererController.CanShowDoodleOnLastPage(pageHeight))
			{
				actionSubPageController.SetDoodlesHolder(_doodlesVisualsController.gameObject);
			}
		}
	}

	public override void SetPageData(bool visible)
	{
		_journalButtonNext.interactable = _survivalData.DailyEventResolved;
		if (visible && _attentionVariable != null && _attentionVariable.Value)
		{
			_attentionVariable.Value = !_survivalData.DailyEventResolved;
		}
	}

	public override bool CanBeDisplayed()
	{
		if (base.CanBeDisplayed())
		{
			return !_shouldDisplaySendExpeditionPage.Value;
		}
		return false;
	}

	public override void InitializePage()
	{
		base.InitializePage();
		if (IsEnabled())
		{
			_attentionVariable.Value = true;
		}
		InitializeDynamicNavigation();
	}

	private void InitializeDynamicNavigation()
	{
		NavigationMappingTemplate component = GetComponent<NavigationMappingTemplate>();
		if (!(component != null))
		{
			return;
		}
		component.CurrentlyMappedTarget = null;
		List<PageController> pages = GetSubpagesList().Pages;
		if (pages.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < pages.Count; i++)
		{
			INavigationMappable[] componentsInChildren = pages[i].gameObject.GetComponentsInChildren<INavigationMappable>();
			if (componentsInChildren == null)
			{
				continue;
			}
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				Selectable selectable = componentsInChildren[j] as Selectable;
				if (component.CurrentlyMappedTarget == null && selectable.interactable)
				{
					component.CurrentlyMappedTarget = selectable;
				}
				NavigationMappingTemplate.NavigationMappingEntry navigationMappingEntry = component.GetNavigationMappingEntry(componentsInChildren[j].MappingTag);
				if (navigationMappingEntry != null)
				{
					componentsInChildren[j].AddNavigationMapping(navigationMappingEntry.Down, navigationMappingEntry.Left, navigationMappingEntry.Up, navigationMappingEntry.Right);
				}
			}
		}
	}

	private IEnumerator WaitFrameAndRefreshMappedTarget()
	{
		yield return new WaitForEndOfFrame();
		RefreshNavigationMappingCurrentlyMappedTarget();
	}

	private void RefreshNavigationMappingCurrentlyMappedTarget()
	{
		NavigationMappingTemplate component = GetComponent<NavigationMappingTemplate>();
		if (!(component != null))
		{
			return;
		}
		component.CurrentlyMappedTarget = null;
		List<PageController> pages = GetSubpagesList().Pages;
		if (pages.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < pages.Count; i++)
		{
			INavigationMappable[] componentsInChildren = pages[i].gameObject.GetComponentsInChildren<INavigationMappable>();
			if (componentsInChildren == null)
			{
				continue;
			}
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				Selectable selectable = componentsInChildren[j] as Selectable;
				if (component.CurrentlyMappedTarget == null && selectable.interactable)
				{
					component.CurrentlyMappedTarget = selectable;
				}
			}
		}
	}
}
