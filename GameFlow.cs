using System;
using System.Collections;
using System.Collections.Generic;
using DunGen;
using FMODUnity;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.Menu;
using RG.Parsecs.Survival;
using RG.SecondsRemaster;
using RG.SecondsRemaster.Menu;
using RG.SecondsRemaster.Scavenge;
using RG.VirtualInput;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlow : MonoBehaviour
{
	public delegate void InteractionReport();

	[SerializeField]
	private CurrentChallengeData _currentChallenge;

	[SerializeField]
	private GlobalBoolVariable _wasScavengeFailed;

	[SerializeField]
	private GlobalBoolVariable _showJournal;

	[SerializeField]
	private GlobalBoolVariable _isTubaDisabled;

	[SerializeField]
	private GameObject _restartPanel;

	[SerializeField]
	private SCamParams _shelterJumpCamParams;

	[SerializeField]
	private SCamParams _duckCamParams;

	[SerializeField]
	private RuntimeMeshBatcherController _runtimeMeshBatcher;

	[SerializeField]
	private HandsController _handsController;

	[SerializeField]
	private ClockController _clockController;

	[SerializeField]
	private SurvivalTransferManager _survivalTransferManager;

	[SerializeField]
	private ScavengeTextController _noRoomText;

	[SerializeField]
	private Animator _explosionAnimator;

	[SerializeField]
	private GameObject _tutorial;

	[SerializeField]
	private Animator _sergeantAnimator;

	[SerializeField]
	private TextMeshProUGUI _objectiveText;

	[SerializeField]
	private SergeantSpeechController _sergeantText;

	[SerializeField]
	private ChallengeItemsController _challengeItemsController;

	[EventRef]
	[SerializeField]
	private string _scavengeMusic;

	[EventRef]
	[SerializeField]
	private string _sirenSound;

	[EventRef]
	[SerializeField]
	private string _bombSound;

	private GameObject[] _specialLevelItems;

	private List<ScavengeItemController> _reportedSpecialItems = new List<ScavengeItemController>();

	private Announcer[] _announcers;

	private List<ScavengeItemController> _itemsToCollect;

	private bool _gameRunning;

	private bool _preGame;

	private bool _forcedHouse = true;

	private bool _terminated;

	private bool _updateCollectGUI;

	private PauseMenuControl _pauseMenuControl;

	private Watch _watch;

	private PlayerInteraction _playerInteraction;

	private ThirdPersonController _thirdPersonController;

	private ShelterInventory _shelterInventory;

	[SerializeField]
	private CharacterStatus _failedScavenge;

	[SerializeField]
	private CharacterList _characterList;

	private bool _addFailedStatus;

	private WaitForSeconds _initPreGameTimeout = new WaitForSeconds(0.1f);

	[SerializeField]
	private Achievement _notMadeIt;

	[SerializeField]
	private Achievement _madeIt;

	[SerializeField]
	private Achievement _dolores;

	[SerializeField]
	private Achievement _allItems;

	[SerializeField]
	private Achievement _souper;

	[SerializeField]
	private Goal _bePreparedGoal;

	[SerializeField]
	private Goal _proGamerGoal;

	[SerializeField]
	private Achievement _familyGuy;

	[SerializeField]
	private Achievement _toraToraTora;

	[SerializeField]
	private Achievement _waterAchievement;

	[Header("Difficulty related achievements")]
	[SerializeField]
	private Achievement _enolaGay;

	[SerializeField]
	private Achievement _manhattanProject;

	[SerializeField]
	private Achievement _deadHand;

	[SerializeField]
	private GlobalBoolVariable _enolaGayWasScavengeWon;

	[SerializeField]
	private List<GlobalBoolVariable> _easyDifficultyCompletionVariables;

	[SerializeField]
	private GlobalBoolVariable _manhattanProjectWasScavengeWon;

	[SerializeField]
	private List<GlobalBoolVariable> _mediumDifficultyCompletionVariables;

	[SerializeField]
	private GlobalBoolVariable _deadHandWasScavengeWon;

	[SerializeField]
	private List<GlobalBoolVariable> _hardDifficultyCompletionVariables;

	[SerializeField]
	private string _challengeConclusionSceneName = "ChallengeConclusion";

	[SerializeField]
	private float _waitTimeBeforeShowingConclusion = 1f;

	private const int ITEMS_TO_SCAVENGE = 15;

	private const int SOUPS_TO_COLLECT = 10;

	private const int FAMILY_MEMBERS_TO_GET = 3;

	private const int TORA_COLLISIONS = 1337;

	private const int SCAVENGED_WATER_FOR_ACHIEVEMENT = 10;

	private const string TALK_PARAM_NAME = "Talk";

	public Watch TheWatch => _watch;

	public bool ForcedHouse => _forcedHouse;

	public bool Terminated
	{
		get
		{
			return _terminated;
		}
		set
		{
			_terminated = value;
		}
	}

	public GameObject[] SpecialLevelItems => _specialLevelItems;

	public bool GameRunning => _gameRunning;

	public bool Paused
	{
		get
		{
			if (!(_pauseMenuControl != null))
			{
				return false;
			}
			return _pauseMenuControl.Paused;
		}
	}

	public HandsController HandsController => _handsController;

	public SurvivalTransferManager SurvivalTransferManager => _survivalTransferManager;

	public ScavengeTextController NoRoomText => _noRoomText;

	public ChallengeItemsController ChallengeItemsController => _challengeItemsController;

	public event InteractionReport NearShelter;

	private void Awake()
	{
		Singleton<GameManager>.Instance.LoadingManager.BeforeUnloadLoadingScene += ScavengeLoaded;
		GameSessionData.Instance.CurrentGameStage = ECurrentGameStage.SCAVENGE;
		_wasScavengeFailed.Value = true;
		if (_isTubaDisabled != null)
		{
			_isTubaDisabled.Value = false;
		}
		ScavengeGUISetup scavengeGUISetup = UnityEngine.Object.FindObjectOfType<ScavengeGUISetup>();
		if (scavengeGUISetup != null)
		{
			scavengeGUISetup.Process();
		}
		if (GameSessionData.Instance.Setup.GameType == EGameType.TUTORIAL)
		{
			base.gameObject.GetComponent<ScavengeTutorialDriver>().enabled = true;
		}
	}

	private void ScavengeLoaded(object sender, EventArgs e)
	{
		LoadingManager.LoadingEventArgs loadingEventArgs = (LoadingManager.LoadingEventArgs)e;
		if (loadingEventArgs.LoadedSceneName.Contains("scavenge") || loadingEventArgs.LoadedSceneName.Contains("challenge") || loadingEventArgs.LoadedSceneName.Contains("tutorial"))
		{
			Singleton<VirtualInputManager>.Instance.VisualManager.MouseCursorVisible = false;
		}
	}

	private void Start()
	{
		GenerateLevel();
		NearShelter += ProcessChallengeConditions;
		_pauseMenuControl = BasePauseMenu.Instance as PauseMenuControl;
		if (GameSessionData.Instance.Setup.AreSpecificItemsToBeCollected())
		{
			_itemsToCollect = new List<ScavengeItemController>(GameSessionData.Instance.Setup.CollectItems);
		}
	}

	private void UpdateCollectGUI(int swapIndex)
	{
		_updateCollectGUI = true;
	}

	private void ProcessChallengeConditions()
	{
		if (GameSessionData.Instance.Setup.GameType == EGameType.CHALLENGE_SCAVENGE)
		{
			bool flag = false;
			if (GameSessionData.Instance.Setup.AreSpecificItemsToBeCollected())
			{
				flag = _itemsToCollect.Count == 0;
			}
			if (flag && _playerInteraction.IsPlayerNearShelter())
			{
				Terminate();
			}
		}
	}

	public void ReportCollectedItem(ScavengeItemController scavengeItemController)
	{
		if (_itemsToCollect == null || _itemsToCollect.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < _itemsToCollect.Count; i++)
		{
			if (_itemsToCollect[i].SurvivalName == scavengeItemController.SurvivalName)
			{
				_itemsToCollect.RemoveAt(i);
				UpdateCollectGUI(i);
				break;
			}
		}
	}

	private void GenerateLevel()
	{
		GameSetup setup = GameSessionData.Instance.Setup;
		int num = 0;
		for (int i = 0; i < setup.LevelItems.Count; i++)
		{
			ScavengeItemController component = setup.LevelItems[i].GetComponent<ScavengeItemController>();
			if (component != null && component.SpecialItem)
			{
				num++;
			}
		}
		_specialLevelItems = new GameObject[num];
		int num2 = 0;
		for (int j = 0; j < setup.LevelItems.Count; j++)
		{
			ScavengeItemController component2 = setup.LevelItems[j].GetComponent<ScavengeItemController>();
			if (component2 != null && component2.SpecialItem)
			{
				_specialLevelItems[num2] = setup.LevelItems[j].gameObject;
				num2++;
			}
		}
		RuntimeDungeon component3 = GetComponent<RuntimeDungeon>();
		if (component3 != null && component3.enabled)
		{
			if (setup.LevelData.FlowData != null)
			{
				if (!_forcedHouse)
				{
					component3.Generator.DungeonFlow = setup.LevelData.FlowData;
					component3.Generator.OnGenerationStatusChanged += OnGenerationStatusChanged;
					component3.Generator.Generate();
				}
			}
			else
			{
				UnityEngine.Object.Instantiate(setup.LevelData.HousePrefab);
				UnityEngine.Object.Destroy(component3);
				Initialize();
			}
		}
		else
		{
			Initialize();
		}
		if (_runtimeMeshBatcher != null)
		{
			_runtimeMeshBatcher.CombineMeshes();
		}
	}

	public void FullExitGame()
	{
		ExitGame(fullExit: true);
	}

	public void Terminate()
	{
		_terminated = true;
	}

	public void Restart(bool value)
	{
		ResetGame.RestartLevel();
	}

	public void ExitGame(bool fullExit)
	{
		AudioManager.Instance.StopPlayingSfxFadeOut();
		if (fullExit)
		{
			Application.Quit();
			return;
		}
		Loading.Loader.NextLevelName = "main_menu";
		Loading.Loader.GoToLoading();
	}

	private void Initialize()
	{
		bool spawnItems = GameSessionData.Instance.Setup.GameType != EGameType.TUTORIAL;
		InitializeNewLevel(GameSessionData.Instance.GetPlayerTemplate(), spawnItems);
		StartCoroutine(InitializePreGame());
	}

	private IEnumerator InitializePreGame()
	{
		GameObject gameObject = null;
		while (gameObject == null)
		{
			yield return _initPreGameTimeout;
			gameObject = GlobalTools.GetPlayer();
		}
		_playerInteraction = gameObject.GetComponent<PlayerInteraction>();
		_thirdPersonController = gameObject.GetComponent<ThirdPersonController>();
		_shelterInventory = GlobalTools.GetShelterInventory();
		_watch = GetComponent<Watch>();
		if (_playerInteraction == null)
		{
			GlobalTools.DebugLogError("Player Interaction not set! (GameFlow)");
		}
		if (_shelterInventory == null)
		{
			GlobalTools.DebugLogError("Shelter Inventory not set! (GameFlow)");
		}
		Shelter shelter = GlobalTools.GetShelter();
		if (shelter != null)
		{
			shelter.SetGuider();
		}
		StartPreGame();
	}

	private void OnGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
	{
		if (status == GenerationStatus.Complete)
		{
			UnityEngine.Object.Instantiate(GameSessionData.Instance.Setup.LevelData.HousePrefab);
			Tile[] array = UnityEngine.Object.FindObjectsOfType<Tile>();
			int num = GameSessionData.Instance.Setup.LevelData.FlowData.Length.Max + GameSessionData.Instance.Setup.LevelData.FlowData.GetUsedArchetypes()[0].BranchCount.Max;
			if (array.Length != num)
			{
				UnityEngine.Object.Destroy(generator.CurrentDungeon.gameObject);
				UnityEngine.Object.Instantiate(GameSessionData.Instance.Setup.LevelData.GetRandomLevelPrefab());
				_forcedHouse = true;
			}
			Initialize();
		}
	}

	private void HideScavengeUI(bool hideHands = true, bool hideClock = true, bool hideChallengeItems = true)
	{
		if (hideHands)
		{
			_handsController.HideHands();
		}
		if (hideClock)
		{
			_clockController.HideClock();
		}
		if (hideChallengeItems && GameSessionData.Instance.Setup.GameType == EGameType.CHALLENGE_SCAVENGE)
		{
			_challengeItemsController.HideChallengeUI();
		}
	}

	public ScavengeItemController ActivateAnnouncer(int index, bool activate, float customTime = -1f, string customItem = null)
	{
		if (_announcers.Length != 0 && index >= 0 && index < _announcers.Length)
		{
			if (customTime >= 0f)
			{
				_announcers[index].Timeout = customTime;
			}
			_announcers[index].Activate(activate, customItem);
			return _announcers[index].AnnouncedScavengeItemController;
		}
		return null;
	}

	public void StartPreGame()
	{
		GlobalTools.DebugLog("StartPreGame");
		Singleton<VirtualInputManager>.Instance.VisualManager.MouseCursorVisible = false;
		GlobalTools.GetShelter().ShowRange(show: false);
		_thirdPersonController.SetMovementLimited(limit: true);
		_playerInteraction.EnableInteraction(enableGrab: false, enableDrop: false, showGrabLimit: false);
		_watch.Initialize();
		_preGame = true;
		StartCoroutine(GUIHelper.DoCircleFade(fadeIn: true, 1f, 0f, destroyCircleFader: false, obscurerEndDeactivate: false));
		if (GameSessionData.Instance.Setup.GameType != EGameType.TUTORIAL)
		{
			StartTimer(2);
		}
	}

	public void StartTimer(int prestartTimeMargin)
	{
		_watch.StartTicking(prestartTimeMargin);
	}

	public void StartGame()
	{
		GlobalTools.DebugLog("StartGame");
		AudioManager.Instance.PlayMusicFadeOut(_scavengeMusic);
		_gameRunning = true;
		_preGame = false;
		_thirdPersonController.SetMovementLimited(limit: false);
		_playerInteraction.EnableInteraction(enableGrab: true, enableDrop: true, showGrabLimit: true);
		EnableInteraction();
		AudioManager.PlaySound(_sirenSound);
		if (GameSessionData.Instance.Setup.GameType == EGameType.TUTORIAL)
		{
			StartTimer(0);
		}
		else
		{
			OpenShelter();
		}
	}

	public void OpenShelter()
	{
		StartCoroutine(OpenShelter(1f));
	}

	private IEnumerator OpenShelter(float openTime)
	{
		Shelter shelter = GlobalTools.GetShelter();
		if (shelter != null)
		{
			shelter.OpenHatch(open: true, openTime);
			yield return new WaitForSeconds(openTime - 0.5f);
			shelter.Flash();
		}
		yield return null;
	}

	public IEnumerator DoTutorialResetStart()
	{
		yield return Singleton<GameManager>.Instance.LoadingManager.ObscurerController.ShowObscurer();
	}

	public IEnumerator DoTutorialResetEnd()
	{
		yield return Singleton<GameManager>.Instance.LoadingManager.ObscurerController.HideObscurer();
	}

	public void EndLevel()
	{
		GlobalTools.DebugLog("Game ended!");
		_gameRunning = false;
		StartCoroutine(FinalizeLevel());
	}

	public IEnumerator FinalizeLevel()
	{
		bool triggerEnd = false;
		_thirdPersonController.SetMovementBlocked(block: true);
		_playerInteraction.EnableInteraction(enableGrab: false, enableDrop: false, showGrabLimit: false);
		Shelter shelter = GlobalTools.GetShelter();
		shelter.ShowRange(show: false);
		GameObject gameObject;
		SCamParams sCamParams;
		if (_playerInteraction.IsPlayerNearShelter())
		{
			_addFailedStatus = false;
			HideScavengeUI();
			_playerInteraction.JumpToShelter();
			if (GameSessionData.Instance.Setup.GameType != EGameType.TUTORIAL && GameSessionData.Instance.Setup.GameType != EGameType.CHALLENGE_SCAVENGE)
			{
				if (!AchievementsSystem.IsAchievementUnlocked(_madeIt))
				{
					AchievementsSystem.UnlockAchievement(_madeIt);
				}
				if (GameSessionData.Instance.Character == ECharacter.MOM && !AchievementsSystem.IsAchievementUnlocked(_dolores))
				{
					AchievementsSystem.UnlockAchievement(_dolores);
				}
			}
			gameObject = GameObject.FindGameObjectWithTag("SecondaryCamera1");
			sCamParams = _shelterJumpCamParams;
			_wasScavengeFailed.Value = false;
		}
		else
		{
			_playerInteraction.DuckAndCover();
			HideScavengeUI();
			if (GameSessionData.Instance.Setup.GameType != EGameType.TUTORIAL)
			{
				AchievementsSystem.UnlockAchievement(_notMadeIt);
			}
			gameObject = GameObject.FindGameObjectWithTag("SecondaryCamera2");
			sCamParams = _duckCamParams;
			_addFailedStatus = true;
		}
		Transform child = gameObject.transform.GetChild(0);
		child.gameObject.SetActive(value: true);
		iTween.MoveBy(child.gameObject, iTween.Hash("x", sCamParams.Relocation.x, "y", sCamParams.Relocation.y, "z", sCamParams.Relocation.z, "time", sCamParams.Time, "easeType", sCamParams.EaseType));
		Camera.main.gameObject.SetActive(value: false);
		while (!triggerEnd && !_playerInteraction.EndInteractionDone())
		{
			yield return new WaitForSeconds(0.05f);
		}
		shelter.OpenHatch(open: false, 2f);
		GUIHelper.MakeObscurerVisible(visible: true);
		if (GameSessionData.Instance.Setup.GameType != EGameType.TUTORIAL)
		{
			_explosionAnimator.gameObject.SetActive(value: true);
			AudioManager.PlaySound(_bombSound);
		}
		yield return new WaitForSeconds(5f);
		if (GameSessionData.Instance.Setup.GameType == EGameType.CHALLENGE_SCAVENGE)
		{
			SubmitResults();
			if (_wasScavengeFailed.Value)
			{
				_isTubaDisabled.Value = true;
				_restartPanel.SetActive(value: true);
			}
			else
			{
				GameSessionData.Instance.CurrentChallenge.Unlock(GameSessionData.Instance.ScavengeFinishedTime);
				StartCoroutine(LoadConclusionSceneCoroutine());
			}
		}
		else
		{
			BasePauseMenu.Instance.currentGameState = BasePauseMenu.EGameState.Menu;
			SubmitResults();
			Singleton<GameManager>.Instance.StartSurvival();
		}
	}

	private IEnumerator LoadConclusionSceneCoroutine()
	{
		BasePauseMenu.Instance.currentGameState = BasePauseMenu.EGameState.Menu;
		yield return Singleton<GameManager>.Instance.LoadingManager.ObscurerController.ShowObscurer();
		yield return new WaitForSeconds(_waitTimeBeforeShowingConclusion);
		yield return SceneManager.LoadSceneAsync(_challengeConclusionSceneName, LoadSceneMode.Additive);
		yield return Singleton<GameManager>.Instance.LoadingManager.ObscurerController.HideObscurer();
		Singleton<GameManager>.Instance.RaycastCatcher.SetCatchingRaycasts(catchingRaycasts: false, null);
	}

	public void SpawnPlayer(GameObject playerTemplate)
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("PlayerSpawnPoint");
		if (gameObject == null)
		{
			GlobalTools.DebugLogError("Player spawn point missing");
			return;
		}
		Spawner component = gameObject.GetComponent<Spawner>();
		component.Template = playerTemplate;
		if (component.Spawn())
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		else
		{
			GlobalTools.DebugLogError("Player spawning failed");
		}
	}

	public void InitializeNewLevel(GameObject playerTemplate, bool spawnItems = true)
	{
		GlobalTools.DebugLog("InitializeNewLevel");
		GameSetup setup = GameSessionData.Instance.Setup;
		setup.SetItemsForCharacter(GameSessionData.Instance.Character);
		if (playerTemplate != null)
		{
			SpawnPlayer(playerTemplate);
		}
		if (!spawnItems)
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("SpawnPoint");
		Dictionary<GameObject, List<Spawner>> dictionary = new Dictionary<GameObject, List<Spawner>>();
		for (int i = 0; i < setup.LevelItems.Count; i++)
		{
			if (!dictionary.ContainsKey(setup.LevelItems[i].gameObject))
			{
				dictionary.Add(setup.LevelItems[i].gameObject, new List<Spawner>());
			}
		}
		for (int num = array.Length - 1; num >= 0; num--)
		{
			Spawner component = array[num].GetComponent<Spawner>();
			if (dictionary.ContainsKey(component.Template))
			{
				dictionary[component.Template].Add(component);
			}
			else
			{
				UnityEngine.Object.Destroy(component.gameObject);
			}
		}
		for (int j = 0; j < setup.LevelItems.Count; j++)
		{
			List<Spawner> list = dictionary[setup.LevelItems[j].gameObject];
			if (list.Count > 0)
			{
				Spawner spawner = list[UnityEngine.Random.Range(0, list.Count)];
				if (spawner.Spawn(setup.LevelItems[j].gameObject))
				{
					list.Remove(spawner);
					UnityEngine.Object.Destroy(spawner.gameObject);
				}
				else
				{
					GlobalTools.DebugLogError("Error spawning item " + setup.LevelItems[j].name + ", spawning aborted");
				}
			}
		}
	}

	public void EnableInteraction()
	{
		if (!HandsController.gameObject.activeSelf)
		{
			HandsController.gameObject.SetActive(value: true);
		}
	}

	public void ReportNearShelter(bool near)
	{
		if (near && this.NearShelter != null)
		{
			this.NearShelter();
		}
	}

	private void SubmitResults()
	{
		bool flag = false;
		if (_playerInteraction.IsPlayerNearShelter())
		{
			flag = true;
			_survivalTransferManager.TransferHeldItems();
			if (GameSessionData.Instance.Setup.GameType != EGameType.CHALLENGE_SCAVENGE)
			{
				StatsManager.Instance.AddGlobalData("NukeDropSurvived", 1);
			}
		}
		else
		{
			if (GameSessionData.Instance.Setup.GameType != EGameType.CHALLENGE_SCAVENGE)
			{
				StatsManager.Instance.AddGlobalData("AtomPerished", 1);
			}
			_showJournal.Value = false;
		}
		_survivalTransferManager.TransferScavengedItems();
		if (GameSessionData.Instance.Setup.GameType == EGameType.CHALLENGE_SCAVENGE)
		{
			_wasScavengeFailed.Value = !IsChallengeConditionAchieved();
			if (!_wasScavengeFailed.Value && _currentChallenge != null && _currentChallenge.RuntimeData.Challenge.Rewards != null)
			{
				for (int i = 0; i < _currentChallenge.RuntimeData.Challenge.Rewards.Count; i++)
				{
					if (_currentChallenge.RuntimeData.Challenge.Rewards[i].ScavengeRewardIsUnlockedVariable != null)
					{
						_currentChallenge.RuntimeData.Challenge.Rewards[i].ScavengeRewardIsUnlockedVariable.SetValue(value: true);
					}
				}
			}
		}
		else if (_addFailedStatus)
		{
			_characterList.GetCaptain().RuntimeData.AddStatus(_failedScavenge);
		}
		if (GameSessionData.Instance.Setup.GameType != EGameType.TUTORIAL && GameSessionData.Instance.Setup.GameType != EGameType.CHALLENGE_SCAVENGE)
		{
			CheckScavengeAchievements(flag);
			CheckTotalItemCollectedRecord();
			CheckPeoplePershied(flag);
		}
	}

	private void CheckPeoplePershied(bool win)
	{
		int num = 0;
		for (int i = 0; i < _survivalTransferManager.ItemList.Items.Count; i++)
		{
			if (_survivalTransferManager.ItemList.Items[i].Character != null && !_survivalTransferManager.ItemList.Items[i].WasTaken)
			{
				num++;
			}
		}
		if (!win)
		{
			num++;
		}
		StatsManager.Instance.AddGlobalData("TotalCharactersDeadOnScavenge", num);
	}

	private void CheckTotalItemCollectedRecord()
	{
		int num = 0;
		for (int i = 0; i < _survivalTransferManager.ItemList.Items.Count; i++)
		{
			num += _survivalTransferManager.ItemList.Items[i].Amount;
		}
		int globalData = StatsManager.Instance.GetGlobalData("MostItemsCollected");
		if (globalData < num)
		{
			StatsManager.Instance.AddGlobalData("MostItemsCollected", num - globalData);
		}
	}

	private void CheckScavengeAchievements(bool scavengeStatus)
	{
		if (!AchievementsSystem.IsAchievementUnlocked(_toraToraTora))
		{
			AchievementsSystem.ProgressAchievement(_toraToraTora, StatsManager.Instance.GetGlobalData("ScavengeCollision"), 1337);
		}
		if (!AchievementsSystem.IsAchievementUnlocked(_souper))
		{
			AchievementsSystem.ProgressAchievement(_souper, StatsManager.Instance.GetGlobalData("ScavengedSoups"), 10);
		}
		if (!AchievementsSystem.IsAchievementUnlocked(_allItems))
		{
			AchievementsSystem.ProgressAchievement(_allItems, StatsManager.Instance.GetGlobalData("ScavengedItemsIDs"), 15);
		}
		if (!AchievementsSystem.IsAchievementUnlocked(_waterAchievement))
		{
			AchievementsSystem.ProgressAchievement(_waterAchievement, StatsManager.Instance.GetGlobalData("ScavengedWater"), 10);
		}
		if (GameSessionData.Instance.Setup.GameType != EGameType.SCAVENGE || !scavengeStatus)
		{
			return;
		}
		switch (GameSessionData.Instance.Difficulty)
		{
		case EGameDifficulty.EASY:
			if (_enolaGay != null && _enolaGayWasScavengeWon != null && !_enolaGay.IsAchieved && !_enolaGayWasScavengeWon.Value)
			{
				_enolaGayWasScavengeWon.Value = true;
				if (CheckDifficultyAchievementVariables(_easyDifficultyCompletionVariables))
				{
					AchievementsSystem.UnlockAchievement(_enolaGay);
				}
			}
			break;
		case EGameDifficulty.NORMAL:
			if (_manhattanProject != null && _manhattanProjectWasScavengeWon != null && !_manhattanProject.IsAchieved && !_manhattanProjectWasScavengeWon.Value)
			{
				_manhattanProjectWasScavengeWon.Value = true;
				if (CheckDifficultyAchievementVariables(_mediumDifficultyCompletionVariables))
				{
					AchievementsSystem.UnlockAchievement(_manhattanProject);
				}
			}
			break;
		case EGameDifficulty.HARD:
			if (_deadHand != null && _deadHandWasScavengeWon != null && !_deadHand.IsAchieved && !_deadHandWasScavengeWon.Value)
			{
				_deadHandWasScavengeWon.Value = true;
				if (CheckDifficultyAchievementVariables(_hardDifficultyCompletionVariables))
				{
					AchievementsSystem.UnlockAchievement(_deadHand);
				}
			}
			break;
		}
	}

	private bool CheckDifficultyAchievementVariables(List<GlobalBoolVariable> difficultyVariables)
	{
		for (int i = 0; i < difficultyVariables.Count; i++)
		{
			if (!difficultyVariables[i].Value)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsChallengeConditionAchieved()
	{
		ScavengeItemList itemList = _survivalTransferManager.ItemList;
		Dictionary<string, int> dictionary = CreateItemsToCollect();
		for (int i = 0; i < itemList.Items.Count; i++)
		{
			if (dictionary.ContainsKey(itemList.Items[i].Guid) && itemList.Items[i].Amount < dictionary[itemList.Items[i].Guid])
			{
				return false;
			}
		}
		return true;
	}

	private Dictionary<string, int> CreateItemsToCollect()
	{
		Challenge currentChallenge = GameSessionData.Instance.CurrentChallenge;
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		for (int i = 0; i < currentChallenge.Collectables.Count; i++)
		{
			if (dictionary.ContainsKey(currentChallenge.Collectables[i].Guid))
			{
				dictionary[currentChallenge.Collectables[i].Guid]++;
			}
			else
			{
				dictionary.Add(currentChallenge.Collectables[i].Guid, 1);
			}
		}
		return dictionary;
	}

	public bool ReportSpecialItem(ScavengeItemController scavengeItemController)
	{
		if (_reportedSpecialItems.Contains(scavengeItemController))
		{
			return false;
		}
		_reportedSpecialItems.Add(scavengeItemController);
		return true;
	}

	public void ShowGoal(bool show, string text)
	{
		_objectiveText.text = text;
		if (_objectiveText.gameObject.activeSelf != show)
		{
			_objectiveText.gameObject.SetActive(value: true);
		}
	}

	public void EnableGmanText(bool enable, bool talk, string text)
	{
		if (enable)
		{
			if (!_sergeantAnimator.gameObject.activeSelf)
			{
				_sergeantAnimator.gameObject.SetActive(value: true);
			}
			if (talk)
			{
				_sergeantAnimator.SetBool("Talk", value: true);
			}
			_sergeantText.ShowText(text);
		}
		else
		{
			if (talk)
			{
				_sergeantAnimator.SetBool("Talk", value: false);
			}
			_sergeantText.HideText();
		}
	}

	public void EnableGmanChar(bool enable)
	{
		_sergeantAnimator.gameObject.SetActive(enable);
	}
}
