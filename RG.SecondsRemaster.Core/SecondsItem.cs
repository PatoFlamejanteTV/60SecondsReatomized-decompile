using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Core;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/Crafting/New Item", fileName = "New Item")]
public class SecondsItem : Item
{
	[SerializeField]
	private bool _isDamageable = true;

	[SerializeField]
	private IconSizeDefinition _iconSizeDefinition;

	public IconSizeDefinition IconSizeDefinition => _iconSizeDefinition;

	public override void SetDamage()
	{
		if (_isDamageable)
		{
			base.SetDamage();
		}
		else
		{
			Remove();
		}
	}

	public override void UseItem(int value)
	{
		SetDamage();
	}

	public override bool IsLockable()
	{
		if (base.IsLockable())
		{
			return !base.RuntimeData.IsDamaged;
		}
		return false;
	}
}
