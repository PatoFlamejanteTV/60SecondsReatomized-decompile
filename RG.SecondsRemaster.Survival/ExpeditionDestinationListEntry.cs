using System;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public class ExpeditionDestinationListEntry
{
	[SerializeField]
	private ExpeditionDestination _expeditionDestination;

	[SerializeField]
	private Character[] _requiredCharacters;

	[SerializeField]
	private CharacterStatus[] _requiredStatuses;

	[SerializeField]
	private bool _availableInTutorial;

	public ExpeditionDestination Destination => _expeditionDestination;

	public Character[] RequiredCharacters => _requiredCharacters;

	public CharacterStatus[] RequiredStatuses => _requiredStatuses;

	public bool AvailableInTutorial => _availableInTutorial;

	public bool CanDestinationBeConsideredInExpedition(Character expeditionCharacter)
	{
		bool flag = _requiredCharacters == null || _requiredCharacters.Length == 0;
		if (_requiredCharacters != null)
		{
			for (int i = 0; i < _requiredCharacters.Length; i++)
			{
				if (expeditionCharacter == _requiredCharacters[i])
				{
					flag = true;
					break;
				}
			}
		}
		bool flag2 = _requiredStatuses == null || _requiredStatuses.Length == 0;
		if (_requiredStatuses != null)
		{
			for (int j = 0; j < _requiredStatuses.Length; j++)
			{
				if (expeditionCharacter.RuntimeData.HasStatus(_requiredStatuses[j].Id))
				{
					flag2 = true;
					break;
				}
			}
		}
		if (flag && flag2)
		{
			return _expeditionDestination.Enabled;
		}
		return false;
	}
}
