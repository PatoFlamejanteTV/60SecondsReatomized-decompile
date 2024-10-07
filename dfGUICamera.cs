using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Daikon Forge/User Interface/GUI Camera")]
public class dfGUICamera : MonoBehaviour
{
	public void Awake()
	{
	}

	public void OnEnable()
	{
	}

	public void Start()
	{
		Camera component = GetComponent<Camera>();
		if (component.orthographicSize <= 0.01f)
		{
			component.orthographicSize = 1f;
		}
		component.transparencySortMode = TransparencySortMode.Orthographic;
		component.useOcclusionCulling = false;
		GetComponent<Camera>().eventMask &= ~GetComponent<Camera>().cullingMask;
	}
}
