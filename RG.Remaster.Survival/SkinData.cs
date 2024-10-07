using System;
using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.Remaster.Survival;

[Serializable]
public class SkinData
{
	public GlobalBoolVariable IsUnlockedVariable;

	[Tooltip("All the variables in this list ,ust also evaluate to true for the skin to be usable")]
	public GlobalBoolVariable[] AdditionalRequirements;

	public SkinId SkinId;
}
