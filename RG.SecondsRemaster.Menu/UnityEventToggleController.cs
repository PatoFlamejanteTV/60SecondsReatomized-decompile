using RG.Parsecs.Common;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class UnityEventToggleController : ToggleController
{
	[SerializeField]
	private OnValueChange _onValueChange;

	public override void OnToggleValueChangedAction(bool toggleValue)
	{
		_onValueChange.Invoke(toggleValue);
	}
}
