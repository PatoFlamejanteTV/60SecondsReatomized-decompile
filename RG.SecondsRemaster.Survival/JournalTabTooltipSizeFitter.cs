using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class JournalTabTooltipSizeFitter : MonoBehaviour
{
	[SerializeField]
	private float _preferredWidth;

	[SerializeField]
	private LayoutElement _layoutElement;

	[SerializeField]
	private RectTransform _rectTransform;

	[SerializeField]
	private CanvasGroup _canvasGroup;

	private bool _isRefreshing = true;

	private const float PREFERRED_WIDTH_DISABLED_VALUE = -1f;

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
			if (_rectTransform.sizeDelta.x > _preferredWidth)
			{
				_layoutElement.preferredWidth = _preferredWidth;
			}
			else
			{
				_layoutElement.preferredWidth = -1f;
			}
			_isRefreshing = false;
			_canvasGroup.alpha = 1f;
			Canvas.ForceUpdateCanvases();
		}
	}
}
