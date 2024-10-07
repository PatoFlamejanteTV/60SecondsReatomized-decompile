using System;
using Steamworks;
using UnityEngine;

namespace RG.Remaster.Common;

[Serializable]
public class RichPresenceManager
{
	private const string STEAM_DISPLAY_STR = "steam_display";

	private const string STEAM_STATUS_STR = "status";

	[SerializeField]
	private ERichPresenceStatus _defaultRichPresenceStatus;

	[SerializeField]
	private RichPresenceData _richPresenceData;

	private ERichPresenceStatus _currentRichPresenceStatus;

	public ERichPresenceStatus CurrentRichPresenceStatus => _currentRichPresenceStatus;

	public void Initialize()
	{
		SetRichPresenceStatus(_defaultRichPresenceStatus);
	}

	private RichPresence GetRichPresence(ERichPresenceStatus statusId)
	{
		if (_richPresenceData != null)
		{
			return _richPresenceData.GetRichPresence(statusId);
		}
		return null;
	}

	public void SetParametrizedRichPresence(ERichPresenceStatus statusId, ERichPresenceParameter paramId, string paramValue)
	{
		SetRichPresenceParameter(statusId, paramId, paramValue);
		SetRichPresenceStatus(statusId);
	}

	public void SetRichPresenceParameter(ERichPresenceStatus statusId, ERichPresenceParameter paramId, string paramValue)
	{
		RichPresence richPresence = GetRichPresence(statusId);
		if (SteamManager.Initialized && SteamAPI.IsSteamRunning() && !string.IsNullOrEmpty(paramValue))
		{
			string parametrizedRichPresence = richPresence.GetParametrizedRichPresence(paramId);
			if (!string.IsNullOrEmpty(parametrizedRichPresence))
			{
				SteamFriends.SetRichPresence(parametrizedRichPresence, paramValue);
			}
		}
	}

	public void SetRichPresenceStatus(ERichPresenceStatus statusId)
	{
		RichPresence richPresence = GetRichPresence(statusId);
		if (richPresence != null)
		{
			_currentRichPresenceStatus = statusId;
			if (SteamManager.Initialized && SteamAPI.IsSteamRunning())
			{
				SteamFriends.SetRichPresence("steam_display", richPresence.Content);
				SteamFriends.SetRichPresence("status", richPresence.Content);
			}
		}
	}
}
