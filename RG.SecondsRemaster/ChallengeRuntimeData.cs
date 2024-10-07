using System;
using UnityEngine;

namespace RG.SecondsRemaster;

[Serializable]
public class ChallengeRuntimeData
{
	[SerializeField]
	private float _time;

	[SerializeField]
	private string _unlockDate;

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

	public string UnlockDate
	{
		get
		{
			return _unlockDate;
		}
		set
		{
			_unlockDate = value;
		}
	}
}
