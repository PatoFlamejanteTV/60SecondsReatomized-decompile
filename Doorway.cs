using System.Collections.Generic;
using DunGen;
using UnityEngine;

[AddComponentMenu("DunGen/Doorway")]
public class Doorway : MonoBehaviour
{
	public DoorwaySocketType SocketGroup;

	public List<GameObject> DoorPrefabs = new List<GameObject>();

	public List<GameObject> AddWhenInUse = new List<GameObject>();

	public List<GameObject> AddWhenNotInUse = new List<GameObject>();

	public Vector2 Size = new Vector2(1f, 2f);

	public int? LockID;

	[SerializeField]
	[HideInInspector]
	private GameObject doorPrefab;

	internal bool placedByGenerator;

	public Tile Tile { get; internal set; }

	public bool IsLocked => LockID.HasValue;

	private void OnDrawGizmos()
	{
		if (!placedByGenerator)
		{
			DebugDraw();
		}
	}

	internal void SetUsedPrefab(GameObject doorPrefab)
	{
		this.doorPrefab = doorPrefab;
	}

	internal void RemoveUsedPrefab()
	{
		if (doorPrefab != null)
		{
			Object.DestroyImmediate(doorPrefab);
		}
	}

	internal void DebugDraw()
	{
		Vector2 vector = Size * 0.5f;
		Gizmos.color = EditorConstants.DoorDirectionColour;
		float num = Mathf.Min(Size.x, Size.y);
		Gizmos.DrawLine(base.transform.position + base.transform.up * vector.y, base.transform.position + base.transform.up * vector.y + base.transform.forward * num);
		Gizmos.color = EditorConstants.DoorRectColour;
		Vector3 vector2 = base.transform.position - base.transform.right * vector.x + base.transform.up * Size.y;
		Vector3 vector3 = base.transform.position + base.transform.right * vector.x + base.transform.up * Size.y;
		Vector3 vector4 = base.transform.position - base.transform.right * vector.x;
		Vector3 vector5 = base.transform.position + base.transform.right * vector.x;
		Gizmos.DrawLine(vector2, vector3);
		Gizmos.DrawLine(vector3, vector5);
		Gizmos.DrawLine(vector5, vector4);
		Gizmos.DrawLine(vector4, vector2);
	}
}
