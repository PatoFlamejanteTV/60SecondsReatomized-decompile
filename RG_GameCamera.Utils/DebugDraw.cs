using System;
using System.Diagnostics;
using UnityEngine;

namespace RG_GameCamera.Utils;

internal class DebugDraw : MonoBehaviour
{
	private class DbgObject
	{
		public GameObject obj;

		public int timer;
	}

	public static bool Enabled = true;

	private static DebugDraw instance;

	private GameObject debugRoot;

	private DbgObject[] dbgObjects;

	private static DebugDraw Instance
	{
		get
		{
			if (!instance)
			{
				instance = CameraInstance.CreateInstance<DebugDraw>("DebugDraw");
			}
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		debugRoot = instance.gameObject;
		dbgObjects = new DbgObject[20];
		for (int i = 0; i < dbgObjects.Length; i++)
		{
			dbgObjects[i] = new DbgObject();
		}
	}

	private void Update()
	{
		debugRoot.SetActive(Enabled);
		DbgObject[] array = dbgObjects;
		foreach (DbgObject dbgObject in array)
		{
			if ((bool)dbgObject.obj)
			{
				dbgObject.timer--;
				if ((float)dbgObject.timer < 0f)
				{
					dbgObject.obj.gameObject.SetActive(value: false);
				}
			}
		}
	}

	[Conditional("UNITY_EDITOR")]
	public static void Sphere(Vector3 pos, float scale, Color color, int time)
	{
		DebugDraw debugDraw = Instance;
		bool flag = false;
		DbgObject dbgObject = null;
		DbgObject[] array = debugDraw.dbgObjects;
		foreach (DbgObject dbgObject2 in array)
		{
			if ((bool)dbgObject2.obj && !Debug.IsActive(dbgObject2.obj))
			{
				dbgObject2.obj.SetActive(value: true);
				dbgObject2.obj.transform.position = pos;
				dbgObject2.obj.transform.localScale = new Vector3(scale, scale, scale);
				dbgObject2.timer = time;
				dbgObject2.obj.GetComponent<MeshRenderer>().material.color = color;
				flag = true;
				break;
			}
			if (!dbgObject2.obj)
			{
				dbgObject = dbgObject2;
			}
		}
		if (!flag && dbgObject != null)
		{
			dbgObject.obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			UnityEngine.Object.Destroy(dbgObject.obj.GetComponent<SphereCollider>());
			dbgObject.obj.transform.position = pos;
			dbgObject.obj.transform.parent = debugDraw.debugRoot.transform;
			dbgObject.timer = time;
			Material material = new Material(Shader.Find("VertexLit"));
			dbgObject.obj.GetComponent<MeshRenderer>().material = material;
			material.color = color;
			flag = true;
		}
		if (!flag)
		{
			Array.Sort(debugDraw.dbgObjects, (DbgObject obj0, DbgObject obj1) => obj0.timer.CompareTo(obj1.timer));
			DbgObject obj2 = debugDraw.dbgObjects[0];
			obj2.obj.SetActive(value: true);
			obj2.obj.transform.position = pos;
			obj2.obj.transform.localScale = new Vector3(scale, scale, scale);
			obj2.timer = time;
			obj2.obj.GetComponent<MeshRenderer>().material.color = color;
		}
	}
}
