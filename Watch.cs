using System.Collections;
using RG.Parsecs.Common;
using RG.SecondsRemaster.Scavenge;
using UnityEngine;

public class Watch : MonoBehaviour
{
	[SerializeField]
	private int _prepareTime = 15;

	[SerializeField]
	private int _gameTime = 60;

	[SerializeField]
	private int _comfortZoneTimeout = 20;

	[SerializeField]
	private int _cautionZoneTimeout = 40;

	[SerializeField]
	private float _tickRotation = 6f;

	[SerializeField]
	private ClockController _clockController;

	private GameFlow _gameFlow;

	private int _timeLeft;

	public int PrepareTime
	{
		get
		{
			return _prepareTime;
		}
		set
		{
			_prepareTime = value;
		}
	}

	public int GameTime
	{
		get
		{
			return _gameTime;
		}
		set
		{
			_gameTime = value;
		}
	}

	public int ComfortZoneTimeout
	{
		get
		{
			return _comfortZoneTimeout;
		}
		set
		{
			_comfortZoneTimeout = value;
		}
	}

	public int CautionZoneTimeout
	{
		get
		{
			return _cautionZoneTimeout;
		}
		set
		{
			_cautionZoneTimeout = value;
		}
	}

	public int TimeLeft => _timeLeft;

	public ClockController ClockController => _clockController;

	public void Initialize()
	{
		_gameFlow = GetComponent<GameFlow>();
	}

	public void StartTicking(int prestartTimeMargin)
	{
		StartCoroutine(WatchTick(prestartTimeMargin));
	}

	private IEnumerator WatchTick(int prestartTimeMargin)
	{
		_prepareTime = GameSessionData.Instance.GetPrepareTime();
		_prepareTime += prestartTimeMargin;
		_gameTime = GameSessionData.Instance.GetGameTime();
		_comfortZoneTimeout = GameSessionData.Instance.GetComfortZoneTimeout();
		_cautionZoneTimeout = GameSessionData.Instance.GetCautionZoneTimeout();
		int totalTime = (_timeLeft = _gameTime + _prepareTime);
		_clockController.Initialize(_gameTime, _comfortZoneTimeout, _cautionZoneTimeout);
		yield return new WaitForSeconds(2f);
		float startTime = 0f;
		WaitForSeconds tickTimeout = new WaitForSeconds(1f);
		for (int i = totalTime; i >= 0; i--)
		{
			_clockController.UpdateHandPosition(i);
			if (i == _gameTime && GameSessionData.Instance.Setup.GameType != EGameType.TUTORIAL)
			{
				_gameFlow.StartGame();
				startTime = Time.time;
			}
			yield return tickTimeout;
			if (_gameFlow.Terminated)
			{
				break;
			}
			_timeLeft = i;
		}
		GameSessionData.Instance.ScavengeFinishedTime = Time.time - startTime;
		AudioManager.Instance.StopPlayingMusicFadeOut();
		if (GameSessionData.Instance.Setup.GameType != EGameType.TUTORIAL)
		{
			_gameFlow.EndLevel();
		}
	}
}
