using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class BezierCurve : MonoBehaviour
{
	public int resolution = 30;

	public Color drawColor = Color.white;

	[SerializeField]
	private bool _close;

	private float _length;

	[SerializeField]
	private BezierPoint[] points = new BezierPoint[0];

	public bool dirty { get; private set; }

	public bool close
	{
		get
		{
			return _close;
		}
		set
		{
			if (_close != value)
			{
				_close = value;
				dirty = true;
			}
		}
	}

	public BezierPoint this[int index] => points[index];

	public int pointCount => points.Length;

	public float length
	{
		get
		{
			if (dirty)
			{
				_length = 0f;
				for (int i = 0; i < points.Length - 1; i++)
				{
					_length += ApproximateLength(points[i], points[i + 1], resolution);
				}
				if (close)
				{
					_length += ApproximateLength(points[points.Length - 1], points[0], resolution);
				}
				dirty = false;
			}
			return _length;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = drawColor;
		if (points.Length > 1)
		{
			for (int i = 0; i < points.Length - 1; i++)
			{
				DrawCurve(points[i], points[i + 1], resolution);
			}
			if (close)
			{
				DrawCurve(points[points.Length - 1], points[0], resolution);
			}
		}
	}

	private void Awake()
	{
		dirty = true;
	}

	public void AddPoint(BezierPoint point)
	{
		List<BezierPoint> list = new List<BezierPoint>(points);
		list.Add(point);
		points = list.ToArray();
		dirty = true;
	}

	public BezierPoint AddPointAt(Vector3 position)
	{
		GameObject obj = new GameObject("Point " + pointCount);
		obj.transform.parent = base.transform;
		obj.transform.position = position;
		BezierPoint bezierPoint = obj.AddComponent<BezierPoint>();
		bezierPoint.curve = this;
		return bezierPoint;
	}

	public void RemovePoint(BezierPoint point)
	{
		List<BezierPoint> list = new List<BezierPoint>(points);
		list.Remove(point);
		points = list.ToArray();
		dirty = false;
	}

	public BezierPoint[] GetAnchorPoints()
	{
		return (BezierPoint[])points.Clone();
	}

	public Vector3 GetPointAt(float t)
	{
		if (t <= 0f)
		{
			return points[0].position;
		}
		if (t >= 1f)
		{
			return points[points.Length - 1].position;
		}
		float num = 0f;
		float num2 = 0f;
		BezierPoint bezierPoint = null;
		BezierPoint p = null;
		for (int i = 0; i < points.Length - 1; i++)
		{
			num2 = ApproximateLength(points[i], points[i + 1]) / length;
			if (num + num2 > t)
			{
				bezierPoint = points[i];
				p = points[i + 1];
				break;
			}
			num += num2;
		}
		if (close && bezierPoint == null)
		{
			bezierPoint = points[points.Length - 1];
			p = points[0];
		}
		t -= num;
		return GetPoint(bezierPoint, p, t / num2);
	}

	public int GetPointIndex(BezierPoint point)
	{
		int result = -1;
		for (int i = 0; i < points.Length; i++)
		{
			if (points[i] == point)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public void SetDirty()
	{
		dirty = true;
	}

	public static void DrawCurve(BezierPoint p1, BezierPoint p2, int resolution)
	{
		int num = resolution + 1;
		float num2 = resolution;
		Vector3 from = p1.position;
		Vector3 zero = Vector3.zero;
		for (int i = 1; i < num; i++)
		{
			zero = GetPoint(p1, p2, (float)i / num2);
			Gizmos.DrawLine(from, zero);
			from = zero;
		}
	}

	public static Vector3 GetPoint(BezierPoint p1, BezierPoint p2, float t)
	{
		if (p1 != null && p2 != null)
		{
			if (p1.handle2 != Vector3.zero)
			{
				if (p2.handle1 != Vector3.zero)
				{
					return GetCubicCurvePoint(p1.position, p1.globalHandle2, p2.globalHandle1, p2.position, t);
				}
				return GetQuadraticCurvePoint(p1.position, p1.globalHandle2, p2.position, t);
			}
			if (p2.handle1 != Vector3.zero)
			{
				return GetQuadraticCurvePoint(p1.position, p2.globalHandle1, p2.position, t);
			}
			return GetLinearPoint(p1.position, p2.position, t);
		}
		return Vector3.zero;
	}

	public static Vector3 GetCubicCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
	{
		t = Mathf.Clamp01(t);
		Vector3 vector = Mathf.Pow(1f - t, 3f) * p1;
		Vector3 vector2 = 3f * Mathf.Pow(1f - t, 2f) * t * p2;
		Vector3 vector3 = 3f * (1f - t) * Mathf.Pow(t, 2f) * p3;
		Vector3 vector4 = Mathf.Pow(t, 3f) * p4;
		return vector + vector2 + vector3 + vector4;
	}

	public static Vector3 GetQuadraticCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01(t);
		Vector3 vector = Mathf.Pow(1f - t, 2f) * p1;
		Vector3 vector2 = 2f * (1f - t) * t * p2;
		Vector3 vector3 = Mathf.Pow(t, 2f) * p3;
		return vector + vector2 + vector3;
	}

	public static Vector3 GetLinearPoint(Vector3 p1, Vector3 p2, float t)
	{
		return p1 + (p2 - p1) * t;
	}

	public static Vector3 GetPoint(float t, params Vector3[] points)
	{
		t = Mathf.Clamp01(t);
		int num = points.Length - 1;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < points.Length; i++)
		{
			Vector3 vector = points[points.Length - i - 1] * ((float)BinomialCoefficient(i, num) * Mathf.Pow(t, num - i) * Mathf.Pow(1f - t, i));
			zero += vector;
		}
		return zero;
	}

	public static float ApproximateLength(BezierPoint p1, BezierPoint p2, int resolution = 10)
	{
		float num = resolution;
		float num2 = 0f;
		Vector3 vector = p1.position;
		for (int i = 0; i < resolution + 1; i++)
		{
			Vector3 point = GetPoint(p1, p2, (float)i / num);
			num2 += (point - vector).magnitude;
			vector = point;
		}
		return num2;
	}

	private static int BinomialCoefficient(int i, int n)
	{
		return Factoral(n) / (Factoral(i) * Factoral(n - i));
	}

	private static int Factoral(int i)
	{
		if (i == 0)
		{
			return 1;
		}
		int num = 1;
		while (i - 1 >= 0)
		{
			num *= i;
			i--;
		}
		return num;
	}
}
