using System.Collections.Generic;
using RG.Core.Base;
using UnityEngine;

namespace RG.SecondsRemaster;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/Challenges/New Challenge List", fileName = "New Challenge List")]
public class ChallengeList : RGScriptableObject
{
	[SerializeField]
	private List<Challenge> _challenges;

	public List<Challenge> Challenges => _challenges;
}
