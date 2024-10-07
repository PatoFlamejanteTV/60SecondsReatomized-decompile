using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Effects;

public class FireKick : Effect
{
	public float KickTime;

	public float KickAngle;

	private float diff;

	private float kickTimeout;

	public override void OnPlay()
	{
		diff = 0f;
		KickTime = Mathf.Clamp(KickTime, 0f, Length);
	}

	public override void OnUpdate()
	{
		Vector3 eulerAngles = unityCamera.transform.rotation.eulerAngles;
		float num = 0f;
		num = ((!(timeout < KickTime)) ? Interpolation.LerpS(t: (timeout - KickTime) / (Length - KickTime), a: KickAngle, b: 0f) : Interpolation.LerpS2(t: timeout / KickTime, a: 0f, b: KickAngle));
		num = 0f - num;
		float x = eulerAngles.x - diff + num;
		diff = num;
		Vector3 euler = eulerAngles;
		euler.x = x;
		unityCamera.transform.rotation = Quaternion.Euler(euler);
	}
}
