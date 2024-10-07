using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class PlanExpeditionPageController : PageController
{
	[SerializeField]
	private ExpeditionData _expeditionData;

	[SerializeField]
	private GlobalBoolVariable _shouldDisplaySendExpeditionPageVariable;

	[SerializeField]
	private GlobalBoolVariable _isTutorialVariable;

	[SerializeField]
	private CanvasesList _canvases;

	[SerializeField]
	private Scheduler _scheduler;

	[SerializeField]
	private FunctionTextController[] _functionTextControllers;

	[SerializeField]
	private CharacterAvailabilityController[] _characterAvailabilityControllers;

	[SerializeField]
	private PlanExpeditionToggleController _planExpeditionToggleController;

	[SerializeField]
	private GlobalBoolVariable _attentionVariable;

	private bool _isVisible;

	public override void SetPageData(bool visible)
	{
		if (visible && _attentionVariable != null && _attentionVariable.Value)
		{
			_attentionVariable.Value = false;
		}
		if (CanRefreshPageToday())
		{
			SetPageNotRefreshableToday();
			for (int i = 0; i < _functionTextControllers.Length; i++)
			{
				_functionTextControllers[i].RefreshText();
			}
			for (int j = 0; j < _characterAvailabilityControllers.Length; j++)
			{
				_characterAvailabilityControllers[j].RefreshCharacterAvailability();
			}
			_planExpeditionToggleController.SetToggleWithoutInvokingValueChange(value: false);
		}
	}

	public override bool CanBeDisplayed()
	{
		if (base.CanBeDisplayed() && EndOfDayManager.Instance.AcutalDay > 1 && _expeditionData.RuntimeData.IsActive && !_expeditionData.RuntimeData.IsOngoing && !IsCurrentEventInForcedExpeditionList() && !_shouldDisplaySendExpeditionPageVariable.Value && !_isTutorialVariable.Value)
		{
			return CanBeDisplayedInDemo();
		}
		return false;
	}

	private bool CanBeDisplayedInDemo()
	{
		if (DemoManager.IS_DEMO_VERSION)
		{
			return EndOfDayManager.Instance.AcutalDay < 8;
		}
		return true;
	}

	private bool IsCurrentEventInForcedExpeditionList()
	{
		return _canvases.Contains(_scheduler.CurrentScheduledDay.SurvivalEvent);
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
