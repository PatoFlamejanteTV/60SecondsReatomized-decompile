using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class SecondsRationingManager : RationingManager
{
	[SerializeField]
	private TimeRationing _timeRationing;

	[SerializeField]
	private GlobalFloatVariable _waterConsumedVariable;

	[SerializeField]
	private GlobalFloatVariable _soupConsumedVariable;

	[SerializeField]
	private CurrentChallengeData _currentChallengeData;

	[SerializeField]
	private GlobalBoolVariable _isTutorial;

	private const float SERVING_SIZE = 0.25f;

	private static SecondsRationingManager _instance;

	public TimeRationing TimeRationing => _timeRationing;

	public new static SecondsRationingManager Instance => _instance;

	protected override void CustomStart()
	{
		base.CustomStart();
		_instance = this;
	}

	protected override void ConsumeRation(RationingData rationingData)
	{
		_timeRationing.IncrementTime();
		for (int i = 0; i < rationingData.Rations.Count; i++)
		{
			if (rationingData.Rations[i].RationedItem is ConsumableRemedium)
			{
				ConsumableRemedium consumableRemedium = (ConsumableRemedium)rationingData.Rations[i].RationedItem;
				if (consumableRemedium.StaticData.ItemId.Equals("item_water") && !_isTutorial.Value && _currentChallengeData.RuntimeData.Challenge == null)
				{
					_waterConsumedVariable.SetValue(_waterConsumedVariable.Value + 0.25f);
				}
				if (consumableRemedium.StaticData.ItemId.Equals("item_food") && !_isTutorial.Value && _currentChallengeData.RuntimeData.Challenge == null)
				{
					_soupConsumedVariable.SetValue(_soupConsumedVariable.Value + 0.25f);
				}
				_timeRationing.ResetLastRationingTime(consumableRemedium, rationingData.Rations[i].CharacterIndex);
			}
		}
	}
}
