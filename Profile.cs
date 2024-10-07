using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;
using UnityEngine;

[Serializable]
public class Profile
{
	public const int UNLOCKED_VAL = int.MaxValue;

	public const int LOCKED_VAL = 0;

	[SerializeField]
	private TextCollection _achievements;

	private List<GameUnlock> _unlocks = new List<GameUnlock>();

	public TextCollection Achievements => _achievements;

	public void Initialize()
	{
		SteamUserStats.RequestCurrentStats();
		LoadUnlocks();
	}

	public void ClearSnapshots()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath);
		if (!directoryInfo.Exists)
		{
			return;
		}
		FileInfo[] files = directoryInfo.GetFiles(".png");
		if (files != null)
		{
			for (int num = files.Length - 1; num >= 0; num--)
			{
				files[num].Delete();
			}
		}
	}

	public void TakeSnapshot(string filename)
	{
		TakeScreenshot(Application.persistentDataPath + "/" + filename);
	}

	public void TakeScreenshot(string path, string format)
	{
		DateTime now = DateTime.Now;
		TakeScreenshot(path + "/" + now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second + format);
	}

	public void TakeScreenshot(string fullPath)
	{
		ScreenCapture.CaptureScreenshot(fullPath);
	}

	public void AddRecord(string name, object val)
	{
		if (val is int)
		{
			int pData = 0;
			if (SteamUserStats.GetStat(name, out pData))
			{
				SteamUserStats.SetStat(name, pData + (int)val);
			}
		}
		else if (val is float)
		{
			float pData2 = 0f;
			if (SteamUserStats.GetStat(name, out pData2))
			{
				SteamUserStats.SetStat(name, pData2 + (float)val);
			}
		}
		SteamUserStats.StoreStats();
	}

	public void SetRecordBitfield(string name, int val)
	{
		int bitfieldValue = GlobalTools.GetBitfieldValue(val);
		SteamUserStats.SetStat(name, bitfieldValue);
	}

	public void AddRecordBitfield(string name, int val)
	{
		int bitfieldValue = GlobalTools.GetBitfieldValue(val);
		int pData = 0;
		if (SteamUserStats.GetStat(name, out pData) && !GlobalTools.TestBitfield(val, pData))
		{
			pData += bitfieldValue;
			SteamUserStats.SetStat(name, pData);
			SteamUserStats.StoreStats();
		}
	}

	public bool TestRecordBitfieldValue(string name, int val)
	{
		int pData = -1;
		SteamUserStats.GetStat(name, out pData);
		if (pData >= 0)
		{
			return GlobalTools.TestBitfieldValue(val, pData);
		}
		return false;
	}

	public bool TestRecordBitfield(string name, int val)
	{
		int pData = -1;
		SteamUserStats.GetStat(name, out pData);
		if (pData >= 0)
		{
			return GlobalTools.TestBitfield(val, pData);
		}
		return false;
	}

	public void IncrementRecord(string name, float val, bool forceLocal = false)
	{
		float num = GetRecord<float>(name, forceLocal) + val;
		SetRecord(name, num, forceLocal);
	}

	public void SetRecord(string name, object val, bool forceLocal = false)
	{
		bool flag = false;
		if (!forceLocal)
		{
			if (val is int)
			{
				SteamUserStats.SetStat(name, (int)val);
			}
			else if (val is float)
			{
				SteamUserStats.SetStat(name, (float)val);
			}
			SteamUserStats.StoreStats();
			return;
		}
		if (val is int val2)
		{
			PlayerPrefs.SetInt(name, val2);
			flag = true;
		}
		else if (val is float val3)
		{
			PlayerPrefs.SetFloat(name, val3);
			flag = true;
		}
		else if (val is string)
		{
			PlayerPrefs.SetString(name, val.ToString());
			flag = true;
		}
		if (flag)
		{
			PlayerPrefs.Save();
		}
	}

	public void Reset()
	{
		SteamUserStats.ResetAllStats(bAchievementsToo: true);
	}

	public T GetRecord<T>(string name, bool forceLocal = false)
	{
		return (T)GetRecordObject<T>(name, forceLocal);
	}

	private object GetRecordObject<T>(string name, bool forceLocal = false)
	{
		if (!forceLocal)
		{
			if (typeof(T) == typeof(int))
			{
				int pData = 0;
				if (SteamUserStats.GetStat(name, out pData))
				{
					return pData;
				}
			}
			else if (typeof(T) == typeof(float))
			{
				float pData2 = 0f;
				if (SteamUserStats.GetStat(name, out pData2))
				{
					return pData2;
				}
			}
		}
		if (typeof(T) == typeof(int))
		{
			return PlayerPrefs.GetInt(name);
		}
		if (typeof(T) == typeof(float))
		{
			return PlayerPrefs.GetFloat(name);
		}
		if (typeof(T) == typeof(string))
		{
			return PlayerPrefs.GetString(name, string.Empty);
		}
		return default(T);
	}

	public void UnlockAchievements(bool unlock)
	{
		for (int i = 0; i < _achievements.Length; i++)
		{
			if (unlock)
			{
				UnlockAchievement(_achievements[i].Text);
			}
			else
			{
				LockAchievement(_achievements[i].Text);
			}
		}
	}

	public void LockAchievement(string name)
	{
		SteamUserStats.ClearAchievement(name);
		SteamUserStats.StoreStats();
	}

	public void UnlockAchievement(string name)
	{
		if (!IsAchievementUnlocked(name))
		{
			SteamUserStats.SetAchievement(name);
			SteamUserStats.StoreStats();
		}
	}

	public int ProgressAchievement(string achName, string recordName, int progress, int maxProgress, bool indicate = false, bool forceProgress = false)
	{
		int record = GetRecord<int>(recordName);
		uint nCurProgress = (uint)progress;
		if (IsAchievementUnlocked(achName))
		{
			return record;
		}
		if (forceProgress)
		{
			SetRecord(recordName, progress);
		}
		else
		{
			AddRecord(recordName, progress);
		}
		int record2 = GetRecord<int>(recordName);
		if (record2 >= maxProgress)
		{
			indicate = true;
			UnlockAchievement(achName);
			nCurProgress = (uint)maxProgress;
		}
		else
		{
			int num = maxProgress / 2;
			if (record < num && record2 >= num)
			{
				indicate = true;
				nCurProgress = (uint)record2;
			}
		}
		if (indicate)
		{
			SteamUserStats.IndicateAchievementProgress(achName, nCurProgress, (uint)maxProgress);
			SteamUserStats.StoreStats();
		}
		return record2;
	}

	public bool IsAchievementUnlocked(string name)
	{
		bool pbAchieved = false;
		SteamUserStats.GetAchievement(name, out pbAchieved);
		return pbAchieved;
	}

	public TextEntry GetAchievementData(string name)
	{
		if (_achievements != null)
		{
			for (int i = 0; i < _achievements.Length; i++)
			{
				if (_achievements[i].Text == name)
				{
					return _achievements[i];
				}
			}
		}
		return null;
	}

	public bool GetAchievementData(int index, bool localize, out string achCode, out string achName, out string achDescr, out string achIcon, out string achProgressMax)
	{
		achCode = string.Empty;
		achName = string.Empty;
		achDescr = string.Empty;
		achIcon = string.Empty;
		achProgressMax = string.Empty;
		if (_achievements != null && index >= 0 && index < _achievements.Length)
		{
			TextEntry textEntry = _achievements[index];
			if (textEntry != null)
			{
				achCode = textEntry.Text;
				achName = (localize ? Settings.Data.LocalizationManager.GetValue(textEntry.GetParam(0)) : textEntry.GetParam(0));
				achDescr = (localize ? Settings.Data.LocalizationManager.GetValue(textEntry.GetParam(1)) : textEntry.GetParam(1));
				achIcon = textEntry.GetParam(2);
				achProgressMax = textEntry.GetParam(3);
				return true;
			}
		}
		return false;
	}

	public bool GetAchievementData(string name, bool localize, out string achName, out string achDescr, out string achIcon)
	{
		TextEntry achievementData = GetAchievementData(name);
		if (achievementData != null)
		{
			achName = (localize ? Settings.Data.LocalizationManager.GetValue(achievementData.GetParam(0)) : achievementData.GetParam(0));
			achDescr = (localize ? Settings.Data.LocalizationManager.GetValue(achievementData.GetParam(1)) : achievementData.GetParam(1));
			achIcon = achievementData.GetParam(2);
			return true;
		}
		achName = string.Empty;
		achDescr = string.Empty;
		achIcon = string.Empty;
		return false;
	}

	public string[] GetUnlockedAchievements()
	{
		if (_achievements != null)
		{
			List<string> list = new List<string>(_achievements.Length);
			for (int i = 0; i < _achievements.Length; i++)
			{
				TextEntry textEntry = _achievements[i];
				bool pbAchieved = false;
				SteamUserStats.GetAchievement(textEntry.Text, out pbAchieved);
				if (pbAchieved)
				{
					list.Add(textEntry.Text);
				}
			}
		}
		return null;
	}

	public static void TryToUnlockDifficultyAchievements(EGameDifficulty difficulty, EGameType gameType)
	{
		string name = string.Empty;
		switch (difficulty)
		{
		case EGameDifficulty.EASY:
			name = "EasyModesDone";
			break;
		case EGameDifficulty.NORMAL:
			name = "NormalModesDone";
			break;
		case EGameDifficulty.HARD:
			name = "HardModesDone";
			break;
		}
		Settings.Data.PlayerProfile.AddRecordBitfield(name, (int)gameType);
		_ = GlobalTools.GetBitfieldValue(2) + GlobalTools.GetBitfieldValue(3);
		GlobalTools.GetBitfieldValue(4);
		bool flag = true;
		for (int i = 0; i < 3; i++)
		{
			flag &= Settings.Data.PlayerProfile.TestRecordBitfield(name, i);
		}
		if (flag)
		{
			switch (difficulty)
			{
			case EGameDifficulty.EASY:
				Settings.Data.PlayerProfile.UnlockAchievement("ACH_EASY");
				break;
			case EGameDifficulty.NORMAL:
				Settings.Data.PlayerProfile.UnlockAchievement("ACH_NORMAL");
				break;
			case EGameDifficulty.HARD:
				Settings.Data.PlayerProfile.UnlockAchievement("ACH_HARD");
				break;
			}
		}
	}

	private void LoadUnlocks()
	{
		GameUnlock[] array = Resources.LoadAll<GameUnlock>("DLC/Unlocks");
		if (array != null && array.Length != 0)
		{
			_unlocks.AddRange(array);
		}
		for (int i = 0; i < _unlocks.Count; i++)
		{
			_unlocks[i].Unlock(IsUnlockAvailable(_unlocks[i].Id));
		}
	}

	public bool IsUnlockAvailable(string name)
	{
		return GetRecord<int>(name, forceLocal: true) == int.MaxValue;
	}

	public void SetUnlockAvailable(string id, bool available)
	{
		for (int i = 0; i < _unlocks.Count; i++)
		{
			if (_unlocks[i].Id == id)
			{
				_unlocks[i].Unlock(available);
				SetRecord(id, available ? int.MaxValue : 0, forceLocal: true);
			}
		}
	}

	public GameUnlock GetUnlock(string id)
	{
		return _unlocks.Find((GameUnlock x) => x.Id == id);
	}

	public bool GetUnlocks(string id, out GameUnlock[] unlocks)
	{
		List<GameUnlock> list = new List<GameUnlock>();
		for (int i = 0; i < _unlocks.Count; i++)
		{
			if (_unlocks[i].Id == id)
			{
				list.Add(_unlocks[i]);
			}
		}
		unlocks = list.ToArray();
		if (unlocks != null)
		{
			return unlocks.Length != 0;
		}
		return false;
	}

	public bool GetUnlocks(EGameUnlockType unlockType, string target, out GameUnlock[] unlocks)
	{
		List<GameUnlock> list = new List<GameUnlock>();
		for (int i = 0; i < _unlocks.Count; i++)
		{
			if (_unlocks[i].Type == unlockType && _unlocks[i].IsTarget(target))
			{
				list.Add(_unlocks[i]);
			}
		}
		unlocks = list.ToArray();
		if (unlocks != null)
		{
			return unlocks.Length != 0;
		}
		return false;
	}

	private static string GetCurrentHatRecordName(string character)
	{
		string result = string.Empty;
		switch (character)
		{
		case "dad":
			result = "DadHat";
			break;
		case "mom":
			result = "MomHat";
			break;
		case "daughter":
			result = "DaughterHat";
			break;
		case "son":
			result = "SonHat";
			break;
		case "dog":
			result = "DogHat";
			break;
		}
		return result;
	}

	public void SetCurrentHat(string character, string hatId)
	{
		SetRecord(GetCurrentHatRecordName(character), string.IsNullOrEmpty(hatId) ? string.Empty : hatId, forceLocal: true);
	}

	public string GetCurrentHat(string character)
	{
		string record = GetRecord<string>(GetCurrentHatRecordName(character), forceLocal: true);
		if (!string.IsNullOrEmpty(record) && IsUnlockAvailable(record))
		{
			return record;
		}
		return null;
	}

	public bool WasChallengeCompleted(string id)
	{
		return GetRecord<int>(GetChallengeDoneRecordId(id), forceLocal: true) > 0;
	}

	public bool GetChallengeCompletionData(string id, out float time, out DateTime date)
	{
		bool result = WasChallengeCompleted(id);
		time = GetRecord<float>(GetChallengeDoneTimeRecordId(id), forceLocal: true);
		string text = GetRecord<int>(GetChallengeDoneDateRecordId(id), forceLocal: true).ToString();
		text = text.Insert(4, "/");
		text = text.Insert(7, "/");
		date = DateTime.Parse(text);
		return result;
	}

	public void CompleteSurvivalChallenge(string id, int days, DateTime date)
	{
		string challengeDoneRecordId = GetChallengeDoneRecordId(id);
		int record = GetRecord<int>(challengeDoneRecordId);
		SetRecord(challengeDoneRecordId, record + 1, forceLocal: true);
		int num = ((record > 0) ? GetRecord<int>(GetChallengeDoneTimeRecordId(id)) : 0);
		if (record == 0 || days < num)
		{
			SetRecord(GetChallengeDoneTimeRecordId(id), days, forceLocal: true);
			string value = date.ToString("yyyyMMdd");
			SetRecord(GetChallengeDoneDateRecordId(id), Convert.ToInt32(value), forceLocal: true);
		}
	}

	public void CompleteScavengeChallenge(string id, float time, DateTime date)
	{
		string challengeDoneRecordId = GetChallengeDoneRecordId(id);
		int record = GetRecord<int>(challengeDoneRecordId);
		SetRecord(challengeDoneRecordId, record + 1, forceLocal: true);
		float num = ((record > 0) ? GetRecord<float>(GetChallengeDoneTimeRecordId(id)) : 999f);
		if (record == 0 || time < num)
		{
			SetRecord(GetChallengeDoneTimeRecordId(id), time, forceLocal: true);
			string value = date.ToString("yyyyMMdd");
			SetRecord(GetChallengeDoneDateRecordId(id), Convert.ToInt32(value), forceLocal: true);
		}
	}

	private string GetChallengeDoneRecordId(string challengeId)
	{
		return challengeId + "Done";
	}

	private string GetChallengeDoneTimeRecordId(string challengeId)
	{
		return challengeId + "DoneTime";
	}

	private string GetChallengeDoneDateRecordId(string challengeId)
	{
		return challengeId + "DoneDate";
	}
}
