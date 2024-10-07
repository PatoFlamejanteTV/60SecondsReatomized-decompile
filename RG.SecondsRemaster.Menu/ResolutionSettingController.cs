using System.Collections.Generic;
using RG.Parsecs.EventEditor;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class ResolutionSettingController : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _valueField;

	[SerializeField]
	private GlobalIntVariable _widthVariable;

	[SerializeField]
	private GlobalIntVariable _heightVariable;

	[SerializeField]
	private GlobalBoolVariable _isFullScreen;

	[SerializeField]
	private bool _applyInstantly = true;

	private List<Resolution> _resolutions;

	private int _currentIndex;

	private const int RESOLUTION_NOT_FOUND = -1;

	private void Awake()
	{
		_resolutions = new List<Resolution>(Screen.resolutions);
	}

	private void OnEnable()
	{
		_currentIndex = GetCurrentResolutionIndex(_widthVariable.Value, _heightVariable.Value);
		if (_currentIndex == -1)
		{
			_currentIndex = Screen.resolutions.Length - 1;
		}
		SetCurrentResolutionValues();
	}

	private int GetCurrentResolutionIndex(int width, int height)
	{
		Resolution[] resolutions = Screen.resolutions;
		int result = -1;
		for (int num = resolutions.Length - 1; num >= 0; num--)
		{
			if (resolutions[num].width == width && resolutions[num].height == height)
			{
				result = num;
				break;
			}
		}
		return result;
	}

	public void SetNext()
	{
		bool flag = false;
		if (_currentIndex + 1 < _resolutions.Count)
		{
			_currentIndex++;
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		Resolution resolution = _resolutions[_currentIndex];
		if (resolution.width == _widthVariable.Value && resolution.height == _heightVariable.Value)
		{
			SetNext();
			return;
		}
		SetCurrentResolutionValues();
		if (_applyInstantly)
		{
			Screen.SetResolution(resolution.width, resolution.height, _isFullScreen.Value);
		}
	}

	public void SetPrevious()
	{
		bool flag = false;
		if (_currentIndex - 1 >= 0)
		{
			_currentIndex--;
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		Resolution resolution = _resolutions[_currentIndex];
		if (resolution.width == _widthVariable.Value && resolution.height == _heightVariable.Value)
		{
			SetPrevious();
			return;
		}
		SetCurrentResolutionValues();
		if (_applyInstantly)
		{
			Screen.SetResolution(resolution.width, resolution.height, _isFullScreen.Value, resolution.refreshRate);
		}
	}

	private void SetCurrentResolutionValues()
	{
		_valueField.text = $"{_resolutions[_currentIndex].width} x {_resolutions[_currentIndex].height}";
		_widthVariable.Value = _resolutions[_currentIndex].width;
		_heightVariable.Value = _resolutions[_currentIndex].height;
	}
}
