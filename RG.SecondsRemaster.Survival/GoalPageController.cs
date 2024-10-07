using System.Collections.Generic;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class GoalPageController : PageController
{
	[SerializeField]
	private GlobalBoolVariable _attentionVariable;

	[SerializeField]
	private JournalContentsList _journalContentsList;

	[SerializeField]
	private EventsRendererController _eventsRendererController;

	public void RenderPages()
	{
		if (_endGameData.RuntimeData.ShouldEndGame)
		{
			AddGoalList(GoalManager.Instance.AchievedGoals, isFailed: false, isAchieved: true);
			AddGoalList(GoalManager.Instance.ActiveGoals, isFailed: true, isAchieved: false);
			AddGoalList(GoalManager.Instance.FailedGoals, isFailed: true, isAchieved: false);
		}
		else
		{
			AddGoalList(GoalManager.Instance.ActiveGoals, isFailed: false, isAchieved: false);
			AddGoalList(GoalManager.Instance.AchievedGoals, isFailed: false, isAchieved: true);
			AddGoalList(GoalManager.Instance.FailedGoals, isFailed: true, isAchieved: false);
		}
		_eventsRendererController.RenderContents();
	}

	public override void SetPageData(bool visible)
	{
		if (visible && _attentionVariable != null && _attentionVariable.Value)
		{
			_attentionVariable.Value = false;
		}
		if (CanRefreshPageToday() && !(GoalManager.Instance == null))
		{
			SetPageNotRefreshableToday();
		}
	}

	public override bool CanBeDisplayed()
	{
		return true;
	}

	private void AddGoalList(List<Goal> goals, bool isFailed, bool isAchieved)
	{
		for (int i = 0; i < goals.Count; i++)
		{
			Goal goal = goals[i];
			if (goal.IsVisible)
			{
				_journalContentsList.AddJournalContent(new GoalJournalContent(goal, isFailed, isAchieved, 0));
			}
		}
	}
}
