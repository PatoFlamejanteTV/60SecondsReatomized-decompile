using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class OnInvokePlaySound : MonoBehaviour
{
	[SerializeField]
	private SoundSlot _soundSlot;

	[SerializeField]
	private int _offSet;

	[SerializeField]
	private GlobalBoolVariable _isTubaDisabled;

	private void Start()
	{
		if (_isTubaDisabled != null)
		{
			_isTubaDisabled.Value = true;
		}
		StopSounds();
		Invoke("PlaySound", _offSet);
	}

	private void PlaySound()
	{
		AudioManager.PlaySound(_soundSlot.SoundEventName);
	}

	private void StopSounds()
	{
		AudioManager.Instance.StopPlayingMusicFadeOut();
		AudioManager.Instance.StopPlayingSfxFadeOut();
		AudioManager.Instance.StopPlayingUiFadeOut();
	}
}
