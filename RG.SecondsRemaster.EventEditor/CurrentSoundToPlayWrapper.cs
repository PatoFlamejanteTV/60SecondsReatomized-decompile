using System;

namespace RG.SecondsRemaster.EventEditor;

[Serializable]
internal struct CurrentSoundToPlayWrapper
{
	public string EventName;

	public int EventPriority;

	public float Volume;

	public float Pan;

	public float Pitch;

	public int Offset;

	public bool OffsetCheck;
}
