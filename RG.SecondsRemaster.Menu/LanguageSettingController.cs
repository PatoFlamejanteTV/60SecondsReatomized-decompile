using System.Collections.Generic;
using I2.Loc;
using RG.SecondsRemaster.EventEditor;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class LanguageSettingController : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _valueField;

	[SerializeField]
	private GlobalStringVariable _valueVariable;

	[SerializeField]
	private List<string> _languages;

	[SerializeField]
	private List<string> _languageCodes;

	[SerializeField]
	private List<LocalizedString> _languageTerms;

	[SerializeField]
	private bool _applyInstantly;

	private int _currentIndex;

	private void OnEnable()
	{
		if (string.IsNullOrEmpty(_valueVariable.Value))
		{
			_currentIndex = _languages.IndexOf(LocalizationManager.CurrentLanguage);
		}
		else
		{
			_currentIndex = _languageCodes.IndexOf(_valueVariable.Value);
		}
		_valueField.text = _languageTerms[_currentIndex];
		_valueVariable.Value = _languageCodes[_currentIndex];
	}

	public void SetNext()
	{
		if (_currentIndex + 1 >= _languages.Count)
		{
			_currentIndex = 0;
		}
		else
		{
			_currentIndex++;
		}
		_valueField.text = _languageTerms[_currentIndex];
		_valueVariable.Value = _languageCodes[_currentIndex];
		if (_applyInstantly)
		{
			LocalizationManager.SetLanguageAndCode(_languages[_currentIndex], _languageCodes[_currentIndex]);
		}
	}

	public void SetPrevious()
	{
		if (_currentIndex - 1 < 0)
		{
			_currentIndex = _languages.Count - 1;
		}
		else
		{
			_currentIndex--;
		}
		_valueField.text = _languageTerms[_currentIndex];
		_valueVariable.Value = _languageCodes[_currentIndex];
		if (_applyInstantly)
		{
			LocalizationManager.SetLanguageAndCode(_languages[_currentIndex], _languageCodes[_currentIndex]);
		}
	}
}
