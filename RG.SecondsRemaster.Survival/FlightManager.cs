using System;
using System.Collections;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class FlightManager : MonoBehaviour
{
	private enum EFlightDirection
	{
		LEFT,
		RIGHT
	}

	[SerializeField]
	private GameObject _fly;

	[SerializeField]
	private GameObject _bat;

	[SerializeField]
	private float _flightTime = 7f;

	[NonSerialized]
	private EFlightDirection _flightDirection;

	[NonSerialized]
	private GameObject _flyingObject;

	[NonSerialized]
	private string _currentPath;

	private const string PATH_RIGHT = "Right";

	private const string PATH_LEFT = "Left";

	private const string I_TWEEN_ARG_PATH = "path";

	private const string I_TWEEN_ARG_TIME = "time";

	private const string I_TWEEN_ARG_ON_COMPLETE = "oncomplete";

	private const string I_TWEEN_ARG_ON_COMPLETE_TARGET = "oncompletetarget";

	private const int DAY_OF_HALLOWEEN = 31;

	private const int MONTH_OF_HALLOWEEN = 10;

	private void Awake()
	{
		SetFlyingObject();
	}

	public void SetFlyingObject()
	{
		DateTime now = DateTime.Now;
		if (now.Day == 31 && now.Month == 10)
		{
			_flyingObject = _bat;
		}
		else
		{
			_flyingObject = _fly;
		}
	}

	private IEnumerator FlyingAnimation()
	{
		SetStartingPosition();
		SelectPath();
		AnimationLaunch();
		yield return null;
	}

	public void StartAnimation()
	{
		StartCoroutine(FlyingAnimation());
	}

	private void SelectPath()
	{
		_flightDirection = ((UnityEngine.Random.Range(0, 2) > 0) ? EFlightDirection.RIGHT : EFlightDirection.LEFT);
		if (_flightDirection == EFlightDirection.RIGHT)
		{
			_flyingObject.GetComponent<SpriteRenderer>().flipX = true;
			_currentPath = "Right" + UnityEngine.Random.Range(1, 4);
		}
		else
		{
			_flyingObject.GetComponent<SpriteRenderer>().flipX = false;
			_currentPath = "Left" + UnityEngine.Random.Range(1, 4);
		}
		if (_currentPath != null)
		{
			_flyingObject.gameObject.SetActive(value: true);
		}
	}

	private void AnimationLaunch()
	{
		iTween.MoveTo(_flyingObject, iTween.Hash("path", iTweenPath.GetPath(_currentPath), "time", _flightTime, "oncomplete", "OnPathCompleted", "oncompletetarget", base.gameObject));
	}

	private void OnPathCompleted()
	{
		_flyingObject.SetActive(value: false);
	}

	private void SetStartingPosition()
	{
		if (_flightDirection == EFlightDirection.RIGHT)
		{
			_flyingObject.transform.position = new Vector3(-100f, 0f, 0f);
		}
		else
		{
			_flyingObject.transform.position = new Vector3(100f, 0f, 0f);
		}
	}
}
