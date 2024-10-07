using System.Collections.Generic;
using RG.Core.Base;
using UnityEngine;

namespace RG.SecondsRemaster.Scavenge;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/Scavenge/Scavenge Item List")]
public class ScavengeItemList : RGScriptableObject
{
	[SerializeField]
	private List<ScavengeItem> _items;

	public List<ScavengeItem> Items => _items;
}
