using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.Remaster.Menu;

public class FlyingObjectBackgroundSoundPlayer : MonoBehaviour
{
	[SerializeField]
	private SoundSlot _defaultFlyingObjectSoundSlot;

	[SerializeField]
	private SoundSlot _alternativeFlyingObjectSoundSlot;

	[SerializeField]
	private SoundSlot _invertedDefaultFlyingObjectSoundSlot;

	[SerializeField]
	private SoundSlot _invertedAlternativeFlyingObjectSoundSlot;

	[SerializeField]
	private GameObject _defaultFlyingObject;

	[SerializeField]
	private GameObject _alternativeFlyingObject;

	[SerializeField]
	private GlobalBoolVariable _isObjectInverted;

	public void PlaySound()
	{
		if (_defaultFlyingObject.activeInHierarchy)
		{
			if (_isObjectInverted.Value)
			{
				AudioManager.PlaySoundAndReturnInstance(_invertedDefaultFlyingObjectSoundSlot.SoundEventName);
			}
			else
			{
				AudioManager.PlaySoundAndReturnInstance(_defaultFlyingObjectSoundSlot.SoundEventName);
			}
		}
		else if (_alternativeFlyingObject.activeInHierarchy)
		{
			if (_isObjectInverted.Value)
			{
				AudioManager.PlaySoundAndReturnInstance(_invertedAlternativeFlyingObjectSoundSlot.SoundEventName);
			}
			else
			{
				AudioManager.PlaySoundAndReturnInstance(_alternativeFlyingObjectSoundSlot.SoundEventName);
			}
		}
	}
}
