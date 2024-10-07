using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameSetup : ScriptableObject
{
	[SerializeField]
	private bool _enabled = true;

	[SerializeField]
	private int _version;

	[SerializeField]
	private string _setupName = string.Empty;

	[SerializeField]
	private string _setupDescription = string.Empty;

	[SerializeField]
	private string _setupIcon = string.Empty;

	[SerializeField]
	private string _rewardId = string.Empty;

	[SerializeField]
	private string _achievementId = string.Empty;

	[SerializeField]
	private EGameType _gameType;

	[SerializeField]
	private int _prepareTime;

	[SerializeField]
	private int _gameTime = 60;

	[SerializeField]
	private int _comfortZoneTimeout = 20;

	[SerializeField]
	private int _cautionZoneTimeout = 40;

	[SerializeField]
	private string _forcedLevelStem = string.Empty;

	[SerializeField]
	private int _forcedLevelMin = 1;

	[SerializeField]
	private int _forcedLevelMax = 1;

	[SerializeField]
	private SLevelData _levelData;

	[SerializeField]
	private List<GameObject> _levelItems = new List<GameObject>();

	[SerializeField]
	private List<ScavengeItemController> levelItems = new List<ScavengeItemController>();

	[SerializeField]
	private List<GameObject> _collectItems = new List<GameObject>();

	[SerializeField]
	private List<ScavengeItemController> collectItems = new List<ScavengeItemController>();

	[SerializeField]
	private SDifficultyCollection _difficulties;

	[SerializeField]
	private List<string> _setupExtraInfo = new List<string>();

	public SLevelData LevelData
	{
		get
		{
			return _levelData;
		}
		set
		{
			_levelData = value;
		}
	}

	public List<ScavengeItemController> LevelItems { get; private set; }

	public List<ScavengeItemController> CollectItems { get; private set; }

	public List<string> SetupExtraInfo => _setupExtraInfo;

	public EGameType GameType
	{
		get
		{
			return _gameType;
		}
		set
		{
			_gameType = value;
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

	public string SetupName
	{
		get
		{
			return _setupName;
		}
		set
		{
			_setupName = value;
		}
	}

	public string SetupIcon
	{
		get
		{
			return _setupIcon;
		}
		set
		{
			_setupIcon = value;
		}
	}

	public string SetupDescription
	{
		get
		{
			return _setupDescription;
		}
		set
		{
			_setupDescription = value;
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

	public SDifficultyCollection Difficulties => _difficulties;

	public string ForcedLevelStem => _forcedLevelStem;

	public int ForcedLevelMax => _forcedLevelMax;

	public int ForcedLevelMin => _forcedLevelMin;

	public string RewardId => _rewardId;

	public string AchievementId => _achievementId;

	public bool Enabled => _enabled;

	public int Version => _version;

	public GameSetup()
	{
		LevelItems = levelItems;
		CollectItems = collectItems;
	}

	public static GameSetup[] LoadGameSetup(EGameType gameType)
	{
		return gameType switch
		{
			EGameType.FULL => Resources.LoadAll<GameSetup>("Setups/Full"), 
			EGameType.TUTORIAL => Resources.LoadAll<GameSetup>("Setups/Tutorial"), 
			EGameType.SURVIVAL => Resources.LoadAll<GameSetup>("Setups/Survival"), 
			EGameType.SCAVENGE => Resources.LoadAll<GameSetup>("Setups/Scavenge"), 
			EGameType.CHALLENGE_SCAVENGE => Resources.LoadAll<GameSetup>("DLC/Setups/Challenge/Scavenge"), 
			EGameType.CHALLENGE_SURVIVAL => Resources.LoadAll<GameSetup>("DLC/Setups/Challenge/Survival"), 
			EGameType.SCENARIO => Resources.LoadAll<GameSetup>("DLC/Setups/Scenario"), 
			_ => null, 
		};
	}

	public bool IsScavengeGame()
	{
		if (_gameType != EGameType.SCAVENGE && _gameType != EGameType.FULL && _gameType != EGameType.CHALLENGE_SCAVENGE)
		{
			return IsTutorialGame();
		}
		return true;
	}

	public bool IsTutorialGame()
	{
		return _gameType == EGameType.TUTORIAL;
	}

	public bool IsSurvivalGame()
	{
		if (_gameType != EGameType.FULL && _gameType != EGameType.SURVIVAL && _gameType != EGameType.CHALLENGE_SURVIVAL)
		{
			return _gameType == EGameType.SCENARIO;
		}
		return true;
	}

	public bool IsChallengeGame()
	{
		if (_gameType != EGameType.CHALLENGE_SCAVENGE)
		{
			return _gameType == EGameType.CHALLENGE_SURVIVAL;
		}
		return true;
	}

	public void SetItemsForCharacter(ECharacter character)
	{
		LevelItems = levelItems.Where((ScavengeItemController x) => x.Character != character).ToList();
		CollectItems = collectItems.Where((ScavengeItemController x) => x.Character != character).ToList();
	}

	public string GetRandomScavengeLevelName()
	{
		if (!string.IsNullOrEmpty(_forcedLevelStem) && _forcedLevelMin < _forcedLevelMax)
		{
			int num = UnityEngine.Random.Range(_forcedLevelMin, _forcedLevelMax + 1);
			return _forcedLevelStem + num;
		}
		return null;
	}

	public bool AreSpecificItemsToBeCollected()
	{
		return _collectItems.Count > 0;
	}

	public bool IsThereExtraInfo()
	{
		return _setupExtraInfo.Count > 0;
	}

	public bool DoesUnlockAchievement()
	{
		return !string.IsNullOrEmpty(_achievementId);
	}

	public bool DoesUnlockRewards()
	{
		return !string.IsNullOrEmpty(_rewardId);
	}
}
