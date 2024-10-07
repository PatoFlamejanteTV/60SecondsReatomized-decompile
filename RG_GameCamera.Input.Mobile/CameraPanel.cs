using UnityEngine;

namespace RG_GameCamera.Input.Mobile;

public class CameraPanel : BaseControl
{
	public Vector2 Sensitivity = new Vector2(0.5f, 0.5f);

	private Rect rect;

	private InputFilter cameraFilter;

	private Vector2 input;

	public override ControlType Type => ControlType.CameraPanel;

	public override void Init(TouchProcessor processor)
	{
		base.Init(processor);
		cameraFilter = new InputFilter(10, 0.5f);
		rect = default(Rect);
		UpdateRect();
	}

	public override void GameUpdate()
	{
		DetectTouches();
		input = Vector2.zero;
		if (TouchIndex != -1)
		{
			SimTouch touch = touchProcessor.GetTouch(TouchIndex);
			if (touch.Status != 0)
			{
				Vector2 deltaPosition = touch.DeltaPosition;
				deltaPosition.x *= Sensitivity.x;
				deltaPosition.y *= Sensitivity.y;
				cameraFilter.AddSample(deltaPosition);
				input = cameraFilter.GetValue();
			}
			else
			{
				TouchIndex = -1;
			}
		}
	}

	public override Vector2 GetInputAxis()
	{
		return input;
	}

	public void UpdateRect()
	{
		rect.x = Position.x * (float)Screen.width;
		rect.y = Position.y * (float)Screen.height;
		rect.width = Position.x * (float)Screen.width;
		rect.height = Position.y * (float)Screen.height;
	}

	public override void Draw()
	{
		_ = HideGUI;
	}
}
