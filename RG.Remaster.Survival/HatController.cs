using System.Collections;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using UnityEngine;
using UnityEngine.Serialization;

namespace RG.Remaster.Survival;

[RequireComponent(typeof(CharacterSlot))]
public class HatController : MonoBehaviour
{
	[SerializeField]
	private GlobalIntVariable _currentHatIndex;

	private int _previousHatIndex;

	[SerializeField]
	private HatDataList _hatList;

	[SerializeField]
	private GlobalBoolVariable _hatChangesAllowed;

	[SerializeField]
	private GlobalBoolVariable _wasHatChangedSuccessfully;

	private Character _character;

	private CharacterSlot _characterSlot;

	[FormerlySerializedAs("_soundSlot")]
	[SerializeField]
	private SoundSlot _successSoundSlot;

	[SerializeField]
	private SoundSlot _declineSoundSlot;

	[SerializeField]
	private Achievement _madHatterAchievement;

	public bool TestOnlyReverseHatClicked;

	private void Start()
	{
		_characterSlot = GetComponent<CharacterSlot>();
		_character = _characterSlot.GetCharacter();
		if (_currentHatIndex != null && _currentHatIndex.Value != 0)
		{
			_character.RuntimeData.AddStatus(_hatList.HatData[_currentHatIndex.Value].IsWornStatus);
		}
	}

	public void HatClicked(bool findNext = false)
	{
		if (_hatChangesAllowed == null || _hatChangesAllowed.Value)
		{
			if (_hatList != null && _hatList.HatData != null && _hatList.HatData.Count > 1)
			{
				RemoveHatRelatedStatusesFromCharacter();
				_previousHatIndex = _currentHatIndex.Value;
				if (findNext)
				{
					IncrementHatIndex();
				}
				else
				{
					DecrementHatIndex();
				}
				_characterSlot.OnEndOfDay();
			}
		}
		else
		{
			PlayDeclineSound();
		}
	}

	private void IncrementHatIndex()
	{
		_currentHatIndex.Value++;
		if (_currentHatIndex.Value < _hatList.HatData.Count)
		{
			if (_hatList.HatData[_currentHatIndex.Value].IsUnlockedVariable.Value && _hatList.HatData[_currentHatIndex.Value].AllowedCharacters.Contains(_character) && !_hatList.HatData[_currentHatIndex.Value].DisallowedMissions.Contains(MissionManager.Instance.GetCurrentMission()))
			{
				if (!(_hatList.HatData[_currentHatIndex.Value].IsWornStatus != null) || !(_character != null))
				{
					return;
				}
				_character.RuntimeData.AddStatus(_hatList.HatData[_currentHatIndex.Value].IsWornStatus);
				if (_wasHatChangedSuccessfully != null)
				{
					_wasHatChangedSuccessfully.SetValue(value: true);
					if (!AchievementsSystem.IsAchievementUnlocked(_madHatterAchievement))
					{
						AchievementsSystem.UnlockAchievement(_madHatterAchievement);
					}
					if (_successSoundSlot != null && !string.IsNullOrEmpty(_successSoundSlot.SoundEventName))
					{
						Singleton<GameManager>.Instance.PlaySoundInvoke(PlaySound(_successSoundSlot.SoundEventName));
					}
				}
			}
			else
			{
				IncrementHatIndex();
			}
		}
		else
		{
			_currentHatIndex.Value = 0;
			if (_currentHatIndex.Value != _previousHatIndex)
			{
				PlaySuccessSound();
			}
			else
			{
				PlayDeclineSound();
			}
		}
	}

	private void DecrementHatIndex()
	{
		_currentHatIndex.Value--;
		if (_currentHatIndex.Value == 0)
		{
			if (_currentHatIndex.Value != _previousHatIndex)
			{
				PlaySuccessSound();
			}
			else
			{
				PlayDeclineSound();
			}
		}
		else if (_currentHatIndex.Value > 0)
		{
			if (_hatList.HatData[_currentHatIndex.Value].IsUnlockedVariable.Value && _hatList.HatData[_currentHatIndex.Value].AllowedCharacters.Contains(_character) && !_hatList.HatData[_currentHatIndex.Value].DisallowedMissions.Contains(MissionManager.Instance.GetCurrentMission()))
			{
				if (!(_hatList.HatData[_currentHatIndex.Value].IsWornStatus != null) || !(_character != null))
				{
					return;
				}
				_character.RuntimeData.AddStatus(_hatList.HatData[_currentHatIndex.Value].IsWornStatus);
				if (_wasHatChangedSuccessfully != null)
				{
					_wasHatChangedSuccessfully.SetValue(value: true);
					if (!AchievementsSystem.IsAchievementUnlocked(_madHatterAchievement))
					{
						AchievementsSystem.UnlockAchievement(_madHatterAchievement);
					}
					if (_successSoundSlot != null && !string.IsNullOrEmpty(_successSoundSlot.SoundEventName))
					{
						Singleton<GameManager>.Instance.PlaySoundInvoke(PlaySound(_successSoundSlot.SoundEventName));
					}
				}
			}
			else
			{
				DecrementHatIndex();
			}
		}
		else
		{
			_currentHatIndex.Value = _hatList.HatData.Count;
			DecrementHatIndex();
		}
	}

	public IEnumerator PlaySound(string eventName)
	{
		AudioManager.PlaySoundAndReturnInstance(eventName);
		yield return null;
	}

	private void PlaySuccessSound()
	{
		if (_successSoundSlot != null && !string.IsNullOrEmpty(_successSoundSlot.SoundEventName))
		{
			Singleton<GameManager>.Instance.PlaySoundInvoke(PlaySound(_successSoundSlot.SoundEventName));
		}
	}

	private void PlayDeclineSound()
	{
		if (_declineSoundSlot != null && !string.IsNullOrEmpty(_declineSoundSlot.SoundEventName))
		{
			Singleton<GameManager>.Instance.PlaySoundInvoke(PlaySound(_declineSoundSlot.SoundEventName));
		}
	}

	private void RemoveHatRelatedStatusesFromCharacter()
	{
		if (!(_character != null))
		{
			return;
		}
		for (int i = 0; i < _hatList.HatData.Count; i++)
		{
			if (_hatList.HatData[i].IsWornStatus != null && _character.RuntimeData.HasStatus(_hatList.HatData[i].IsWornStatus.Id))
			{
				_character.RuntimeData.RemoveStatus(_hatList.HatData[i].IsWornStatus);
			}
		}
	}
}
