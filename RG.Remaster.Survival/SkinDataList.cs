using System;
using System.Collections.Generic;
using RG.Core.Base;
using UnityEngine;

namespace RG.Remaster.Survival;

[Serializable]
[CreateAssetMenu(menuName = "60 Seconds Remaster!/SkinDataList", fileName = "New SkinDataList")]
public class SkinDataList : RGScriptableObject
{
	[SerializeField]
	private List<SkinData> _skinData = new List<SkinData>();

	public List<SkinData> SkinData => _skinData;

	public bool IsValid()
	{
		return true;
	}
}
