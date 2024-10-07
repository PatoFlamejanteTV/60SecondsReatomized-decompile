using System;
using System.Collections.Generic;
using I2.Loc;

namespace RG.SecondsRemaster.Scavenge;

[Serializable]
public class ScavengeTutorialState
{
	public ScavengeTutorialDriver.ETutorialStage State;

	public List<LocalizedString> Texts;

	public LocalizedString Goal;

	public List<LocalizedString> Success;

	public List<LocalizedString> Fail;
}
