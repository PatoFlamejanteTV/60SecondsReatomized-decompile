using UnityEngine;

namespace RG_GameCamera.Input;

public class InputFilter
{
	private Vector2 value;

	private readonly Vector2[] samples;

	private readonly float weightCoef;

	private readonly int numSamples;

	public InputFilter(int samplesNum, float coef)
	{
		value = default(Vector2);
		weightCoef = coef;
		numSamples = samplesNum;
		samples = new Vector2[samplesNum];
	}

	public void AddSample(Vector2 sample)
	{
		Vector2 vector = default(Vector2);
		float num = 0f;
		float num2 = 1f;
		float num3 = 1f;
		Vector2 vector2 = samples[0];
		samples[0] = sample;
		for (int i = 1; i < numSamples; i++)
		{
			num += num3;
			vector += samples[i - 1] * num3;
			Vector2 vector3 = samples[i];
			samples[i] = vector2;
			vector2 = vector3;
			num3 = num2 * weightCoef;
			num2 = num3;
		}
		value = vector / num;
	}

	public Vector2 GetValue()
	{
		return value;
	}

	public Vector2[] GetSamples()
	{
		return samples;
	}

	public void Reset(Vector2 resetVal)
	{
		for (int i = 0; i < numSamples; i++)
		{
			samples[i] = resetVal;
		}
	}
}
