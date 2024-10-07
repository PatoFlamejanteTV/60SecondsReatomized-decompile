using RG.Parsecs.EventEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Menu;

public class RotationSensitivitySettingController : MonoBehaviour
{
	[SerializeField]
	private Slider _slider;

	[SerializeField]
	private GlobalFloatVariable _rotationSensitivityMouse;

	[SerializeField]
	private GlobalFloatVariable _rotationSensitivityGamepad;

	[SerializeField]
	private GlobalFloatVariable _rotationSensitivityKeyboard;

	[SerializeField]
	private GlobalIntVariable _controlModeVariable;

	[SerializeField]
	private bool _controllerForSpecificControlMode;

	[SerializeField]
	private EPlayerInput _specificControlMode;

	[SerializeField]
	private GlobalFloatVariable _gamepadSensitivityMinValue;

	[SerializeField]
	private GlobalFloatVariable _gamepadSensitivityMaxValue;

	[SerializeField]
	private GlobalFloatVariable _mouseKeyboardSensitivityMinValue;

	[SerializeField]
	private GlobalFloatVariable _mouseKeyboardSensitivityMaxValue;

	[SerializeField]
	private GlobalFloatVariable _keyboardSensitivityMinValue;

	[SerializeField]
	private GlobalFloatVariable _keyboardSensitivityMaxValue;

	private EPlayerInput _controlMode;

	private bool _blockChangingSensitivity;

	private void OnEnable()
	{
		if (_controllerForSpecificControlMode)
		{
			_controlMode = _specificControlMode;
		}
		else
		{
			_controlMode = (EPlayerInput)_controlModeVariable.Value;
		}
		SetupSlider();
	}

	private void SetupSlider()
	{
		_blockChangingSensitivity = true;
		SetSliderRange();
		_blockChangingSensitivity = false;
		switch (_controlMode)
		{
		case EPlayerInput.MOUSE_ONLY:
			if (_rotationSensitivityMouse.Value >= _slider.minValue)
			{
				_slider.value = _rotationSensitivityMouse.Value;
			}
			break;
		case EPlayerInput.KEYBOARD:
			if (_rotationSensitivityKeyboard.Value >= _slider.minValue)
			{
				_slider.value = _rotationSensitivityKeyboard.Value;
			}
			break;
		case EPlayerInput.GAMEPAD:
			if (_rotationSensitivityGamepad.Value >= _slider.minValue)
			{
				_slider.value = _rotationSensitivityGamepad.Value;
			}
			break;
		case EPlayerInput.KEYBOARD_MOUSE:
			if (_rotationSensitivityMouse.Value >= _slider.minValue)
			{
				_slider.value = _rotationSensitivityMouse.Value;
			}
			break;
		case EPlayerInput.TOUCH_ANALOGUE:
		case EPlayerInput.TOUCH_DIGITAL:
			break;
		}
	}

	private void Update()
	{
		if (!_controllerForSpecificControlMode && _controlModeVariable.Value != (int)_controlMode)
		{
			_controlMode = (EPlayerInput)_controlModeVariable.Value;
			SetupSlider();
		}
	}

	public void ChangeSensitivity(float value)
	{
		if (!_blockChangingSensitivity)
		{
			switch (_controlMode)
			{
			case EPlayerInput.MOUSE_ONLY:
				_rotationSensitivityMouse.Value = value;
				break;
			case EPlayerInput.KEYBOARD:
				_rotationSensitivityKeyboard.Value = value;
				break;
			case EPlayerInput.GAMEPAD:
				_rotationSensitivityGamepad.Value = value;
				break;
			case EPlayerInput.KEYBOARD_MOUSE:
				_rotationSensitivityMouse.Value = value;
				break;
			case EPlayerInput.TOUCH_ANALOGUE:
			case EPlayerInput.TOUCH_DIGITAL:
				break;
			}
		}
	}

	private void SetSliderRange()
	{
		switch (_controlMode)
		{
		case EPlayerInput.KEYBOARD_MOUSE:
		case EPlayerInput.MOUSE_ONLY:
			_slider.minValue = _mouseKeyboardSensitivityMinValue.Value;
			_slider.maxValue = _mouseKeyboardSensitivityMaxValue.Value;
			break;
		case EPlayerInput.KEYBOARD:
			_slider.minValue = _keyboardSensitivityMinValue.Value;
			_slider.maxValue = _keyboardSensitivityMaxValue.Value;
			break;
		case EPlayerInput.GAMEPAD:
			_slider.minValue = _gamepadSensitivityMinValue.Value;
			_slider.maxValue = _gamepadSensitivityMaxValue.Value;
			break;
		case EPlayerInput.TOUCH_ANALOGUE:
		case EPlayerInput.TOUCH_DIGITAL:
			break;
		}
	}
}
