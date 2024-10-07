using UnityEngine;

namespace RG_GameCamera.Input.Mobile;

public class TouchProcessor
{
	private readonly SimTouch[] touches;

	public TouchProcessor(int numberOfTouches)
	{
		touches = new SimTouch[numberOfTouches];
		for (int i = 0; i < touches.Length; i++)
		{
			touches[i] = new SimTouch(i, KeyCode.LeftAlt);
		}
	}

	public SimTouch[] GetTouches()
	{
		return touches;
	}

	public int GetTouchCount()
	{
		return touches.Length;
	}

	public int GetActiveTouchCount()
	{
		int num = 0;
		SimTouch[] array = touches;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Status != 0)
			{
				num++;
			}
		}
		return num;
	}

	public SimTouch GetTouch(int index)
	{
		return touches[index];
	}

	public void ScanInput()
	{
		for (int i = 0; i < touches.Length; i++)
		{
			touches[i].ScanInput();
		}
	}

	public void ShowDebug(bool status)
	{
	}
}
