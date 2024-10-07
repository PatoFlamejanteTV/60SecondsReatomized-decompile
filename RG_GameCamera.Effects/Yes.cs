using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Effects;

public class Yes : Effect
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
		Vector3 eulerAngles = unityCamera.transform.rotation.eulerAngles;
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
		float x = eulerAngles.x - diff + num;
		diff = num;
		Vector3 euler = eulerAngles;
		euler.x = x;
		unityCamera.transform.position = currPos;
		unityCamera.transform.rotation = Quaternion.Euler(euler);
	}
}
