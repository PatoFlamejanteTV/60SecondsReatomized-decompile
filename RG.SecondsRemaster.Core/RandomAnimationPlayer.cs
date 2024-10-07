using System.Collections.Generic;
using UnityEngine;

namespace RG.SecondsRemaster.Core;

public class RandomAnimationPlayer : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private List<string> _animationTriggerNames;

	[SerializeField]
	private float _minCooldown = 10f;

	[SerializeField]
	private float _maxCooldown = 60f;

	private float _lastTriggerTime;

	private float _triggerDelay;

	private void Start()
	{
		ValidateSetup();
		_lastTriggerTime = Time.time;
		_triggerDelay = Random.Range(_minCooldown, _maxCooldown);
	}

	private void ValidateSetup()
	{
		if (_animator == null)
		{
			Debug.LogError("Animator not set up properly in " + base.gameObject.name);
		}
		if (_animationTriggerNames == null || _animationTriggerNames.Count < 1 || _animationTriggerNames.Contains(null))
		{
			Debug.LogError("Animation Trigger Names not set up properly in " + base.gameObject.name);
		}
		if (_minCooldown < 0f || _maxCooldown < 0f || _maxCooldown < _minCooldown)
		{
			Debug.LogError("Cooldown times not set up properly in " + base.gameObject.name);
		}
	}

	private void Update()
	{
		if (Time.time > _lastTriggerTime + _triggerDelay)
		{
			FireAnimationTrigger();
			_lastTriggerTime = Time.time;
			_triggerDelay = Random.Range(_minCooldown, _maxCooldown);
		}
	}

	private void FireAnimationTrigger()
	{
		int index = ((_animationTriggerNames.Count > 1) ? Random.Range(0, _animationTriggerNames.Count) : 0);
		_animator.SetTrigger(_animationTriggerNames[index]);
	}
}
