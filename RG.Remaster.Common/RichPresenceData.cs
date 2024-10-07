using System;
using RG.Core.Base;
using UnityEngine;

namespace RG.Remaster.Common;

[Serializable]
[CreateAssetMenu(menuName = "60 Seconds Remaster!/Platform/New Rich Presence Data", fileName = "New Rich Presence Data")]
public class RichPresenceData : RGScriptableObject
{
	[SerializeField]
	private RichPresence[] _richPresences;

	public RichPresence GetRichPresence(ERichPresenceStatus statusId)
	{
		if (_richPresences != null)
		{
			for (int i = 0; i < _richPresences.Length; i++)
			{
				if (_richPresences[i].StatusId == statusId)
				{
					return _richPresences[i];
				}
			}
		}
		return null;
	}

	public string GetRichPresenceContent(ERichPresenceStatus statusId)
	{
		return GetRichPresence(statusId)?.Content;
	}

	public string GetParametrizedRichPresence(ERichPresenceStatus statusId, ERichPresenceParameter paramId)
	{
		return GetRichPresence(statusId)?.GetParametrizedRichPresence(paramId);
	}
}
