using System;
using RG.Parsecs.Survival;
using UnityEngine;
using UnityEngine.Events;

namespace RG.SecondsRemaster.Survival;

public class CharacterAvailabilityController : MonoBehaviour
{
	[Serializable]
	public class OnRefreshCharacterAvailability : UnityEvent<bool>
	{
	}

	[SerializeField]
	private Character _character;

	[SerializeField]
	private bool _refreshOnStart;

	[SerializeField]
	private bool _checkExpeditionSendPossibility;

	[SerializeField]
	private OnRefreshCharacterAvailability _onCharacterAvailabilityChange;

	private void Start()
	{
		if (_refreshOnStart)
		{
			RefreshCharacterAvailability();
		}
	}

	public void RefreshCharacterAvailability()
	{
		bool flag = CharacterManager.Instance.GetCharacterList().CharactersInGame.Contains(_character) && _character.RuntimeData.IsAlive() && _character.RuntimeData.IsDrawnOnShip() && !_character.RuntimeData.IsOnExpedition();
		if (_checkExpeditionSendPossibility)
		{
			flag = flag && !_character.RuntimeData.HasStatusPreventingGoingOnExpeditions();
		}
		_onCharacterAvailabilityChange.Invoke(flag);
	}
}
