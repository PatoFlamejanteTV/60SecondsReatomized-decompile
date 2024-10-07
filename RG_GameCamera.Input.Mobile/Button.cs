using UnityEngine;

namespace RG_GameCamera.Input.Mobile;

public class Button : BaseControl
{
	public enum ButtonState
	{
		Pressed,
		Begin,
		End,
		None
	}

	public bool Toggle;

	public bool HoldDrag;

	public bool InvalidateOnDrag;

	public float HoldTimeout = 0.3f;

	public Texture2D TextureDefault;

	public Texture2D TexturePressed;

	public ButtonState State;

	private Rect rect;

	private bool pressed;

	private Vector2 startTouch;

	public override ControlType Type => ControlType.Button;

	public override void Init(TouchProcessor processor)
	{
		base.Init(processor);
		rect = default(Rect);
		UpdateRect();
		State = ButtonState.None;
		Side = ControlSide.Arbitrary;
	}

	public bool ContainPoint(Vector2 point)
	{
		point.y = (float)Screen.height - point.y;
		return rect.Contains(point);
	}

	public void Press()
	{
		if (Toggle)
		{
			pressed = !pressed;
		}
		else
		{
			pressed = true;
		}
	}

	public bool IsPressed()
	{
		return pressed;
	}

	public void Reset()
	{
		pressed = false;
	}

	private void CheckForMove(Vector2 touch)
	{
		if (InvalidateOnDrag && (touch - startTouch).sqrMagnitude > 10f)
		{
			State = ButtonState.None;
			pressed = false;
		}
	}

	protected override void DetectTouches()
	{
		int activeTouchCount = touchProcessor.GetActiveTouchCount();
		bool flag = false;
		if (activeTouchCount > 0)
		{
			for (int i = 0; i < activeTouchCount; i++)
			{
				SimTouch touch = touchProcessor.GetTouch(i);
				if (ContainPoint(touch.StartPosition) && touch.Status == TouchStatus.Start)
				{
					Press();
					State = ButtonState.Begin;
					startTouch = touch.StartPosition;
					TouchIndex = i;
				}
				if (TouchIndex == i)
				{
					switch (touch.Status)
					{
					case TouchStatus.Stationary:
					case TouchStatus.Moving:
						State = ButtonState.Pressed;
						CheckForMove(touch.Position);
						break;
					case TouchStatus.End:
						State = ButtonState.End;
						CheckForMove(touch.Position);
						flag = true;
						break;
					case TouchStatus.Invalid:
						flag = true;
						break;
					}
				}
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			if (TouchIndex == -1)
			{
				State = ButtonState.None;
			}
			else if (!HoldDrag && IsHoldDrag())
			{
				State = ButtonState.None;
			}
			TouchIndex = -1;
			if (!Toggle)
			{
				Reset();
			}
		}
	}

	public override void GameUpdate()
	{
		DetectTouches();
	}

	public override void Draw()
	{
		UpdateRect();
		if (!HideGUI)
		{
			Texture2D texture2D = (pressed ? TexturePressed : TextureDefault);
			if ((bool)texture2D)
			{
				GUI.DrawTexture(rect, texture2D);
			}
		}
	}

	public void UpdateRect()
	{
		rect.x = Position.x * (float)Screen.width;
		rect.y = Position.y * (float)Screen.height;
		rect.width = Size.x * (float)Screen.width;
		rect.height = Size.y * (float)Screen.height;
	}

	private bool IsHoldDrag()
	{
		if (TouchIndex != -1)
		{
			return touchProcessor.GetTouch(TouchIndex).PressTime > HoldTimeout;
		}
		return false;
	}
}
