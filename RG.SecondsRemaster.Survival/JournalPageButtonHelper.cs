using System.Collections;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

[RequireComponent(typeof(Button))]
public class JournalPageButtonHelper : MonoBehaviour
{
	[SerializeField]
	private JournalController _journalController;

	[SerializeField]
	private float _timeNeededToClick = 1f;

	[SerializeField]
	private Image _fillImage;

	[SerializeField]
	private bool _isNextPageButton;

	[SerializeField]
	private bool _ignoreJournal;

	private Button _button;

	private Player _player;

	private bool _startedOnThisButton;

	private float _timePassed;

	private void Awake()
	{
		_player = ReInput.players.GetPlayer(0);
		_button = GetComponent<Button>();
	}

	public void Update()
	{
		int actionId = (_isNextPageButton ? 39 : 40);
		if (_player.GetButtonDown(actionId) && _button.interactable && (_journalController.CurrentJournalState == JournalController.JournalState.VISIBLE || _ignoreJournal))
		{
			_startedOnThisButton = true;
		}
		if (_player.GetButton(actionId) && _startedOnThisButton)
		{
			_timePassed += Time.deltaTime;
			_fillImage.fillAmount = _timePassed / _timeNeededToClick;
			if (_timePassed >= _timeNeededToClick)
			{
				_button.onClick.Invoke();
				if (!_ignoreJournal)
				{
					_journalController.StartCoroutine(WaitTimeAndResetFillAmount());
				}
				_timePassed = 0f;
				_startedOnThisButton = false;
			}
		}
		if (_player.GetButtonUp(actionId))
		{
			_fillImage.fillAmount = 0f;
			_timePassed = 0f;
			_startedOnThisButton = false;
		}
	}

	public void ResetButton()
	{
		_fillImage.fillAmount = 0f;
		_timePassed = 0f;
		_startedOnThisButton = false;
	}

	private IEnumerator WaitTimeAndResetFillAmount()
	{
		yield return new WaitForSeconds(0.15f);
		_fillImage.fillAmount = 0f;
	}
}
