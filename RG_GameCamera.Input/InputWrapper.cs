using RG_GameCamera.Input.Mobile;
using UnityEngine;

namespace RG_GameCamera.Input;

public class InputWrapper
{
	public static bool Mobile;

	public static bool GetButton(string key)
	{
		if (Mobile)
		{
			return MobileControls.Instance.GetButton(key);
		}
		return UnityEngine.Input.GetButton(key);
	}

	public static float GetZoom(string key)
	{
		if (Mobile)
		{
			return MobileControls.Instance.GetZoom(key);
		}
		return 0f;
	}

	public static float GetAxis(string key)
	{
		if (Mobile)
		{
			return MobileControls.Instance.GetAxis(key);
		}
		return InputHandler.Instance.GetAxis(key);
	}

	public static bool GetButtonDown(string buttonName)
	{
		if (Mobile)
		{
			return MobileControls.Instance.GetButtonDown(buttonName);
		}
		return UnityEngine.Input.GetButtonDown(buttonName);
	}

	public static bool GetButtonUp(string buttonName)
	{
		if (Mobile)
		{
			return MobileControls.Instance.GetButtonUp(buttonName);
		}
		return UnityEngine.Input.GetButton(buttonName);
	}
}
