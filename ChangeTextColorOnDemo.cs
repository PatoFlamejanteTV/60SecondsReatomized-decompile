using RG.Parsecs.EventEditor;
using TMPro;
using UnityEngine;

public class ChangeTextColorOnDemo : MonoBehaviour
{
	[SerializeField]
	private GlobalBoolVariable _isDemoVariable;

	[SerializeField]
	private TextMeshProUGUI _textToChangeColorOnDemo;

	[SerializeField]
	private Color _demoColor;

	[SerializeField]
	private Color _fullVersionColor;

	private void Start()
	{
		if (_isDemoVariable != null && _textToChangeColorOnDemo != null)
		{
			_textToChangeColorOnDemo.color = (_isDemoVariable.Value ? _demoColor : _fullVersionColor);
		}
	}
}
