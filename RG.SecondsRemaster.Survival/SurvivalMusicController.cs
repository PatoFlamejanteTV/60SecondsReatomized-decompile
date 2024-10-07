using FMODUnity;
using RG.Parsecs.Common;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class SurvivalMusicController : MonoBehaviour
{
	[EventRef]
	[SerializeField]
	private string _musicEvent;

	private void Start()
	{
		AudioManager.Instance.PlayMusicFadeOut(_musicEvent);
	}
}
