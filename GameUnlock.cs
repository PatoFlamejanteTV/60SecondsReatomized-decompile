using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameUnlock : ScriptableObject
{
	[SerializeField]
	private string _id = string.Empty;

	[SerializeField]
	private EGameUnlockType _type;

	[SerializeField]
	private string _name = string.Empty;

	[SerializeField]
	private string _descr = string.Empty;

	[SerializeField]
	private List<string> _targets = new List<string>();

	[SerializeField]
	private string _icon = string.Empty;

	[SerializeField]
	private List<SKeyValuePair> _parameters = new List<SKeyValuePair>();

	private bool _unlocked;

	public string Id => _id;

	public EGameUnlockType Type => _type;

	public string Icon => _icon;

	public string Name => _name;

	public string Description => _descr;

	public List<SKeyValuePair> Values => _parameters;

	public bool Unlocked => _unlocked;

	public List<string> Targets => _targets;

	public void Unlock(bool unlock)
	{
		_unlocked = unlock;
	}

	public string GetParameter(string key)
	{
		string result = null;
		for (int i = 0; i < _parameters.Count; i++)
		{
			if (_parameters[i].Id == key)
			{
				return _parameters[i].Val;
			}
		}
		return result;
	}

	public bool GetParameterBool(string key, ref bool val)
	{
		string parameter = GetParameter(key);
		if (!string.IsNullOrEmpty(parameter))
		{
			val = bool.Parse(parameter);
			return true;
		}
		return false;
	}

	public bool GetParameterInt(string key, ref int val)
	{
		if (!string.IsNullOrEmpty(GetParameter(key)))
		{
			val = int.Parse(key);
			return true;
		}
		return false;
	}

	public bool IsTarget(string target)
	{
		if (_targets.Count > 0)
		{
			for (int i = 0; i < _targets.Count; i++)
			{
				if (_targets[i] == target)
				{
					return true;
				}
			}
		}
		return false;
	}

	public string GetTarget()
	{
		if (_targets.Count > 0)
		{
			return _targets[0];
		}
		return null;
	}

	public string[] GetTargets()
	{
		if (_targets.Count > 0)
		{
			return _targets.ToArray();
		}
		return null;
	}
}
