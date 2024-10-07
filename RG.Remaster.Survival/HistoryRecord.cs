using System;
using System.Collections.Generic;
using I2.Loc;
using RG.Core.Base;
using RG.Core.SaveSystem;
using UnityEngine;

namespace RG.Remaster.Survival;

[Serializable]
[CreateAssetMenu(fileName = "HistoryRecord", menuName = "60 Seconds Remaster!/New HistoryRecord")]
public class HistoryRecord : RGScriptableObject, ISaveable
{
	internal struct HistoryRecordWrapper
	{
		public List<LocalizedString> Entries;

		public List<int> Days;
	}

	[SerializeField]
	private List<LocalizedString> _entries = new List<LocalizedString>();

	[SerializeField]
	private List<int> _days = new List<int>();

	[SerializeField]
	private LocalizedString _dayTerm;

	[SerializeField]
	private LocalizedString _dayTitleSeparatorTerm;

	[Tooltip("Event to which data will be save and from which be loaded")]
	[SerializeField]
	private SaveEvent _saveEvent;

	public List<LocalizedString> Entries => _entries;

	public List<int> Days => _days;

	public LocalizedString DayTerm => _dayTerm;

	public LocalizedString DayTitleSeparatorTerm => _dayTitleSeparatorTerm;

	public string ID => Guid;

	private void OnEnable()
	{
		_saveEvent.RegisterListener(this);
	}

	public string Serialize()
	{
		HistoryRecordWrapper historyRecordWrapper = default(HistoryRecordWrapper);
		historyRecordWrapper.Entries = new List<LocalizedString>();
		historyRecordWrapper.Days = new List<int>();
		HistoryRecordWrapper historyRecordWrapper2 = historyRecordWrapper;
		for (int i = 0; i < _entries.Count; i++)
		{
			historyRecordWrapper2.Entries.Add(_entries[i]);
		}
		for (int j = 0; j < _days.Count; j++)
		{
			historyRecordWrapper2.Days.Add(_days[j]);
		}
		return JsonUtility.ToJson(historyRecordWrapper2);
	}

	public void Deserialize(string jsonData)
	{
		HistoryRecordWrapper historyRecordWrapper = JsonUtility.FromJson<HistoryRecordWrapper>(jsonData);
		Reset();
		for (int i = 0; i < historyRecordWrapper.Entries.Count; i++)
		{
			if ((string)historyRecordWrapper.Entries[i] != null && !string.IsNullOrEmpty(historyRecordWrapper.Entries[i]))
			{
				_entries.Add(historyRecordWrapper.Entries[i]);
				continue;
			}
			Debug.LogWarningFormat("Current HistoryRecord entry is null or empty - index {0}", i);
		}
		for (int j = 0; j < historyRecordWrapper.Days.Count; j++)
		{
			Days.Add(historyRecordWrapper.Days[j]);
		}
	}

	public void DefaultData()
	{
	}

	public void Register()
	{
		_saveEvent.RegisterListener(this);
	}

	public void Unregister()
	{
		_saveEvent.UnregisterListener(this);
	}

	public void ResetData()
	{
		Reset();
	}

	public void Reset()
	{
		_entries.Clear();
		_days.Clear();
	}

	public void AddEntry(LocalizedString entry, int day)
	{
		_entries.Add(entry);
		_days.Add(day);
		if (_entries.Count > 1 && _days.Count > 1 && _days[_days.Count - 1] < _days[_days.Count - 2])
		{
			_days.Reverse(_days.Count - 2, 2);
			_entries.Reverse(_entries.Count - 2, 2);
		}
	}

	public bool HasValidSetup()
	{
		if (_entries == null || _days == null)
		{
			return false;
		}
		if ((string)_dayTerm == null || string.IsNullOrEmpty(_dayTerm))
		{
			return false;
		}
		if ((string)_dayTitleSeparatorTerm == null || string.IsNullOrEmpty(_dayTitleSeparatorTerm))
		{
			return false;
		}
		return true;
	}
}
