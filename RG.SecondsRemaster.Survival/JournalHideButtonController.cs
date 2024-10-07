using Rewired;
using RG.VirtualInput;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class JournalHideButtonController : MonoBehaviour
{
	[SerializeField]
	private JournalController _journalController;

	[SerializeField]
	private VirtualInputClosablePanel _journal;

	[SerializeField]
	private VirtualInputClosablePanel _closed;

	[SerializeField]
	private Button _button;

	private Player _player;

	private float _buttonHeldDownTime;

	private bool _shouldCheck;

	public void Awake()
	{
		_player = ReInput.players.GetPlayer(0);
	}

	public void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (_player != null && _player.GetButtonDown(38))
		{
			_shouldCheck = _journalController.CurrentJournalState == JournalController.JournalState.VISIBLE || _journalController.CurrentJournalState == JournalController.JournalState.PARTIALLY_HIDDEN;
		}
		if (_player != null && _player.GetButton(38) && _shouldCheck)
		{
			_buttonHeldDownTime += Time.deltaTime;
		}
		if (_player != null && _player.GetButtonUp(38) && _shouldCheck)
		{
			if (_buttonHeldDownTime <= 0.17f)
			{
				_button.onClick.Invoke();
			}
			_buttonHeldDownTime = 0f;
		}
	}

	public void OnClick()
	{
		Activate();
	}

	public bool Show()
	{
		if (_journalController.CurrentJournalState == JournalController.JournalState.PARTIALLY_HIDDEN)
		{
			_journalController.Show();
			_closed.Hide();
			_journal.Show();
			return true;
		}
		return false;
	}

	public void Activate()
	{
		if (_journalController.CurrentJournalState != JournalController.JournalState.HIDE && _journalController.CurrentJournalState != 0 && !Show())
		{
			_journalController.PartiallyHide();
			_journal.Hide();
			_closed.Show();
		}
	}
}
