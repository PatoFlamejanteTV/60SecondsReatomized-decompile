using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Scavenge;

public class SergeantSpeechController : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _textShow;

	[SerializeField]
	private TextMeshProUGUI _textHide;

	[SerializeField]
	private Animator _animator;

	private const string SHOW_TEXT_PARAM_NAME = "Show";

	private const string HIDE_TEXT_PARAM_NAME = "Hide";

	private const string SWITCH_TEXT_PARAM_NAME = "Switch";

	public void ShowText(string text)
	{
		if (!_textShow.gameObject.activeInHierarchy)
		{
			_textShow.text = text;
			_animator.SetTrigger("Show");
		}
		else
		{
			_textHide.text = _textShow.text;
			_animator.SetTrigger("Switch");
			_textShow.text = text;
		}
	}

	public void HideText()
	{
		_textHide.text = _textShow.text;
		_animator.SetTrigger("Hide");
	}
}
