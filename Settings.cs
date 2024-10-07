using System;
using System.Collections.Generic;
using System.IO;
using RG.SecondsRemaster;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
	public delegate void Report();

	public static Settings Data;

	public static string VersionPlatform = "STEAM ";

	public static int VersionId = 1402;

	public static int SaveVersion = 13001;

	protected const string DEFAULT_SETTINGS_FILENAME = "Settings.dat";

	protected const string DEFAULT_DATA_FOLDERNAME = "Logs";

	protected const string FALLBACK_LANGUAGE = "EN";

	protected const string LANGUAGE_STR = "Language";

	protected const string SUBTITLES_STR = "Subtitles";

	protected const string QUALITY_INDEX_STR = "QualityIndex";

	protected const string SCREEN_WIDTH_STR = "ScreenWidth";

	protected const string SCREEN_HEIGHT_STR = "ScreenHeight";

	protected const string FULL_SCREEN_STR = "FullScreen";

	protected const string GAMMA_STR = "Gamma";

	protected const string VOLUME_SFX_STR = "VolumeSfx";

	protected const string VOLUME_MUSIC_STR = "VolumeMusic";

	protected const string CONTROL_MODE_STR = "ControlMode";

	protected const string CONTROL_ROTATION_SENSITIVITY_STR = "ControlRotationSensitivity";

	protected const string SKIP_INTRO_STR = "SkipIntro";

	protected const string DISABLE_FLASHES_STR = "DisableFlashes";

	protected const string DATA_LOGGING_STR = "DataLogging";

	protected const string RESEARCH_LOGGING_STR = "ResearchLogging";

	protected const string DAD_HAT_STR = "DadHat";

	protected const string MOM_HAT_STR = "MomHat";

	protected const string SON_HAT_STR = "SonHat";

	protected const string DAUGHTER_HAT_STR = "DaughterHat";

	protected const string RESOLUTION_FORMAT = "{0} x {1}";

	protected const string SETTING_FORMAT = "{0}={1}";

	protected const char DEFAULT_SETTINGS_DIVIDER = '=';

	protected const char DEFAULT_TIME_DIVIDER = ':';

	protected const char DEFAULT_DATA_SEPARATOR = ';';

	protected const char DEFAULT_BOOK_SEPARATOR = ',';

	[SerializeField]
	private string _versionInfo = string.Empty;

	[SerializeField]
	private string _versionDate = DateTime.Now.ToShortDateString();

	[SerializeField]
	private Profile _profile = new Profile();

	[SerializeField]
	protected float _volumeSfx = 1f;

	[SerializeField]
	protected float _volumeMusic = 1f;

	[SerializeField]
	protected float _gamma = 0.85f;

	[SerializeField]
	protected int _screenWidth = 1920;

	[SerializeField]
	protected int _screenHeight = 1080;

	[SerializeField]
	protected bool _fullscreen = true;

	[SerializeField]
	protected string _language = "EN";

	[SerializeField]
	protected bool _subtitles;

	[SerializeField]
	protected int _controlMode;

	[SerializeField]
	protected bool _skipIntro;

	[SerializeField]
	protected bool _disableFlashes;

	[SerializeField]
	protected bool _dataLogging;

	[SerializeField]
	protected bool _researchLogging;

	[SerializeField]
	protected ControlMode[] _supportedControlModes;

	[SerializeField]
	protected string[] _supportedLanguages;

	[SerializeField]
	protected QualitySetting[] _supportedQualitySettings;

	protected Dictionary<string, string> _controls = new Dictionary<string, string>();

	protected string _selectedResStr = string.Empty;

	protected int _viewedResIndex;

	protected int _selectedResX;

	protected int _selectedResY;

	protected string _selectedLanguageStr = string.Empty;

	protected int _selectedLanguage = -1;

	protected int _viewedQualityIndex;

	protected int _selectedQualityIndex;

	protected bool _fullscreenForce;

	protected bool _firstSetup = true;

	protected bool _languageSet = true;

	protected int _activeGamepad = -1;

	protected string _version = string.Empty;

	protected bool _windowedForce;

	[SerializeField]
	private Localization _localization = new Localization();

	private CursorLockMode _lockCursor = CursorLockMode.Confined;

	private bool _showCursor;

	public string SettingsFilepath => Application.persistentDataPath + "/Settings.dat";

	public string DataFilepath => Application.persistentDataPath + "/Logs/";

	public string VersionInfo
	{
		get
		{
			return _versionInfo;
		}
		set
		{
			_versionInfo = value;
		}
	}

	public string VersionDate
	{
		get
		{
			return _versionDate;
		}
		set
		{
			_versionDate = value;
		}
	}

	public float VolumeSfx
	{
		get
		{
			return _volumeSfx;
		}
		set
		{
			_volumeSfx = value;
		}
	}

	public float VolumeMusic
	{
		get
		{
			return _volumeMusic;
		}
		set
		{
			_volumeMusic = value;
		}
	}

	public float Gamma
	{
		get
		{
			return _gamma;
		}
		set
		{
			_gamma = value;
		}
	}

	public int ResX
	{
		get
		{
			return _screenWidth;
		}
		set
		{
			_screenWidth = value;
		}
	}

	public int ResY
	{
		get
		{
			return _screenHeight;
		}
		set
		{
			_screenHeight = value;
		}
	}

	public bool Fullscreen
	{
		get
		{
			return _fullscreen;
		}
		set
		{
			_fullscreen = value;
		}
	}

	public bool FullscreenForce
	{
		get
		{
			return _fullscreenForce;
		}
		set
		{
			_fullscreenForce = value;
		}
	}

	public bool WindowedForce
	{
		get
		{
			return _windowedForce;
		}
		set
		{
			_windowedForce = value;
		}
	}

	public string Language
	{
		get
		{
			return _language;
		}
		set
		{
			_language = value;
		}
	}

	public bool Subtitles
	{
		get
		{
			return _subtitles;
		}
		set
		{
			_subtitles = value;
		}
	}

	public bool LanguageSet => _languageSet;

	public string CurrentResolution
	{
		get
		{
			return _selectedResStr;
		}
		set
		{
			_selectedResStr = value;
		}
	}

	public string CurrentLanguage => _selectedLanguageStr;

	public string CurrentLanguageCode
	{
		get
		{
			if (_supportedLanguages == null || _supportedLanguages.Length <= _selectedLanguage || _selectedLanguage < 0)
			{
				return "EN";
			}
			return _supportedLanguages[_selectedLanguage];
		}
	}

	public QualitySetting CurrentQualitySetting
	{
		get
		{
			if (_supportedQualitySettings == null || _supportedQualitySettings.Length <= _viewedQualityIndex || _viewedQualityIndex < 0)
			{
				return null;
			}
			return _supportedQualitySettings[_viewedQualityIndex];
		}
	}

	public Localization LocalizationManager => _localization;

	public Profile PlayerProfile => _profile;

	public ControlMode ControlMode => _supportedControlModes[_controlMode];

	public string ControlModeName => ControlMode.Name;

	public string ControlModeIcon => ControlMode.Icon;

	public float ControlModeRotationSensitivity
	{
		get
		{
			return ControlMode.RotationSensitivity;
		}
		set
		{
			ControlMode.RotationSensitivity = value;
		}
	}

	public CursorLockMode LockCursor
	{
		get
		{
			return _lockCursor;
		}
		set
		{
			_lockCursor = value;
		}
	}

	public bool ShowCursor
	{
		get
		{
			return _showCursor;
		}
		set
		{
			_showCursor = value;
		}
	}

	public bool SkipIntro
	{
		get
		{
			return _skipIntro;
		}
		set
		{
			_skipIntro = value;
		}
	}

	public bool DisableFlashes
	{
		get
		{
			return _disableFlashes;
		}
		set
		{
			_disableFlashes = value;
		}
	}

	public bool ResearchLogging
	{
		get
		{
			return _researchLogging;
		}
		set
		{
			_researchLogging = value;
		}
	}

	public bool DataLogging
	{
		get
		{
			return _dataLogging;
		}
		set
		{
			_dataLogging = value;
		}
	}

	public Dictionary<string, string> Controls => _controls;

	public int ActiveGamepad => _activeGamepad;

	public string Version => _version;

	public event Report OnSettingsUpdated;

	public void Awake()
	{
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	protected void Initialize()
	{
		if (Data == null)
		{
			Data = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		bool flag = LoadSettings(SettingsFilepath);
		ForceSteamBigPicture();
		if (!flag)
		{
			string key = "EN";
			switch (SteamApps.GetCurrentGameLanguage())
			{
			case "english":
				key = "EN";
				break;
			case "polish":
				key = "PL";
				break;
			case "german":
				key = "DE";
				break;
			case "spanish":
				key = "ES";
				break;
			case "russian":
				key = "RU";
				break;
			case "japanese":
				key = "JA";
				break;
			case "italian":
				key = "IT";
				break;
			case "brazillian":
				key = "BR";
				break;
			case "french":
				key = "FR";
				break;
			}
			SelectLanguage(key);
		}
		ForceGamepad(force: false);
		SelectLanguage();
		UpdateSettings();
		ForceScreenNativeResolution(flag, maxResolution: true);
		UpdateResolutionStr();
		SetDelimiter();
		_profile.Initialize();
		InputHandler.Instance.Reload(_controls, GetControlMode(_controlMode));
		GetComponent<ResolutionHandler>().HandleResolution();
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		ReloadSettings();
		if (DoesCurrentLanguageWordwrap())
		{
			dfRichTextLabel[] array = UnityEngine.Object.FindObjectsOfType<dfRichTextLabel>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].PreserveWhitespace = true;
			}
		}
	}

	protected void ReloadSettings()
	{
	}

	protected void LoadSupportedLanguages(string bookText)
	{
		List<string> list = new List<string>();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < bookText.Length; i++)
		{
			bool flag = bookText[i] == '\n';
			if (bookText[i] == ',' || bookText[i] == ';' || flag)
			{
				num2++;
				if (num2 > 1)
				{
					list.Add(bookText.Substring(num, (flag ? (i - 1) : i) - num));
				}
				num = i + 1;
			}
			if (flag)
			{
				break;
			}
		}
		if (list.Count > 0)
		{
			_supportedLanguages = list.ToArray();
		}
	}

	protected void CreateSettingsFile()
	{
		if (!File.Exists(SettingsFilepath))
		{
			File.Create(SettingsFilepath);
		}
	}

	protected void DeleteSetting(string key)
	{
	}

	protected void DeleteSettingsFile()
	{
		File.Delete(SettingsFilepath);
	}

	protected string LoadSetting(string key)
	{
		return null;
	}

	protected bool ParseSetting(string rawVal, ref bool val)
	{
		if (!string.IsNullOrEmpty(rawVal))
		{
			return bool.TryParse(rawVal, out val);
		}
		return false;
	}

	protected bool ParseSetting(string rawVal, ref int val)
	{
		if (!string.IsNullOrEmpty(rawVal))
		{
			return int.TryParse(rawVal, out val);
		}
		return false;
	}

	protected bool ParseSetting(string rawVal, ref float val)
	{
		if (!string.IsNullOrEmpty(rawVal))
		{
			return float.TryParse(rawVal, out val);
		}
		return false;
	}

	protected bool LoadSettings(string filepath, bool update = false)
	{
		bool result = true;
		try
		{
			using StreamReader streamReader = new StreamReader(filepath);
			while (!streamReader.EndOfStream)
			{
				string text = streamReader.ReadLine();
				try
				{
					for (int i = 0; i < text.Length; i++)
					{
						if (text[i] != '=')
						{
							continue;
						}
						i++;
						string text2 = text.Substring(0, i - 1);
						if (text2.Contains("ControlRotationSensitivity"))
						{
							int result2 = 0;
							if (int.TryParse(text2.Substring(text2.Length - 1, 1), out result2))
							{
								float result3 = 1f;
								if (float.TryParse(text.Substring(i, text.Length - i), out result3))
								{
									GetControlMode(result2).RotationSensitivity = result3;
								}
							}
						}
						bool flag = false;
						switch (text2)
						{
						case "Language":
						{
							string text3 = text.Substring(i, text.Length - i);
							for (int j = 0; j < _supportedLanguages.Length; j++)
							{
								if (_supportedLanguages[j] == text3)
								{
									_selectedLanguage = j;
									break;
								}
							}
							break;
						}
						case "Subtitles":
							bool.TryParse(text.Substring(i, text.Length - i), out _subtitles);
							break;
						case "QualityIndex":
							int.TryParse(text.Substring(i, text.Length - i), out _selectedQualityIndex);
							break;
						case "ScreenWidth":
							int.TryParse(text.Substring(i, text.Length - i), out _screenWidth);
							_selectedResX = _screenWidth;
							break;
						case "ScreenHeight":
							int.TryParse(text.Substring(i, text.Length - i), out _screenHeight);
							_selectedResY = _screenHeight;
							break;
						case "FullScreen":
							bool.TryParse(text.Substring(i, text.Length - i), out _fullscreen);
							break;
						case "Gamma":
							float.TryParse(text.Substring(i, text.Length - i), out _gamma);
							_gamma = Mathf.Clamp(_gamma, 0.25f, 2f);
							break;
						case "VolumeSfx":
							float.TryParse(text.Substring(i, text.Length - i), out _volumeSfx);
							break;
						case "VolumeMusic":
							float.TryParse(text.Substring(i, text.Length - i), out _volumeMusic);
							break;
						case "ControlMode":
							int.TryParse(text.Substring(i, text.Length - i), out _controlMode);
							if (_controlMode >= 0)
							{
								SetControlMode(_controlMode);
							}
							break;
						case "SkipIntro":
							bool.TryParse(text.Substring(i, text.Length - i), out _skipIntro);
							break;
						case "DisableFlashes":
							bool.TryParse(text.Substring(i, text.Length - i), out _disableFlashes);
							break;
						case "DataLogging":
							bool.TryParse(text.Substring(i, text.Length - i), out _dataLogging);
							break;
						case "ResearchLogging":
							bool.TryParse(text.Substring(i, text.Length - i), out _researchLogging);
							break;
						case "KM1_SCAVENGE_FORWARD":
							flag = true;
							break;
						case "KM1_SCAVENGE_BACKWARD":
							flag = true;
							break;
						case "KM1_SCAVENGE_ROTATE_LEFT":
							flag = true;
							break;
						case "KM1_SCAVENGE_ROTATE_RIGHT":
							flag = true;
							break;
						case "KM1_SCAVENGE_INTERACTION":
							flag = true;
							break;
						case "KM1_GLOBAL_MENU":
							flag = true;
							break;
						case "KM2_SCAVENGE_FORWARD":
							flag = true;
							break;
						case "KM2_SCAVENGE_BACKWARD":
							flag = true;
							break;
						case "KM2_SCAVENGE_STRAFE_LEFT":
							flag = true;
							break;
						case "KM2_SCAVENGE_STRAFE_RIGHT":
							flag = true;
							break;
						case "KM2_GLOBAL_MENU":
							flag = true;
							break;
						case "KM2_SCAVENGE_INTERACTION":
							flag = true;
							break;
						case "JOY_SCAVENGE_INTERACTION":
							flag = true;
							break;
						case "JOY_GLOBAL_MENU":
							flag = true;
							break;
						case "JOY_GLOBAL_ACTION1":
							flag = true;
							break;
						case "JOY_GLOBAL_ACTION2":
							flag = true;
							break;
						case "JOY_GLOBAL_ALTCHOICE1":
							flag = true;
							break;
						case "JOY_GLOBAL_ALTCHOICE2":
							flag = true;
							break;
						case "JOY_GLOBAL_ALTCHOICE3":
							flag = true;
							break;
						case "JOY_GLOBAL_ALTCHOICE4":
							flag = true;
							break;
						case "JOY_GLOBAL_CHOICE1":
							flag = true;
							break;
						case "JOY_GLOBAL_CHOICE2":
							flag = true;
							break;
						case "JOY_GLOBAL_CHOICE3":
							flag = true;
							break;
						case "JOY_GLOBAL_CHOICE4":
							flag = true;
							break;
						case "JOY_GLOBAL_NEXT":
							flag = true;
							break;
						case "JOY_GLOBAL_PREV":
							flag = true;
							break;
						case "MO_SCAVENGE_INTERACTION":
							flag = true;
							break;
						case "MO_SCAVENGE_FORWARD":
							flag = true;
							break;
						case "MO_GLOBAL_MENU":
							flag = true;
							break;
						}
						if (flag)
						{
							string value = text.Substring(i, text.Length - i);
							if (_controls.ContainsKey(text2))
							{
								_controls[text2] = value;
							}
							else
							{
								_controls.Add(text2, value);
							}
						}
						break;
					}
				}
				catch
				{
					result = false;
				}
			}
		}
		catch
		{
			result = false;
		}
		if (update)
		{
			UpdateSettings();
		}
		return result;
	}

	public void RefreshFullscreen(bool fullscreen = true)
	{
		Fullscreen = fullscreen;
		SetResolution(commit: true);
	}

	protected void SaveSetting(string key, string val, StreamWriter wr)
	{
		wr.WriteLine($"{key}={val}");
	}

	protected void SaveSettings(string filepath = null, bool cleanup = false)
	{
		using (StreamWriter streamWriter = new StreamWriter(filepath))
		{
			SaveSetting("Language", CurrentLanguageCode, streamWriter);
			SaveSetting("Subtitles", _subtitles.ToString(), streamWriter);
			_screenWidth = _selectedResX;
			_screenHeight = _selectedResY;
			SaveSetting("ScreenWidth", _screenWidth.ToString(), streamWriter);
			SaveSetting("ScreenHeight", _screenHeight.ToString(), streamWriter);
			SaveSetting("QualityIndex", _selectedQualityIndex.ToString(), streamWriter);
			SaveSetting("FullScreen", _fullscreen.ToString(), streamWriter);
			SaveSetting("Gamma", _gamma.ToString(), streamWriter);
			SaveSetting("VolumeSfx", _volumeSfx.ToString(), streamWriter);
			SaveSetting("VolumeMusic", _volumeMusic.ToString(), streamWriter);
			SaveSetting("ControlMode", _controlMode.ToString(), streamWriter);
			SaveSetting("SkipIntro", _skipIntro.ToString(), streamWriter);
			SaveSetting("DisableFlashes", _disableFlashes.ToString(), streamWriter);
			SaveSetting("DataLogging", _dataLogging.ToString(), streamWriter);
			SaveSetting("ResearchLogging", _researchLogging.ToString(), streamWriter);
			for (int i = 0; i < _supportedControlModes.Length; i++)
			{
				SaveSetting("ControlRotationSensitivity" + i, GetControlMode(i).RotationSensitivity.ToString(), streamWriter);
			}
			foreach (string key in _controls.Keys)
			{
				SaveSetting(key, _controls[key].ToString(), streamWriter);
			}
			streamWriter.Flush();
			streamWriter.Close();
		}
		SetDelimiter();
		if (cleanup && this.OnSettingsUpdated != null)
		{
			this.OnSettingsUpdated();
		}
	}

	public void ResetSettings()
	{
		DeleteSettingsFile();
		if (this.OnSettingsUpdated != null)
		{
			this.OnSettingsUpdated();
		}
	}

	public void ApplySettingsManually(bool cleanup)
	{
		SaveSettings(SettingsFilepath, cleanup);
	}

	public void ApplySettings()
	{
		SaveSettings(SettingsFilepath, cleanup: true);
		InputHandler.Instance.Reload(_controls, GetControlMode(_controlMode));
	}

	public void RejectSettings()
	{
		LoadSettings(SettingsFilepath, update: true);
	}

	protected void PropagateSettings()
	{
		UpdateSettings();
	}

	public void UpdateSettings(bool advanced = true)
	{
		_localization.Bind(CurrentLanguageCode);
		SelectLanguage();
		_languageSet = true;
		if (advanced)
		{
			SetQuality(commit: true);
			if (!FullscreenForce)
			{
				SetResolution(commit: true);
			}
		}
		SetGamma();
		SetSfxVolume();
		SetMusicVolume();
		TryToAddMissingControl("KM1_SCAVENGE_INTERACTION", KeyCode.Space.ToString());
		TryToAddMissingControl("KM1_SCAVENGE_BACKWARD", KeyCode.S.ToString());
		TryToAddMissingControl("KM1_SCAVENGE_FORWARD", KeyCode.W.ToString());
		TryToAddMissingControl("KM1_GLOBAL_MENU", KeyCode.Escape.ToString());
		TryToAddMissingControl("KM1_SCAVENGE_ROTATE_LEFT", KeyCode.A.ToString());
		TryToAddMissingControl("KM1_SCAVENGE_ROTATE_RIGHT", KeyCode.D.ToString());
		TryToAddMissingControl("KM2_SCAVENGE_BACKWARD", KeyCode.S.ToString());
		TryToAddMissingControl("KM2_SCAVENGE_FORWARD", KeyCode.W.ToString());
		TryToAddMissingControl("KM2_GLOBAL_MENU", KeyCode.Escape.ToString());
		TryToAddMissingControl("KM2_SCAVENGE_STRAFE_LEFT", KeyCode.A.ToString());
		TryToAddMissingControl("KM2_SCAVENGE_STRAFE_RIGHT", KeyCode.D.ToString());
		TryToAddMissingControl("KM2_SCAVENGE_INTERACTION", "MOUSE_0");
		TryToAddMissingControl("MO_SCAVENGE_INTERACTION", "MOUSE_0");
		TryToAddMissingControl("MO_SCAVENGE_FORWARD", "MOUSE_1");
		TryToAddMissingControl("MO_GLOBAL_MENU", "MOUSE_2");
		TryToAddMissingControl("JOY_SCAVENGE_INTERACTION", "JOY_" + InputHandler.GetJoyButtonCode(0));
		TryToAddMissingControl("JOY_GLOBAL_MENU", "JOY_" + InputHandler.GetJoyButtonCode(7));
		TryToAddMissingControl("JOY_GLOBAL_ACTION1", "JOY_" + InputHandler.GetJoyButtonCode(4));
		TryToAddMissingControl("JOY_GLOBAL_ACTION2", "JOY_" + InputHandler.GetJoyButtonCode(5));
		TryToAddMissingControl("JOY_GLOBAL_ALTCHOICEX", "JoyAxis" + InputHandler.GetJoyAxis(6));
		TryToAddMissingControl("JOY_GLOBAL_ALTCHOICEY", "JoyAxis" + InputHandler.GetJoyAxis(7));
		TryToAddMissingControl("JOY_GLOBAL_CHOICE1", "JOY_" + InputHandler.GetJoyButtonCode(0));
		TryToAddMissingControl("JOY_GLOBAL_CHOICE2", "JOY_" + InputHandler.GetJoyButtonCode(1));
		TryToAddMissingControl("JOY_GLOBAL_CHOICE3", "JOY_" + InputHandler.GetJoyButtonCode(3));
		TryToAddMissingControl("JOY_GLOBAL_CHOICE4", "JOY_" + InputHandler.GetJoyButtonCode(2));
		TryToAddMissingControl("JOY_GLOBAL_NEXT", "JoyAxis" + InputHandler.GetJoyAxis(10));
		TryToAddMissingControl("JOY_GLOBAL_PREV", "JoyAxis" + InputHandler.GetJoyAxis(9));
	}

	private void TryToAddMissingControl(string key, string val)
	{
		if (!_controls.ContainsKey(key))
		{
			_controls.Add(key, val);
		}
	}

	private void ChangeControlMode(bool next)
	{
		bool flag = false;
		int num = _controlMode;
		while (!flag)
		{
			int num2 = num + (next ? 1 : (-1));
			if (num2 < 0)
			{
				num2 = _supportedControlModes.Length - 1;
			}
			else if (num2 >= _supportedControlModes.Length)
			{
				num2 = 0;
			}
			num = num2;
			if (_supportedControlModes[num2].Enabled && !_supportedControlModes[num2].Mobile && (_supportedControlModes[num2].ScavengeControl != EPlayerInput.GAMEPAD || GetValidGamepad() >= 0))
			{
				SetControlMode(num2);
				flag = true;
			}
		}
	}

	private void SetControlMode(int index)
	{
		_controlMode = index;
		if (ControlMode.IsGamepad())
		{
			_activeGamepad = GetValidGamepad();
		}
	}

	public void ChangeControlMode(string key)
	{
		if (_supportedControlModes == null)
		{
			return;
		}
		for (int i = 0; i < _supportedControlModes.Length; i++)
		{
			if (_supportedControlModes[i].Key == key)
			{
				SetControlMode(i);
				break;
			}
		}
	}

	public void NextControl()
	{
		ChangeControlMode(next: true);
	}

	public void PrevControl()
	{
		ChangeControlMode(next: false);
	}

	public ControlMode GetControlMode(int index)
	{
		if (_supportedControlModes != null && index >= 0 && _supportedControlModes.Length > index)
		{
			return _supportedControlModes[index];
		}
		return null;
	}

	public ControlMode GetControlMode(EPlayerInput moveType)
	{
		if (_supportedControlModes != null)
		{
			for (int i = 0; i < _supportedControlModes.Length; i++)
			{
				if (_supportedControlModes[i].ScavengeControl == moveType)
				{
					return _supportedControlModes[i];
				}
			}
		}
		return null;
	}

	public void SetQuality(bool commit)
	{
		FindCurrentQuality();
		QualitySetting currentQualitySetting = CurrentQualitySetting;
		if (commit)
		{
			currentQualitySetting?.Set();
		}
	}

	public void SetResolution(bool commit)
	{
		FindCurrentResolution();
		SelectResolution();
		if (commit)
		{
			if (_windowedForce)
			{
				_fullscreen = false;
			}
			Screen.SetResolution(_screenWidth, _screenHeight, _fullscreen);
		}
	}

	public void SetGamma()
	{
		GammaCorrectionEffect[] array = UnityEngine.Object.FindObjectsOfType<GammaCorrectionEffect>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gamma = _gamma;
		}
	}

	public void NextQualitySetting()
	{
		FindViewedQuality(next: true);
	}

	public void PrevQualitySetting()
	{
		FindViewedQuality(next: false);
	}

	public void NextResolution()
	{
		FindViewedRes(next: true);
	}

	public void PrevResolution()
	{
		FindViewedRes(next: false);
	}

	private bool SelectQuality()
	{
		if (_viewedQualityIndex >= 0 && _viewedQualityIndex < _supportedQualitySettings.Length)
		{
			_selectedQualityIndex = _viewedQualityIndex;
			return true;
		}
		return false;
	}

	private bool SelectResolution()
	{
		Resolution[] resolutions = GetComponent<ResolutionHandler>().Resolutions;
		if (_viewedResIndex >= 0 && _viewedResIndex < resolutions.Length)
		{
			_selectedResX = resolutions[_viewedResIndex].width;
			_selectedResY = resolutions[_viewedResIndex].height;
			UpdateResolutionStr();
			return true;
		}
		return false;
	}

	private void UpdateResolutionStr()
	{
		_selectedResStr = $"{_selectedResX} x {_selectedResY}";
	}

	private bool FindCurrentQuality()
	{
		if (_supportedQualitySettings != null && _selectedQualityIndex >= 0 && _supportedQualitySettings.Length != 0 && _supportedQualitySettings.Length > _selectedQualityIndex)
		{
			_viewedQualityIndex = _selectedQualityIndex;
			return true;
		}
		_viewedQualityIndex = -1;
		return false;
	}

	private int GetValidGamepad()
	{
		string[] joystickNames = Input.GetJoystickNames();
		if (joystickNames.Length != 0)
		{
			for (int i = 0; i < joystickNames.Length; i++)
			{
				if (!string.IsNullOrEmpty(joystickNames[i]))
				{
					return i + 1;
				}
			}
		}
		return -1;
	}

	private bool IsGamepadValid(int index)
	{
		string[] joystickNames = Input.GetJoystickNames();
		if (joystickNames.Length > index)
		{
			return !string.IsNullOrEmpty(joystickNames[index]);
		}
		return false;
	}

	private void ForceGamepad(bool force = true, bool skipSafetyCheck = false)
	{
		bool flag = false;
		Input.GetJoystickNames();
		if (force)
		{
			if (GetValidGamepad() >= 0 || skipSafetyCheck)
			{
				ChangeControlMode("JOY");
				flag = true;
			}
		}
		else if (GetValidGamepad() < 0 && ControlMode.ScavengeControl == EPlayerInput.GAMEPAD)
		{
			ChangeControlMode("KM2");
			flag = true;
		}
		if (flag)
		{
			SaveSettings(SettingsFilepath);
		}
	}

	private void ForceSteamBigPicture()
	{
		if (SteamManager.Initialized)
		{
			int result = 0;
			if (int.TryParse(Environment.GetEnvironmentVariable("SteamTenfoot"), out result) && result > 0)
			{
				ForceGamepad();
			}
		}
	}

	private void ForceScreenNativeResolution(bool fileLoaded, bool maxResolution)
	{
		if (fileLoaded)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		if (maxResolution)
		{
			Resolution[] resolutions = GetComponent<ResolutionHandler>().Resolutions;
			if (resolutions.Length != 0)
			{
				num = resolutions[^1].width;
				num2 = resolutions[^1].height;
			}
		}
		else
		{
			num = Screen.width;
			num2 = Screen.height;
		}
		if (num > 0 && num2 > 0)
		{
			_screenWidth = num;
			_selectedResX = num;
			_screenHeight = num2;
			_selectedResY = num2;
			SetResolution(commit: true);
		}
	}

	private void FindCurrentResolution()
	{
		Resolution[] resolutions = GetComponent<ResolutionHandler>().Resolutions;
		for (int i = 0; i < resolutions.Length; i++)
		{
			if (_screenWidth == resolutions[i].width && _screenHeight == resolutions[i].height)
			{
				_viewedResIndex = i;
				break;
			}
		}
	}

	public void FindViewedQuality(bool next)
	{
		if (next)
		{
			_viewedQualityIndex++;
		}
		else
		{
			_viewedQualityIndex--;
		}
		_viewedQualityIndex = Mathf.Clamp(_viewedQualityIndex, 0, _supportedQualitySettings.Length - 1);
		SelectQuality();
	}

	public void FindViewedRes(bool next)
	{
		Resolution[] resolutions = GetComponent<ResolutionHandler>().Resolutions;
		if (next)
		{
			_viewedResIndex++;
		}
		else
		{
			_viewedResIndex--;
		}
		_viewedResIndex = Mathf.Clamp(_viewedResIndex, 0, resolutions.Length - 1);
		SelectResolution();
	}

	public void SetSfxVolume()
	{
		SoundManager.Instance.VolumeSfx = _volumeSfx;
	}

	public void SetMusicVolume()
	{
		SoundManager.Instance.VolumeMusic = _volumeMusic;
	}

	public void NextLanguage()
	{
		_languageSet = false;
		_selectedLanguage++;
		if (_selectedLanguage >= _supportedLanguages.Length)
		{
			_selectedLanguage = 0;
		}
		SelectLanguage();
	}

	public void PrevLanguage()
	{
		_languageSet = false;
		_selectedLanguage--;
		if (_selectedLanguage < 0)
		{
			_selectedLanguage = _supportedLanguages.Length - 1;
		}
		SelectLanguage();
	}

	public bool SelectLanguage(string key)
	{
		if (_supportedLanguages != null)
		{
			for (int i = 0; i < _supportedLanguages.Length; i++)
			{
				if (_supportedLanguages[i] == key)
				{
					_selectedLanguage = i;
					return true;
				}
			}
		}
		return false;
	}

	public void SelectLanguage()
	{
		string text = null;
		if (_supportedLanguages != null && _selectedLanguage >= 0 && _selectedLanguage < _supportedLanguages.Length)
		{
			text = _supportedLanguages[_selectedLanguage];
		}
		if (string.IsNullOrEmpty(text))
		{
			text = "EN";
		}
		text = "lang_" + text;
		_selectedLanguageStr = text;
	}

	public bool DoesCurrentLanguageWordwrap()
	{
		if (CurrentLanguageCode == "JA")
		{
			return true;
		}
		if (CurrentLanguageCode == "ZH")
		{
			return true;
		}
		return false;
	}

	private void SetDelimiter()
	{
		if (DoesCurrentLanguageWordwrap())
		{
			WordWrapper instance = WordWrapper.GetInstance();
			instance.WordSeparator = string.Empty;
			instance.DefaultDelimiter = '`';
			if (CurrentLanguageCode == "ZH")
			{
				instance.RecalculateLatinCharacters = true;
			}
		}
	}
}
