using System;
using UnityEngine;

public static class dfPivotExtensions
{
	public static Vector2 AsOffset(this dfPivotPoint pivot)
	{
		return pivot switch
		{
			dfPivotPoint.TopLeft => Vector2.zero, 
			dfPivotPoint.TopCenter => new Vector2(0.5f, 0f), 
			dfPivotPoint.TopRight => new Vector2(1f, 0f), 
			dfPivotPoint.MiddleLeft => new Vector2(0f, 0.5f), 
			dfPivotPoint.MiddleCenter => new Vector2(0.5f, 0.5f), 
			dfPivotPoint.MiddleRight => new Vector2(1f, 0.5f), 
			dfPivotPoint.BottomLeft => new Vector2(0f, 1f), 
			dfPivotPoint.BottomCenter => new Vector2(0.5f, 1f), 
			dfPivotPoint.BottomRight => new Vector2(1f, 1f), 
			_ => Vector2.zero, 
		};
	}

	public static Vector3 TransformToCenter(this dfPivotPoint pivot, Vector2 size)
	{
		return pivot switch
		{
			dfPivotPoint.TopLeft => new Vector2(0.5f * size.x, 0.5f * (0f - size.y)), 
			dfPivotPoint.TopCenter => new Vector2(0f, 0.5f * (0f - size.y)), 
			dfPivotPoint.TopRight => new Vector2(0.5f * (0f - size.x), 0.5f * (0f - size.y)), 
			dfPivotPoint.MiddleLeft => new Vector2(0.5f * size.x, 0f), 
			dfPivotPoint.MiddleCenter => new Vector2(0f, 0f), 
			dfPivotPoint.MiddleRight => new Vector2(0.5f * (0f - size.x), 0f), 
			dfPivotPoint.BottomLeft => new Vector2(0.5f * size.x, 0.5f * size.y), 
			dfPivotPoint.BottomCenter => new Vector2(0f, 0.5f * size.y), 
			dfPivotPoint.BottomRight => new Vector2(0.5f * (0f - size.x), 0.5f * size.y), 
			_ => throw new Exception("Unhandled " + pivot.GetType().Name + " value: " + pivot), 
		};
	}

	public static Vector3 UpperLeftToTransform(this dfPivotPoint pivot, Vector2 size)
	{
		return pivot.TransformToUpperLeft(size).Scale(-1f, -1f, 1f);
	}

	public static Vector3 TransformToUpperLeft(this dfPivotPoint pivot, Vector2 size)
	{
		return pivot switch
		{
			dfPivotPoint.TopLeft => new Vector2(0f, 0f), 
			dfPivotPoint.TopCenter => new Vector2(0.5f * (0f - size.x), 0f), 
			dfPivotPoint.TopRight => new Vector2(0f - size.x, 0f), 
			dfPivotPoint.MiddleLeft => new Vector2(0f, 0.5f * size.y), 
			dfPivotPoint.MiddleCenter => new Vector2(0.5f * (0f - size.x), 0.5f * size.y), 
			dfPivotPoint.MiddleRight => new Vector2(0f - size.x, 0.5f * size.y), 
			dfPivotPoint.BottomLeft => new Vector2(0f, size.y), 
			dfPivotPoint.BottomCenter => new Vector2(0.5f * (0f - size.x), size.y), 
			dfPivotPoint.BottomRight => new Vector2(0f - size.x, size.y), 
			_ => throw new Exception("Unhandled " + pivot.GetType().Name + " value: " + pivot), 
		};
	}
}
