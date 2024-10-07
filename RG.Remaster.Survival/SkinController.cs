using System.Collections;
using System.Collections.Generic;
using Rewired;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace RG.Remaster.Survival;

public class SkinController : MonoBehaviour
{
	private const int RIGHT_MOUSE_BUTTON = 1;

	[SerializeField]
	private bool _allowSkinChange = true;

	[SerializeField]
	private bool _useRightClick = true;

	[SerializeField]
	private bool _useLeftClick;

	[SerializeField]
	private GlobalIntVariable _currentSkinIndex;

	[SerializeField]
	private List<Skin> _skins = new List<Skin>();

	[SerializeField]
	private SkinDataList _dataList;

	[FormerlySerializedAs("_soundSlot")]
	[SerializeField]
	private SoundSlot _successSoundSlot;

	[SerializeField]
	private SoundSlot _declineSoundSlot;

	private Player _player;

	private bool isMouseOver;

	public GlobalIntVariable CurrentSkinIndex => _currentSkinIndex;

	public SkinDataList DataList => _dataList;

	private void Awake()
	{
		_player = ReInput.players.GetPlayer(0);
		Skin[] componentsInChildren = GetComponentsInChildren<Skin>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			_skins.Add(componentsInChildren[i]);
		}
		SkinManager.Instance.RegisterSkinController(this);
	}

	private void Start()
	{
		RefreshSkins();
	}

	private void OnMouseUpAsButton()
	{
		if (_useLeftClick)
		{
			CheckClick(moveForward: true);
		}
	}

	private void Update()
	{
		if (isMouseOver)
		{
			if (_useRightClick && _player.GetButtonUp(33))
			{
				CheckClick(moveForward: true);
			}
			else if (_useRightClick && _player.GetButtonUp(37))
			{
				CheckClick(moveForward: false);
			}
		}
	}

	private void OnMouseEnter()
	{
		isMouseOver = true;
	}

	private void OnMouseExit()
	{
		isMouseOver = false;
	}

	private void CheckClick(bool moveForward)
	{
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			ChangeSkin(moveForward);
		}
	}

	private void ChangeSkin(bool moveForward)
	{
		if (_allowSkinChange && _skins.Count > 0 && _dataList != null && _dataList.IsValid())
		{
			if (moveForward)
			{
				UpdateSkinIndexToNext();
			}
			else
			{
				UpdateSkinIndexToPrevious();
			}
			RefreshSkins();
		}
		else
		{
			PlayDeclineSound();
		}
	}

	private void RefreshSkins()
	{
		for (int i = 0; i < _skins.Count; i++)
		{
			if (_skins[i].Id.Equals(_dataList.SkinData[_currentSkinIndex.Value].SkinId))
			{
				_skins[i].gameObject.SetActive(value: true);
			}
			else
			{
				_skins[i].gameObject.SetActive(value: false);
			}
		}
	}

	public void AllowSkinChange()
	{
		_allowSkinChange = true;
	}

	public void ForceSkinUse(SkinId forcedSkinId)
	{
		_allowSkinChange = false;
		for (int i = 0; i < _dataList.SkinData.Count; i++)
		{
			if (_dataList.SkinData[i].SkinId.Equals(forcedSkinId))
			{
				_currentSkinIndex.Value = i;
			}
		}
		RefreshSkins();
	}

	public IEnumerator PlaySound(string eventName)
	{
		AudioManager.PlaySoundAndReturnInstance(eventName);
		yield return null;
	}

	private void PlaySuccessSound()
	{
		if (_successSoundSlot != null && !string.IsNullOrEmpty(_successSoundSlot.SoundEventName))
		{
			Singleton<GameManager>.Instance.PlaySoundInvoke(PlaySound(_successSoundSlot.SoundEventName));
		}
	}

	private void PlayDeclineSound()
	{
		if (_declineSoundSlot != null && !string.IsNullOrEmpty(_declineSoundSlot.SoundEventName))
		{
			Singleton<GameManager>.Instance.PlaySoundInvoke(PlaySound(_declineSoundSlot.SoundEventName));
		}
	}

	private void UpdateSkinIndexToNext()
	{
		int num = 0;
		for (int i = _currentSkinIndex.Value + 1; i < _dataList.SkinData.Count; i++)
		{
			if (!_dataList.SkinData[i].IsUnlockedVariable.Value)
			{
				continue;
			}
			if (_dataList.SkinData[i].AdditionalRequirements != null && _dataList.SkinData[i].AdditionalRequirements.Length != 0)
			{
				bool flag = true;
				for (int j = 0; j < _dataList.SkinData[i].AdditionalRequirements.Length; j++)
				{
					if (!_dataList.SkinData[i].AdditionalRequirements[j].Value)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					num = i;
					break;
				}
				continue;
			}
			num = i;
			break;
		}
		if (_currentSkinIndex.Value != num)
		{
			PlaySuccessSound();
		}
		else
		{
			PlayDeclineSound();
		}
		_currentSkinIndex.Value = num;
	}

	private void UpdateSkinIndexToPrevious()
	{
		int num = 0;
		for (int num2 = ((_currentSkinIndex.Value == 0) ? (_dataList.SkinData.Count - 1) : (_currentSkinIndex.Value - 1)); num2 > 0; num2--)
		{
			if (_dataList.SkinData[num2].IsUnlockedVariable.Value)
			{
				if (_dataList.SkinData[num2].AdditionalRequirements == null || _dataList.SkinData[num2].AdditionalRequirements.Length == 0)
				{
					num = num2;
					break;
				}
				bool flag = true;
				for (int i = 0; i < _dataList.SkinData[num2].AdditionalRequirements.Length; i++)
				{
					if (!_dataList.SkinData[num2].AdditionalRequirements[i].Value)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					num = num2;
					break;
				}
			}
		}
		if (_currentSkinIndex.Value != num)
		{
			PlaySuccessSound();
		}
		else
		{
			PlayDeclineSound();
		}
		_currentSkinIndex.Value = num;
	}
}
