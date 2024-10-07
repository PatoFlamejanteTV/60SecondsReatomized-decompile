using System.Collections.Generic;
using I2.Loc;
using Rewired;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.SecondsRemaster.EventEditor;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace RG.SecondsRemaster.Menu;

public class SettingsManager : MonoBehaviour
{
	[SerializeField]
	[Header("Main Settings")]
	private GlobalStringVariable _languageVariable;

	[SerializeField]
	private GlobalIntVariable _resolutionWidthVariable;

	[SerializeField]
	private GlobalIntVariable _resolutionHeightVariable;

	[SerializeField]
	private GlobalIntVariable _qualityVariable;

	[SerializeField]
	private GlobalBoolVariable _fullscreenVariable;

	[SerializeField]
	private GlobalFloatVariable _masterVolumeVariable;

	[SerializeField]
	private GlobalFloatVariable _soundVolumeVariable;

	[SerializeField]
	private GlobalFloatVariable _musicVolumeVariable;

	[SerializeField]
	private GlobalFloatVariable _gammaVariable;

	[SerializeField]
	private PostProcessingProfile[] _postProcessingProfiles;

	[SerializeField]
	[Header("Control Settings")]
	private GlobalStringVariable _joystickMapsSaveDataVariable;

	[SerializeField]
	private GlobalStringVariable _keyboardMapsSaveDataVariable;

	[SerializeField]
	private GlobalStringVariable _mouseMapsSaveDataVariable;

	[SerializeField]
	private GlobalIntVariable _currentControlModeVariable;

	[SerializeField]
	private bool _loadOnStart;

	[SerializeField]
	[Header("Steam Deck Settings Defaults")]
	private GlobalIntVariable _steamDeckDefaultWidthVariable;

	[SerializeField]
	private GlobalIntVariable _steamDeckDefaultHeightVariable;

	[SerializeField]
	private GlobalIntVariable _steamDeckDefaultQualityVariable;

	[SerializeField]
	private GlobalIntVariable _steamDeckDefaultControlModeVariable;

	public void Start()
	{
		if (_loadOnStart)
		{
			LoadSettings();
		}
	}

	public void LoadSettings()
	{
		SetCurrentLanguage();
		SetCurrentResolution();
		SetCurrentQuality();
		SetCurrentAudio();
		SetCurrentGamma();
		SetSteamDeckSpecificSettings();
		LoadControlMaps();
	}

	private void SetSteamDeckSpecificSettings()
	{
		if (SteamManager.IsRunningOnSteamDeck())
		{
			Screen.SetResolution(_steamDeckDefaultWidthVariable.Value, _steamDeckDefaultHeightVariable.Value, fullscreen: true);
			QualitySettings.SetQualityLevel(_steamDeckDefaultQualityVariable.Value);
			_currentControlModeVariable.Value = _steamDeckDefaultControlModeVariable.Value;
		}
	}

	private void SetCurrentLanguage()
	{
		if (!string.IsNullOrEmpty(_languageVariable.Value))
		{
			LocalizationManager.SetLanguageAndCode(LocalizationManager.GetLanguageFromCode(_languageVariable.Value), _languageVariable.Value);
		}
	}

	private void SetCurrentResolution()
	{
		List<Resolution> list = new List<Resolution>(Screen.resolutions);
		if (!IsResolutionValid(_resolutionWidthVariable.Value, _resolutionHeightVariable.Value))
		{
			_resolutionWidthVariable.Value = list[list.Count - 1].width;
			_resolutionHeightVariable.Value = list[list.Count - 1].height;
			Screen.SetResolution(list[list.Count - 1].width, list[list.Count - 1].height, _fullscreenVariable.Value);
		}
		else
		{
			Screen.SetResolution(_resolutionWidthVariable.Value, _resolutionHeightVariable.Value, _fullscreenVariable.Value);
		}
	}

	private bool IsResolutionValid(int width, int height)
	{
		Resolution[] resolutions = Screen.resolutions;
		bool result = false;
		for (int num = resolutions.Length - 1; num >= 0; num--)
		{
			if (resolutions[num].width == width && resolutions[num].height == height)
			{
				result = true;
			}
		}
		return result;
	}

	private void SetCurrentQuality()
	{
		if (_qualityVariable.Value >= 0)
		{
			QualitySettings.SetQualityLevel(_qualityVariable.Value, applyExpensiveChanges: true);
		}
	}

	private void SetCurrentAudio()
	{
		float num = ((_masterVolumeVariable.Value < 0f) ? 1f : _masterVolumeVariable.Value);
		float num2 = ((_soundVolumeVariable.Value < 0f) ? 1f : _soundVolumeVariable.Value);
		float num3 = ((_musicVolumeVariable.Value < 0f) ? 1f : _musicVolumeVariable.Value);
		AudioManager.Instance.SetMusicVolume(num3 * num);
		AudioManager.Instance.SetSfxVolume(num2 * num);
		AudioManager.Instance.SetUiVolume(num2 * num);
	}

	private void SetCurrentGamma()
	{
		for (int i = 0; i < _postProcessingProfiles.Length; i++)
		{
			ColorGradingModel.Settings settings = _postProcessingProfiles[i].colorGrading.settings;
			settings.basic.postExposure = _gammaVariable.Value;
			_postProcessingProfiles[i].colorGrading.settings = settings;
		}
	}

	private void LoadControlMaps()
	{
		Player player = ReInput.players.GetPlayer(0);
		if (!string.IsNullOrEmpty(_joystickMapsSaveDataVariable.Value))
		{
			MapsDataWrapper mapsDataWrapper = JsonUtility.FromJson<MapsDataWrapper>(_joystickMapsSaveDataVariable.Value);
			player.controllers.maps.ClearMaps(ControllerType.Joystick, userAssignableOnly: true);
			for (int i = 0; i < player.controllers.Joysticks.Count; i++)
			{
				for (int j = 0; j < mapsDataWrapper.MapSaveData.Count; j++)
				{
					player.controllers.maps.AddMapFromJson(ControllerType.Joystick, i, mapsDataWrapper.MapSaveData[j]);
				}
			}
			bool flag = false;
			foreach (ControllerMap item in player.controllers.maps.GetAllMapsInCategory("Gamepad", ControllerType.Joystick))
			{
				if (item.AllMaps.Count < 5)
				{
					flag = true;
				}
			}
			if (flag)
			{
				player.controllers.maps.LoadDefaultMaps(ControllerType.Joystick);
				List<string> list = new List<string>();
				for (int k = 0; k < player.controllers.Joysticks.Count; k++)
				{
					ControllerMapSaveData[] mapSaveData = player.controllers.maps.GetMapSaveData(ControllerType.Joystick, k, userAssignableMapsOnly: true);
					for (int l = 0; l < mapSaveData.Length; l++)
					{
						list.Add(mapSaveData[l].map.ToJsonString());
					}
				}
				MapsDataWrapper mapsDataWrapper2 = default(MapsDataWrapper);
				mapsDataWrapper2.MapSaveData = list;
				MapsDataWrapper mapsDataWrapper3 = mapsDataWrapper2;
				_joystickMapsSaveDataVariable.Value = JsonUtility.ToJson(mapsDataWrapper3);
			}
		}
		else
		{
			Debug.Log("Loading default joystick maps!");
			player.controllers.maps.LoadDefaultMaps(ControllerType.Joystick);
		}
		if (!string.IsNullOrEmpty(_keyboardMapsSaveDataVariable.Value))
		{
			MapsDataWrapper mapsDataWrapper4 = JsonUtility.FromJson<MapsDataWrapper>(_keyboardMapsSaveDataVariable.Value);
			player.controllers.maps.ClearMaps(ControllerType.Keyboard, userAssignableOnly: true);
			for (int m = 0; m < mapsDataWrapper4.MapSaveData.Count; m++)
			{
				player.controllers.maps.AddMapFromJson(ControllerType.Keyboard, 0, mapsDataWrapper4.MapSaveData[m]);
			}
		}
		else if (!string.IsNullOrEmpty(_keyboardMapsSaveDataVariable.GetInitialValue()))
		{
			MapsDataWrapper mapsDataWrapper5 = JsonUtility.FromJson<MapsDataWrapper>(_keyboardMapsSaveDataVariable.GetInitialValue());
			player.controllers.maps.ClearMaps(ControllerType.Keyboard, userAssignableOnly: true);
			for (int n = 0; n < mapsDataWrapper5.MapSaveData.Count; n++)
			{
				player.controllers.maps.AddMapFromJson(ControllerType.Keyboard, 0, mapsDataWrapper5.MapSaveData[n]);
			}
		}
		else
		{
			Debug.Log("Loading default keyboard maps!");
			player.controllers.maps.LoadDefaultMaps(ControllerType.Keyboard);
		}
		if (!string.IsNullOrEmpty(_mouseMapsSaveDataVariable.Value))
		{
			MapsDataWrapper mapsDataWrapper6 = JsonUtility.FromJson<MapsDataWrapper>(_mouseMapsSaveDataVariable.Value);
			player.controllers.maps.ClearMaps(ControllerType.Mouse, userAssignableOnly: true);
			for (int num = 0; num < mapsDataWrapper6.MapSaveData.Count; num++)
			{
				player.controllers.maps.AddMapFromJson(ControllerType.Mouse, 0, mapsDataWrapper6.MapSaveData[num]);
			}
		}
		else if (!string.IsNullOrEmpty(_mouseMapsSaveDataVariable.GetInitialValue()))
		{
			MapsDataWrapper mapsDataWrapper7 = JsonUtility.FromJson<MapsDataWrapper>(_mouseMapsSaveDataVariable.GetInitialValue());
			player.controllers.maps.ClearMaps(ControllerType.Mouse, userAssignableOnly: true);
			for (int num2 = 0; num2 < mapsDataWrapper7.MapSaveData.Count; num2++)
			{
				player.controllers.maps.AddMapFromJson(ControllerType.Mouse, 0, mapsDataWrapper7.MapSaveData[num2]);
			}
		}
		else
		{
			Debug.Log("Loading default mouse maps!");
			player.controllers.maps.LoadDefaultMaps(ControllerType.Mouse);
		}
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
		switch (ePlayerInput)
		{
		case EPlayerInput.KEYBOARD:
			player.controllers.maps.SetMapsEnabled(state: false, 8);
			player.controllers.maps.SetMapsEnabled(state: true, ReInput.mapping.GetMapCategory(7).name, layoutName);
			player.controllers.maps.SetMapsEnabled(state: false, ReInput.mapping.GetMapCategory(7).name, layoutName2);
			player.controllers.maps.SetMapsEnabled(state: false, 9);
			player.controllers.maps.SetMapsEnabled(state: false, 6);
			break;
		case EPlayerInput.KEYBOARD_MOUSE:
			player.controllers.maps.SetMapsEnabled(state: false, 8);
			player.controllers.maps.SetMapsEnabled(state: false, 7);
			player.controllers.maps.SetMapsEnabled(state: true, ReInput.mapping.GetMapCategory(9).name, layoutName);
			player.controllers.maps.SetMapsEnabled(state: false, ReInput.mapping.GetMapCategory(9).name, layoutName2);
			player.controllers.maps.SetMapsEnabled(state: false, 6);
			break;
		case EPlayerInput.GAMEPAD:
			player.controllers.maps.SetMapsEnabled(state: true, 8);
			player.controllers.maps.SetMapsEnabled(state: false, 7);
			player.controllers.maps.SetMapsEnabled(state: false, 9);
			player.controllers.maps.SetMapsEnabled(state: false, 6);
			break;
		case EPlayerInput.MOUSE_ONLY:
			player.controllers.maps.SetMapsEnabled(state: false, 8);
			player.controllers.maps.SetMapsEnabled(state: false, 7);
			player.controllers.maps.SetMapsEnabled(state: false, 9);
			player.controllers.maps.SetMapsEnabled(state: true, 6);
			break;
		}
		DisableKeyboardSnappingCursorButtons();
	}

	private void DisableKeyboardSnappingCursorButtons()
	{
		Player player = ReInput.players.GetPlayer(0);
		int num = 25;
		int num2 = 28;
		foreach (ControllerMap item in player.controllers.maps.GetAllMapsInCategory("Default", ControllerType.Keyboard))
		{
			foreach (ActionElementMap allMap in item.AllMaps)
			{
				if (allMap.actionId == num || allMap.actionId == num2)
				{
					allMap.enabled = false;
				}
			}
		}
	}
}
