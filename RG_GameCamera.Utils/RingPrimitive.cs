using System;
using UnityEngine;

namespace RG_GameCamera.Utils;

public class RingPrimitive
{
	public static GameObject Create(float radiusA, float radiusB, float thickness, int segments, Color color)
	{
		GameObject gameObject = new GameObject("DebugRing");
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("VertexLit"))
		{
			color = color
		};
		if (meshFilter.sharedMesh == null)
		{
			meshFilter.sharedMesh = new Mesh();
		}
		GenerateGeometry(meshFilter.sharedMesh, radiusA, radiusB, thickness, segments);
		gameObject.transform.Rotate(Vector3.right, 180f);
		return gameObject;
	}

	public static void Generate(GameObject obj, float radiusA, float radiusB, float thickness, int segments)
	{
		GenerateGeometry(obj.GetComponent<MeshFilter>().sharedMesh, radiusA, radiusB, thickness, segments);
	}

	private static void GenerateGeometry(Mesh mesh, float radiusA, float radiusB, float thickness, int segments)
	{
		radiusA = Mathf.Clamp(radiusA, 0f, 100f);
		radiusB = Mathf.Clamp(radiusB, 0f, 100f);
		thickness = Mathf.Clamp(thickness, 0f, 100f);
		segments = Mathf.Clamp(segments, 3, 100);
		mesh.Clear();
		int num = segments * 2;
		int num2 = segments * 2;
		if (num > 60000)
		{
			UnityEngine.Debug.LogError("Too much vertices!");
		}
		Vector3[] array = new Vector3[num];
		Vector3[] array2 = new Vector3[num];
		Vector2[] array3 = new Vector2[num];
		int[] array4 = new int[num2 * 3];
		int num3 = 0;
		for (int i = 0; i < segments; i++)
		{
			float f = (float)i / (float)segments * (float)System.Math.PI * 2f;
			Vector3 vector = new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f));
			float num4 = 0.5f * (radiusA / radiusB);
			Vector2 vector2 = new Vector2(vector.x * 0.5f, vector.z * 0.5f);
			Vector2 vector3 = new Vector2(vector.x * num4, vector.z * num4);
			Vector2 vector4 = new Vector2(0.5f, 0.5f);
			array[num3] = new Vector3(vector.x * radiusA, 0f, vector.z * radiusB);
			array2[num3] = new Vector3(0f, 1f, 0f);
			array3[num3] = vector4 + vector3;
			array[num3 + 1] = new Vector3(vector.x * (radiusA - thickness), 0f, vector.z * (radiusB - thickness));
			array2[num3 + 1] = new Vector3(0f, 1f, 0f);
			array3[num3 + 1] = vector4 + vector2;
			num3 += 2;
		}
		int num5 = 0;
		int num6 = 0;
		for (int j = 0; j < segments; j++)
		{
			array4[num6] = num5;
			array4[num6 + 1] = num5 + 1;
			array4[num6 + 2] = num5 + 3;
			array4[num6 + 3] = num5 + 2;
			array4[num6 + 4] = num5;
			array4[num6 + 5] = num5 + 3;
			if (j == segments - 1)
			{
				array4[num6 + 2] = 1;
				array4[num6 + 3] = 0;
				array4[num6 + 5] = 1;
			}
			num5 += 2;
			num6 += 6;
		}
		for (int k = 0; k < array2.Length; k++)
		{
			array2[k] = -array2[k];
		}
		for (int l = 0; l < array4.Length; l += 3)
		{
			int num7 = array4[l];
			array4[l] = array4[l + 1];
			array4[l + 1] = num7;
		}
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.uv = array3;
		mesh.triangles = array4;
		mesh.RecalculateBounds();
	}
}
