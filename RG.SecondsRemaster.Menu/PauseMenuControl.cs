using Rewired;
using RG.Core.SaveSystem;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Menu;
using RG.VirtualInput;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RG.SecondsRemaster.Menu;

public class PauseMenuControl : BasePauseMenu
{
	public enum EGameType
	{
		NONE,
		SCAVENGE,
		SURVIVAL
	}

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private GameObject _pauseMenuObject;

	[SerializeField]
	private GameObject[] _scavengeOnlyObjects;

	[SerializeField]
	private GameObject _cursorSensitivitySlider;

	[SerializeField]
	private GlobalIntVariable _controlModeVariable;

	[SerializeField]
	private bool _pauseOnFocusLostInEditor;

	[SerializeField]
	private UnityEvent _onShowEvent;

	[SerializeField]
	private UnityEvent _onHideEvent;

	private EGameType _previousGameType;

	private bool _visible;

	private static readonly int ShowHash = Animator.StringToHash("Show");

	private static readonly int HideHash = Animator.StringToHash("Hide");

	private const int PLAYER_INDEX = 0;

	public static bool IS_AFTER_PAUSE = false;

	private Player _player;

	protected Callback<GameOverlayActivated_t> _gameOverlayActivated;

	public bool Paused { get; private set; }

	public EGameType GameType { get; set; }

	private void Start()
	{
		_player = ReInput.players.GetPlayer(0);
		if (SteamManager.Initialized)
		{
			_gameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
		}
	}

	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		if (pCallback.m_bActive != 0)
		{
			HandlePause(pause: true);
			Debug.Log("Steam Overlay has been activated");
		}
		else
		{
			Debug.Log("Steam Overlay has been closed");
		}
	}

	public void ExitToMenu()
	{
		if (Paused)
		{
			HandlePause(pause: false);
		}
		AudioManager.Instance.StopPlayingMusicFadeOut();
		AudioManager.Instance.StopPlayingSfxFadeOut();
		Singleton<GameManager>.Instance.ResetGame();
	}

	public void BackToGame()
	{
		HandlePause(pause: false);
	}

	public void RestartScavenge()
	{
		if (Paused)
		{
			HandlePause(pause: false);
		}
		AudioManager.Instance.StopPlayingMusicFadeOut();
		AudioManager.Instance.StopPlayingSfxFadeOut();
		ResetGame.RestartLevel();
	}

	public void Show()
	{
		if (!_visible)
		{
			_visible = true;
			_animator.SetTrigger(ShowHash);
			if (_onShowEvent != null)
			{
				_onShowEvent.Invoke();
			}
		}
	}

	public void Hide()
	{
		if (_visible)
		{
			_visible = false;
			_animator.SetTrigger(HideHash);
			if (_onHideEvent != null)
			{
				_onHideEvent.Invoke();
			}
		}
	}

	public void Update()
	{
		if (_player.GetButtonDown(12) || (Paused && _player.GetButtonDown(30)))
		{
			HandlePause(!Paused);
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus)
		{
			SetPause(paused: true);
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			SetPause(paused: true);
		}
	}

	private void SetVisibilityOfScavengeMenuObjects(bool active)
	{
		for (int i = 0; i < _scavengeOnlyObjects.Length; i++)
		{
			_scavengeOnlyObjects[i].SetActive(active);
		}
		if (!active && _controlModeVariable.Value == 3)
		{
			_cursorSensitivitySlider.SetActive(value: true);
		}
		else
		{
			_cursorSensitivitySlider.SetActive(value: false);
		}
	}

	private void HandlePause(bool pause)
	{
		if (pause && (currentGameState != EGameState.GamePlay || Singleton<GameManager>.Instance.IsLoadingObscurerVisible))
		{
			return;
		}
		if (Paused && !pause)
		{
			if (GameType == EGameType.SCAVENGE)
			{
				IS_AFTER_PAUSE = true;
			}
			Time.timeScale = 1f;
			Paused = false;
			Hide();
			if (GameType == EGameType.SCAVENGE)
			{
				Singleton<VirtualInputManager>.Instance.VisualManager.MouseCursorVisible = false;
			}
			if (GameType == EGameType.SCAVENGE)
			{
				AudioManager.Instance.SetMusicPaused(Paused);
				AudioManager.Instance.SetSfxPaused(Paused);
			}
			AudioListener.pause = Paused;
			SaveSettings();
			EventSystem.current.SetSelectedGameObject(null);
		}
		else if (!Paused && pause)
		{
			_pauseMenuObject.SetActive(value: true);
			Paused = true;
			Show();
			if (GameType == EGameType.SCAVENGE)
			{
				Singleton<VirtualInputManager>.Instance.VisualManager.MouseCursorVisible = true;
			}
			if (GameType != _previousGameType)
			{
				SetVisibilityOfScavengeMenuObjects(GameType == EGameType.SCAVENGE);
			}
			if (GameType == EGameType.SCAVENGE)
			{
				AudioManager.Instance.SetMusicPaused(Paused);
				AudioManager.Instance.SetSfxPaused(Paused);
			}
			AudioListener.pause = Paused;
		}
	}

	public override void TogglePauseGame()
	{
		HandlePause(!Paused);
	}

	public override void SetPause(bool paused)
	{
		HandlePause(paused);
	}

	public void SaveSettings()
	{
		StorageDataManager.TheInstance.Save("settings", delegate
		{
			Debug.Log("Saved Settings.");
		}, null);
	}
}
