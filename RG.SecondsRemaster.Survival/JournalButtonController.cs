using System.Collections;
using Rewired;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using RG.VirtualInput;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class JournalButtonController : MonoBehaviour, IRefreshInteractable
{
	[SerializeField]
	private JournalController _journalController;

	[SerializeField]
	private float _journalShowDelay = 1.5f;

	[SerializeField]
	private float _journalFirstDayShowDelay = 4.5f;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private EndGameData _endGameData;

	[SerializeField]
	private GlobalBoolVariable _showJournal;

	[SerializeField]
	private GameObject[] _normalJournalButtonObjects;

	[SerializeField]
	private GameObject[] _destroyedJournalButtonObjects;

	[SerializeField]
	private GlobalBoolVariable _displayJournalDestroyed;

	[SerializeField]
	private Button _button;

	[SerializeField]
	private VirtualInputButton _gamepadButton;

	[SerializeField]
	private SurvivalPrompt _showJournalPrompt;

	[Header("Demo")]
	[SerializeField]
	private GlobalBoolVariable _isDemoVariable;

	[SerializeField]
	private SurvivalData _survivalData;

	[SerializeField]
	private GlobalBoolVariable _isTutorialVariable;

	[SerializeField]
	private GameObject _survivalDemoEndPopup;

	private const string SHOW_TRIGGER_NAME = "Show";

	private const string HIDE_TRIGGER_NAME = "Hide";

	private bool _isInteractable;

	private Player _player;

	public void Start()
	{
		_isInteractable = false;
		Invoke("ShowDelayed", _journalFirstDayShowDelay);
		_player = ReInput.players.GetPlayer(0);
	}

	public void Update()
	{
		if (Time.timeScale != 0f && _player != null && _button.interactable && _player.GetButtonDown(38))
		{
			StartCoroutine(WaitFrameAndClickJournal());
		}
	}

	private IEnumerator WaitFrameAndClickJournal()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		_button.onClick.Invoke();
	}

	public void OnClick()
	{
		_button.interactable = false;
		_isInteractable = false;
		if (_isDemoVariable.Value && _survivalData.CurrentDay >= 11 && !_isTutorialVariable.Value)
		{
			_survivalDemoEndPopup.SetActive(value: true);
		}
		else if (!_showJournal.Value)
		{
			_endGameData.RuntimeData.ShouldEndGame = true;
			EndGameManager.Instance.LoadEndGameScene();
		}
		else
		{
			EventSystem.current.SetSelectedGameObject(null);
			_journalController.Show();
		}
		_showJournalPrompt.gameObject.SetActive(value: false);
		_animator.SetTrigger("Hide");
	}

	public void Show()
	{
		if (_displayJournalDestroyed != null)
		{
			SetJournalGraphics(_displayJournalDestroyed.Value);
		}
		_showJournalPrompt.gameObject.SetActive(value: true);
		Invoke("ShowDelayed", _journalShowDelay);
	}

	private void ShowDelayed()
	{
		_button.interactable = true;
		_isInteractable = true;
		_animator.SetTrigger("Show");
		_gamepadButton.SelectThisSelectable();
	}

	public void Hide()
	{
		_showJournalPrompt.gameObject.SetActive(value: false);
		_animator.SetTrigger("Hide");
	}

	public void RefreshInteractable()
	{
		_button.interactable = _isInteractable;
		_gamepadButton.SetInteractable();
	}

	public void SetJournalGraphics(bool isDestroyed)
	{
		for (int i = 0; i < _normalJournalButtonObjects.Length; i++)
		{
			_normalJournalButtonObjects[i].SetActive(!isDestroyed);
		}
		for (int j = 0; j < _destroyedJournalButtonObjects.Length; j++)
		{
			_destroyedJournalButtonObjects[j].SetActive(isDestroyed);
		}
	}
}
