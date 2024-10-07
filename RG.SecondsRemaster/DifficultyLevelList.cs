using System.Collections.Generic;
using RG.Core.Base;
using UnityEngine;

namespace RG.SecondsRemaster;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/New Difficulty Level List", fileName = "New Difficulty Level List")]
public class DifficultyLevelList : RGScriptableObject
{
	[SerializeField]
	private List<DifficultyLevel> _levels;

	public List<DifficultyLevel> Levels => _levels;
}
