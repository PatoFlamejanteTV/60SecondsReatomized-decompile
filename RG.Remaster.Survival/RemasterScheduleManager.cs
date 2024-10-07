using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.Remaster.Survival;

public class RemasterScheduleManager : SchedulerManager
{
	[SerializeField]
	private GlobalBoolVariable _isGameShouldBeSave;

	[SerializeField]
	private GlobalBoolVariable _allowSurvivalInteraction;

	[SerializeField]
	private EndGameData _endGameData;

	[SerializeField]
	private GlobalBoolVariable _isContinueAvailable;

	[SerializeField]
	private NodeFunction _metagameFunction;

	private new void Start()
	{
		_saveFirstDay = _isGameShouldBeSave.Value && !_endGameData.RuntimeData.ShouldEndGame && _allowSurvivalInteraction.Value;
		_metagameFunction.Execute(null);
		if (_isContinueAvailable != null && _saveFirstDay)
		{
			_isContinueAvailable.Value = true;
		}
		base.Start();
	}
}
