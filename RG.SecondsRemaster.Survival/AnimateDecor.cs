using System.Collections;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class AnimateDecor : MonoBehaviour
{
	[SerializeField]
	private FlightManager _flightManager;

	[SerializeField]
	private FlickerLights _flickerLights;

	[SerializeField]
	private EndGameData _endGameData;

	[Range(1f, float.MaxValue)]
	[SerializeField]
	private float _minTime = 5f;

	[Range(1f, float.MaxValue)]
	[SerializeField]
	private float _maxTIme = 25f;

	[SerializeField]
	private EndOfDayListenerList _endOfDay;

	private bool _animationEnabled = true;

	private void Start()
	{
		StartCoroutine(RunAnimations());
		_endOfDay.RegisterOnEndOfDay(DisableAnimations, "EndGame", 3, this);
		_endOfDay.RegisterOnDayStarted(EnableAnimations, "ChangeFinished", 1);
	}

	private void OnDestroy()
	{
		_endOfDay.UnregisterOnEndOfDay(DisableAnimations, "EndGame");
		_endOfDay.UnregisterOnDayStarted(EnableAnimations, "ChangeFinished");
	}

	private void DisableAnimations()
	{
		_animationEnabled = false;
	}

	private void EnableAnimations()
	{
		_animationEnabled = true;
	}

	private IEnumerator RunAnimations()
	{
		while (!_endGameData.RuntimeData.ShouldEndGame)
		{
			yield return new WaitForSeconds(Random.Range(_minTime, _maxTIme));
			if (!_animationEnabled)
			{
				continue;
			}
			if (Random.Range(0, 100) < 30)
			{
				if (_flightManager.gameObject.activeInHierarchy)
				{
					_flightManager.StartAnimation();
				}
			}
			else
			{
				_flickerLights.StartFlicking();
			}
		}
	}
}
