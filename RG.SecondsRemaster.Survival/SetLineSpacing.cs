using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SetLineSpacing : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _textField;

	[SerializeField]
	private LinesDescription _linesDescription;

	private void OnEnable()
	{
		if (_textField == null)
		{
			_textField = GetComponent<TextMeshProUGUI>();
		}
		_textField.lineSpacing = _linesDescription.GetLinesDescriptionForCurrentLanguage().LineSpacing;
	}
}
