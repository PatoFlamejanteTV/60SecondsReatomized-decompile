using System;
using UnityEngine;

[Serializable]
public class Localization
{
	[SerializeField]
	protected TextAsset _bookAsset;

	protected dfLanguageManager _localLanguageManager;

	public bool IsAvailable
	{
		get
		{
			if (_localLanguageManager != null)
			{
				return _bookAsset != null;
			}
			return false;
		}
	}

	public TextAsset Book => _bookAsset;

	public void Bind(string langCode)
	{
		dfLanguageManager dfLanguageManager2 = UnityEngine.Object.FindObjectOfType<dfLanguageManager>();
		if (dfLanguageManager2 == null)
		{
			dfGUIManager dfGUIManager2 = UnityEngine.Object.FindObjectOfType<dfGUIManager>();
			if (dfGUIManager2 != null)
			{
				dfLanguageManager2 = dfGUIManager2.gameObject.AddComponent<dfLanguageManager>();
			}
		}
		if (dfLanguageManager2 != null)
		{
			_localLanguageManager = dfLanguageManager2;
			dfLanguageManager2.DataFile = _bookAsset;
			SetLanguage(langCode);
		}
	}

	public void SetLanguage(string code)
	{
		if (_localLanguageManager != null)
		{
			dfLanguageCode language = (dfLanguageCode)Enum.Parse(typeof(dfLanguageCode), code, ignoreCase: true);
			_localLanguageManager.LoadLanguage(language);
		}
	}

	public string GetValue(string key)
	{
		if (_localLanguageManager != null)
		{
			string value = _localLanguageManager.GetValue(key);
			if (string.IsNullOrEmpty(value))
			{
				return key;
			}
			return value;
		}
		return key;
	}
}
