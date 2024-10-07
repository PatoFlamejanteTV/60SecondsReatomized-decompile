using I2.Loc;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class NewGameTooltipContent : TooltipContent
{
	[SerializeField]
	private LocalizedString _textTerm;

	[SerializeField]
	private GlobalBoolVariable _isContinueAvailable;

	public LocalizedString TextTerm => _textTerm;

	public override bool IsValid()
	{
		if ((string)TextTerm != null && !TextTerm.IsNullOrEmpty() && _isContinueAvailable != null)
		{
			return _isContinueAvailable.Value;
		}
		return false;
	}
}
