using System.Text;
using I2.Loc;
using RG.Core.Base;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.Remaster.Survival;

public class SimpleHistoryManager : RGMonoBehaviour
{
	private const string DAY_NUMBER_SEPARATOR = " ";

	private static SimpleHistoryManager _instance;

	[SerializeField]
	private HistoryRecord _historyRecord;

	[SerializeField]
	private SurvivalData _survivalData;

	public static SimpleHistoryManager Instance
	{
		get
		{
			if (_instance == null)
			{
				FindInstanceInScene();
			}
			return _instance;
		}
	}

	private static void FindInstanceInScene()
	{
		_instance = Object.FindObjectOfType<SimpleHistoryManager>();
		if (_instance == null)
		{
			Debug.LogError("No SimpleHistoryManager in scene!");
		}
	}

	protected override void CustomAwake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else if (_instance != this)
		{
			Debug.LogWarning("Duplicate instance of SimpleHistoryManager detected! Removing " + base.gameObject.name);
			Object.Destroy(this);
		}
	}

	public string RenderHistoryToString()
	{
		string empty = string.Empty;
		if (_historyRecord != null)
		{
			if (_historyRecord.HasValidSetup())
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < _historyRecord.Entries.Count; i++)
				{
					stringBuilder.Append(string.Format(_historyRecord.DayTerm, _historyRecord.Days[i]));
					stringBuilder.Append(_historyRecord.DayTitleSeparatorTerm);
					stringBuilder.Append(_historyRecord.Entries[i]);
					stringBuilder.AppendLine();
				}
				return stringBuilder.ToString();
			}
			Debug.LogError("HistoryRecord used in SimpleHistoryManager has invalid setup!");
			return empty;
		}
		Debug.LogError("No HistoryRecord set up in SimpleHistoryManager!");
		return empty;
	}

	public void AddEntry(LocalizedString entry, int day, bool useCurrentDay)
	{
		if (!string.IsNullOrEmpty(entry))
		{
			if (useCurrentDay)
			{
				if (_survivalData != null)
				{
					_historyRecord.AddEntry(entry, _survivalData.DisplayDay);
				}
				else
				{
					Debug.LogWarning("No SurvivalData set in SimpleHistoryManager, no entry added.");
				}
			}
			else
			{
				_historyRecord.AddEntry(entry, day);
			}
		}
		else
		{
			Debug.LogWarning("Empty entry term - was not added to HistoryRecord.");
		}
	}
}
