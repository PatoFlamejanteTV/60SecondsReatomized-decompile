using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Effects;

public class Stomp : Effect
{
	public float Mass;

	public float Distance;

	public float Strength;

	public float Damping;

	private Spring spring;

	public override void Init()
	{
		base.Init();
		spring = new Spring();
	}

	public override void OnPlay()
	{
		spring.Setup(Mass, Distance, Strength, Damping);
	}

	public override void OnUpdate()
	{
		float num = spring.Calculate(Time.deltaTime);
		float num2 = 1f;
		switch (fadeState)
		{
		case FadeState.FadeIn:
			num2 = Interpolation.LerpS3(0f, num, 1f - fadeInNormalized);
			break;
		case FadeState.FadeOut:
			num2 = Interpolation.LerpS2(num, 0f, fadeOutNormalized);
			break;
		}
		unityCamera.transform.position += Vector3.up * num * num2;
	}
}
