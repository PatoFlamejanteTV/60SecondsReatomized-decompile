using UnityEngine;

namespace RG_GameCamera.Input;

public class PositionFilter
{
	private Vector3 value;

	private readonly Vector3[] samples;

	private readonly float weightCoef;

	private readonly int numSamples;

	public PositionFilter(int samplesNum, float coef)
	{
		value = default(Vector3);
		weightCoef = coef;
		numSamples = samplesNum;
		samples = new Vector3[samplesNum];
	}

	public void AddSample(Vector3 sample)
	{
		Vector3 vector = default(Vector3);
		float num = 0f;
		float num2 = 1f;
		float num3 = 1f;
		Vector3 vector2 = samples[0];
		samples[0] = sample;
		for (int i = 1; i < numSamples; i++)
		{
			num += num3;
			vector += samples[i - 1] * num3;
			Vector3 vector3 = samples[i];
			samples[i] = vector2;
			vector2 = vector3;
			num3 = num2 * weightCoef;
			num2 = num3;
		}
		value = vector / num;
	}

	public Vector3 GetValue()
	{
		return value;
	}

	public Vector3[] GetSamples()
	{
		return samples;
	}

	public void Reset(Vector3 resetVal)
	{
		for (int i = 0; i < numSamples; i++)
		{
			samples[i] = resetVal;
		}
	}
}
