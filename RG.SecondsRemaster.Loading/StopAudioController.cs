using RG.Parsecs.Common;
using UnityEngine;

namespace RG.SecondsRemaster.Loading;

public class StopAudioController : MonoBehaviour
{
	private void Start()
	{
		AudioManager.Instance.StopPlayingMusicFadeOut();
		AudioManager.Instance.StopPlayingSfxFadeOut();
		AudioManager.Instance.StopPlayingUiFadeOut();
	}
}
