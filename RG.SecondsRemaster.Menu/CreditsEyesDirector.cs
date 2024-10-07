using System.Collections.Generic;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class CreditsEyesDirector : MonoBehaviour
{
	[SerializeField]
	private List<CreditsEyeGroup> _eyesGroups;

	[SerializeField]
	private float _refreshInterval;

	private float _lastTriggerTime;

	private CreditsEyesController[] _currentlyPlayingControllers;

	private void OnEnable()
	{
		if (_currentlyPlayingControllers == null)
		{
			_currentlyPlayingControllers = new CreditsEyesController[_eyesGroups.Count];
		}
		for (int i = 0; i < _eyesGroups.Count; i++)
		{
			_eyesGroups[i].Initialize();
		}
		AssignEyesTypeToGroups();
		StartInitialAnimations();
		_lastTriggerTime = Time.time;
	}

	private void Update()
	{
		if (!(Time.time >= _lastTriggerTime + _refreshInterval))
		{
			return;
		}
		int num = CheckIfAnimationHasFinishedAndReturnIndex();
		if (num != -1)
		{
			CreditsEyesController creditsEyesController = _currentlyPlayingControllers[num].ParentGroup.StartAnimationAtRandomPointAndReturnInstance();
			if (creditsEyesController != null)
			{
				_currentlyPlayingControllers[num] = creditsEyesController;
			}
		}
		_lastTriggerTime = Time.time;
	}

	private void StartInitialAnimations()
	{
		for (int i = 0; i < _eyesGroups.Count; i++)
		{
			CreditsEyesController creditsEyesController = _eyesGroups[i].StartAnimationAtRandomPointAndReturnInstance();
			if (creditsEyesController != null)
			{
				_currentlyPlayingControllers[i] = creditsEyesController;
			}
		}
	}

	private int CheckIfAnimationHasFinishedAndReturnIndex()
	{
		for (int i = 0; i < _currentlyPlayingControllers.Length; i++)
		{
			if (_currentlyPlayingControllers[i].IsFree)
			{
				return i;
			}
		}
		return -1;
	}

	private void AssignEyesTypeToGroups()
	{
		bool flag = Random.Range(0, 2) > 0;
		for (int i = 0; i < _eyesGroups.Count; i++)
		{
			_eyesGroups[i].GroupEyesType = (flag ? EEyesType.TED : EEyesType.DOLORES);
			flag = !flag;
		}
	}
}
