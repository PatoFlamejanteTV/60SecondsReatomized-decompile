using System;
using System.Collections.Generic;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;

namespace RG.Remaster.Survival;

[Serializable]
public class HatData
{
	public GlobalBoolVariable IsUnlockedVariable;

	public CharacterStatus IsWornStatus;

	public List<Character> AllowedCharacters = new List<Character>();

	public List<Mission> DisallowedMissions = new List<Mission>();
}
