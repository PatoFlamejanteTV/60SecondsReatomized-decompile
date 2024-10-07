using UnityEngine;

namespace RG_GameCamera.Input.Mobile;

public class Stick : BaseControl
{
	public float CircleSize = 160f;

	public float HitSize = 32f;

	public Texture2D MoveControlsCircle;

	public Texture2D MoveControlsHit;

	private Rect rect;

	private bool pressed;

	private Vector2 input;

	public override ControlType Type => ControlType.Stick;

	public override void GameUpdate()
	{
		DetectTouches();
		input = Vector2.zero;
		if (TouchIndex == -1)
		{
			return;
		}
		SimTouch touch = touchProcessor.GetTouch(TouchIndex);
		if (touch.Status != 0)
		{
			Vector2 vector = touch.Position - touch.StartPosition;
			float magnitude = vector.magnitude;
			if (magnitude > Mathf.Epsilon)
			{
				float num = CircleSize / 2f - HitSize / 2f;
				float num2 = magnitude / num;
				Vector2 vector2 = vector * num2;
				vector2.x = Mathf.Clamp(vector2.x, 0f - num, num);
				vector2.y = Mathf.Clamp(vector2.y, 0f - num, num);
				input = vector2 / num;
			}
		}
		else
		{
			TouchIndex = -1;
		}
	}

	public override Vector2 GetInputAxis()
	{
		return input;
	}

	public override void Draw()
	{
		if (!HideGUI && TouchIndex != -1)
		{
			SimTouch touch = touchProcessor.GetTouch(TouchIndex);
			float num = (0f - CircleSize) * 0.5f;
			if ((bool)MoveControlsCircle)
			{
				GUI.DrawTexture(new Rect(num + touch.StartPosition.x, num + ((float)Screen.height - touch.StartPosition.y), CircleSize, CircleSize), MoveControlsCircle, ScaleMode.StretchToFill);
			}
			if ((bool)MoveControlsHit)
			{
				num = (0f - HitSize) * 0.5f;
				GUI.DrawTexture(new Rect(num + touch.Position.x, num + ((float)Screen.height - touch.Position.y), HitSize, HitSize), MoveControlsHit, ScaleMode.StretchToFill);
			}
		}
	}
}
