using System;
using I2.Loc;
using RG.Core.Base;
using UnityEngine;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/Lines Description")]
public class LinesDescription : RGScriptableObject
{
	[Serializable]
	public struct LineDescription
	{
		public int LineHeight;

		public int FirstLineHeight;

		public float LineSpacing;

		public float TooltipLayoutSpacing;

		public float AchievementTooltipLayoutSpacing;

		public float TextMargin;

		public string Language;

		public float ActionPageTextSize;

		public float ReportPageTextSize;
	}

	[SerializeField]
	private LineDescription[] _linesDescriptions;

	[SerializeField]
	private LineDescription _fallbackLineDescription;

	[SerializeField]
	private string _currentLanguage;

	private LineDescription _currentLineDescription;

	public LineDescription GetLinesDescriptionForCurrentLanguage()
	{
		string currentLanguage = LocalizationManager.CurrentLanguage;
		if (string.IsNullOrEmpty(_currentLanguage) || !_currentLanguage.Equals(currentLanguage))
		{
			_currentLanguage = currentLanguage;
			bool flag = false;
			for (int i = 0; i < _linesDescriptions.Length; i++)
			{
				if (_linesDescriptions[i].Language.Equals(_currentLanguage))
				{
					_currentLineDescription = _linesDescriptions[i];
					flag = true;
				}
			}
			if (!flag)
			{
				_currentLineDescription = _fallbackLineDescription;
			}
		}
		return _currentLineDescription;
	}

	private void OnEnable()
	{
		_currentLanguage = null;
	}
}
