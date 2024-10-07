using System;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Input;

public class InputManager : MonoBehaviour
{
	public static float DoubleClickTimeout = 0.25f;

	public bool FilterInput = true;

	private static InputManager instance;

	public InputPreset InputPreset;

	public bool MobileInput;

	[HideInInspector]
	public bool IsValid;

	private Input[] inputs;

	private GameInput[] GameInputs;

	private GameInput currInput;

	public static InputManager Instance
	{
		get
		{
			if (!instance)
			{
				instance = CameraInstance.CreateInstance<InputManager>("CameraInput");
			}
			return instance;
		}
	}

	public Input GetInput(InputType type)
	{
		return inputs[(int)type];
	}

	public T GetInput<T>(InputType type, T defaultValue)
	{
		Input input = inputs[(int)type];
		if (input.Valid)
		{
			return (T)input.Value;
		}
		return defaultValue;
	}

	public void SetInputPreset(InputPreset preset)
	{
		if (preset == InputPreset.None)
		{
			currInput = null;
			InputPreset = InputPreset.None;
			return;
		}
		GameInput[] gameInputs = GameInputs;
		foreach (GameInput gameInput in gameInputs)
		{
			if (gameInput.PresetType == preset)
			{
				currInput = gameInput;
				InputPreset = preset;
				break;
			}
		}
	}

	private void Awake()
	{
		instance = this;
		inputs = new Input[14];
		int num = 0;
		InputType[] array = (InputType[])Enum.GetValues(typeof(InputType));
		foreach (InputType type in array)
		{
			inputs[num++] = new Input
			{
				Type = type,
				Valid = false,
				Value = null
			};
		}
		GameInputs = base.gameObject.GetComponents<GameInput>();
		SetInputPreset(InputPreset);
	}

	private void Start()
	{
	}

	public void GameUpdate()
	{
		InputWrapper.Mobile = MobileInput;
		IsValid = true;
		Input[] array = inputs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Valid = false;
		}
		if (currInput != null && currInput.PresetType != InputPreset)
		{
			SetInputPreset(InputPreset);
		}
		if (currInput != null)
		{
			currInput.UpdateInput(inputs);
		}
	}
}
