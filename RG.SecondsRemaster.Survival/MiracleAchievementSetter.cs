using FMOD.Studio;
using FMODUnity;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RG.SecondsRemaster.Survival;

public class MiracleAchievementSetter : OnClickSoundPlayer
{
	[SerializeField]
	private SurvivalData _survivalData;

	[SerializeField]
	private GlobalBoolVariable _wednesdayClicked;

	[SerializeField]
	private GlobalBoolVariable _sundayClicked;

	[SerializeField]
	private Achievement _miracle;

	[EventRef]
	[SerializeField]
	private string _firstSFX;

	[EventRef]
	[SerializeField]
	private string _secondSFX;

	[SerializeField]
	private int _clickToPlaySpecialSound = 3;

	[SerializeField]
	private EndOfDayListenerList _endOfDayListenerList;

	[SerializeField]
	private IItem _radio;

	private int _clickCounter;

	private EventInstance _playEvent;

	private const int WEDNESDAY = 3;

	private const int SUNDAY = 0;

	private const int WEEK = 7;

	private void Start()
	{
		_endOfDayListenerList.RegisterOnEndOfDay(ResetCounter, "Reset", 40, this);
	}

	private void OnDestroy()
	{
		_endOfDayListenerList.UnregisterOnEndOfDay(ResetCounter, "Reset");
	}

	private void ResetCounter()
	{
		_clickCounter = 0;
	}

	private void OnMouseUpAsButton()
	{
		if (EventSystem.current.IsPointerOverGameObject() || _soundSlot == null || !IsLastPlayFinished())
		{
			return;
		}
		if (_clickCounter == _clickToPlaySpecialSound - 1)
		{
			if (_survivalData.CurrentDay % 7 == 3 && !_wednesdayClicked.Value && !_radio.IsDamaged())
			{
				_wednesdayClicked.Value = true;
				_playEvent = AudioManager.PlaySoundAndReturnInstance(_firstSFX);
			}
			else if (_survivalData.CurrentDay % 7 == 0 && !_radio.IsDamaged())
			{
				_sundayClicked.Value = true;
				_playEvent = AudioManager.PlaySoundAndReturnInstance(_secondSFX);
			}
			else
			{
				PlayStandardRadio();
			}
			if (!AchievementsSystem.IsAchievementUnlocked(_miracle) && _wednesdayClicked.Value && _sundayClicked.Value)
			{
				AchievementsSystem.UnlockAchievement(_miracle);
			}
		}
		else
		{
			PlayStandardRadio();
		}
		_clickCounter++;
	}

	private bool IsLastPlayFinished()
	{
		_playEvent.getPlaybackState(out var state);
		return state == PLAYBACK_STATE.STOPPED;
	}

	private void PlayStandardRadio()
	{
		if (_soundSlot != null && !string.IsNullOrEmpty(_soundSlot.SoundEventName))
		{
			_playEvent = AudioManager.PlaySoundAndReturnInstance(_soundSlot.SoundEventName);
		}
	}
}
