using System;
using UnityEngine;

namespace RG.SecondsRemaster.Core;

[Serializable]
public class SecondsRemediumStaticData
{
	[SerializeField]
	private bool _isDamaged;

	public bool IsDamaged => _isDamaged;
}
