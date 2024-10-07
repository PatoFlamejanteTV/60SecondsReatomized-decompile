using RG.Parsecs.Common;
using Steamworks;
using UnityEngine;

public class DemoSurvivalBlockedPopupController : MonoBehaviour
{
	private const string SHOW_POPUP_TRIGGER_NAME = "ShowPopup";

	[SerializeField]
	private Animator _survivalPopupAnimator;

	public void OnEnable()
	{
		_survivalPopupAnimator.SetTrigger("ShowPopup");
	}

	public void ClickedGoBackToMenu()
	{
		AudioManager.Instance.StopPlayingMusicFadeOut();
		AudioManager.Instance.StopPlayingSfxFadeOut();
		Singleton<GameManager>.Instance.ResetGame();
	}

	public void ClickedGoToStore()
	{
		if (SteamManager.Initialized && SteamAPI.IsSteamRunning())
		{
			SteamFriends.ActivateGameOverlayToStore(new AppId_t(DemoManager.REATOMIZED_FULL_VERSION_APP_ID), EOverlayToStoreFlag.k_EOverlayToStoreFlag_None);
		}
		else
		{
			Application.OpenURL("https://store.steampowered.com/app/1012880/60_Seconds_Reatomized/");
		}
	}
}
