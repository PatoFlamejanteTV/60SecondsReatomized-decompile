using System;
using UnityEngine;

[Serializable]
public class TextEntry
{
	private const int DEF_PARAM_LIMIT = 3;

	[SerializeField]
	private string[] _params = new string[3];

	[SerializeField]
	private string _text = string.Empty;

	[SerializeField]
	private int _paramCount = 3;

	public string Text => _text;

	public int ParamCount
	{
		get
		{
			return _paramCount;
		}
		set
		{
			if (value != _paramCount && value >= 0)
			{
				string[] array = new string[value];
				_paramCount = value;
				for (int i = 0; i < _params.Length && i < value; i++)
				{
					array[i] = _params[i];
				}
				_params = array;
			}
		}
	}

	public TextEntry()
	{
	}

	public TextEntry(string txt, string p1 = null, string p2 = null, string p3 = null)
	{
		_text = txt;
		_paramCount = ((p1 != null) ? 1 : 0) + ((p2 != null) ? 1 : 0) + ((p3 != null) ? 1 : 0);
		if (p1 != null)
		{
			_params[0] = p1;
		}
		if (p2 != null)
		{
			_params[1] = p2;
		}
		if (p3 != null)
		{
			_params[2] = p3;
		}
	}

	public string GetParam(int index)
	{
		if (index >= 0 && index < _params.Length)
		{
			return _params[index];
		}
		return null;
	}

	public bool TestParam(string param)
	{
		for (int i = 0; i < _params.Length && !string.IsNullOrEmpty(_params[i]); i++)
		{
			if (_params[i] == param)
			{
				return true;
			}
		}
		return false;
	}
}
