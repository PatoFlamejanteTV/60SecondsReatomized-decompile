using FMODUnity;
using RG.Parsecs.Common;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Scavenge;

public class ClockController : MonoBehaviour
{
	[SerializeField]
	private RectTransform _watchHand;

	[SerializeField]
	private Animator _watchAnimator;

	[SerializeField]
	private Image _redFill;

	[SerializeField]
	private GameObject _alarm;

	[SerializeField]
	private GameObject _hands;

	[SerializeField]
	private Vector3 _shakeAmount = new Vector3(10f, 1f, 0f);

	[SerializeField]
	private float _shakeLength = 5f;

	[EventRef]
	[SerializeField]
	private string _tickSoundName;

	[EventRef]
	[SerializeField]
	private string _watchRingSoundName;

	private Image[] _uiImages;

	private float _alarmTime = 20f;

	private float _watchPulseTime = 40f;

	private float _scavengeTime = 60f;

	private bool _watchPulsing;

	private const string PULSE_PARAMETER_NAME = "Pulse";

	private void Awake()
	{
		_uiImages = GetComponentsInChildren<Image>();
	}

	public void Initialize(float scavengeTime, float alarmTime, float watchPulseTime)
	{
		_scavengeTime = scavengeTime;
		_watchPulseTime = watchPulseTime;
		_alarmTime = alarmTime;
		_watchPulsing = false;
		_watchAnimator.enabled = true;
	}

	public void UpdateHandPosition(float currentTimeLeft)
	{
		AudioManager.PlaySound(_tickSoundName);
		if (currentTimeLeft - _scavengeTime <= 0f)
		{
			if (!_redFill.gameObject.activeSelf)
			{
				_redFill.gameObject.SetActive(value: true);
			}
			if (!_hands.gameObject.activeSelf)
			{
				_hands.gameObject.SetActive(value: true);
			}
			_redFill.fillAmount = 1f - currentTimeLeft / _scavengeTime;
		}
		if (!_alarm.activeSelf && currentTimeLeft <= _alarmTime)
		{
			_alarm.SetActive(value: true);
		}
		if (!_watchPulsing && currentTimeLeft <= _watchPulseTime)
		{
			PulseClock(pulse: true);
		}
		float z = 0f - 360f * (1f - currentTimeLeft / _scavengeTime);
		_watchHand.localRotation = Quaternion.Euler(0f, 0f, z);
		if (currentTimeLeft <= 0f)
		{
			PulseClock(pulse: false);
			_alarm.SetActive(value: false);
			iTween.ShakePosition(base.gameObject, new Vector3(_shakeAmount.x, _shakeAmount.y, _shakeAmount.z), _shakeLength);
			AudioManager.PlaySound(_watchRingSoundName);
		}
	}

	public void ResetRedFill()
	{
		_redFill.fillAmount = 0f;
	}

	public void PulseClock(bool pulse)
	{
		_watchPulsing = pulse;
		_watchAnimator.SetBool("Pulse", pulse);
	}

	public void HideClock()
	{
		for (int i = 0; i < _uiImages.Length; i++)
		{
			_uiImages[i].color = Color.clear;
		}
	}
}
