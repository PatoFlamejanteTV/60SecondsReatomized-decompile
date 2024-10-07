using System.Collections.Generic;
using RG.Core.Base;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[CreateAssetMenu(fileName = "New Expedition Destination List", menuName = "60 Seconds Remaster!/New Expedition Destination List")]
public class ExpeditionDestinationList : RGScriptableObject
{
	[SerializeField]
	private List<ExpeditionDestinationListEntry> _expeditionDestinations;

	[Tooltip("This field is used as default destination for mutant character")]
	[SerializeField]
	private ExpeditionDestination _mutantExpeditionDestination;

	[SerializeField]
	private CharacterStatus _specialCharacterStatus;

	private const int ACTIVE_DESTINATIONS_THRESHOLD_TO_ACTIVATE_INACTIVE_DESTINATIONS = 2;

	public List<ExpeditionDestinationListEntry> ExpeditionDestinations => _expeditionDestinations;

	public ExpeditionDestination GetRandomDestination(Character character, bool isTutorial)
	{
		if (character == null)
		{
			return null;
		}
		int num = 0;
		for (int i = 0; i < _expeditionDestinations.Count; i++)
		{
			if (_expeditionDestinations[i].Destination.Enabled)
			{
				num++;
			}
		}
		if (num <= 2)
		{
			for (int j = 0; j < _expeditionDestinations.Count; j++)
			{
				if (!_expeditionDestinations[j].Destination.DynamicData.Enabled)
				{
					_expeditionDestinations[j].Destination.DynamicData.Enabled = true;
				}
			}
		}
		if (character.RuntimeData.HasStatus(_specialCharacterStatus.Id))
		{
			return _mutantExpeditionDestination;
		}
		List<ExpeditionDestination> list = new List<ExpeditionDestination>();
		for (int k = 0; k < _expeditionDestinations.Count; k++)
		{
			if (_expeditionDestinations[k].CanDestinationBeConsideredInExpedition(character) && _expeditionDestinations[k].AvailableInTutorial == isTutorial)
			{
				list.Add(_expeditionDestinations[k].Destination);
			}
		}
		return list[Random.Range(0, list.Count)];
	}
}
