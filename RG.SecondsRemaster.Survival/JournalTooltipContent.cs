using I2.Loc;
using RG.Parsecs.Common;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class JournalTooltipContent : TooltipContent
{
	[SerializeField]
	private Button _tab;

	[SerializeField]
	private LocalizedString _inactiveTab;

	[SerializeField]
	private LocalizedString _name;

	public virtual LocalizedString Name()
	{
		if (_tab != null && !_tab.interactable && !string.IsNullOrEmpty(_inactiveTab))
		{
			return _inactiveTab;
		}
		return _name;
	}

	public override bool IsValid()
	{
		if ((string)_name == null || string.IsNullOrEmpty(_name))
		{
			return false;
		}
		return true;
	}
}
