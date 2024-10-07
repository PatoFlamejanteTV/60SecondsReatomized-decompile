using System;
using System.Collections.Generic;
using RG.Core.Base;
using UnityEngine;

namespace RG.Remaster.Survival;

[Serializable]
[CreateAssetMenu(menuName = "60 Seconds Remaster!/Characters/HatDataList", fileName = "New HatDataList")]
public class HatDataList : RGScriptableObject
{
	[SerializeField]
	private List<HatData> _hatData = new List<HatData>();

	public List<HatData> HatData => _hatData;
}
