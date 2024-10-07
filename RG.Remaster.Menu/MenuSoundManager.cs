using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.Remaster.Menu;

public class MenuSoundManager : MonoBehaviour
{
	[SerializeField]
	private GlobalBoolVariable _isContinueAvailable;

	[SerializeField]
	private int _ambientParameterIndex;

	[Header("Default Menu")]
	[SerializeField]
	private SoundSlot _defaultMenuMusic;

	[SerializeField]
	private SoundSlot _defaultMenuAmbient;

	[Header("Alternative Menu")]
	[SerializeField]
	private SoundSlot _alternativeMenuMusic;

	[SerializeField]
	private SoundSlot _alternativeMenuAmbient;

	private const string AMBIENT_PARAMETER = "Ambient";

	private void Start()
	{
		PlayMusic();
		PlayAmbient();
	}

	private void PlayMusic()
	{
		if (_isContinueAvailable.Value)
		{
			AudioManager.PlaySound(_alternativeMenuMusic.SoundEventName);
		}
		else
		{
			AudioManager.PlaySound(_defaultMenuMusic.SoundEventName);
		}
	}

	private void PlayAmbient()
	{
		int num = Random.Range(0, _ambientParameterIndex);
		if (_isContinueAvailable.Value)
		{
			AudioManager.PlaySoundWithParameter(_alternativeMenuAmbient.SoundEventName, "Ambient", num);
		}
		else
		{
			AudioManager.PlaySoundWithParameter(_defaultMenuAmbient.SoundEventName, "Ambient", num);
		}
	}
}
