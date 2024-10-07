using System;

namespace DunGen;

public class FloatRange
{
	public float Min;

	public float Max;

	public FloatRange()
	{
	}

	public FloatRange(float min, float max)
	{
		Min = min;
		Max = max;
	}

	public float GetRandom(Random random)
	{
		if (Min > Max)
		{
			float min = Min;
			Max = Min;
			Min = min;
		}
		float num = Max - Min;
		return Min + (float)random.NextDouble() * num;
	}
}
