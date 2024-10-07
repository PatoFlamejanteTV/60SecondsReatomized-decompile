using System;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Input;

[Serializable]
public class FPSInput : GameInput
{
	public bool AlwaysAim;

	public override InputPreset PresetType => InputPreset.FPS;

	public override void UpdateInput(Input[] inputs)
	{
		Vector2 vector = new Vector2(InputWrapper.GetAxis("Horizontal_R"), InputWrapper.GetAxis("Vertical_R"));
		SetInput(inputs, InputType.Rotate, vector);
		if (vector.sqrMagnitude < Mathf.Epsilon && CursorLocking.IsLocked)
		{
			SetInput(inputs, InputType.Rotate, new Vector2(InputWrapper.GetAxis("Mouse X"), InputWrapper.GetAxis("Mouse Y")));
		}
		float axis = InputWrapper.GetAxis("Horizontal");
		float axis2 = InputWrapper.GetAxis("Vertical");
		Vector2 sample = new Vector2(axis, axis2);
		padFilter.AddSample(sample);
		SetInput(inputs, InputType.Move, padFilter.GetValue());
		float axis3 = InputWrapper.GetAxis("Aim");
		float axis4 = InputWrapper.GetAxis("Fire");
		bool button = InputWrapper.GetButton("Aim");
		bool button2 = InputWrapper.GetButton("Fire");
		SetInput(inputs, InputType.Aim, AlwaysAim || axis3 > 0.5f || button);
		SetInput(inputs, InputType.Fire, axis4 > 0.5f || button2);
		SetInput(inputs, InputType.Crouch, UnityEngine.Input.GetKey(KeyCode.C) || InputWrapper.GetButton("Crouch"));
		SetInput(inputs, InputType.Walk, InputWrapper.GetButton("Walk"));
		SetInput(inputs, InputType.Jump, InputWrapper.GetButton("Jump"));
		SetInput(inputs, InputType.Sprint, InputWrapper.GetButton("Sprint"));
	}
}
