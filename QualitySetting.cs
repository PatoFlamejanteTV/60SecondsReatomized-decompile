using System;
using UnityEngine;

[Serializable]
public class QualitySetting
{
	[SerializeField]
	private string _name = string.Empty;

	[SerializeField]
	private string _key = string.Empty;

	public string Name => _name;

	public string Key => _key;

	public void Set()
	{
		string[] names = QualitySettings.names;
		for (int i = 0; i < names.Length; i++)
		{
			if (names[i] == _key)
			{
				QualitySettings.SetQualityLevel(i, applyExpensiveChanges: true);
				break;
			}
		}
	}
}
