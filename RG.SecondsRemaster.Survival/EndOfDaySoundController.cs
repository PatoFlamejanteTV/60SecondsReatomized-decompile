using RG.Parsecs.Common;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.EventEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class EndOfDaySoundController : MonoBehaviour
{
	[SerializeField]
	private CurrentSoundToPlay[] _currentSoundToPlay;

	[SerializeField]
	protected EndOfDayListenerList _endOfDayListenerList;

	private int _soundIndex;

	private void OnEnable()
	{
		_endOfDayListenerList.RegisterOnEndOfDay(RegisterCurrentSoundToPlay, "Visuals", 9, this);
	}

	private void OnDisable()
	{
		_endOfDayListenerList.UnregisterOnEndOfDay(RegisterCurrentSoundToPlay, "Visuals");
	}

	private void RegisterCurrentSoundToPlay()
	{
		for (int i = 0; i < _currentSoundToPlay.Length && !(_currentSoundToPlay[i].EventName == string.Empty); i++)
		{
			if (_currentSoundToPlay[i].OffsetCheck)
			{
				Invoke("PlaySound", _currentSoundToPlay[i].Offset);
			}
			else
			{
				PlaySound();
			}
		}
	}

	private void PlaySound()
	{
		AudioManager.PlaySound(_currentSoundToPlay[_soundIndex].EventName, _currentSoundToPlay[_soundIndex].Volume);
		_currentSoundToPlay[_soundIndex].ResetData();
	}
}
