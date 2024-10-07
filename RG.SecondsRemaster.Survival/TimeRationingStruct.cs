using System;
using System.Collections.Generic;
using RG.Parsecs.Survival;

namespace RG.SecondsRemaster.Survival;

[Serializable]
internal class TimeRationingStruct
{
	public ConsumableRemedium ConsumableRemedium;

	public List<int> Characters;
}
