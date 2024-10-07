using System.Collections.Generic;
using RG.Parsecs.Common;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class RemasterExpeditionManager : ExpeditionManager
{
	[SerializeField]
	private EndGameData _endGameData;

	protected override void Awake()
	{
		if (ExpeditionManager.Instance == null)
		{
			ExpeditionManager.Instance = this;
		}
		_endOfDayListenerList.RegisterOnEndOfDay(base.SendExpeditionIfPlannedAndResetView, "PreEventSystems", 4, this);
		_endOfDayListenerList.RegisterOnEndOfDay(base.RevokeExpeditionAndUpdateExpeditionTime, "Update", 10, this);
		_endOfDayListenerList.RegisterOnEndOfDay(base.LandOnPlanet, "SystemEvents", 25, this);
		_endOfDayListenerList.RegisterOnEndOfDay(AddFailedExpeditionStatEntry, "Reset", 998, this);
		_statsManager = StatsManager.Instance;
	}

	protected override void OnDestroy()
	{
		_endOfDayListenerList.UnregisterOnEndOfDay(base.SendExpeditionIfPlannedAndResetView, "PreEventSystems");
		_endOfDayListenerList.UnregisterOnEndOfDay(base.RevokeExpeditionAndUpdateExpeditionTime, "Update");
		_endOfDayListenerList.UnregisterOnEndOfDay(base.LandOnPlanet, "SystemEvents");
		_endOfDayListenerList.UnregisterOnEndOfDay(AddFailedExpeditionStatEntry, "Reset");
	}

	private void AddFailedExpeditionStatEntry()
	{
		if (_endGameData != null && _endGameData.RuntimeData.ShouldEndGame && _expeditionData.RuntimeData.IsOngoing && _expeditionData.RuntimeData.OngoingExpeditionData.ExpeditionCharacter.RuntimeData.HasStatusPreventingReturnFromExpedition())
		{
			ExpeditionStatsEntry expeditionStatsEntry = new ExpeditionStatsEntry();
			expeditionStatsEntry.DestinationId = _expeditionData.RuntimeData.OngoingExpeditionData.ChosenDestination.StaticData.Id;
			expeditionStatsEntry.CharacterId = _expeditionData.RuntimeData.OngoingExpeditionData.ExpeditionCharacter.ID;
			expeditionStatsEntry.IdsOfItemsTaken = new List<string>();
			for (int i = 0; i < _expeditionData.RuntimeData.OngoingExpeditionData.ExpeditionItems.Count; i++)
			{
				expeditionStatsEntry.IdsOfItemsTaken.Add(_expeditionData.RuntimeData.OngoingExpeditionData.ExpeditionItems[i].BaseStaticData.ItemId);
			}
			expeditionStatsEntry.IsSuccessful = false;
			_statsManager.AddExpeditionStatsEntry(expeditionStatsEntry);
		}
	}
}
