using System;
using System.Collections.Generic;
using RG.SecondsRemaster;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
	public enum EControl
	{
		NONE,
		GLOBAL_MENU,
		SCAVENGE_FORWARD,
		SCAVENGE_BACKWARD,
		SCAVENGE_ROTATE_LEFT,
		SCAVENGE_ROTATE_RIGHT,
		SCAVENGE_STRAFE_LEFT,
		SCAVENGE_STRAFE_RIGHT,
		SCAVENGE_INTERACTION,
		GLOBAL_ACTION1,
		GLOBAL_ACTION2,
		GLOBAL_ALTCHOICEX,
		GLOBAL_ALTCHOICEY,
		GLOBAL_CHOICE1,
		GLOBAL_CHOICE2,
		GLOBAL_CHOICE3,
		GLOBAL_CHOICE4,
		GLOBAL_NEXT,
		GLOBAL_PREV
	}

	private class ControlAxis
	{
		private string _nativeAxisName;

		private EControl _min;

		private EControl _max;

		private float _filter;

		private bool _minActive;

		private bool _maxActive;

		private float _minActivationTime;

		private float _maxActivationTime;

		private bool _nativeAxis;

		public bool MinActive => _minActive;

		public bool MaxActive => _maxActive;

		public float MinActivationTime => _minActivationTime;

		public float MaxActivationTime => _maxActivationTime;

		public EControl Min => _min;

		public EControl Max => _max;

		public bool NativeAxis => _nativeAxis;

		public ControlAxis(EControl min, EControl max, float filter, string nativeAxisName = null)
		{
			_min = min;
			_max = max;
			_filter = filter;
			_nativeAxisName = nativeAxisName;
			_nativeAxis = !string.IsNullOrEmpty(_nativeAxisName);
		}

		public float GetAxisRaw()
		{
			if (_nativeAxis)
			{
				return Input.GetAxisRaw(_nativeAxisName);
			}
			float num = 0f;
			if (_maxActive)
			{
				num += 1f;
			}
			if (_minActive)
			{
				num += -1f;
			}
			return num;
		}

		public float GetAxis()
		{
			if (_nativeAxis)
			{
				return Input.GetAxis(_nativeAxisName);
			}
			float num = 0f;
			if (_maxActive)
			{
				num += Mathf.Lerp(0f, 1f, (Time.time - _maxActivationTime) / _filter);
			}
			if (_minActive)
			{
				num += Mathf.Lerp(0f, -1f, (Time.time - _minActivationTime) / _filter);
			}
			return num;
		}

		public void Activate(bool max, bool activate)
		{
			if (max)
			{
				if (!_maxActive && activate)
				{
					_maxActivationTime = Time.time;
				}
				_maxActive = activate;
			}
			else
			{
				if (!_minActive && activate)
				{
					_minActivationTime = Time.time;
				}
				_minActive = activate;
			}
		}
	}

	public static InputHandler Instance;

	private const string JOY_BUTTON_PREFIX = "joystick {0} button {1}";

	public const int LEFT_MOUSE_BUTTON = 0;

	public const int RIGHT_MOUSE_BUTTON = 1;

	public const int MIDDLE_MOUSE_BUTTON = 2;

	private Dictionary<EControl, KeyCode> _keyboardSetup = new Dictionary<EControl, KeyCode>();

	private Dictionary<EControl, int> _mouseSetup = new Dictionary<EControl, int>();

	private Dictionary<EControl, string> _joySetup = new Dictionary<EControl, string>();

	private Dictionary<string, ControlAxis> _axisD = new Dictionary<string, ControlAxis>();

	private List<ControlAxis> _axisL = new List<ControlAxis>();

	private int _lastMouseButtonPressed = -1;

	public int LastMouseButtonPressed => _lastMouseButtonPressed;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			_lastMouseButtonPressed = 0;
		}
		if (Input.GetMouseButtonDown(1))
		{
			_lastMouseButtonPressed = 1;
		}
		UpdateAxis();
	}

	private void UpdateAxis()
	{
		for (int i = 0; i < _axisL.Count; i++)
		{
			if (!_axisL[i].NativeAxis)
			{
				_axisL[i].Activate(max: false, IsHeld(_axisL[i].Min));
				_axisL[i].Activate(max: true, IsHeld(_axisL[i].Max));
			}
		}
	}

	public void SetKeyboardControl(EControl e, KeyCode k)
	{
		if (_keyboardSetup.ContainsKey(e))
		{
			_keyboardSetup[e] = k;
		}
		else
		{
			_keyboardSetup.Add(e, k);
		}
	}

	public void Reload(Dictionary<string, string> controls, ControlMode mode)
	{
		_axisL.Clear();
		_axisD.Clear();
		_keyboardSetup.Clear();
		_mouseSetup.Clear();
		_joySetup.Clear();
		_lastMouseButtonPressed = -1;
		foreach (string key in controls.Keys)
		{
			string text = key.Substring(0, key.IndexOf("_"));
			if (!text.Contains(mode.Key))
			{
				continue;
			}
			int num = text.Length + 1;
			string text2 = key.Substring(num, key.Length - num);
			try
			{
				EControl eControl = (EControl)Enum.Parse(typeof(EControl), text2);
				if (eControl == EControl.NONE || _keyboardSetup.ContainsKey(eControl))
				{
					continue;
				}
				if (controls[key].Contains("MOUSE_"))
				{
					int num2 = ResolveMouseButton(controls[key]);
					if (num2 >= 0)
					{
						_mouseSetup.Add(eControl, num2);
					}
				}
				else if (controls[key].Contains("JOY_"))
				{
					int num3 = ResolveJoyButton(controls[key]);
					if (num3 >= 0)
					{
						_joySetup.Add(eControl, $"joystick {Settings.Data.ActiveGamepad} button {num3}");
					}
				}
				else if (controls[key].Contains("JoyAxis"))
				{
					_joySetup.Add(eControl, key);
					RegisterAxis(key, EControl.NONE, EControl.NONE, 1f, controls[key]);
				}
				else
				{
					KeyCode keyCode = ResolveKeyboardKey(controls[key]);
					if (keyCode != 0)
					{
						_keyboardSetup.Add(eControl, keyCode);
					}
				}
			}
			catch
			{
				Debug.LogError(text2);
			}
		}
		switch (mode.ScavengeControl)
		{
		case EPlayerInput.KEYBOARD:
			RegisterAxis("Vertical", EControl.SCAVENGE_BACKWARD, EControl.SCAVENGE_FORWARD);
			RegisterAxis("Rotate", EControl.SCAVENGE_ROTATE_LEFT, EControl.SCAVENGE_ROTATE_RIGHT, 0.35f);
			break;
		case EPlayerInput.KEYBOARD_MOUSE:
			RegisterAxis("Vertical", EControl.SCAVENGE_BACKWARD, EControl.SCAVENGE_FORWARD);
			RegisterAxis("Horizontal", EControl.SCAVENGE_STRAFE_LEFT, EControl.SCAVENGE_STRAFE_RIGHT);
			break;
		case EPlayerInput.GAMEPAD:
			RegisterAxis("Vertical", EControl.NONE, EControl.NONE, 1f, "JoyAxisY");
			RegisterAxis("Horizontal", EControl.NONE, EControl.NONE, 1f, "JoyAxisX");
			RegisterAxis("Rotate", EControl.NONE, EControl.NONE, 1f, "JoyAxis" + GetJoyAxis(4));
			break;
		}
	}

	private KeyCode ResolveKeyboardKey(string val)
	{
		return (KeyCode)Enum.Parse(typeof(KeyCode), val);
	}

	private int ResolveMouseButton(string val)
	{
		int result = -1;
		int.TryParse(val.Substring("MOUSE_".Length, val.Length - "MOUSE_".Length), out result);
		return result;
	}

	private int ResolveJoyButton(string val)
	{
		int result = -1;
		string text = "JOY_";
		if (val.Contains("JOY1_"))
		{
			text = val;
		}
		int.TryParse(val.Substring(text.Length, val.Length - text.Length), out result);
		return result;
	}

	public int GetControlNullable(EControl control)
	{
		if (_joySetup.ContainsKey(control))
		{
			int num = 0;
			try
			{
				num = (Input.GetKey(_joySetup[control]) ? 1 : 0);
			}
			catch
			{
			}
			if (num == 0)
			{
				num = GetAxisPolar(_joySetup[control]);
			}
			return num;
		}
		if (_keyboardSetup.ContainsKey(control))
		{
			if (!Input.GetKey(_keyboardSetup[control]))
			{
				return 0;
			}
			return 1;
		}
		if (_mouseSetup.ContainsKey(control))
		{
			if (!Input.GetMouseButton(_mouseSetup[control]))
			{
				return 0;
			}
			return 1;
		}
		return 0;
	}

	public bool GetControl(EControl control)
	{
		if (_joySetup.ContainsKey(control))
		{
			return Input.GetKey(_joySetup[control]);
		}
		if (_keyboardSetup.ContainsKey(control))
		{
			return Input.GetKey(_keyboardSetup[control]);
		}
		if (_mouseSetup.ContainsKey(control))
		{
			return Input.GetMouseButton(_mouseSetup[control]);
		}
		return false;
	}

	public bool GetControlDownUp(EControl control, bool keyboardTestDown = true, bool mouseTestDown = true, bool gamepadTestDown = true)
	{
		if (_joySetup.ContainsKey(control))
		{
			if (gamepadTestDown)
			{
				return Input.GetKeyDown(_joySetup[control]);
			}
			return Input.GetKeyUp(_joySetup[control]);
		}
		if (_keyboardSetup.ContainsKey(control))
		{
			if (keyboardTestDown)
			{
				return Input.GetKeyDown(_keyboardSetup[control]);
			}
			return Input.GetKeyUp(_keyboardSetup[control]);
		}
		if (_mouseSetup.ContainsKey(control))
		{
			if (mouseTestDown)
			{
				return Input.GetMouseButtonDown(_mouseSetup[control]);
			}
			return Input.GetMouseButtonUp(_mouseSetup[control]);
		}
		return false;
	}

	public bool IsDown(EControl control)
	{
		return Input.GetKeyDown(_keyboardSetup[control]);
	}

	public bool IsUp(EControl control)
	{
		return Input.GetKeyUp(_keyboardSetup[control]);
	}

	public bool IsHeld(EControl control)
	{
		return Input.GetKey(_keyboardSetup[control]);
	}

	public void RegisterAxis(string name, EControl min, EControl max, float filter = 1f, string nativeAxisName = null)
	{
		ControlAxis controlAxis = new ControlAxis(min, max, filter, nativeAxisName);
		_axisL.Add(controlAxis);
		_axisD.Add(name, controlAxis);
	}

	public float GetAxis(string key)
	{
		return _axisD[key].GetAxis();
	}

	public bool TestAxisPositive(string key, bool positive)
	{
		float axis = GetAxis(key);
		if (positive)
		{
			return axis > 0f;
		}
		return axis < 0f;
	}

	public int GetAxisPolar(string key)
	{
		float axis = GetAxis(key);
		if (axis > 0f)
		{
			return 1;
		}
		if (axis < 0f)
		{
			return -1;
		}
		return 0;
	}

	public float GetAxisRaw(string key)
	{
		return _axisD[key].GetAxisRaw();
	}

	public int GetRawAxisPolar(string key)
	{
		float axisRaw = GetAxisRaw(key);
		if (axisRaw > 0f)
		{
			return 1;
		}
		if (axisRaw < 0f)
		{
			return -1;
		}
		return 0;
	}

	public bool WasLastMouseButtonPressed(int button)
	{
		return _lastMouseButtonPressed == button;
	}

	public static int GetJoyAxis(int code)
	{
		return code;
	}

	public static int GetJoyButtonCode(int code)
	{
		return code;
	}
}
