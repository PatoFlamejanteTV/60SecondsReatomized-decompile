using UnityEngine;

namespace RG_GameCamera.Input.Mobile;

public class Zoom : BaseControl
{
	public float ZoomDelta;

	public float Sensitivity = 1f;

	public bool ReverseZoom;

	private Rect rect;

	private bool zooming;

	private float lastDistance;

	public override ControlType Type => ControlType.Zoom;

	public override void Init(TouchProcessor processor)
	{
		base.Init(processor);
		rect = default(Rect);
		UpdateRect();
		ZoomDelta = 0f;
		Side = ControlSide.Arbitrary;
		Priority = 2;
	}

	public bool ContainPoint(Vector2 point)
	{
		point.y = (float)Screen.height - point.y;
		return rect.Contains(point);
	}

	public bool IsZooming()
	{
		return zooming;
	}

	public override bool AbortUpdateOtherControls()
	{
		return zooming;
	}

	protected override void DetectTouches()
	{
		int activeTouchCount = touchProcessor.GetActiveTouchCount();
		bool flag = false;
		if (activeTouchCount > 1)
		{
			if (!zooming)
			{
				for (int i = 0; i < activeTouchCount; i++)
				{
					SimTouch touch = touchProcessor.GetTouch(i);
					if (ContainPoint(touch.StartPosition) && touch.Status != 0)
					{
						if (TouchIndex == -1)
						{
							TouchIndex = i;
						}
						else if (TouchIndexAux == -1)
						{
							TouchIndexAux = i;
						}
					}
				}
				zooming = TouchIndex != -1 && TouchIndexAux != -1;
			}
			else
			{
				SimTouch touch2 = touchProcessor.GetTouch(TouchIndex);
				SimTouch touch3 = touchProcessor.GetTouch(TouchIndexAux);
				if (touch2.Status != 0 && touch3.Status != 0)
				{
					float magnitude = (touch2.Position - touch3.Position).magnitude;
					if (lastDistance > 0f)
					{
						ZoomDelta = (lastDistance - magnitude) * 0.01f * Sensitivity;
						if (ReverseZoom)
						{
							ZoomDelta = 0f - ZoomDelta;
						}
					}
					else
					{
						ZoomDelta = 0f;
					}
					lastDistance = magnitude;
				}
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			lastDistance = 0f;
			zooming = false;
			TouchIndex = -1;
			TouchIndexAux = -1;
			ZoomDelta = 0f;
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
			GUI.Box(rect, "Zoom area");
		}
	}

	public void UpdateRect()
	{
		rect.x = Position.x * (float)Screen.width;
		rect.y = Position.y * (float)Screen.height;
		rect.width = Size.x * (float)Screen.width;
		rect.height = Size.y * (float)Screen.height;
	}
}
