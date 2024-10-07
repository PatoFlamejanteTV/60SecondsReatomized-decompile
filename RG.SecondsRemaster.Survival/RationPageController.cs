using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class RationPageController : PageController
{
	[SerializeField]
	private JournalRationingHeadController[] _heads;

	[SerializeField]
	private RationAllController[] _rationAllControllers;

	[SerializeField]
	private GlobalBoolVariable _attentionVariable;

	public override void SetPageData(bool visible)
	{
		if (visible && _attentionVariable != null && _attentionVariable.Value)
		{
			_attentionVariable.Value = false;
		}
		for (int i = 0; i < _heads.Length; i++)
		{
			_heads[i].UpdateMedkitScratch();
		}
		if (CanRefreshPageToday())
		{
			for (int j = 0; j < _heads.Length; j++)
			{
				_heads[j].ResetHead();
				_heads[j].UpdateHeadVisibility();
				_heads[j].SetScratchVisibility();
			}
			for (int k = 0; k < _rationAllControllers.Length; k++)
			{
				_rationAllControllers[k].UpdateFill();
			}
			SetPageNotRefreshableToday();
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

	private void OnEnable()
	{
		for (int i = 0; i < _rationAllControllers.Length; i++)
		{
			_rationAllControllers[i].UpdateFill();
		}
	}
}
