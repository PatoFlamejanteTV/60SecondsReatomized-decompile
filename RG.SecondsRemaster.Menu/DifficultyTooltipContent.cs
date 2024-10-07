using I2.Loc;
using RG.Parsecs.Common;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class DifficultyTooltipContent : TooltipContent
{
	[SerializeField]
	[Tooltip("Difficulty params, the number of elements must match the number of headers in Content Handler!")]
	private LocalizedString[] _difficultyLevelTexts;

	public LocalizedString[] DifficultyLevelTexts => _difficultyLevelTexts;

	public override bool IsValid()
	{
		return _difficultyLevelTexts != null;
	}
}
