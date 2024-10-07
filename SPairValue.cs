using System;
using UnityEngine;

[Serializable]
public struct SPairValue
{
	[SerializeField]
	private int _value1;

	[SerializeField]
	private int _value2;

	public int Value1
	{
		get
		{
			return _value1;
		}
		set
		{
			_value1 = value;
		}
	}

	public int Value2
	{
		get
		{
			return _value2;
		}
		set
		{
			_value2 = value;
		}
	}

	public int Value => UnityEngine.Random.Range(_value1, _value2 + 1);

	public bool Available
	{
		get
		{
			if (_value1 <= 0)
			{
				return _value2 > 0;
			}
			return true;
		}
	}

	public void GetValues(out string v1, out string sep, out string v2)
	{
		sep = string.Empty;
		v2 = string.Empty;
		v1 = _value1.ToString();
		if (_value1 != _value2)
		{
			v2 = _value2.ToString();
			sep = "-";
		}
	}
}
