using System;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Input.Mobile;

[ExecuteInEditMode]
public class MobileControls : MonoBehaviour
{
	private static MobileControls instance;

	public int LeftPanelIndex;

	public int RightPanelIndex;

	private TouchProcessor touchProcessor;

	public static MobileControls Instance
	{
		get
		{
			if (!instance)
			{
				CameraInstance.CreateInstance<MobileControls>("MobileControls");
			}
			return instance;
		}
	}

	private void Awake()
	{
		Init();
	}

	private void Init()
	{
		if (!(instance == null))
		{
			return;
		}
		instance = this;
		touchProcessor = new TouchProcessor(2);
		BaseControl[] controls = GetControls();
		if (controls != null)
		{
			BaseControl[] array = controls;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Init(touchProcessor);
			}
		}
	}

	public BaseControl[] GetControls()
	{
		BaseControl[] components = base.gameObject.GetComponents<BaseControl>();
		Array.Sort(components, (BaseControl a, BaseControl b) => b.Priority.CompareTo(a.Priority));
		return components;
	}

	public Button CreateButton(string btnName)
	{
		Button button = base.gameObject.AddComponent<Button>();
		button.Init(touchProcessor);
		button.InputKey0 = btnName;
		return button;
	}

	public Zoom CreateZoom(string btnName)
	{
		Zoom zoom = base.gameObject.AddComponent<Zoom>();
		zoom.Init(touchProcessor);
		zoom.InputKey0 = btnName;
		return zoom;
	}

	public void DuplicateButtonValues(Button target, Button source)
	{
		target.Position = source.Position;
		target.Size = source.Size;
		target.PreserveTextureRatio = source.PreserveTextureRatio;
		target.Toggle = source.Toggle;
		target.TextureDefault = source.TextureDefault;
		target.TexturePressed = source.TexturePressed;
	}

	public void DuplicateZoomValues(Zoom target, Zoom source)
	{
		target.Position = source.Position;
		target.Size = source.Size;
	}

	public void RemoveControl(BaseControl button)
	{
		RG_GameCamera.Utils.Debug.Destroy(button, allowDestroyingAssets: true);
	}

	private BaseControl DeserializeMasterControl(ControlType type)
	{
		BaseControl baseControl = null;
		switch (type)
		{
		case ControlType.CameraPanel:
			baseControl = base.gameObject.AddComponent<CameraPanel>();
			break;
		case ControlType.Stick:
			baseControl = base.gameObject.AddComponent<Stick>();
			break;
		}
		if (baseControl != null)
		{
			baseControl.Init(touchProcessor);
		}
		return baseControl;
	}

	public BaseControl CreateMasterControl(string axis0, string axis1, ControlType type, ControlSide side)
	{
		RemoveMasterControl(side);
		BaseControl baseControl = null;
		switch (type)
		{
		case ControlType.CameraPanel:
			baseControl = base.gameObject.AddComponent<CameraPanel>();
			break;
		case ControlType.Stick:
			baseControl = base.gameObject.AddComponent<Stick>();
			break;
		}
		if (baseControl != null)
		{
			baseControl.Init(touchProcessor);
			baseControl.Side = side;
			baseControl.InputKey0 = axis0;
			baseControl.InputKey1 = axis1;
		}
		return baseControl;
	}

	public void RemoveMasterControl(ControlSide side)
	{
		BaseControl[] controls = GetControls();
		if (controls == null)
		{
			return;
		}
		BaseControl[] array = controls;
		foreach (BaseControl baseControl in array)
		{
			if (baseControl.Side == side)
			{
				RemoveControl(baseControl);
			}
		}
	}

	public bool GetButton(string key)
	{
		if (TryGetControl(key, out var ctrl))
		{
			if (ctrl.Type == ControlType.Button)
			{
				return ((Button)ctrl).IsPressed();
			}
			return false;
		}
		return false;
	}

	public float GetZoom(string key)
	{
		if (TryGetControl(key, out var ctrl) && ctrl.Type == ControlType.Zoom)
		{
			return ((Zoom)ctrl).ZoomDelta;
		}
		return 0f;
	}

	public float GetAxis(string key)
	{
		if (TryGetControl(key, out var ctrl) && (ctrl.Type == ControlType.Stick || ctrl.Type == ControlType.CameraPanel))
		{
			Vector2 inputAxis = ctrl.GetInputAxis();
			if (key == ctrl.InputKey0)
			{
				return inputAxis.x;
			}
			if (key == ctrl.InputKey1)
			{
				return inputAxis.y;
			}
			return 0f;
		}
		return 0f;
	}

	public bool GetButtonDown(string buttonName)
	{
		if (TryGetControl(buttonName, out var ctrl))
		{
			if (ctrl.Type == ControlType.Button)
			{
				return ((Button)ctrl).State == Button.ButtonState.Begin;
			}
			return false;
		}
		return false;
	}

	public bool GetButtonUp(string buttonName)
	{
		if (TryGetControl(buttonName, out var ctrl))
		{
			if (ctrl.Type == ControlType.Button)
			{
				return ((Button)ctrl).State == Button.ButtonState.End;
			}
			return false;
		}
		return false;
	}

	private bool TryGetControl(string key, out BaseControl ctrl)
	{
		BaseControl[] controls = GetControls();
		if (controls != null)
		{
			BaseControl[] array = controls;
			foreach (BaseControl baseControl in array)
			{
				if (baseControl.InputKey0 == key || baseControl.InputKey1 == key)
				{
					ctrl = baseControl;
					return true;
				}
			}
		}
		ctrl = null;
		return false;
	}

	private void Update()
	{
		Init();
		touchProcessor.ScanInput();
		BaseControl[] controls = GetControls();
		if (controls == null)
		{
			return;
		}
		BaseControl[] array = controls;
		foreach (BaseControl obj in array)
		{
			obj.GameUpdate();
			if (obj.AbortUpdateOtherControls())
			{
				break;
			}
		}
	}

	private void OnGUI()
	{
		if (Event.current.type != EventType.Repaint)
		{
			return;
		}
		BaseControl[] controls = GetControls();
		if (controls != null)
		{
			BaseControl[] array = controls;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Draw();
			}
		}
	}
}
