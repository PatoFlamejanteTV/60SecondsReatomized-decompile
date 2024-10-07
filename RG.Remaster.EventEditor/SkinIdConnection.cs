using System;
using NodeEditorFramework;
using RG.Remaster.Survival;
using UnityEngine;

namespace RG.Remaster.EventEditor;

public class SkinIdConnection : IConnectionTypeDeclaration
{
	public const string ID = "SkinId";

	public string Identifier => "SkinId";

	public Type Type => typeof(SkinId);

	public Color Color => new Color(0.34901962f, 89f / 180f, 0.03137255f);

	public string InKnobTex => "Textures/In_Knob.png";

	public string OutKnobTex => "Textures/Out_Knob.png";

	public string InKnobFilledTex => "Textures/In_Knob_Filled.png";

	public string OutKnobFilledTex => "Textures/In_Knob_Filled.png";
}
