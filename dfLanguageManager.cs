using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class dfLanguageManager : MonoBehaviour
{
	[SerializeField]
	private dfLanguageCode currentLanguage;

	[SerializeField]
	private TextAsset dataFile;

	private Dictionary<string, string> strings = new Dictionary<string, string>();

	public dfLanguageCode CurrentLanguage => currentLanguage;

	public TextAsset DataFile
	{
		get
		{
			return dataFile;
		}
		set
		{
			if (value != dataFile)
			{
				dataFile = value;
				LoadLanguage(currentLanguage);
			}
		}
	}

	public void Start()
	{
		dfLanguageCode language = currentLanguage;
		if (currentLanguage == dfLanguageCode.None)
		{
			language = SystemLanguageToLanguageCode(Application.systemLanguage);
		}
		LoadLanguage(language);
	}

	public void LoadLanguage(dfLanguageCode language)
	{
		currentLanguage = language;
		strings.Clear();
		if (dataFile != null)
		{
			parseDataFile();
		}
		dfControl[] componentsInChildren = GetComponentsInChildren<dfControl>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Localize();
		}
	}

	public string GetValue(string key)
	{
		string value = string.Empty;
		if (strings.TryGetValue(key, out value))
		{
			return value;
		}
		return key;
	}

	private void parseDataFile()
	{
		string text = dataFile.text.Replace("\r\n", "\n").Trim();
		List<string> list = new List<string>();
		int num = parseLine(text, list, 0);
		int num2 = list.IndexOf(currentLanguage.ToString());
		if (num2 < 0)
		{
			return;
		}
		List<string> list2 = new List<string>();
		while (num < text.Length)
		{
			num = parseLine(text, list2, num);
			if (list2.Count != 0)
			{
				string key = list2[0];
				string value = ((num2 < list2.Count) ? list2[num2] : "");
				strings[key] = value;
			}
		}
	}

	private int parseLine(string data, List<string> values, int index)
	{
		values.Clear();
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder(256);
		for (; index < data.Length; index++)
		{
			char c = data[index];
			switch (c)
			{
			case '"':
				if (!flag)
				{
					flag = true;
				}
				else if (index + 1 < data.Length && data[index + 1] == c)
				{
					index++;
					stringBuilder.Append(c);
				}
				else
				{
					flag = false;
				}
				continue;
			case ',':
				if (flag)
				{
					stringBuilder.Append(c);
					continue;
				}
				values.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
				continue;
			case '\n':
				if (flag)
				{
					stringBuilder.Append(c);
					continue;
				}
				break;
			default:
				stringBuilder.Append(c);
				continue;
			}
			index++;
			break;
		}
		if (stringBuilder.Length > 0)
		{
			values.Add(stringBuilder.ToString());
		}
		return index;
	}

	private dfLanguageCode SystemLanguageToLanguageCode(SystemLanguage language)
	{
		return language switch
		{
			SystemLanguage.Afrikaans => dfLanguageCode.AF, 
			SystemLanguage.Arabic => dfLanguageCode.AR, 
			SystemLanguage.Basque => dfLanguageCode.EU, 
			SystemLanguage.Belarusian => dfLanguageCode.BE, 
			SystemLanguage.Bulgarian => dfLanguageCode.BG, 
			SystemLanguage.Catalan => dfLanguageCode.CA, 
			SystemLanguage.Chinese => dfLanguageCode.ZH, 
			SystemLanguage.Czech => dfLanguageCode.CS, 
			SystemLanguage.Danish => dfLanguageCode.DA, 
			SystemLanguage.Dutch => dfLanguageCode.NL, 
			SystemLanguage.English => dfLanguageCode.EN, 
			SystemLanguage.Estonian => dfLanguageCode.ES, 
			SystemLanguage.Faroese => dfLanguageCode.FO, 
			SystemLanguage.Finnish => dfLanguageCode.FI, 
			SystemLanguage.French => dfLanguageCode.FR, 
			SystemLanguage.German => dfLanguageCode.DE, 
			SystemLanguage.Greek => dfLanguageCode.EL, 
			SystemLanguage.Hebrew => dfLanguageCode.HE, 
			SystemLanguage.Hungarian => dfLanguageCode.HU, 
			SystemLanguage.Icelandic => dfLanguageCode.IS, 
			SystemLanguage.Indonesian => dfLanguageCode.ID, 
			SystemLanguage.Italian => dfLanguageCode.IT, 
			SystemLanguage.Japanese => dfLanguageCode.JA, 
			SystemLanguage.Korean => dfLanguageCode.KO, 
			SystemLanguage.Latvian => dfLanguageCode.LV, 
			SystemLanguage.Lithuanian => dfLanguageCode.LT, 
			SystemLanguage.Norwegian => dfLanguageCode.NO, 
			SystemLanguage.Polish => dfLanguageCode.PL, 
			SystemLanguage.Portuguese => dfLanguageCode.PT, 
			SystemLanguage.Romanian => dfLanguageCode.RO, 
			SystemLanguage.Russian => dfLanguageCode.RU, 
			SystemLanguage.SerboCroatian => dfLanguageCode.SH, 
			SystemLanguage.Slovak => dfLanguageCode.SK, 
			SystemLanguage.Slovenian => dfLanguageCode.SL, 
			SystemLanguage.Spanish => dfLanguageCode.ES, 
			SystemLanguage.Swedish => dfLanguageCode.SV, 
			SystemLanguage.Thai => dfLanguageCode.TH, 
			SystemLanguage.Turkish => dfLanguageCode.TR, 
			SystemLanguage.Ukrainian => dfLanguageCode.UK, 
			SystemLanguage.Unknown => dfLanguageCode.EN, 
			SystemLanguage.Vietnamese => dfLanguageCode.VI, 
			_ => throw new ArgumentException("Unknown system language: " + language), 
		};
	}
}
