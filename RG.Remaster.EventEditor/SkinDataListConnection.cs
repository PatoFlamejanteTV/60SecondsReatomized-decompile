using System;
using NodeEditorFramework;
using RG.Remaster.Survival;
using UnityEngine;

namespace RG.Remaster.EventEditor;

public class SkinDataListConnection : IConnectionTypeDeclaration
{
	public const string ID = "SkinDataList";

	public string Identifier => "SkinDataList";

	public Type Type => typeof(SkinDataList);

	public Color Color => new Color(0.1764706f, 89f / 180f, 0.47058824f);

	public string InKnobTex => "Textures/In_Knob.png";

	public string OutKnobTex => "Textures/Out_Knob.png";

	public string InKnobFilledTex => "Textures/In_Knob_Filled.png";

	public string OutKnobFilledTex => "Textures/In_Knob_Filled.png";
}
