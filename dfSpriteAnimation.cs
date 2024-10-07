using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

[Serializable]
[AddComponentMenu("Daikon Forge/Tweens/Sprite Animator")]
public class dfSpriteAnimation : dfTweenPlayableBase
{
	[SerializeField]
	private string animationName = "ANIMATION";

	[SerializeField]
	private dfAnimationClip clip;

	[SerializeField]
	private dfComponentMemberInfo memberInfo = new dfComponentMemberInfo();

	[SerializeField]
	private dfTweenLoopType loopType = dfTweenLoopType.Loop;

	[SerializeField]
	private float length = 1f;

	[SerializeField]
	private bool autoStart;

	[SerializeField]
	private bool skipToEndOnStop;

	[SerializeField]
	private dfPlayDirection playDirection;

	private bool autoRunStarted;

	private bool isRunning;

	private bool isPaused;

	private dfObservableProperty target;

	public dfAnimationClip Clip
	{
		get
		{
			return clip;
		}
		set
		{
			clip = value;
		}
	}

	public dfComponentMemberInfo Target
	{
		get
		{
			return memberInfo;
		}
		set
		{
			memberInfo = value;
		}
	}

	public bool AutoRun
	{
		get
		{
			return autoStart;
		}
		set
		{
			autoStart = value;
		}
	}

	public float Length
	{
		get
		{
			return length;
		}
		set
		{
			length = Mathf.Max(value, 0.03f);
		}
	}

	public dfTweenLoopType LoopType
	{
		get
		{
			return loopType;
		}
		set
		{
			loopType = value;
		}
	}

	public dfPlayDirection Direction
	{
		get
		{
			return playDirection;
		}
		set
		{
			playDirection = value;
			if (IsPlaying)
			{
				Play();
			}
		}
	}

	public bool IsPaused
	{
		get
		{
			if (isRunning)
			{
				return isPaused;
			}
			return false;
		}
		set
		{
			if (value != IsPaused)
			{
				if (value)
				{
					Pause();
				}
				else
				{
					Resume();
				}
			}
		}
	}

	public override bool IsPlaying => isRunning;

	public override string TweenName
	{
		get
		{
			return animationName;
		}
		set
		{
			animationName = value;
		}
	}

	public event TweenNotification AnimationStarted;

	public event TweenNotification AnimationStopped;

	public event TweenNotification AnimationPaused;

	public event TweenNotification AnimationResumed;

	public event TweenNotification AnimationReset;

	public event TweenNotification AnimationCompleted;

	public void Awake()
	{
	}

	public void Start()
	{
	}

	public void LateUpdate()
	{
		if (AutoRun && !IsPlaying && !autoRunStarted)
		{
			autoRunStarted = true;
			Play();
		}
	}

	public void PlayForward()
	{
		playDirection = dfPlayDirection.Forward;
		Play();
	}

	public void PlayReverse()
	{
		playDirection = dfPlayDirection.Reverse;
		Play();
	}

	public void Pause()
	{
		if (isRunning)
		{
			isPaused = true;
			onPaused();
		}
	}

	public void Resume()
	{
		if (isRunning && isPaused)
		{
			isPaused = false;
			onResumed();
		}
	}

	public override void Play()
	{
		if (IsPlaying)
		{
			Stop();
		}
		if (base.enabled && base.gameObject.activeSelf && base.gameObject.activeInHierarchy)
		{
			if (memberInfo == null)
			{
				throw new NullReferenceException("Animation target is NULL");
			}
			if (!memberInfo.IsValid)
			{
				throw new InvalidOperationException("Invalid property binding configuration on " + getPath(base.gameObject.transform) + " - " + target);
			}
			target = memberInfo.GetProperty();
			StartCoroutine(Execute());
		}
	}

	public override void Reset()
	{
		List<string> list = ((clip != null) ? clip.Sprites : null);
		if (memberInfo.IsValid && list != null && list.Count > 0)
		{
			SetProperty(memberInfo.Component, memberInfo.MemberName, list[0]);
		}
		if (isRunning)
		{
			StopAllCoroutines();
			isRunning = false;
			isPaused = false;
			onReset();
			target = null;
		}
	}

	public override void Stop()
	{
		if (isRunning)
		{
			List<string> list = ((clip != null) ? clip.Sprites : null);
			if (skipToEndOnStop && list != null)
			{
				setFrame(Mathf.Max(list.Count - 1, 0));
			}
			StopAllCoroutines();
			isRunning = false;
			isPaused = false;
			onStopped();
			target = null;
		}
	}

	protected void onPaused()
	{
		SendMessage("AnimationPaused", this, SendMessageOptions.DontRequireReceiver);
		if (this.AnimationPaused != null)
		{
			this.AnimationPaused(this);
		}
	}

	protected void onResumed()
	{
		SendMessage("AnimationResumed", this, SendMessageOptions.DontRequireReceiver);
		if (this.AnimationResumed != null)
		{
			this.AnimationResumed(this);
		}
	}

	protected void onStarted()
	{
		SendMessage("AnimationStarted", this, SendMessageOptions.DontRequireReceiver);
		if (this.AnimationStarted != null)
		{
			this.AnimationStarted(this);
		}
	}

	protected void onStopped()
	{
		SendMessage("AnimationStopped", this, SendMessageOptions.DontRequireReceiver);
		if (this.AnimationStopped != null)
		{
			this.AnimationStopped(this);
		}
	}

	protected void onReset()
	{
		SendMessage("AnimationReset", this, SendMessageOptions.DontRequireReceiver);
		if (this.AnimationReset != null)
		{
			this.AnimationReset(this);
		}
	}

	protected void onCompleted()
	{
		SendMessage("AnimationCompleted", this, SendMessageOptions.DontRequireReceiver);
		if (this.AnimationCompleted != null)
		{
			this.AnimationCompleted(this);
		}
	}

	internal static void SetProperty(object target, string property, object value)
	{
		if (target == null)
		{
			throw new NullReferenceException("Target is null");
		}
		MemberInfo[] member = target.GetType().GetMember(property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (member == null || member.Length == 0)
		{
			throw new IndexOutOfRangeException("Property not found: " + property);
		}
		MemberInfo memberInfo = member[0];
		if (memberInfo is FieldInfo)
		{
			((FieldInfo)memberInfo).SetValue(target, value);
			return;
		}
		if (memberInfo is PropertyInfo)
		{
			((PropertyInfo)memberInfo).SetValue(target, value, null);
			return;
		}
		throw new InvalidOperationException("Member type not supported: " + memberInfo.GetMemberType());
	}

	private IEnumerator Execute()
	{
		if (clip == null || clip.Sprites == null || clip.Sprites.Count == 0)
		{
			yield break;
		}
		isRunning = true;
		isPaused = false;
		onStarted();
		float startTime = Time.realtimeSinceStartup;
		int direction = ((playDirection == dfPlayDirection.Forward) ? 1 : (-1));
		int lastFrameIndex = ((direction != 1) ? (clip.Sprites.Count - 1) : 0);
		setFrame(lastFrameIndex);
		while (true)
		{
			yield return null;
			if (IsPaused)
			{
				continue;
			}
			int num = clip.Sprites.Count - 1;
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float num2 = realtimeSinceStartup - startTime;
			int num3 = Mathf.RoundToInt(Mathf.Clamp01(num2 / length) * (float)num);
			if (num2 >= length)
			{
				switch (loopType)
				{
				case dfTweenLoopType.Once:
					isRunning = false;
					onCompleted();
					yield break;
				case dfTweenLoopType.Loop:
					startTime = realtimeSinceStartup;
					num3 = 0;
					break;
				case dfTweenLoopType.PingPong:
					startTime = realtimeSinceStartup;
					direction *= -1;
					num3 = 0;
					break;
				}
			}
			if (direction == -1)
			{
				num3 = num - num3;
			}
			if (lastFrameIndex != num3)
			{
				lastFrameIndex = num3;
				setFrame(num3);
			}
		}
	}

	private string getPath(Transform obj)
	{
		StringBuilder stringBuilder = new StringBuilder();
		while (obj != null)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Insert(0, "\\");
				stringBuilder.Insert(0, obj.name);
			}
			else
			{
				stringBuilder.Append(obj.name);
			}
			obj = obj.parent;
		}
		return stringBuilder.ToString();
	}

	private void setFrame(int frameIndex)
	{
		List<string> sprites = clip.Sprites;
		if (sprites.Count != 0)
		{
			frameIndex = Mathf.Max(0, Mathf.Min(frameIndex, sprites.Count - 1));
			if (target != null)
			{
				target.Value = sprites[frameIndex];
			}
		}
	}
}
