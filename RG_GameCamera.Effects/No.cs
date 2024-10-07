using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Effects;

public class No : Effect
{
	public float Angle = 1f;

	public float Speed = 10f;

	private float diff;

	private float size;

	private Vector3 origPos;

	private Vector3 currPos;

	public override void OnPlay()
	{
		diff = 0f;
		origPos = unityCamera.transform.position;
	}

	public override void OnUpdate()
	{
		Vector3 localEulerAngles = unityCamera.transform.localEulerAngles;
		switch (fadeState)
		{
		case FadeState.FadeIn:
			size = Interpolation.LerpS3(0f, Angle, 1f - fadeInNormalized);
			currPos = origPos;
			break;
		case FadeState.FadeOut:
			size = Interpolation.LerpS2(Angle, 0f, fadeOutNormalized);
			currPos = Interpolation.LerpS2(origPos, unityCamera.transform.position, fadeOutNormalized);
			break;
		case FadeState.Full:
			size = Angle;
			currPos = origPos;
			break;
		}
		float num = Mathf.Sin(timeout * Speed) * size;
		float y = localEulerAngles.y - diff + num;
		diff = num;
		Vector3 euler = localEulerAngles;
		euler.y = y;
		unityCamera.transform.position = currPos;
		unityCamera.transform.localRotation = Quaternion.Euler(euler);
	}
}
