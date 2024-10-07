using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioCollection : ScriptableObject
{
	private const string MUSIC_GROUP_TAG = "Music";

	[SerializeField]
	private AudioEntry[] _musicEntries;

	[SerializeField]
	private AudioEntry[] _sfxEntries;

	private Dictionary<string, List<AudioEntry>> _groupedEntries = new Dictionary<string, List<AudioEntry>>();

	public void Initialize()
	{
		if (_sfxEntries == null)
		{
			return;
		}
		for (int i = 0; i < _sfxEntries.Length; i++)
		{
			string group = _sfxEntries[i].Group;
			if (!_groupedEntries.ContainsKey(group))
			{
				_groupedEntries.Add(group, new List<AudioEntry>());
			}
			_groupedEntries[group].Add(_sfxEntries[i]);
		}
	}

	public AudioEntry GetEntry(string name)
	{
		for (int i = 0; i < _sfxEntries.Length; i++)
		{
			if (_sfxEntries[i].Name == name)
			{
				return _sfxEntries[i];
			}
		}
		return null;
	}

	public AudioEntry[] GetMusicGroup()
	{
		return _musicEntries;
	}

	public AudioEntry[] GetGroup(string group)
	{
		if (_groupedEntries.ContainsKey(group))
		{
			return _groupedEntries[group].ToArray();
		}
		return null;
	}

	public AudioEntry GetRandomEntry(string group)
	{
		if (_groupedEntries.ContainsKey(group))
		{
			return _groupedEntries[group][UnityEngine.Random.Range(0, _groupedEntries[group].Count)];
		}
		return null;
	}
}
