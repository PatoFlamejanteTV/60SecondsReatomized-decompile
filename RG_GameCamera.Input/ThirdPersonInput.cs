using System;
using RG.SecondsRemaster;
using UnityEngine;

namespace RG_GameCamera.Input;

[Serializable]
public class ThirdPersonInput : GameInput
{
	[SerializeField]
	private bool _freeRotation = true;

	[SerializeField]
	private bool _gamepad;

	[SerializeField]
	private float _gamepadInputMultiplier = 1f;

	[SerializeField]
	private float _mouseInputMultiplier = 1f;

	[SerializeField]
	private float _keyboardInputMultiplier = 1f;

	[SerializeField]
	private float _globalInputMultiplier = 1f;

	private bool _paused;

	public bool Paused
	{
		get
		{
			return _paused;
		}
		set
		{
			_paused = value;
		}
	}

	public float GlobalInputMultiplier
	{
		get
		{
			return _globalInputMultiplier;
		}
		set
		{
			_globalInputMultiplier = value;
		}
	}

	public float GamepadInputMultiplier
	{
		get
		{
			return _gamepadInputMultiplier;
		}
		set
		{
			_gamepadInputMultiplier = value;
		}
	}

	public float MouseInputMultiplier
	{
		get
		{
			return _mouseInputMultiplier;
		}
		set
		{
			_mouseInputMultiplier = value;
		}
	}

	public float KeyboardInputMultiplier
	{
		get
		{
			return _keyboardInputMultiplier;
		}
		set
		{
			_keyboardInputMultiplier = value;
		}
	}

	public bool Gamepad
	{
		get
		{
			return _gamepad;
		}
		set
		{
			_gamepad = value;
		}
	}

	public bool FreeRotation
	{
		get
		{
			return _freeRotation;
		}
		set
		{
			_freeRotation = value;
		}
	}

	public override InputPreset PresetType => InputPreset.ThirdPerson;

	public override void UpdateInput(Input[] inputs)
	{
		if (_paused)
		{
			return;
		}
		if (_freeRotation)
		{
			SetInput(inputs, InputType.Rotate, new Vector2(UnityEngine.Input.GetAxis("Mouse X") * _mouseInputMultiplier * _globalInputMultiplier, 0f));
			if (Settings.Data.ControlMode.ScavengeControl != EPlayerInput.MOUSE_ONLY)
			{
				float axis = InputWrapper.GetAxis("Vertical");
				Vector2 sample = new Vector2(0f, axis);
				padFilter.AddSample(sample);
				SetInput(inputs, InputType.Move, padFilter.GetValue());
			}
		}
		else if (_gamepad)
		{
			SetInput(inputs, InputType.Rotate, new Vector2(InputHandler.Instance.GetAxis("Rotate") * _gamepadInputMultiplier * _globalInputMultiplier, 0f));
			SetInput(inputs, InputType.Move, new Vector2(0f, UnityEngine.Input.GetAxis("Vertical")));
		}
		else
		{
			InputWrapper.GetAxis("Rotate");
			Vector2 vector = new Vector2(InputWrapper.GetAxis("Rotate") * _keyboardInputMultiplier * _globalInputMultiplier * 2f, 0f);
			SetInput(inputs, InputType.Rotate, vector);
		}
	}
}
