using System.Collections.Generic;
using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class FlyingVisualsManager : MonoBehaviour
{
	public bool IsAnimationPlaying;

	private static readonly int _triggerHash = Animator.StringToHash("Show");

	private readonly Vector3 _regularScale = new Vector3(1f, 1f, 1f);

	private readonly Vector3 _invertedScale = new Vector3(-1f, 1f, 1f);

	[SerializeField]
	private float _minTimeBeforeFirstFlier = 20f;

	[SerializeField]
	private float _maxTimeBeforeFirstFlier = 40f;

	[SerializeField]
	private float _minTimeBeforeSubsequentFliers = 12f;

	[SerializeField]
	private float _maxTimeBeforeSubsequentFliers = 30f;

	[SerializeField]
	private List<Animator> _flyingObjects = new List<Animator>();

	[SerializeField]
	private List<bool> _canFlierBeFlipped = new List<bool>();

	private bool _fliersShouldBeDisplayed;

	private float _lastFlierDisplayTime;

	private float _currentFlierCooldown;

	[SerializeField]
	private GlobalBoolVariable _isObjectInverted;

	private void Start()
	{
		float time = Random.Range(_minTimeBeforeFirstFlier, _maxTimeBeforeFirstFlier);
		Invoke("EnableFliers", time);
	}

	private void Update()
	{
		if (!_fliersShouldBeDisplayed)
		{
			return;
		}
		if (!IsAnimationPlaying)
		{
			if (!(Time.time > _lastFlierDisplayTime + _currentFlierCooldown))
			{
				return;
			}
			_lastFlierDisplayTime = Time.time;
			_currentFlierCooldown = Random.Range(_minTimeBeforeSubsequentFliers, _maxTimeBeforeSubsequentFliers);
			int index = Random.Range(0, _flyingObjects.Count);
			if (_canFlierBeFlipped[index])
			{
				if ((double)Random.value > 0.5)
				{
					_flyingObjects[index].transform.parent.transform.localScale = _invertedScale;
					_isObjectInverted.Value = true;
				}
				else
				{
					_flyingObjects[index].transform.parent.transform.localScale = _regularScale;
					_isObjectInverted.Value = false;
				}
			}
			IsAnimationPlaying = true;
			_flyingObjects[index].SetTrigger(_triggerHash);
		}
		else
		{
			_lastFlierDisplayTime = Time.time;
		}
	}

	private void EnableFliers()
	{
		_fliersShouldBeDisplayed = true;
	}
}
