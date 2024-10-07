using Rewired;
using RG.Parsecs.Common;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class ClosePanelOnCancelPress : MonoBehaviour
{
	[SerializeField]
	private GameObject _panelToClose;

	[SerializeField]
	private OnUIClickedSoundPlayer _soundEmitter;

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
			_panelToClose.SetActive(value: false);
			if (_soundEmitter != null)
			{
				_soundEmitter.PlaySound();
			}
		}
	}

	public void SetCloseActionBlocked(bool block)
	{
		_blocked = block;
	}
}
