using System.Collections.Generic;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class SettingsPanelController : MonoBehaviour
{
	[SerializeField]
	private List<CanvasGroup> _canvasGroupsToDisableOnSteamDeck = new List<CanvasGroup>();

	[SerializeField]
	private ControlsPanelController _controlsPanelController;

	private void Start()
	{
		if (SteamManager.IsRunningOnSteamDeck())
		{
			for (int i = 0; i <= _canvasGroupsToDisableOnSteamDeck.Count - 1; i++)
			{
				_canvasGroupsToDisableOnSteamDeck[i].alpha = 0.3f;
				_canvasGroupsToDisableOnSteamDeck[i].interactable = false;
				_canvasGroupsToDisableOnSteamDeck[i].blocksRaycasts = false;
			}
		}
	}

	private void OnDisable()
	{
		if (_controlsPanelController != null)
		{
			_controlsPanelController.SaveSettings();
		}
	}
}
