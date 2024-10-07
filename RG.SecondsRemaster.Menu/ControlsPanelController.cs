using System.Collections.Generic;
using Rewired;
using RG.Core.SaveSystem;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.SecondsRemaster.EventEditor;
using RG.VirtualInput;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Menu;

public class ControlsPanelController : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _controlModes;

	[SerializeField]
	private EPlayerInput[] _playerMovements;

	[SerializeField]
	private EMapCategory[] _mapCategories;

	[SerializeField]
	private GlobalStringVariable _joystickMapsSaveDataVariable;

	[SerializeField]
	private GlobalStringVariable _keyboardMapsSaveDataVariable;

	[SerializeField]
	private GlobalStringVariable _mouseMapsSaveDataVariable;

	[SerializeField]
	private GlobalIntVariable _controlModeVariable;

	[SerializeField]
	private EPlayerInput _defaultPlayerMovement;

	[SerializeField]
	private ClosePanelOnCancelPress _mainPanel;

	[SerializeField]
	private List<Button> _buttonsToDisableOnSteamDeck = new List<Button>();

	private int _currentIndex;

	private Player _player;

	private void Awake()
	{
		_player = ReInput.players.GetPlayer(0);
	}

	public void OnEnable()
	{
		if (_controlModeVariable.Value < 0)
		{
			_controlModeVariable.Value = (int)_defaultPlayerMovement;
		}
		_mainPanel.SetCloseActionBlocked(block: true);
		SetCurrentControlMode();
		DisableSteamDeckButtons();
	}

	private void DisableSteamDeckButtons()
	{
		if (SteamManager.IsRunningOnSteamDeck())
		{
			for (int i = 0; i <= _buttonsToDisableOnSteamDeck.Count - 1; i++)
			{
				_buttonsToDisableOnSteamDeck[i].interactable = false;
			}
		}
	}

	public void OnDisable()
	{
		_mainPanel.SetCloseActionBlocked(block: false);
		SaveControlMaps();
	}

	private void SetCurrentControlMode()
	{
		for (int i = 0; i < _playerMovements.Length; i++)
		{
			if (_controlModeVariable.Value == (int)_playerMovements[i])
			{
				_currentIndex = i;
				_controlModes[i].SetActive(value: true);
				_player.controllers.maps.SetMapsEnabled(state: true, (int)_mapCategories[i]);
			}
			else
			{
				_controlModes[i].SetActive(value: false);
				_player.controllers.maps.SetMapsEnabled(state: false, (int)_mapCategories[i]);
			}
		}
	}

	public void SetNextControlMode()
	{
		_controlModes[_currentIndex].SetActive(value: false);
		_player.controllers.maps.SetMapsEnabled(state: false, (int)_mapCategories[_currentIndex]);
		if (_currentIndex + 1 >= _playerMovements.Length)
		{
			_currentIndex = 0;
		}
		else
		{
			_currentIndex++;
		}
		_controlModeVariable.Value = (int)_playerMovements[_currentIndex];
		_controlModes[_currentIndex].SetActive(value: true);
		_player.controllers.maps.SetMapsEnabled(state: true, (int)_mapCategories[_currentIndex]);
		Singleton<VirtualInputManager>.Instance.RefreshSelectablesModeSelection();
	}

	public void SetPreviousControlMode()
	{
		_controlModes[_currentIndex].SetActive(value: false);
		_player.controllers.maps.SetMapsEnabled(state: false, (int)_mapCategories[_currentIndex]);
		if (_currentIndex - 1 < 0)
		{
			_currentIndex = _playerMovements.Length - 1;
		}
		else
		{
			_currentIndex--;
		}
		_controlModeVariable.Value = (int)_playerMovements[_currentIndex];
		_controlModes[_currentIndex].SetActive(value: true);
		_player.controllers.maps.SetMapsEnabled(state: true, (int)_mapCategories[_currentIndex]);
		Singleton<VirtualInputManager>.Instance.RefreshSelectablesModeSelection();
	}

	public void RefreshMap()
	{
		if (_player != null)
		{
			if (_playerMovements[_currentIndex] == EPlayerInput.KEYBOARD || _playerMovements[_currentIndex] == EPlayerInput.KEYBOARD_MOUSE)
			{
				int layoutId = ((Application.systemLanguage == SystemLanguage.French) ? 1 : 0);
				int layoutId2 = ((Application.systemLanguage != SystemLanguage.French) ? 1 : 0);
				string layoutName = ReInput.mapping.GetLayout(ControllerType.Keyboard, layoutId).name;
				string layoutName2 = ReInput.mapping.GetLayout(ControllerType.Keyboard, layoutId2).name;
				string categoryName = ReInput.mapping.GetMapCategory((int)_mapCategories[_currentIndex]).name;
				_player.controllers.maps.SetMapsEnabled(state: false, (int)_mapCategories[_currentIndex]);
				_player.controllers.maps.SetMapsEnabled(state: true, categoryName, layoutName);
				_player.controllers.maps.SetMapsEnabled(state: false, categoryName, layoutName2);
			}
			else
			{
				_player.controllers.maps.SetMapsEnabled(state: false, (int)_mapCategories[_currentIndex]);
				_player.controllers.maps.SetMapsEnabled(state: true, (int)_mapCategories[_currentIndex]);
			}
		}
	}

	public void SaveControlMaps()
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		MapsDataWrapper mapsDataWrapper;
		if (_player.controllers.joystickCount > 0)
		{
			for (int i = 0; i < _player.controllers.Joysticks.Count; i++)
			{
				ControllerMapSaveData[] mapSaveData = _player.controllers.maps.GetMapSaveData(ControllerType.Joystick, i, userAssignableMapsOnly: true);
				for (int j = 0; j < mapSaveData.Length; j++)
				{
					list.Add(mapSaveData[j].map.ToJsonString());
				}
			}
			mapsDataWrapper = default(MapsDataWrapper);
			mapsDataWrapper.MapSaveData = list;
			MapsDataWrapper mapsDataWrapper2 = mapsDataWrapper;
			_joystickMapsSaveDataVariable.Value = JsonUtility.ToJson(mapsDataWrapper2);
		}
		ControllerMapSaveData[] mapSaveData2 = _player.controllers.maps.GetMapSaveData(ControllerType.Keyboard, 0, userAssignableMapsOnly: true);
		for (int k = 0; k < mapSaveData2.Length; k++)
		{
			list2.Add(mapSaveData2[k].map.ToJsonString());
		}
		mapsDataWrapper = default(MapsDataWrapper);
		mapsDataWrapper.MapSaveData = list2;
		MapsDataWrapper mapsDataWrapper3 = mapsDataWrapper;
		_keyboardMapsSaveDataVariable.Value = JsonUtility.ToJson(mapsDataWrapper3);
		ControllerMapSaveData[] mapSaveData3 = _player.controllers.maps.GetMapSaveData(ControllerType.Mouse, 0, userAssignableMapsOnly: true);
		for (int l = 0; l < mapSaveData3.Length; l++)
		{
			list3.Add(mapSaveData3[l].map.ToJsonString());
		}
		mapsDataWrapper = default(MapsDataWrapper);
		mapsDataWrapper.MapSaveData = list3;
		MapsDataWrapper mapsDataWrapper4 = mapsDataWrapper;
		_mouseMapsSaveDataVariable.Value = JsonUtility.ToJson(mapsDataWrapper4);
	}

	public void SaveSettings()
	{
		RefreshMap();
		StorageDataManager.TheInstance.Save("settings", delegate
		{
			Debug.Log("Saved Settings.");
		}, null);
	}
}
