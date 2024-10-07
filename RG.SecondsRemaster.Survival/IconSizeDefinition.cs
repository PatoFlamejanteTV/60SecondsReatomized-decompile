using RG.Core.Base;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/Icon Size Definition")]
public class IconSizeDefinition : RGScriptableObject
{
	[SerializeField]
	private Vector2 _size;

	public Vector2 Size => _size;
}
