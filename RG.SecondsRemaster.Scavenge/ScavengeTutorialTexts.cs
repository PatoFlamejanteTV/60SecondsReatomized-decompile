using System.Collections.Generic;
using RG.Core.Base;
using UnityEngine;

namespace RG.SecondsRemaster.Scavenge;

[CreateAssetMenu(fileName = "New Scavenge Tutorial Texts", menuName = "60 Seconds Remaster!/Scavenge/New Scavenge Tutorial Texts")]
public class ScavengeTutorialTexts : RGScriptableObject
{
	[SerializeField]
	private List<ScavengeTutorialState> _states;

	public ScavengeTutorialState GetTexts(ScavengeTutorialDriver.ETutorialStage state)
	{
		for (int i = 0; i < _states.Count; i++)
		{
			if (_states[i].State == state)
			{
				return _states[i];
			}
		}
		return null;
	}
}
