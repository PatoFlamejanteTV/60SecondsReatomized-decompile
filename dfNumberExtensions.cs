using UnityEngine;

public static class dfNumberExtensions
{
	public static int Quantize(this int value, int stepSize)
	{
		if (stepSize <= 0)
		{
			return value;
		}
		return value / stepSize * stepSize;
	}

	public static float Quantize(this float value, float stepSize)
	{
		if (stepSize <= 0f)
		{
			return value;
		}
		return Mathf.Floor(value / stepSize) * stepSize;
	}

	public static int RoundToNearest(this int value, int stepSize)
	{
		if (stepSize <= 0)
		{
			return value;
		}
		int num = value / stepSize * stepSize;
		if (value % stepSize >= stepSize / 2)
		{
			return num + stepSize;
		}
		return num;
	}

	public static float RoundToNearest(this float value, float stepSize)
	{
		if (stepSize <= 0f)
		{
			return value;
		}
		float num = Mathf.Floor(value / stepSize) * stepSize;
		if (value - stepSize * Mathf.Floor(value / stepSize) >= stepSize * 0.5f - float.Epsilon)
		{
			return num + stepSize;
		}
		return num;
	}
}
