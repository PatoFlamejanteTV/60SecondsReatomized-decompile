using Rewired;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class GoBackOnCancelPress : MonoBehaviour
{
	[SerializeField]
	private ScreensController _screensController;

	private Player _player;

	private bool _blocked;

	private void Awake()
	{
		_player = ReInput.players.GetPlayer(0);
	}

	private void Update()
	{
		if (!_blocked && _player.GetButtonDown(30))
		{
			_screensController.GoBack();
		}
	}

	public void SetCloseActionBlocked(bool block)
	{
		_blocked = block;
	}
}
