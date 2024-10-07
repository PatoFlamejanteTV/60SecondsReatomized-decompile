using UnityEngine;

namespace RG_GameCamera.Utils;

public static class Interpolation
{
	public static Vector3 Cubic(Vector3 y0, Vector3 y1, Vector3 y2, Vector3 y3, float t)
	{
		float num = t * t;
		Vector3 vector = y3 - y2 - y0 + y1;
		Vector3 vector2 = y0 - y1 - vector;
		Vector3 vector3 = y2 - y0;
		return vector * t * num + vector2 * num + vector3 * t + y1;
	}

	public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		float num = t * t;
		float num2 = num * t;
		return p0 * (-0.5f * num2 + num - 0.5f * t) + p1 * (1.5f * num2 + -2.5f * num + 1f) + p2 * (-1.5f * num2 + 2f * num + 0.5f * t) + p3 * (0.5f * num2 - 0.5f * num);
	}

	public static float EaseInOutCubic(float t, float b, float c, float d)
	{
		t /= d / 2f;
		if (t < 1f)
		{
			return c / 2f * t * t * t + b;
		}
		t -= 2f;
		return c / 2f * (t * t * t + 2f) + b;
	}

	public static float InterpolateTowards(float pPrev, float pNext, float pSpeed, float pDt)
	{
		float num = pNext - pPrev;
		float num2 = pSpeed * pDt;
		if (!(pPrev + num >= 0f))
		{
			return Mathf.Max(num, 0f - num2);
		}
		return Mathf.Min(num, num2);
	}

	public static float Lerp(float a, float b, float t)
	{
		return a * (1f - t) + b * t;
	}

	public static float LerpS(float a, float b, float t)
	{
		float num = t * t;
		float num2 = 3f * num - 2f * num * t;
		return a * (1f - num2) + b * num2;
	}

	public static Vector2 LerpS(Vector2 a, Vector2 b, float t)
	{
		float num = t * t;
		float num2 = 3f * num - 2f * num * t;
		return a * (1f - num2) + b * num2;
	}

	public static Vector3 LerpS(Vector3 a, Vector3 b, float t)
	{
		float num = t * t;
		float num2 = 3f * num - 2f * num * t;
		return a * (1f - num2) + b * num2;
	}

	public static float LerpS2(float a, float b, float t)
	{
		float num = t * t;
		float num2 = t + num - num * t;
		return a * (1f - num2) + b * num2;
	}

	public static Vector3 LerpS2(Vector3 a, Vector3 b, float t)
	{
		float num = t * t;
		float num2 = t + num - num * t;
		return a * (1f - num2) + b * num2;
	}

	public static Vector2 LerpS2(Vector2 a, Vector2 b, float t)
	{
		float num = t * t;
		float num2 = t + num - num * t;
		return a * (1f - num2) + b * num2;
	}

	public static float LerpS3(float a, float b, float t)
	{
		float num = t * t;
		float num2 = 1f - t - num + num * t;
		return a * (1f - num2) + b * num2;
	}

	public static Vector2 LerpS3(Vector2 a, Vector2 b, float t)
	{
		float num = t * t;
		float num2 = 1f - t - num + num * t;
		return a * (1f - num2) + b * num2;
	}

	public static Vector3 LerpS3(Vector3 a, Vector3 b, float t)
	{
		float num = t * t;
		float num2 = 1f - t - num + num * t;
		return a * (1f - num2) + b * num2;
	}

	public static float LerpExpN(float a, float b, float t, float n)
	{
		float num = Mathf.Exp(n * Mathf.Log(t));
		return a * (1f - num) + b * num;
	}
}
