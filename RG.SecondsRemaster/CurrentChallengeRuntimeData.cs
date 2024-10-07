using System;
using UnityEngine;

namespace RG.SecondsRemaster;

[Serializable]
public class CurrentChallengeRuntimeData
{
	[SerializeField]
	private Challenge _challenge;

	public Challenge Challenge
	{
		get
		{
			return _challenge;
		}
		set
		{
			_challenge = value;
		}
	}
}
