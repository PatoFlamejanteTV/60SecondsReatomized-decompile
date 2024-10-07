using UnityEngine;

namespace RG_GameCamera.Effects;

public abstract class Effect : MonoBehaviour
{
	protected enum FadeState
	{
		FadeIn,
		Full,
		FadeOut
	}

	public bool Loop;

	public float Length = 1f;

	public float FadeIn = 0.5f;

	public float FadeOut = 0.5f;

	protected float timeout;

	protected float timeoutNormalized;

	protected float fadeInNormalized;

	protected float fadeOutNormalized;

	protected FadeState fadeState;

	protected Camera unityCamera;

	public bool Playing { get; protected set; }

	private void Start()
	{
		if (!unityCamera)
		{
			EffectManager.Instance.Register(this);
			Init();
		}
	}

	public virtual void Init()
	{
		Playing = false;
		unityCamera = CameraManager.Instance.UnityCamera;
	}

	public void Play()
	{
		Playing = true;
		timeout = 0f;
		FadeIn = Mathf.Clamp(FadeIn, 0f, Length);
		FadeOut = Mathf.Clamp(FadeOut, 0f, Length);
		OnPlay();
	}

	public void Stop()
	{
		Playing = false;
		OnStop();
	}

	public virtual void OnPlay()
	{
	}

	public virtual void OnStop()
	{
	}

	public virtual void OnUpdate()
	{
	}

	public void PostUpdate()
	{
		timeout += Time.deltaTime;
		timeoutNormalized = Mathf.Clamp01(timeout / Length);
		fadeState = FadeState.Full;
		if (FadeIn > 0f)
		{
			if (timeout < FadeIn)
			{
				fadeInNormalized = timeout / FadeIn;
				fadeState = FadeState.FadeIn;
			}
			else
			{
				fadeInNormalized = 1f;
			}
		}
		if (FadeOut > 0f)
		{
			if (timeout > Length - FadeOut)
			{
				fadeOutNormalized = (timeout - (Length - FadeOut)) / FadeOut;
				fadeState = FadeState.FadeOut;
			}
			else
			{
				fadeOutNormalized = 0f;
			}
		}
		if (timeout > Length)
		{
			if (Loop)
			{
				Play();
			}
			else
			{
				Stop();
			}
		}
		OnUpdate();
	}

	public void Delete()
	{
		EffectManager.Instance.Delete(this);
	}
}
