using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using RG.SecondsRemaster;
using UnityEngine;

public class GameSessionData : MonoBehaviour
{
	public static GameSessionData Instance;

	private bool _toLoad;

	private bool _loaded;

	private ECurrentGameStage _currentGameStage;

	[SerializeField]
	private GameSetup _setup;

	[SerializeField]
	private EGameDifficulty _difficulty;

	[SerializeField]
	private ECharacter _character = ECharacter.DAD;

	[SerializeField]
	private GameObject _forcedPlayerTemplate;

	[SerializeField]
	private int _prepareTime;

	[SerializeField]
	private int _gameTime = 60;

	[SerializeField]
	private int _comfortZoneTimeout = 20;

	[SerializeField]
	private int _cautionZoneTimeout = 40;

	[SerializeField]
	private bool _customGameTime;

	[SerializeField]
	private Challenge _currentChallenge;

	[SerializeField]
	private bool _gotToBunker;

	[SerializeField]
	private bool _alive;

	private int _daysSurvived = 1;

	private float _scavengeFinishedTime;

	[SerializeField]
	private string[] _collectedItems;

	private List<string> _remainingItems = new List<string>();

	private List<string> _reportedItems = new List<string>();

	private List<string> _damagedItems = new List<string>();

	private List<string> _shelterStock = new List<string>();

	private List<string> _suitcaseStock = new List<string>();

	private int _shelterStockFood;

	private int _shelterStockWater;

	[SerializeField]
	private string _surname = string.Empty;

	[SerializeField]
	private string _majorCharName = string.Empty;

	[SerializeField]
	private string _minorChar1Name = string.Empty;

	[SerializeField]
	private string _minorChar2Name = string.Empty;

	[SerializeField]
	private string _minorChar3Name = string.Empty;

	[SerializeField]
	private string _customEventsPath = string.Empty;

	private string _saveFilename;

	private ConclusionData _conclusionData;

	public bool CustomGameTime
	{
		get
		{
			return _customGameTime;
		}
		set
		{
			_customGameTime = value;
		}
	}

	public EGameDifficulty Difficulty
	{
		get
		{
			return _difficulty;
		}
		set
		{
			_difficulty = value;
		}
	}

	public ECharacter Character
	{
		get
		{
			return _character;
		}
		set
		{
			_character = value;
		}
	}

	public GameObject ForcedPlayerTemplate
	{
		get
		{
			return _forcedPlayerTemplate;
		}
		set
		{
			_forcedPlayerTemplate = value;
		}
	}

	public int TotalCollectedItems => _collectedItems.Length;

	public bool Alive
	{
		get
		{
			return _alive;
		}
		set
		{
			_alive = value;
		}
	}

	public List<string> DamagedItems => _damagedItems;

	public List<string> RemainingItems
	{
		get
		{
			return _remainingItems;
		}
		set
		{
			_remainingItems = value;
		}
	}

	public List<string> ReportedItems
	{
		get
		{
			return _reportedItems;
		}
		set
		{
			_reportedItems = value;
		}
	}

	public bool GotToBunker
	{
		get
		{
			return _gotToBunker;
		}
		set
		{
			_gotToBunker = value;
		}
	}

	public string Surname
	{
		get
		{
			return _surname;
		}
		set
		{
			_surname = value;
		}
	}

	public string[] CollectedItems
	{
		get
		{
			return _collectedItems;
		}
		set
		{
			_collectedItems = value;
		}
	}

	public int DaysSurvived
	{
		get
		{
			return _daysSurvived;
		}
		set
		{
			_daysSurvived = value;
			Instance.Conclusion.DaysSurvived = value;
		}
	}

	public string DaysSurvivedStr => _daysSurvived.ToString();

	public string MajorCharName
	{
		get
		{
			return _majorCharName;
		}
		set
		{
			_majorCharName = value;
		}
	}

	public string MinorChar1Name
	{
		get
		{
			return _minorChar1Name;
		}
		set
		{
			_minorChar1Name = value;
		}
	}

	public string MinorChar2Name
	{
		get
		{
			return _minorChar2Name;
		}
		set
		{
			_minorChar2Name = value;
		}
	}

	public string MinorChar3Name
	{
		get
		{
			return _minorChar3Name;
		}
		set
		{
			_minorChar3Name = value;
		}
	}

	public GameSetup Setup
	{
		get
		{
			return _setup;
		}
		set
		{
			_setup = value;
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

	public bool ToLoad
	{
		get
		{
			return _toLoad;
		}
		set
		{
			_toLoad = value;
			_loaded = true;
		}
	}

	public bool Loaded
	{
		get
		{
			return _loaded;
		}
		set
		{
			_loaded = value;
		}
	}

	public string CustomEventsPath
	{
		get
		{
			return _customEventsPath;
		}
		set
		{
			_customEventsPath = value;
		}
	}

	public ConclusionData Conclusion => _conclusionData;

	public List<string> SuitcaseStock => _suitcaseStock;

	public List<string> ShelterStock => _shelterStock;

	public int ShelterStockFood => _shelterStockFood;

	public int ShelterStockWater => _shelterStockWater;

	public ECurrentGameStage CurrentGameStage
	{
		get
		{
			return _currentGameStage;
		}
		set
		{
			_currentGameStage = value;
		}
	}

	public float ScavengeFinishedTime
	{
		get
		{
			return _scavengeFinishedTime;
		}
		set
		{
			_scavengeFinishedTime = value;
		}
	}

	public Challenge CurrentChallenge
	{
		get
		{
			return _currentChallenge;
		}
		set
		{
			_currentChallenge = value;
		}
	}

	private void Awake()
	{
		_saveFilename = Application.persistentDataPath + "/LastGame.sav";
		_conclusionData = new ConclusionData();
		if (Instance != null)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
			Object.DontDestroyOnLoad(this);
		}
		ValidateSaveFile();
	}

	private void Start()
	{
	}

	public bool DeleteSavefile()
	{
		FileInfo fileInfo = new FileInfo(_saveFilename);
		if (fileInfo.Exists)
		{
			fileInfo.Delete();
			return true;
		}
		return false;
	}

	public bool ValidateSaveFile(bool deleteOnError = true, string filename = "/LastGame.sav")
	{
		bool flag = true;
		if (!string.IsNullOrEmpty(filename))
		{
			string path = Application.persistentDataPath + filename;
			if (File.Exists(path))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				FileStream fileStream = File.Open(path, FileMode.Open);
				int num = 0;
				try
				{
					num = (int)binaryFormatter.Deserialize(fileStream);
				}
				catch
				{
					flag = false;
				}
				finally
				{
					fileStream.Close();
				}
				if (num != Settings.SaveVersion)
				{
					flag = false;
				}
				if (!flag && deleteOnError)
				{
					DeleteSavefile();
				}
			}
		}
		return flag;
	}

	public void SetScavengeData(int prepareTime = 0, int gameTime = 0)
	{
		_customGameTime = prepareTime > 0 || gameTime > 0;
		_prepareTime = prepareTime;
		_gameTime = gameTime;
	}

	public int GetCollectedItemCount(string name)
	{
		int num = 0;
		for (int i = 0; i < _collectedItems.Length; i++)
		{
			if (_collectedItems[i] == name)
			{
				num++;
			}
		}
		return num;
	}

	public GameObject GetPlayerTemplate()
	{
		if (_forcedPlayerTemplate != null)
		{
			return _forcedPlayerTemplate;
		}
		if ((bool)_setup)
		{
			switch (_character)
			{
			case ECharacter.DAD:
				return _setup.LevelData.PlayerTemplate;
			case ECharacter.MOM:
				return _setup.LevelData.DoloresTemplate;
			}
		}
		if (!(_setup == null))
		{
			return _setup.LevelData.PlayerTemplate;
		}
		return null;
	}

	public int GetGameTime()
	{
		if (_customGameTime)
		{
			return _gameTime;
		}
		return _setup.GameTime;
	}

	public int GetPrepareTime()
	{
		if (_customGameTime)
		{
			return _prepareTime;
		}
		return _setup.PrepareTime;
	}

	public int GetComfortZoneTimeout()
	{
		if (_customGameTime)
		{
			return _comfortZoneTimeout;
		}
		return _setup.ComfortZoneTimeout;
	}

	public int GetCautionZoneTimeout()
	{
		if (_customGameTime)
		{
			return _cautionZoneTimeout;
		}
		return _setup.CautionZoneTimeout;
	}

	public SDifficultyData GetCurrentDifficulty()
	{
		return _difficulty switch
		{
			EGameDifficulty.EASY => _setup.Difficulties.Easy, 
			EGameDifficulty.NORMAL => _setup.Difficulties.Normal, 
			EGameDifficulty.HARD => _setup.Difficulties.Hard, 
			_ => default(SDifficultyData), 
		};
	}
}
