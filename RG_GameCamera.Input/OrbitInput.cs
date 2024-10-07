using System;
using UnityEngine;

namespace RG_GameCamera.Input;

[Serializable]
public class OrbitInput : GameInput
{
	public override InputPreset PresetType => InputPreset.Orbit;

	public override void UpdateInput(Input[] inputs)
	{
		mouseFilter.AddSample(UnityEngine.Input.mousePosition);
		Vector2 vector = (InputManager.Instance.FilterInput ? mouseFilter.GetValue() : new Vector2(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y));
		if (InputWrapper.GetButton("Pan"))
		{
			SetInput(inputs, InputType.Pan, vector);
		}
		float axis = InputWrapper.GetAxis("Mouse ScrollWheel");
		if (Mathf.Abs(axis) > Mathf.Epsilon)
		{
			SetInput(inputs, InputType.Zoom, axis);
		}
		if (InputManager.Instance.MobileInput)
		{
			float zoom = InputWrapper.GetZoom("Zoom");
			if (Mathf.Abs(zoom) > Mathf.Epsilon)
			{
				SetInput(inputs, InputType.Zoom, zoom);
			}
		}
		Vector2 vector2 = new Vector2(InputWrapper.GetAxis("Horizontal_R"), InputWrapper.GetAxis("Vertical_R"));
		if (vector2.sqrMagnitude > Mathf.Epsilon)
		{
			SetInput(inputs, InputType.Rotate, new Vector2(vector2.x, vector2.y));
		}
		if (UnityEngine.Input.GetMouseButton(1))
		{
			SetInput(inputs, InputType.Rotate, new Vector2(InputWrapper.GetAxis("Mouse X"), InputWrapper.GetAxis("Mouse Y")));
		}
		SetInput(inputs, InputType.Reset, UnityEngine.Input.GetKey(KeyCode.R));
		doubleClickTimeout += Time.deltaTime;
		if (UnityEngine.Input.GetMouseButtonDown(2))
		{
			if (doubleClickTimeout < InputManager.DoubleClickTimeout)
			{
				SetInput(inputs, InputType.Reset, true);
			}
			doubleClickTimeout = 0f;
		}
		float axis2 = InputWrapper.GetAxis("Horizontal");
		float axis3 = InputWrapper.GetAxis("Vertical");
		Vector2 sample = new Vector2(axis2, axis3);
		padFilter.AddSample(sample);
		SetInput(inputs, InputType.Move, padFilter.GetValue());
	}
}
