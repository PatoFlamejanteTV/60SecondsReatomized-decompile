using RG.Parsecs.EventEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Menu;

public class PostApoTooltipController : MonoBehaviour
{
	[SerializeField]
	private Sprite _normalSprite;

	[SerializeField]
	private Sprite _postApoSprite;

	[SerializeField]
	private Image _tooltipBackgroundImage;

	[SerializeField]
	private GlobalBoolVariable _isContinueAvailableVariable;

	private void OnEnable()
	{
		_tooltipBackgroundImage.sprite = (_isContinueAvailableVariable.Value ? _postApoSprite : _normalSprite);
	}
}
