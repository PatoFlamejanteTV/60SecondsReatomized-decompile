using System;
using RG.Core.Base;
using RG.Core.SaveSystem;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/New GroupID")]
public class TextJournalGroupId : RGScriptableObject, ISaveable
{
	[SerializeField]
	private SaveEvent _saveEvent;

	public string ID => Guid;

	public string Serialize()
	{
		throw new NotImplementedException();
	}

	public void Deserialize(string jsonData)
	{
		throw new NotImplementedException();
	}

	public void Register()
	{
		throw new NotImplementedException();
	}

	public void Unregister()
	{
		throw new NotImplementedException();
	}

	public void ResetData()
	{
		throw new NotImplementedException();
	}
}
