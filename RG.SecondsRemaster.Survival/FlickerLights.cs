using System;
using System.Collections;
using FMODUnity;
using RG.Parsecs.Common;
using RG.Parsecs.Menu;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class FlickerLights : MonoBehaviour
{
	[EventRef]
	[SerializeField]
	private string _flickerSound;

	[SerializeField]
	private GameObject _graphicLightOn;

	[SerializeField]
	private GameObject _graphicLightOff;

	[SerializeField]
	private SettingsSO _settings;

	[SerializeField]
	private Animator _lightsObscurerAnimator;

	[Space(25f)]
	[Tooltip("Minimum time between flickers")]
	[Range(0.001f, float.PositiveInfinity)]
	[SerializeField]
	private float _minDelay = 0.001f;

	[Tooltip("Maximum time between flickers")]
	[Range(0.001f, float.PositiveInfinity)]
	[SerializeField]
	private float _maxDelay = 0.1f;

	[Tooltip("How long lights are disable between flickering.")]
	[Range(0.01f, 1f)]
	[SerializeField]
	private float _disableTime = 0.15f;

	[Tooltip("Minimum amount of times lights will flicker when clicked")]
	[SerializeField]
	private int _minNumberOfFlickers = 1;

	[Tooltip("Maximum amount of times lights will flicker when clicked")]
	[SerializeField]
	private int _maxNumberOfFlickers = 5;

	private WaitForSeconds _flickeringDurationForSeconds;

	private bool _inProgress;

	private const string OBSCURER_ANIMATOR_BOOL = "ShowObscurer";

	private void Awake()
	{
		_flickeringDurationForSeconds = new WaitForSeconds(_disableTime);
	}

	public void StartFlicking()
	{
		if (!(_graphicLightOn == null) && !_inProgress && !AreFlashesDisabled() && base.gameObject.activeInHierarchy)
		{
			_inProgress = true;
			StartCoroutine(Flicker(UnityEngine.Random.Range(_minNumberOfFlickers, _maxNumberOfFlickers)));
		}
	}

	private bool AreFlashesDisabled()
	{
		if (_settings.runtimeData.HasKey("DisableFlashes"))
		{
			return Convert.ToBoolean(_settings.runtimeData.GetString("DisableFlashes"));
		}
		return false;
	}

	private IEnumerator Flicker(int count = 2)
	{
		for (int i = 0; i < count; i++)
		{
			_graphicLightOff.SetActive(value: true);
			_graphicLightOn.SetActive(value: false);
			StartCoroutine(PlayFlickerSound());
			_lightsObscurerAnimator.SetBool("ShowObscurer", value: true);
			yield return _flickeringDurationForSeconds;
			_graphicLightOn.SetActive(value: true);
			_graphicLightOff.SetActive(value: false);
			_lightsObscurerAnimator.SetBool("ShowObscurer", value: false);
			yield return new WaitForSeconds(UnityEngine.Random.Range(_minDelay, _maxDelay));
		}
		_inProgress = false;
	}

	private IEnumerator PlayFlickerSound()
	{
		AudioManager.PlaySound(_flickerSound);
		yield return null;
	}
}
