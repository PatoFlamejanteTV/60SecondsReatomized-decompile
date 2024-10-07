using UnityEngine;

namespace RG_GameCamera.Input.Mobile;

public abstract class BaseControl : MonoBehaviour
{
	public Vector2 Position;

	public Vector2 Size;

	public bool PreserveTextureRatio = true;

	public ControlSide Side;

	public int TouchIndex;

	public int TouchIndexAux;

	public string InputKey0;

	public string InputKey1;

	public bool HideGUI;

	public int Priority = 1;

	protected TouchProcessor touchProcessor;

	public abstract ControlType Type { get; }

	public virtual void Init(TouchProcessor processor)
	{
		base.hideFlags = HideFlags.HideInInspector;
		touchProcessor = processor;
		TouchIndex = -1;
	}

	public abstract void GameUpdate();

	public abstract void Draw();

	public virtual Vector2 GetInputAxis()
	{
		return Vector2.zero;
	}

	public virtual bool AbortUpdateOtherControls()
	{
		return false;
	}

	protected virtual void DetectTouches()
	{
		int activeTouchCount = touchProcessor.GetActiveTouchCount();
		if (activeTouchCount == 0)
		{
			TouchIndex = -1;
		}
		else
		{
			if (TouchIndex != -1)
			{
				return;
			}
			for (int i = 0; i < activeTouchCount; i++)
			{
				SimTouch touch = touchProcessor.GetTouch(i);
				if (touch.Status != 0 && IsSide(touch.StartPosition) && TouchIndex == -1)
				{
					TouchIndex = i;
				}
			}
		}
	}

	protected bool IsSide(Vector2 pos)
	{
		if (Side == ControlSide.Arbitrary)
		{
			return true;
		}
		if (pos.x < (float)Screen.width * 0.5f)
		{
			return Side == ControlSide.Left;
		}
		return Side == ControlSide.Right;
	}
}
