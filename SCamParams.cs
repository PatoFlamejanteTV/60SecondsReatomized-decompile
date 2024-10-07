using System;
using UnityEngine;

[Serializable]
public struct SCamParams
{
	[SerializeField]
	private Vector3 _relocation;

	[SerializeField]
	private float _time;

	[SerializeField]
	private string _easeType;

	public Vector3 Relocation
	{
		get
		{
			return _relocation;
		}
		set
		{
			_relocation = value;
		}
	}

	public float Time
	{
		get
		{
			return _time;
		}
		set
		{
			_time = value;
		}
	}

	public string EaseType
	{
		get
		{
			return _easeType;
		}
		set
		{
			_easeType = value;
		}
	}
}
