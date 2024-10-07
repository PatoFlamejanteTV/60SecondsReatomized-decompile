using System.Collections;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class SwitchKnobController : MonoBehaviour
{
	[SerializeField]
	private RectTransform _knobTransform;

	[SerializeField]
	private float _smoothness;

	[SerializeField]
	private float _accuracy;

	[SerializeField]
	[Range(0f, 360f)]
	private float _minimumRotation;

	[SerializeField]
	[Range(0f, 360f)]
	private float _maximumRotation;

	[SerializeField]
	private Animator _staticNoiseAnimator;

	private bool _rotationOngoing;

	private const int LEFT = -1;

	private const int RIGHT = 1;

	private const string SHOW_NOISE_PARAM_NAME = "Show";

	public void SetRandomRotation()
	{
		if (!_rotationOngoing)
		{
			_staticNoiseAnimator.SetTrigger("Show");
			_rotationOngoing = true;
			int num = ((Random.Range(0, 2) <= 0) ? 1 : (-1));
			float totalRotation = Random.Range(_minimumRotation, _maximumRotation);
			StartCoroutine(Rotate(totalRotation, num));
		}
	}

	private IEnumerator Rotate(float totalRotation, float direction)
	{
		float amountRotated = 0f;
		float rotationAmount = totalRotation * _smoothness * Time.deltaTime;
		while (amountRotated + _accuracy <= totalRotation)
		{
			_knobTransform.Rotate(0f, 0f, direction * rotationAmount);
			amountRotated += rotationAmount;
			yield return new WaitForEndOfFrame();
		}
		_rotationOngoing = false;
	}
}
