using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class RemasterGoalManager : GoalManager
{
	[SerializeField]
	private GlobalBoolVariable _goalTabAttentionVariable;

	[SerializeField]
	private GlobalBoolVariable _goalTabVisibleVariable;

	protected override void StartGlowing()
	{
		if (_goalTabVisibleVariable != null)
		{
			_goalTabVisibleVariable.Value = true;
		}
		if (_goalTabAttentionVariable != null)
		{
			_goalTabAttentionVariable.Value = true;
		}
		_shouldGlow = true;
	}
}
