using System.Collections.Generic;
using RG.Core.Base;
using RG.Core.SaveSystem;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[CreateAssetMenu(fileName = "New Time Rationing", menuName = "60 Seconds Remaster!/Characters/New Time Rationing")]
public class TimeRationing : RGScriptableObject, ISaveable
{
	private const int RESET_TIME_VALUE = 0;

	[SerializeField]
	private SaveEvent _saveEvent;

	[SerializeField]
	private List<TimeRationingStruct> _timeRationing;

	[SerializeField]
	private CharacterList _characterList;

	private const int CHARACTER_NOT_FOUND = -1;

	public string ID => Guid;

	private void OnEnable()
	{
		_characterList.RegisterCharacterListChangedListener(OnCharacterListChange);
		Register();
	}

	public int GetLastRationingTime(ConsumableRemedium consumableRemedium, Character character)
	{
		int charIndex = GetCharIndex(character);
		if (charIndex == -1)
		{
			return 0;
		}
		for (int i = 0; i < _timeRationing.Count; i++)
		{
			if (_timeRationing[i].ConsumableRemedium == consumableRemedium)
			{
				return _timeRationing[i].Characters[charIndex];
			}
		}
		return 0;
	}

	private int GetCharIndex(Character character)
	{
		for (int i = 0; i < _characterList.CharactersInGame.Count; i++)
		{
			if (_characterList.CharactersInGame[i] == character)
			{
				return i;
			}
		}
		return -1;
	}

	public void ResetLastRationingTime(ConsumableRemedium consumableRemedium, int characterIndex)
	{
		SetLastRationingTime(consumableRemedium, characterIndex, 0);
	}

	public void SetLastRationingTime(ConsumableRemedium consumableRemedium, Character character, int value)
	{
		SetLastRationingTime(consumableRemedium, GetCharIndex(character), value);
	}

	public void SetLastRationingTime(ConsumableRemedium consumableRemedium, int characterIndex, int value)
	{
		if (characterIndex == -1)
		{
			return;
		}
		for (int i = 0; i < _timeRationing.Count; i++)
		{
			if (_timeRationing[i].ConsumableRemedium == consumableRemedium)
			{
				_timeRationing[i].Characters[characterIndex] = value;
				break;
			}
		}
	}

	public void IncrementTime()
	{
		for (int i = 0; i < _timeRationing.Count; i++)
		{
			for (int j = 0; j < _characterList.GetCharacterCount(); j++)
			{
				_timeRationing[i].Characters[j]++;
			}
		}
	}

	private void OnCharacterListChange()
	{
		int num = _characterList.GetCharacterCount() - 1;
		if (num < 4)
		{
			for (int i = 0; i < _timeRationing.Count; i++)
			{
				_timeRationing[i].Characters[num] = 0;
			}
		}
	}

	public string Serialize()
	{
		List<TimeRationingWrapper> list = new List<TimeRationingWrapper>();
		for (int i = 0; i < _timeRationing.Count; i++)
		{
			list.Add(new TimeRationingWrapper
			{
				ConsumableRemediumID = _timeRationing[i].ConsumableRemedium.ID,
				Characters = new List<int>(_timeRationing[i].Characters)
			});
		}
		return JsonUtility.ToJson(list);
	}

	public void Deserialize(string jsonData)
	{
		List<TimeRationingWrapper> list = JsonUtility.FromJson<List<TimeRationingWrapper>>(jsonData);
		for (int i = 0; i < list.Count; i++)
		{
			for (int j = 0; j < _timeRationing.Count; j++)
			{
				if (_timeRationing[j].ConsumableRemedium.ID == list[i].ConsumableRemediumID)
				{
					_timeRationing[j].Characters.Clear();
					_timeRationing[j].Characters.AddRange(list[i].Characters);
				}
			}
		}
	}

	public void Register()
	{
		_saveEvent.RegisterListener(this);
	}

	public void Unregister()
	{
		_saveEvent.UnregisterListener(this);
	}

	public void ResetData()
	{
		for (int i = 0; i < _timeRationing.Count; i++)
		{
			for (int j = 0; j < _timeRationing[i].Characters.Count; j++)
			{
				_timeRationing[i].Characters[j] = 0;
			}
		}
	}
}
