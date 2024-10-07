using System.Collections;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupLerp : MonoBehaviour
{
	private const float ALPHA_VISIBLE_VALUE = 1f;

	private const float ALPHA_INVISIBLE_VALUE = 0f;

	[SerializeField]
	[Tooltip("How fast will transition occur?")]
	private float _lerpSpeed = 5f;

	[SerializeField]
	[Tooltip("Lerping will slow down when it get's to marginal [0 and 1] values. This value will determine how fast it should be interrupted (e.g: 0.1f means that after reaching 0.9 of transparency it'll set transparency to 1)")]
	private float _lerpEpsilon = 0.1f;

	private CanvasGroup _canvasGroup;

	private void Awake()
	{
		_canvasGroup = GetComponent<CanvasGroup>();
	}

	public IEnumerator HideCanvasGroup()
	{
		while (_canvasGroup.alpha > _lerpEpsilon)
		{
			_canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 0f, _lerpSpeed * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		_canvasGroup.alpha = 0f;
		yield return null;
	}

	public IEnumerator ShowCanvasGroup()
	{
		while (_canvasGroup.alpha < 1f - _lerpEpsilon)
		{
			_canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1f, _lerpSpeed * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		_canvasGroup.alpha = 1f;
		yield return null;
	}
}
