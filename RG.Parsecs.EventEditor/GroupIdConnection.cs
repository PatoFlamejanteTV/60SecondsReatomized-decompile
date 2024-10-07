using System;
using NodeEditorFramework;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.Parsecs.EventEditor;

public class GroupIdConnection : IConnectionTypeDeclaration
{
	public const string ID = "GroupId";

	public string Identifier => "GroupId";

	public Type Type => typeof(TextJournalGroupId);

	public Color Color => new Color(0f, 0.5882353f, 20f / 51f);

	public string InKnobTex => "Textures/In_Knob.png";

	public string OutKnobTex => "Textures/In_Knob.png";

	public string InKnobFilledTex => "Textures/In_Knob_Filled.png";

	public string OutKnobFilledTex => "Textures/In_Knob_Filled.png";
}
