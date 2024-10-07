using RG.Parsecs.Common;
using RG.VirtualInput;
using UnityEngine;

namespace RG.Remaster.Scavenge;

public class RestartPanelController : MonoBehaviour
{
	public void Start()
	{
		Singleton<VirtualInputManager>.Instance.VisualManager.MouseCursorVisible = true;
	}

	public void Restart()
	{
		AudioManager.Instance.StopPlayingMusicFadeOut();
		AudioManager.Instance.StopPlayingSfxFadeOut();
		ResetGame.RestartLevel();
	}

	public void Exit()
	{
		AudioManager.Instance.StopPlayingMusicFadeOut();
		AudioManager.Instance.StopPlayingSfxFadeOut();
		Singleton<GameManager>.Instance.ResetGame();
	}
}
