using System;
using System.Collections.Generic;
using RG_GameCamera.ThirdParty;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Config;

public abstract class Config : MonoBehaviour
{
	public delegate void OnTransitMode(string newMode, float t);

	public delegate void OnTransitionStart(string oldMode, string newMode);

	public enum ConfigValue
	{
		Bool,
		Range,
		Vector3,
		Vector2,
		String,
		Selection
	}

	public interface Param
	{
		ConfigValue Type { get; }

		object[] Serialize();

		void Deserialize(object[] data);

		void Interpolate(Param p0, Param p1, float t);

		void Set(Param p);

		Param Clone();
	}

	public class RangeParam : Param
	{
		public float value;

		public float min;

		public float max;

		public ConfigValue Type => ConfigValue.Range;

		public object[] Serialize()
		{
			return new object[4]
			{
				ConfigValue.Range.ToString(),
				value,
				min,
				max
			};
		}

		public void Deserialize(object[] data)
		{
			value = Convert.ToSingle(data[1]);
			min = Convert.ToSingle(data[2]);
			max = Convert.ToSingle(data[3]);
		}

		public void Interpolate(Param p0, Param p1, float t)
		{
			RangeParam rangeParam = (RangeParam)p0;
			RangeParam rangeParam2 = (RangeParam)p1;
			value = Interpolation.LerpS2(rangeParam.value, rangeParam2.value, t);
			min = Interpolation.LerpS2(rangeParam.min, rangeParam2.min, t);
			max = Interpolation.LerpS2(rangeParam.max, rangeParam2.max, t);
		}

		public void Set(Param p)
		{
			RangeParam rangeParam = (RangeParam)p;
			value = rangeParam.value;
			min = rangeParam.min;
			max = rangeParam.max;
		}

		public Param Clone()
		{
			RangeParam rangeParam = new RangeParam();
			rangeParam.Set(this);
			return rangeParam;
		}
	}

	public struct Vector3Param : Param
	{
		public Vector3 value;

		public ConfigValue Type => ConfigValue.Vector3;

		public object[] Serialize()
		{
			return new object[4]
			{
				ConfigValue.Vector3.ToString(),
				value.x,
				value.y,
				value.z
			};
		}

		public void Deserialize(object[] data)
		{
			value.x = Convert.ToSingle(data[1]);
			value.y = Convert.ToSingle(data[2]);
			value.z = Convert.ToSingle(data[3]);
		}

		public void Interpolate(Param p0, Param p1, float t)
		{
			Vector3Param vector3Param = (Vector3Param)(object)p0;
			Vector3Param vector3Param2 = (Vector3Param)(object)p1;
			value = Interpolation.LerpS2(vector3Param.value, vector3Param2.value, t);
		}

		public void Set(Param p)
		{
			value = ((Vector3Param)(object)p).value;
		}

		public Param Clone()
		{
			Vector3Param vector3Param = default(Vector3Param);
			vector3Param.Set(this);
			return vector3Param;
		}
	}

	public struct Vector2Param : Param
	{
		public Vector2 value;

		public ConfigValue Type => ConfigValue.Vector2;

		public object[] Serialize()
		{
			return new object[3]
			{
				ConfigValue.Vector2.ToString(),
				value.x,
				value.y
			};
		}

		public void Deserialize(object[] data)
		{
			value.x = Convert.ToSingle(data[1]);
			value.y = Convert.ToSingle(data[2]);
		}

		public void Interpolate(Param p0, Param p1, float t)
		{
			Vector2Param vector2Param = (Vector2Param)(object)p0;
			Vector2Param vector2Param2 = (Vector2Param)(object)p1;
			value = Interpolation.LerpS2(vector2Param.value, vector2Param2.value, t);
		}

		public void Set(Param p)
		{
			value = ((Vector2Param)(object)p).value;
		}

		public Param Clone()
		{
			Vector2Param vector2Param = default(Vector2Param);
			vector2Param.Set(this);
			return vector2Param;
		}
	}

	public struct StringParam : Param
	{
		public string value;

		public ConfigValue Type => ConfigValue.String;

		public object[] Serialize()
		{
			return new object[2]
			{
				ConfigValue.String.ToString(),
				value
			};
		}

		public void Deserialize(object[] data)
		{
			value = Convert.ToString(data[1]);
		}

		public void Interpolate(Param p0, Param p1, float t)
		{
			value = ((StringParam)(object)p1).value;
		}

		public void Set(Param p)
		{
			value = ((StringParam)(object)p).value;
		}

		public Param Clone()
		{
			StringParam stringParam = default(StringParam);
			stringParam.Set(this);
			return stringParam;
		}
	}

	public struct BoolParam : Param
	{
		public bool value;

		public ConfigValue Type => ConfigValue.Bool;

		public object[] Serialize()
		{
			return new object[2]
			{
				ConfigValue.Bool.ToString(),
				value
			};
		}

		public void Deserialize(object[] data)
		{
			value = Convert.ToBoolean(data[1]);
		}

		public void Interpolate(Param p0, Param p1, float t)
		{
			value = ((BoolParam)(object)p1).value;
		}

		public void Set(Param p)
		{
			value = ((BoolParam)(object)p).value;
		}

		public Param Clone()
		{
			BoolParam boolParam = default(BoolParam);
			boolParam.Set(this);
			return boolParam;
		}
	}

	public struct SelectionParam : Param
	{
		public int index;

		public string[] value;

		public ConfigValue Type => ConfigValue.Selection;

		public object[] Serialize()
		{
			object[] array = new object[value.Length + 2];
			array[0] = ConfigValue.Selection.ToString();
			array[1] = index;
			value.CopyTo(array, 2);
			return array;
		}

		public int Find(string val)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == val)
				{
					return i;
				}
			}
			return -1;
		}

		public void Deserialize(object[] data)
		{
			index = Convert.ToInt32(data[1]);
			value = new string[data.Length - 2];
			for (int i = 2; i < data.Length; i++)
			{
				value[i - 2] = Convert.ToString(data[i]);
			}
		}

		public void Interpolate(Param p0, Param p1, float t)
		{
			index = ((SelectionParam)(object)p1).index;
		}

		public void Set(Param p)
		{
			index = ((SelectionParam)(object)p).index;
		}

		public Param Clone()
		{
			SelectionParam selectionParam = default(SelectionParam);
			selectionParam.Set(this);
			return selectionParam;
		}
	}

	public Dictionary<string, Dictionary<string, Param>> Params;

	public Dictionary<string, float> Transitions;

	public TextAsset ResourceAsset;

	public OnTransitMode TransitCallback;

	public OnTransitionStart TransitionStartCallback;

	public int ModeIndex;

	protected string currentMode;

	private float transitionTime;

	private Dictionary<string, Param> currParams = new Dictionary<string, Param>();

	private Dictionary<string, Param> oldParams = new Dictionary<string, Param>();

	private bool enableLiveGUI;

	private Vector2 scrolling;

	private Vector2 WindowPos = new Vector2(10f, 10f);

	private Vector2 WindowSize = new Vector2(400f, 800f);

	private int modeIndex;

	private bool showTransitions;

	public string DefaultConfigPath => ResourceDir + GetType().Name + ".json";

	public string ResourceDir => Application.dataPath + "/External/GameCamera/Resources/Config/";

	public string ResourceDirRel => "Config/";

	public virtual void LoadDefault()
	{
		currentMode = "Default";
		CopyParams(Params[currentMode], ref currParams);
		CopyParams(Params[currentMode], ref oldParams);
	}

	protected virtual void Awake()
	{
	}

	public bool SetCameraMode(string mode)
	{
		if (Params.ContainsKey(mode) && mode != currentMode)
		{
			if (TransitionStartCallback != null)
			{
				TransitionStartCallback(currentMode, mode);
			}
			currentMode = mode;
			transitionTime = 0f;
			CopyParams(currParams, ref oldParams);
			return true;
		}
		return false;
	}

	public string GetCurrentMode()
	{
		return currentMode;
	}

	private float GetTransitionTime(string key)
	{
		float num = transitionTime / Transitions[key];
		if (num > 1f)
		{
			num = 1f;
		}
		return num;
	}

	private void Update()
	{
		transitionTime += Time.deltaTime;
		Dictionary<string, Param> dictionary = Params[currentMode];
		float num = GetTransitionTime(currentMode);
		if (num > 0f && num < 1f && TransitCallback != null)
		{
			TransitCallback(currentMode, num);
		}
		using Dictionary<string, Dictionary<string, Param>>.ValueCollection.Enumerator enumerator = Params.Values.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return;
		}
		foreach (string key in enumerator.Current.Keys)
		{
			currParams[key].Interpolate(oldParams[key], dictionary[key], num);
		}
	}

	private void CopyParams(Dictionary<string, Param> src, ref Dictionary<string, Param> dst)
	{
		foreach (KeyValuePair<string, Param> item in src)
		{
			if (dst.TryGetValue(item.Key, out var value))
			{
				value.Set(item.Value);
			}
			else
			{
				dst.Add(item.Key, item.Value.Clone());
			}
		}
	}

	public bool GetBool(string mode, string key)
	{
		if (Params.TryGetValue(mode, out var value) && value.TryGetValue(key, out var value2))
		{
			return ((BoolParam)value2).value;
		}
		return false;
	}

	public bool GetBool(string key)
	{
		return GetBool(currentMode, key);
	}

	public bool IsBool(string key)
	{
		if (Params.ContainsKey(currentMode))
		{
			return Params[currentMode].ContainsKey(key);
		}
		return false;
	}

	public void SetBool(string mode, string key, bool inputValue)
	{
		if (Params.TryGetValue(mode, out var value) && value.TryGetValue(key, out var value2))
		{
			BoolParam boolParam = (BoolParam)(object)value2;
			boolParam.value = inputValue;
			value[key] = boolParam;
		}
	}

	public float GetFloat(string mode, string key)
	{
		if (Params.TryGetValue(mode, out var value) && value.TryGetValue(key, out var value2))
		{
			return ((RangeParam)value2).value;
		}
		return -1f;
	}

	public float GetFloat(string key)
	{
		return ((RangeParam)currParams[key]).value;
	}

	public float GetFloatMin(string mode, string key)
	{
		if (Params.TryGetValue(mode, out var value) && value.TryGetValue(key, out var value2))
		{
			return ((RangeParam)value2).min;
		}
		return -1f;
	}

	public float GetFloatMin(string key)
	{
		return GetFloatMin(currentMode, key);
	}

	public float GetFloatMax(string mode, string key)
	{
		if (Params.TryGetValue(mode, out var value) && value.TryGetValue(key, out var value2))
		{
			return ((RangeParam)value2).max;
		}
		return -1f;
	}

	public float GetFloatMax(string key)
	{
		return GetFloatMax(currentMode, key);
	}

	public void SetFloat(string mode, string key, float inputValue)
	{
		if (Params.TryGetValue(mode, out var value) && value.TryGetValue(key, out var value2))
		{
			RangeParam rangeParam = (RangeParam)value2;
			rangeParam.value = inputValue;
			value[key] = rangeParam;
		}
	}

	public Vector3 GetVector3(string mode, string key)
	{
		if (Params.TryGetValue(mode, out var value) && value.TryGetValue(key, out var value2))
		{
			return ((Vector3Param)value2).value;
		}
		return Vector3.zero;
	}

	public Vector3 GetVector3(string key)
	{
		return ((Vector3Param)currParams[key]).value;
	}

	public bool IsVector3(string key)
	{
		if (currParams != null && currParams.ContainsKey(key))
		{
			return true;
		}
		return false;
	}

	public Vector3 GetVector3Direct(string key)
	{
		return ((Vector3Param)Params[currentMode][key]).value;
	}

	public void SetVector3(string mode, string key, Vector3 inputValue)
	{
		if (Params.TryGetValue(mode, out var value) && value.TryGetValue(key, out var value2))
		{
			Vector3Param vector3Param = (Vector3Param)(object)value2;
			vector3Param.value = inputValue;
			value[key] = vector3Param;
		}
	}

	public Vector2 GetVector2(string mode, string key)
	{
		if (Params.TryGetValue(mode, out var value) && value.TryGetValue(key, out var value2))
		{
			return ((Vector2Param)value2).value;
		}
		return Vector2.zero;
	}

	public Vector2 GetVector2(string key)
	{
		return ((Vector2Param)currParams[key]).value;
	}

	public void SetVector2(string mode, string key, Vector2 inputValue)
	{
		if (Params.TryGetValue(mode, out var value) && value.TryGetValue(key, out var value2))
		{
			Vector2Param vector2Param = (Vector2Param)(object)value2;
			vector2Param.value = inputValue;
			value[key] = vector2Param;
		}
	}

	public string GetString(string mode, string key)
	{
		if (Params.TryGetValue(mode, out var value) && value.TryGetValue(key, out var value2))
		{
			return ((StringParam)value2).value;
		}
		return null;
	}

	public string GetString(string key)
	{
		return GetString(currentMode, key);
	}

	public void SetString(string mode, string key, string inputValue)
	{
		if (Params.TryGetValue(mode, out var value) && value.TryGetValue(key, out var value2))
		{
			StringParam stringParam = (StringParam)(object)value2;
			stringParam.value = inputValue;
			value[key] = stringParam;
		}
	}

	public string GetSelection(string mode, string key)
	{
		if (Params.TryGetValue(mode, out var value) && value.TryGetValue(key, out var value2))
		{
			SelectionParam selectionParam = (SelectionParam)(object)value2;
			return selectionParam.value[selectionParam.index];
		}
		return null;
	}

	public string GetSelection(string key)
	{
		return GetSelection(currentMode, key);
	}

	public void SetSelection(string mode, string key, int inputValue)
	{
		if (Params.TryGetValue(mode, out var value) && value.TryGetValue(key, out var value2))
		{
			SelectionParam selectionParam = (SelectionParam)(object)value2;
			selectionParam.index = inputValue;
			value[key] = selectionParam;
		}
	}

	public void SetSelection(string mode, string key, string inputValue)
	{
		if (Params.TryGetValue(mode, out var value) && value.TryGetValue(key, out var value2))
		{
			SelectionParam selectionParam = (SelectionParam)(object)value2;
			int num = selectionParam.Find(inputValue);
			if (num != -1)
			{
				selectionParam.index = num;
				value[key] = selectionParam;
			}
		}
	}

	public void AddMode(string cfgName)
	{
		Dictionary<string, Param> dictionary = Params["Default"];
		Dictionary<string, Param> dst = new Dictionary<string, Param>(dictionary.Count);
		CopyParams(dictionary, ref dst);
		Params.Add(cfgName, dst);
		Transitions.Add(cfgName, 0.25f);
	}

	public void DeleteMode(string cfgName)
	{
		Params.Remove(cfgName);
		Transitions.Remove(cfgName);
	}

	public void Serialize(string file)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>(Params.Count);
		foreach (KeyValuePair<string, Dictionary<string, Param>> param in Params)
		{
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>(param.Value.Count);
			foreach (KeyValuePair<string, Param> item in param.Value)
			{
				object[] value = item.Value.Serialize();
				dictionary2.Add(item.Key, value);
			}
			dictionary.Add(param.Key, dictionary2);
		}
		if (Params.Count > 1)
		{
			dictionary.Add("Transitions", Transitions);
		}
		string text = Json.Serialize(dictionary);
		if (!string.IsNullOrEmpty(text))
		{
			IO.WriteTextFile(file, text);
		}
	}

	public void Deserialize(string file)
	{
		string text = IO.ReadTextFile(file);
		if (string.IsNullOrEmpty(text))
		{
			if (!ResourceAsset)
			{
				RefreshResourceAsset();
			}
			if ((bool)ResourceAsset)
			{
				text = ResourceAsset.text;
			}
		}
		if (string.IsNullOrEmpty(text) || !(Json.Deserialize(text) is Dictionary<string, object> dictionary))
		{
			return;
		}
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			if (!(item.Value is Dictionary<string, object> dictionary2))
			{
				continue;
			}
			foreach (KeyValuePair<string, object> item2 in dictionary2)
			{
				if (item.Key == "Transitions")
				{
					Transitions[item2.Key] = Convert.ToSingle(item2.Value);
					continue;
				}
				List<object> list = item2.Value as List<object>;
				object[] data = list.ToArray();
				ConfigValue configValue = (ConfigValue)Enum.Parse(typeof(ConfigValue), list[0] as string);
				Param param = null;
				switch (configValue)
				{
				case ConfigValue.Bool:
					param = default(BoolParam);
					break;
				case ConfigValue.Range:
					param = new RangeParam();
					break;
				case ConfigValue.Selection:
					param = default(SelectionParam);
					break;
				case ConfigValue.String:
					param = default(StringParam);
					break;
				case ConfigValue.Vector2:
					param = default(Vector2Param);
					break;
				case ConfigValue.Vector3:
					param = default(Vector3Param);
					break;
				}
				param.Deserialize(data);
				Params[item.Key][item2.Key] = param;
			}
		}
	}

	public void RefreshResourceAsset()
	{
		ResourceAsset = Resources.Load<TextAsset>(ResourceDirRel + GetType().Name);
	}

	public void EnableLiveGUI(bool status)
	{
		enableLiveGUI = status;
	}

	private void OnGUI()
	{
		if (enableLiveGUI)
		{
			float num = WindowSize.y;
			float num2 = WindowSize.x;
			if (num > (float)Screen.height)
			{
				num = Screen.height;
			}
			if (num2 > (float)Screen.width)
			{
				num2 = Screen.width;
			}
			GUISkin guiSkin = CameraManager.Instance.GuiSkin;
			if ((bool)guiSkin)
			{
				GUI.skin = guiSkin;
			}
			GUILayout.Window(0, new Rect((float)Screen.width - num2 - WindowPos.x, WindowPos.y, num2, num), GUIWindow, "Live GUI");
		}
	}

	private void GUIWindow(int id)
	{
		if (Params == null)
		{
			return;
		}
		scrolling = GUILayout.BeginScrollView(scrolling);
		string[] array = new string[Params.Keys.Count + 1];
		array[0] = "All";
		Params.Keys.CopyTo(array, 1);
		GUIUtils.Selection("Show modes", array, ref modeIndex);
		foreach (KeyValuePair<string, Dictionary<string, Param>> param in Params)
		{
			bool flag = false;
			if (array[modeIndex] != "All" && array[modeIndex] != param.Key)
			{
				continue;
			}
			GUIUtils.Separator(param.Key, 23f);
			foreach (KeyValuePair<string, Param> item in param.Value)
			{
				string key = item.Key;
				Param value = item.Value;
				switch (value.Type)
				{
				case ConfigValue.Bool:
				{
					BoolParam boolParam = (BoolParam)(object)value;
					if (GUIUtils.Toggle(key, ref boolParam.value))
					{
						param.Value[key] = boolParam;
						flag = true;
					}
					break;
				}
				case ConfigValue.Range:
				{
					RangeParam rangeParam = (RangeParam)value;
					if (GUIUtils.SliderEdit(item.Key, rangeParam.min, rangeParam.max, ref rangeParam.value))
					{
						param.Value[key] = rangeParam;
						flag = true;
					}
					break;
				}
				case ConfigValue.Selection:
				{
					SelectionParam selectionParam = (SelectionParam)(object)value;
					if (GUIUtils.Selection(item.Key, selectionParam.value, ref selectionParam.index))
					{
						param.Value[key] = selectionParam;
						flag = true;
					}
					break;
				}
				case ConfigValue.String:
				{
					StringParam stringParam = (StringParam)(object)value;
					if (GUIUtils.String(item.Key, ref stringParam.value))
					{
						param.Value[key] = stringParam;
						flag = true;
					}
					break;
				}
				case ConfigValue.Vector2:
				{
					Vector2Param vector2Param = (Vector2Param)(object)value;
					if (GUIUtils.Vector2(item.Key, ref vector2Param.value))
					{
						param.Value[key] = vector2Param;
						flag = true;
					}
					break;
				}
				case ConfigValue.Vector3:
				{
					Vector3Param vector3Param = (Vector3Param)(object)value;
					if (GUIUtils.Vector3(item.Key, ref vector3Param.value))
					{
						param.Value[key] = vector3Param;
						flag = true;
					}
					break;
				}
				}
				if (flag)
				{
					break;
				}
			}
		}
		if (Params.Count > 1)
		{
			if (!showTransitions)
			{
				GUIUtils.Separator("", 1f);
			}
			GUIUtils.Toggle("Show transitions", ref showTransitions);
			if (showTransitions)
			{
				GUIUtils.Separator("Transitions", 23f);
				foreach (KeyValuePair<string, float> transition in Transitions)
				{
					float value2 = transition.Value;
					if (GUIUtils.SliderEdit(transition.Key, 0f, 2f, ref value2))
					{
						Transitions[transition.Key] = value2;
						break;
					}
				}
			}
		}
		GUILayout.EndScrollView();
	}
}
