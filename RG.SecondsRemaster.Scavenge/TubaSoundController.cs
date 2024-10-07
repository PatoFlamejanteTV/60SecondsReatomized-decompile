using FMOD.Studio;
using FMODUnity;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Scavenge;

public class TubaSoundController : MonoBehaviour
{
	[SerializeField]
	private GlobalBoolVariable _wasScavengeFailed;

	[SerializeField]
	private GlobalBoolVariable _isTubaDisabled;

	[EventRef]
	[SerializeField]
	private string _defaultTuba;

	[EventRef]
	[SerializeField]
	private string _sadTrombone;

	[EventRef]
	[SerializeField]
	private string _tubaSqueek;

	[SerializeField]
	private bool _isPlayedOnce;

	private EventInstance _eventInstance;

	private void Start()
	{
		_isPlayedOnce = false;
	}

	private void OnDisable()
	{
		_eventInstance.getPlaybackState(out var state);
		if (state == PLAYBACK_STATE.PLAYING)
		{
			_eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			AudioManager.PlaySoundAtPoint(_tubaSqueek, base.transform);
		}
	}

	public void PlayWithAudioManager()
	{
		if (!(_isTubaDisabled != null) || !_isTubaDisabled.Value)
		{
			if (_wasScavengeFailed.Value)
			{
				_eventInstance = RuntimeManager.CreateInstance(_defaultTuba);
				_eventInstance.set3DAttributes(base.gameObject.transform.To3DAttributes());
				_eventInstance.start();
				_eventInstance.release();
			}
			else if (!_isPlayedOnce)
			{
				_eventInstance = RuntimeManager.CreateInstance(_sadTrombone);
				_eventInstance.set3DAttributes(base.gameObject.transform.To3DAttributes());
				_eventInstance.start();
				_eventInstance.release();
				_isPlayedOnce = true;
			}
		}
	}
}
