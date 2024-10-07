using System;
using UnityEngine;

namespace RG_GameCamera.Utils;

internal static class GUIUtils
{
	private static float labelMaxWidth = 130f;

	public static bool SliderEdit(string label, float min, float max, ref float value)
	{
		GUILayout.BeginHorizontal();
		float num = value;
		GUILayout.Label(label, GUILayout.Width(labelMaxWidth));
		value = GUILayout.HorizontalSlider(value, min, max);
		GUILayout.EndHorizontal();
		value = Mathf.Clamp(value, min, max);
		return num != value;
	}

	public static bool SliderEdit(string label, int min, int max, ref int value)
	{
		GUILayout.BeginHorizontal();
		int num = value;
		GUILayout.Label(label);
		value = (int)GUILayout.HorizontalSlider(value, min, max);
		GUILayout.EndHorizontal();
		value = Mathf.Clamp(value, min, max);
		return num != value;
	}

	public static bool Toggle(string label, ref bool value)
	{
		bool flag = value;
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, GUILayout.Width(labelMaxWidth));
		value = GUILayout.Toggle(value, string.Empty);
		GUILayout.EndHorizontal();
		return flag != value;
	}

	public static bool Selection(string label, string[] labels, ref int index)
	{
		return false;
	}

	public static Enum EnumSelection(string label, Enum selected, ref bool changed)
	{
		return null;
	}

	public static bool String(string label, ref string input)
	{
		string text = input;
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, GUILayout.Width(labelMaxWidth));
		input = GUILayout.TextField(input);
		GUILayout.EndHorizontal();
		return input != text;
	}

	public static bool Vector2(string label, ref Vector2 input)
	{
		Vector2 vector = input;
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, GUILayout.Width(labelMaxWidth));
		string value = GUILayout.TextField(input.x.ToString());
		string value2 = GUILayout.TextField(input.y.ToString());
		try
		{
			input.x = Convert.ToSingle(value);
			input.y = Convert.ToSingle(value2);
		}
		catch
		{
		}
		GUILayout.EndHorizontal();
		return input != vector;
	}

	public static bool Vector3(string label, ref Vector3 input)
	{
		Vector3 vector = input;
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, GUILayout.Width(labelMaxWidth));
		string value = GUILayout.TextField(input.x.ToString());
		string value2 = GUILayout.TextField(input.y.ToString());
		string value3 = GUILayout.TextField(input.z.ToString());
		try
		{
			input.x = Convert.ToSingle(value);
			input.y = Convert.ToSingle(value2);
			input.z = Convert.ToSingle(value3);
		}
		catch
		{
		}
		GUILayout.EndHorizontal();
		return input != vector;
	}

	public static void Separator(string label, float height)
	{
		GUILayout.Box(label, GUILayout.ExpandWidth(expand: true), GUILayout.Height(height));
	}
}
