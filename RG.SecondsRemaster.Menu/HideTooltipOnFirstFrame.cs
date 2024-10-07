using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class HideTooltipOnFirstFrame : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup _canvasGroup;

	private bool _isRefreshing;

	private void OnEnable()
	{
		_isRefreshing = true;
		_canvasGroup.alpha = 0f;
	}

	private void Update()
	{
		if (_isRefreshing)
		{
			Canvas.ForceUpdateCanvases();
			_canvasGroup.alpha = 1f;
			_isRefreshing = false;
		}
	}
}
