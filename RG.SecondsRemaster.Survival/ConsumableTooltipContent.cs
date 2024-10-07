using I2.Loc;
using RG.Parsecs.Common;
using RG.SecondsRemaster.Core;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class ConsumableTooltipContent : TooltipContent
{
	[SerializeField]
	private LocalizedString _generalInfo;

	[SerializeField]
	private LocalizedString _containerName;

	[SerializeField]
	private SecondsConsumableRemedium _consumable;

	public SecondsConsumableRemedium Consumable => _consumable;

	public LocalizedString GeneralInfo => _generalInfo;

	public LocalizedString ContainerName => _containerName;

	public override bool IsValid()
	{
		if (string.IsNullOrEmpty(_generalInfo) || string.IsNullOrEmpty(_containerName) || _consumable == null)
		{
			return false;
		}
		return true;
	}
}
