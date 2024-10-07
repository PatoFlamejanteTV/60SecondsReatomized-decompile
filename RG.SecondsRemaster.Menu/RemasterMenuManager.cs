using System.Collections;
using Rewired;
using RG.Core.SaveSystem;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using RG.Remaster.Common;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class RemasterMenuManager : MonoBehaviour
{
	[SerializeField]
	[Header("Main Data")]
	private CharacterList _characterList;

	[SerializeField]
	private Character _dad;

	[SerializeField]
	private Character _mom;

	[SerializeField]
	private SaveEvent _survivalSaveEvent;

	[SerializeField]
	private SaveEvent _globalGameDataSaveEvent;

	[SerializeField]
	private float _exitBlackScreenTime;

	[SerializeField]
	[Header("Main Gameplay Variables")]
	private GlobalBoolVariable _isTutorial;

	[SerializeField]
	private GlobalBoolVariable _isSurvivalOnly;

	[SerializeField]
	private GlobalBoolVariable _isScavengeOnly;

	[SerializeField]
	private GlobalBoolVariable _showJournal;

	[SerializeField]
	private GlobalBoolVariable _shouldGameBeSaved;

	[SerializeField]
	[Header("Survival Data")]
	private GlobalIntVariable _survivalDifficulty;

	[SerializeField]
	private Mission _survivalMission;

	[SerializeField]
	private Mission _scavengeMission;

	[SerializeField]
	[Header("Scavenge Data")]
	private GameSetup _scavengeSetup;

	[SerializeField]
	[Header("Tutorial Data")]
	private GameSetup _tutorialSetup;

	[SerializeField]
	private Mission _tutorialMission;

	[SerializeField]
	private string _tutorialLevel;

	[SerializeField]
	[Header("Challenges Data")]
	private CurrentChallengeData _currentChallengeData;

	[SerializeField]
	[Header("Other Data")]
	private GlobalIntVariable _currentControlModeVariable;

	private const int MEDIUM_SURVIVAL_DIFFICULTY = 1;

	public DifficultyLevel CurrentDifficultyLevel { get; set; }

	public Character CurrentCharacter { get; set; }

	public Challenge CurrentChallenge
	{
		get
		{
			return _currentChallengeData.RuntimeData.Challenge;
		}
		set
		{
			_currentChallengeData.RuntimeData.Challenge = value;
		}
	}

	public void Start()
	{
		ResetData();
		SaveGlobalData();
		Singleton<PlatformManager>.Instance.RichPresenceManager.SetRichPresenceStatus(ERichPresenceStatus.MENU);
	}

	private void SaveGlobalData()
	{
		StorageDataManager.TheInstance.Save(_globalGameDataSaveEvent.DataTag, delegate
		{
			Debug.Log("Saved GlobalGameData");
		}, delegate
		{
			Debug.Log("Failed saving GlobalGameData");
		});
	}

	private void ResetData()
	{
		_survivalSaveEvent.ResetGame();
		_isTutorial.ResetData();
		_isSurvivalOnly.ResetData();
		_isScavengeOnly.ResetData();
		_showJournal.ResetData();
		_survivalDifficulty.ResetData();
	}

	public void StartFullGame()
	{
		SetGameplayVariables(isTutorial: false, isSurvivalOnly: false, isScavengeOnly: false, showJournal: true, shouldGameBeSaved: true);
		SetScavengeData(CurrentDifficultyLevel.Setup, DemoManager.IS_DEMO_VERSION ? "level_scavenge_11" : "");
		SetSurvivalData(_survivalMission);
		Singleton<PlatformManager>.Instance.RichPresenceManager.SetRichPresenceStatus(ERichPresenceStatus.SCAVENGE);
		Singleton<GameManager>.Instance.StartScavenge();
	}

	public void StartScavenge()
	{
		SetGameplayVariables(isTutorial: false, isSurvivalOnly: false, isScavengeOnly: true, showJournal: false, shouldGameBeSaved: false);
		SetScavengeData(_scavengeSetup);
		SetSurvivalData(_scavengeMission);
		Singleton<PlatformManager>.Instance.RichPresenceManager.SetRichPresenceStatus(ERichPresenceStatus.SCAVENGE);
		Singleton<GameManager>.Instance.StartScavenge();
	}

	public void StartSurvival()
	{
		CurrentCharacter = ((Random.Range(0, 2) > 0) ? _dad : _mom);
		SetGameplayVariables(isTutorial: false, isSurvivalOnly: true, isScavengeOnly: false, showJournal: true, shouldGameBeSaved: true);
		SetSurvivalData(_survivalMission);
		Singleton<GameManager>.Instance.StartSurvival();
	}

	public void StartTutorial()
	{
		CurrentCharacter = _dad;
		SetGameplayVariables(isTutorial: true, isSurvivalOnly: false, isScavengeOnly: false, showJournal: true, shouldGameBeSaved: false);
		SetScavengeData(_tutorialSetup, _tutorialLevel);
		SetSurvivalData(_tutorialMission);
		Singleton<PlatformManager>.Instance.RichPresenceManager.SetRichPresenceStatus(ERichPresenceStatus.SCAVENGE);
		Singleton<GameManager>.Instance.StartScavenge();
	}

	public void Continue()
	{
		_shouldGameBeSaved.Value = true;
		Singleton<GameManager>.Instance.StartSurvival(_survivalSaveEvent.DataTag);
	}

	public void ExitGame()
	{
		StartCoroutine(ActuallyExitGame());
	}

	private IEnumerator ActuallyExitGame()
	{
		yield return Singleton<GameManager>.Instance.LoadingManager.ObscurerController.ShowObscurer();
		yield return new WaitForSeconds(_exitBlackScreenTime);
		Application.Quit();
	}

	public void StartChallenge()
	{
		CurrentCharacter = _dad;
		if (!(CurrentChallenge == null))
		{
			switch (CurrentChallenge.ChallengeType)
			{
			case Challenge.EChallengeType.SURVIVAL:
				SetGameplayVariables(isTutorial: false, isSurvivalOnly: true, isScavengeOnly: false, showJournal: true, shouldGameBeSaved: true);
				SetSurvivalData(CurrentChallenge.Mission, isChallenge: true);
				Singleton<PlatformManager>.Instance.RichPresenceManager.SetRichPresenceStatus(ERichPresenceStatus.CHALLENGE_SURVIVAL_INTRO);
				Singleton<GameManager>.Instance.StartSurvival();
				break;
			case Challenge.EChallengeType.SCAVENGE:
				SetGameplayVariables(isTutorial: false, isSurvivalOnly: false, isScavengeOnly: false, showJournal: false, shouldGameBeSaved: false);
				SetScavengeData(CurrentChallenge.GameSetup, CurrentChallenge.ScavengeLevel, isChallenge: true);
				SetSurvivalData(CurrentChallenge.Mission, isChallenge: true);
				Singleton<PlatformManager>.Instance.RichPresenceManager.SetRichPresenceStatus(ERichPresenceStatus.CHALLENGE_SCAVENGE);
				Singleton<GameManager>.Instance.StartScavenge();
				break;
			}
		}
	}

	private void SetGameplayVariables(bool isTutorial, bool isSurvivalOnly, bool isScavengeOnly, bool showJournal, bool shouldGameBeSaved)
	{
		_shouldGameBeSaved.Value = shouldGameBeSaved;
		_isTutorial.Value = isTutorial;
		_isSurvivalOnly.Value = isSurvivalOnly;
		_isScavengeOnly.Value = isScavengeOnly;
		_showJournal.Value = showJournal;
		if (CurrentCharacter == null)
		{
			CurrentCharacter = _dad;
		}
	}

	private void SetScavengeData(GameSetup setup, string scavengeLevel = null, bool isChallenge = false)
	{
		GameSessionData instance = GameSessionData.Instance;
		instance.CurrentChallenge = (isChallenge ? CurrentChallenge : null);
		instance.Setup = setup;
		instance.Difficulty = (CurrentDifficultyLevel ? CurrentDifficultyLevel.ScavengeDifficulty : EGameDifficulty.NORMAL);
		if (!isChallenge)
		{
			switch (instance.Difficulty)
			{
			case EGameDifficulty.EASY:
				instance.SetScavengeData(instance.Setup.Difficulties.Easy.PrepareTime, instance.Setup.Difficulties.Easy.ScavengeTime);
				break;
			case EGameDifficulty.NORMAL:
				instance.SetScavengeData(instance.Setup.Difficulties.Normal.PrepareTime, instance.Setup.Difficulties.Normal.ScavengeTime);
				break;
			case EGameDifficulty.HARD:
				instance.SetScavengeData(instance.Setup.Difficulties.Hard.PrepareTime, instance.Setup.Difficulties.Hard.ScavengeTime);
				break;
			}
			_currentChallengeData.RuntimeData.Challenge = null;
		}
		else
		{
			instance.SetScavengeData(setup.PrepareTime, setup.GameTime);
		}
		if (CurrentCharacter == _dad)
		{
			instance.Character = ECharacter.DAD;
		}
		else if (CurrentCharacter == _mom)
		{
			instance.Character = ECharacter.MOM;
		}
		else
		{
			Debug.LogWarningFormat("Incorrect character was chosen for Scavenge ('{0}'). Scavenge character will fallback to DAD.", CurrentCharacter);
			instance.Character = ECharacter.DAD;
		}
		if (string.IsNullOrEmpty(scavengeLevel))
		{
			Singleton<GameManager>.Instance.ScavangeSceneName = instance.Setup.GetRandomScavengeLevelName();
		}
		else
		{
			Singleton<GameManager>.Instance.ScavangeSceneName = scavengeLevel;
		}
		ReInitializeInput();
	}

	private void ReInitializeInput()
	{
		Player player = ReInput.players.GetPlayer(0);
		EPlayerInput ePlayerInput = (EPlayerInput)_currentControlModeVariable.Value;
		if (player.controllers.joystickCount == 0 && ePlayerInput == EPlayerInput.GAMEPAD)
		{
			ePlayerInput = EPlayerInput.KEYBOARD_MOUSE;
			_currentControlModeVariable.Value = (int)ePlayerInput;
		}
		int layoutId = ((Application.systemLanguage == SystemLanguage.French) ? 1 : 0);
		int layoutId2 = ((Application.systemLanguage != SystemLanguage.French) ? 1 : 0);
		string layoutName = ReInput.mapping.GetLayout(ControllerType.Keyboard, layoutId).name;
		string layoutName2 = ReInput.mapping.GetLayout(ControllerType.Keyboard, layoutId2).name;
		player.controllers.maps.SetMapsEnabled(state: false, 8);
		player.controllers.maps.SetMapsEnabled(state: false, ReInput.mapping.GetMapCategory(7).name);
		player.controllers.maps.SetMapsEnabled(state: false, ReInput.mapping.GetMapCategory(7).name);
		player.controllers.maps.SetMapsEnabled(state: false, 9);
		player.controllers.maps.SetMapsEnabled(state: false, 6);
		switch (ePlayerInput)
		{
		case EPlayerInput.KEYBOARD:
			player.controllers.maps.SetMapsEnabled(state: true, ReInput.mapping.GetMapCategory(7).name, layoutName);
			player.controllers.maps.SetMapsEnabled(state: true, ReInput.mapping.GetMapCategory(7).name, layoutName2);
			break;
		case EPlayerInput.KEYBOARD_MOUSE:
			player.controllers.maps.SetMapsEnabled(state: true, ReInput.mapping.GetMapCategory(9).name, layoutName);
			player.controllers.maps.SetMapsEnabled(state: true, ReInput.mapping.GetMapCategory(9).name, layoutName2);
			break;
		case EPlayerInput.GAMEPAD:
			player.controllers.maps.SetMapsEnabled(state: true, ReInput.mapping.GetMapCategory(7).name, layoutName);
			player.controllers.maps.SetMapsEnabled(state: true, ReInput.mapping.GetMapCategory(9).name, layoutName);
			player.controllers.maps.SetMapsEnabled(state: true, 8);
			break;
		case EPlayerInput.MOUSE_ONLY:
			player.controllers.maps.SetMapsEnabled(state: true, 6);
			break;
		case EPlayerInput.TOUCH_ANALOGUE:
		case EPlayerInput.TOUCH_DIGITAL:
			break;
		}
	}

	private void SetSurvivalData(Mission mission, bool isChallenge = false)
	{
		_survivalDifficulty.Value = ((!CurrentDifficultyLevel) ? 1 : CurrentDifficultyLevel.SurvivalDifficulty);
		if (!mission.SetCaptainExternal && mission.Captain != null)
		{
			CurrentCharacter = mission.Captain;
		}
		if (!isChallenge)
		{
			_currentChallengeData.RuntimeData.Challenge = null;
		}
		_characterList.AddCharToList(CurrentCharacter);
		CurrentCharacter.RuntimeData.IsCaptain = true;
		MissionManager.Instance.SetActualMission(mission);
	}
}
