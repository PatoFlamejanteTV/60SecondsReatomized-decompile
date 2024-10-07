using RG_GameCamera.Config;
using UnityEngine;

namespace RG_GameCamera.Modes;

[RequireComponent(typeof(EmptyConfig))]
public class EmptyCameraMode : CameraMode
{
	public override Type Type => Type.None;

	public override void Init()
	{
		base.Init();
		UnityCamera.transform.LookAt(cameraTarget);
		config = GetComponent<EmptyConfig>();
	}
}
