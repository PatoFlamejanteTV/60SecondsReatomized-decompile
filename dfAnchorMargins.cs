using System;
using UnityEngine;

[Serializable]
public class dfAnchorMargins
{
	[SerializeField]
	public float left;

	[SerializeField]
	public float top;

	[SerializeField]
	public float right;

	[SerializeField]
	public float bottom;

	public override string ToString()
	{
		return $"[L:{left},T:{top},R:{right},B:{bottom}]";
	}
}
