using System.Collections.Generic;

public class ConclusionData
{
	private List<string> _survivedChars = new List<string>();

	private List<string> _parameters = new List<string>();

	private bool _controlCharsAlive;

	private bool _goalFailed;

	private bool _goalAchieved;

	private EGameResolution _resolution;

	private bool _absenceFail;

	private int _timesDefendedBunker;

	private float _waterConsumed;

	private float _foodConsumed;

	private int _daysSurvived;

	private int _expeditionCount;

	private int _successfulExpeditions;

	private int _yesCount;

	private int _noCount;

	private int _itemsBroughtBack;

	private string _customConclusionText = string.Empty;

	private string _customConclusionAudio = string.Empty;

	public bool ControlCharsAlive => _controlCharsAlive;

	public bool GoalFailed => _goalFailed;

	public bool GoalAchieved => _goalAchieved;

	public bool AbsenceFail => _absenceFail;

	public EGameResolution Resolution => _resolution;

	public List<string> SurvivedChars => _survivedChars;

	public float WaterConsumed
	{
		get
		{
			return _waterConsumed;
		}
		private set
		{
			_waterConsumed = value;
		}
	}

	public float FoodConsumed
	{
		get
		{
			return _foodConsumed;
		}
		private set
		{
			_foodConsumed = value;
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
		}
	}

	public int TimesDefendedBunker => _timesDefendedBunker;

	public int ExpeditionCount
	{
		get
		{
			return _expeditionCount;
		}
		private set
		{
			_expeditionCount = value;
		}
	}

	public int SuccessfulExpeditions
	{
		get
		{
			return _successfulExpeditions;
		}
		private set
		{
			_successfulExpeditions = value;
		}
	}

	public int YesCount
	{
		get
		{
			return _yesCount;
		}
		private set
		{
			_yesCount = value;
		}
	}

	public int NoCount
	{
		get
		{
			return _noCount;
		}
		private set
		{
			_noCount = value;
		}
	}

	public int ItemsBroughtBack
	{
		get
		{
			return _itemsBroughtBack;
		}
		private set
		{
			_itemsBroughtBack = value;
		}
	}

	public string CustomConclusionText
	{
		get
		{
			return _customConclusionText;
		}
		set
		{
			_customConclusionText = value;
		}
	}

	public string CustomConclusionAudio
	{
		get
		{
			return _customConclusionAudio;
		}
		set
		{
			_customConclusionAudio = value;
		}
	}

	public List<string> Parameters => _parameters;

	public void FillFinalConclusionData(bool controlCharsAlive, bool goalFailed, bool goalAchieved, bool absenceFail, EGameResolution resolution, int timesDefendedBunker)
	{
		_controlCharsAlive = controlCharsAlive;
		_goalFailed = goalFailed;
		_goalAchieved = goalAchieved;
		_absenceFail = absenceFail;
		_resolution = resolution;
		_timesDefendedBunker = timesDefendedBunker;
	}

	public void ClearConclusionData()
	{
		_survivedChars.Clear();
		_controlCharsAlive = false;
		_goalFailed = false;
		_goalAchieved = false;
		_resolution = EGameResolution.NONE;
		_absenceFail = false;
		_timesDefendedBunker = 0;
		_waterConsumed = 0f;
		_foodConsumed = 0f;
		_daysSurvived = 0;
		_expeditionCount = 0;
		_successfulExpeditions = 0;
		_yesCount = 0;
		_noCount = 0;
		_itemsBroughtBack = 0;
		_customConclusionText = string.Empty;
		_customConclusionAudio = string.Empty;
		_parameters.Clear();
	}

	public void AddSurvivingChar(string characterName)
	{
		if (!_survivedChars.Contains(characterName))
		{
			_survivedChars.Add(characterName);
		}
	}

	public void AddParameter(string param)
	{
		if (!_parameters.Contains(param))
		{
			_parameters.Add(param);
		}
	}

	public bool HasSurvived(string characterName)
	{
		return _survivedChars.Contains(characterName);
	}

	public void AddWaterConsumed(float value = 1f)
	{
		WaterConsumed += value;
	}

	public void AddSoupConsumed(float value = 1f)
	{
		FoodConsumed += value;
	}

	public void AddDaysSurvived(int value = 1)
	{
		DaysSurvived += value;
	}

	public void AddExpeditionCount(int value = 1)
	{
		ExpeditionCount += value;
	}

	public void AddSuccessfulExpeditions(int value = 1)
	{
		SuccessfulExpeditions += value;
	}

	public void AddYesCount(int value = 1)
	{
		YesCount += value;
	}

	public void AddNoCount(int value = 1)
	{
		NoCount += value;
	}

	public void AddItemsBroughtBack(int value = 1)
	{
		ItemsBroughtBack += value;
	}
}
