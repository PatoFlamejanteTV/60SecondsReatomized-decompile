using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TextCollection : ScriptableObject
{
	[SerializeField]
	private List<TextEntry> _texts = new List<TextEntry>();

	private List<TextEntry> _approvedTexts = new List<TextEntry>();

	public int Length => _texts.Count;

	public TextEntry this[int key]
	{
		get
		{
			if (key < 0 || key >= _texts.Count)
			{
				return null;
			}
			return _texts[key];
		}
	}

	public string GetParametrisedEntry(List<string> modParameters, string singleParam = null, string[] constParams = null, bool localized = true)
	{
		_approvedTexts.Clear();
		int num = 0;
		bool flag = !string.IsNullOrEmpty(singleParam);
		if (_texts != null && (modParameters != null || flag || constParams != null))
		{
			ICollection<string> collection = null;
			if (!flag)
			{
				if (modParameters == null)
				{
					collection = constParams;
					num = constParams.Length;
				}
				else
				{
					collection = modParameters;
					num = modParameters.Count;
				}
			}
			for (int i = 0; i < _texts.Count; i++)
			{
				bool flag2 = true;
				if (collection != null)
				{
					if (_texts[i].ParamCount == num)
					{
						foreach (string item in collection)
						{
							if (string.IsNullOrEmpty(item))
							{
								break;
							}
							if (!_texts[i].TestParam(item))
							{
								flag2 = false;
								break;
							}
						}
					}
					else
					{
						flag2 = false;
					}
				}
				else if (_texts[i].ParamCount != 1 || !_texts[i].TestParam(singleParam))
				{
					flag2 = false;
				}
				if (flag2)
				{
					_approvedTexts.Add(_texts[i]);
				}
			}
		}
		if (_approvedTexts.Count > 0)
		{
			string text = _approvedTexts[UnityEngine.Random.Range(0, _approvedTexts.Count)].Text;
			_approvedTexts.Clear();
			if (localized)
			{
				return Settings.Data.LocalizationManager.GetValue(text);
			}
			return text;
		}
		return string.Empty;
	}

	public TextEntry GetRandomText()
	{
		if (_texts.Count > 0)
		{
			return _texts[UnityEngine.Random.Range(0, _texts.Count)];
		}
		return null;
	}
}
