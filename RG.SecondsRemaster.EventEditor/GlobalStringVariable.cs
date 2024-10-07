using System;
using RG.Core.SaveSystem;
using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.SecondsRemaster.EventEditor;

[CreateAssetMenu(fileName = "New String Variable", menuName = "60 Parsecs!/Event Editor/Variables/Global/New String Variable")]
public class GlobalStringVariable : Variable<string>, ISaveable
{
	private string _typeConnection = "String";

	private const string GLOBAL_STRING_VAR_PREFIX = "GlobalStringVar";

	public override string TypeConnection => _typeConnection;

	public override Type GetVariableType => typeof(string);

	public string ID => Guid;

	public override string GuidPrefix => "GlobalStringVar";

	public override object GetValue()
	{
		return Value;
	}

	public string GetInitialValue()
	{
		return _initialValue;
	}

	private void OnEnable()
	{
		Register();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Unregister();
	}

	public string Serialize()
	{
		StringWrapper stringWrapper = default(StringWrapper);
		stringWrapper.Value = Value;
		return JsonUtility.ToJson(stringWrapper);
	}

	public void Deserialize(string jsonData)
	{
		Value = JsonUtility.FromJson<StringWrapper>(jsonData).Value;
	}

	public void Register()
	{
		if (!_isStatic)
		{
			if ((bool)_saveEvent)
			{
				_saveEvent.RegisterListener(this);
			}
			else
			{
				Debug.Log($"Global string variable {base.name} has not set Save Event");
			}
		}
	}

	public void Unregister()
	{
		if (!_isStatic && (bool)_saveEvent)
		{
			_saveEvent.UnregisterListener(this);
		}
	}

	public void ResetData()
	{
		Value = _initialValue;
	}
}
