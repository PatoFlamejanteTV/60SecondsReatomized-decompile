using UnityEngine;

public static class dfRectExtensions
{
	public static RectOffset ConstrainPadding(this RectOffset borders)
	{
		if (borders == null)
		{
			return new RectOffset();
		}
		borders.left = Mathf.Max(0, borders.left);
		borders.right = Mathf.Max(0, borders.right);
		borders.top = Mathf.Max(0, borders.top);
		borders.bottom = Mathf.Max(0, borders.bottom);
		return borders;
	}

	public static bool IsEmpty(this Rect rect)
	{
		if (rect.xMin != rect.xMax)
		{
			return rect.yMin == rect.yMax;
		}
		return true;
	}

	public static Rect Intersection(this Rect a, Rect b)
	{
		if (!a.Intersects(b))
		{
			return default(Rect);
		}
		float xmin = Mathf.Max(a.xMin, b.xMin);
		float xmax = Mathf.Min(a.xMax, b.xMax);
		float ymin = Mathf.Max(a.yMin, b.yMin);
		float ymax = Mathf.Min(a.yMax, b.yMax);
		return Rect.MinMaxRect(xmin, ymin, xmax, ymax);
	}

	public static Rect Union(this Rect a, Rect b)
	{
		float xmin = Mathf.Min(a.xMin, b.xMin);
		float xmax = Mathf.Max(a.xMax, b.xMax);
		float ymin = Mathf.Min(a.yMin, b.yMin);
		float ymax = Mathf.Max(a.yMax, b.yMax);
		return Rect.MinMaxRect(xmin, ymin, xmax, ymax);
	}

	public static bool Contains(this Rect rect, Rect other)
	{
		bool num = rect.x <= other.x;
		bool flag = rect.x + rect.width >= other.x + other.width;
		bool flag2 = rect.yMin <= other.yMin;
		bool flag3 = rect.y + rect.height >= other.y + other.height;
		return num && flag && flag2 && flag3;
	}

	public static bool Intersects(this Rect rect, Rect other)
	{
		return !(rect.xMax < other.xMin) && !(rect.yMax < other.yMin) && !(rect.xMin > other.xMax) && !(rect.yMin > other.yMax);
	}

	public static Rect RoundToInt(this Rect rect)
	{
		return new Rect(Mathf.RoundToInt(rect.x), Mathf.RoundToInt(rect.y), Mathf.RoundToInt(rect.width), Mathf.RoundToInt(rect.height));
	}

	public static string Debug(this Rect rect)
	{
		return $"[{rect.xMin},{rect.yMin},{rect.xMax},{rect.yMax}]";
	}
}
