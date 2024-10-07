using I2.Loc;
using RG.Parsecs.EventEditor;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class QualitySettingController : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _valueField;

	[SerializeField]
	private GlobalIntVariable _valueVariable;

	[SerializeField]
	private int _maxQualityLevel = 3;

	[SerializeField]
	private LocalizedString[] _qualityNames;

	[SerializeField]
	private bool _applyInstantly;

	private const int MIN_QUALITY_LEVEL = 0;

	private void OnEnable()
	{
		if (_valueVariable.Value < 0)
		{
			_valueVariable.Value = QualitySettings.GetQualityLevel();
		}
		_valueField.text = _qualityNames[_valueVariable.Value];
		LocalizationManager.OnLocalizeEvent += TranslateText;
	}

	private void OnDisable()
	{
		LocalizationManager.OnLocalizeEvent -= TranslateText;
	}

	private void TranslateText()
	{
		_valueField.text = _qualityNames[_valueVariable.Value];
	}

	public void SetNext()
	{
		if (_valueVariable.Value + 1 > _maxQualityLevel)
		{
			QualitySettings.SetQualityLevel(0, _applyInstantly);
		}
		else
		{
			QualitySettings.IncreaseLevel(_applyInstantly);
		}
		_valueVariable.Value = QualitySettings.GetQualityLevel();
		_valueField.text = _qualityNames[_valueVariable.Value];
	}

	public void SetPrevious()
	{
		if (_valueVariable.Value - 1 < 0)
		{
			QualitySettings.SetQualityLevel(_maxQualityLevel, _applyInstantly);
		}
		else
		{
			QualitySettings.DecreaseLevel(_applyInstantly);
		}
		_valueVariable.Value = QualitySettings.GetQualityLevel();
		_valueField.text = _qualityNames[_valueVariable.Value];
	}
}
