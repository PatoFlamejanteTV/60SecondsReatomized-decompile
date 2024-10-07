using System.Collections;
using FMOD.Studio;
using FMODUnity;
using RG.Parsecs.Common;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class FanRotation : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private AnimationCurve _lerpCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	[SerializeField]
	private float _lerpSpeed = 1f;

	[Tooltip("Delay after we can start/stop fan again (in seconds)")]
	[SerializeField]
	private float _delayBetweenOperations = 10f;

	private readonly WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();

	private const string ROTATION_SPPED_CHANGE = "SpeedRotation";

	private bool _animationInProgress;

	private float _animationTime;

	[EventRef]
	[SerializeField]
	private string _fanEvent;

	private EventInstance _eventInstance;

	private void Start()
	{
		PlaySound();
	}

	private void OnMouseDown()
	{
	}

	private void OnMouseUp()
	{
	}

	private void OnMouseOver()
	{
	}

	private void OnMouseEnter()
	{
	}

	private void OnMouseExit()
	{
	}

	private void OnMouseUpAsButton()
	{
		if (!_animationInProgress)
		{
			_animationTime = 0f;
			_animationInProgress = true;
			if (Mathf.Approximately(_animator.GetFloat("SpeedRotation"), 0f))
			{
				PlaySound();
				StartCoroutine(ChangeRotation());
			}
			else
			{
				StopSound();
				StartCoroutine(ChangeRotation(stop: true));
			}
		}
	}

	private void PlaySound()
	{
		_eventInstance = AudioManager.PlaySoundAndReturnInstance(_fanEvent);
	}

	private void StopSound()
	{
		_eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	private IEnumerator ChangeRotation(bool stop = false)
	{
		float min = (stop ? 1 : 0);
		float max = ((!stop) ? 1 : 0);
		while (true)
		{
			_animationTime += Time.deltaTime * _lerpSpeed;
			float value = Mathf.Lerp(min, max, _lerpCurve.Evaluate(_animationTime));
			_animator.SetFloat("SpeedRotation", value);
			if (Mathf.Approximately(_animator.GetFloat("SpeedRotation"), max))
			{
				break;
			}
			yield return _endOfFrame;
		}
		_animator.SetFloat("SpeedRotation", max);
		yield return new WaitForSeconds(_delayBetweenOperations);
		_animationInProgress = false;
	}
}
