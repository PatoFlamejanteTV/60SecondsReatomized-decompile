using System;
using UnityEngine;

namespace RG.SecondsRemaster.Core;

[Serializable]
public class SecondsRemediumRuntimeData
{
	[SerializeField]
	private bool _isDamaged;

	public bool IsDamaged
	{
		get
		{
			return _isDamaged;
		}
		set
		{
			_isDamaged = value;
		}
	}
}
