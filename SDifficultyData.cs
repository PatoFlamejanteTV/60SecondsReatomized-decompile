using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public struct SDifficultyData
{
	[SerializeField]
	private int _prepareTime;

	[SerializeField]
	private int _scavengeTime;

	[SerializeField]
	private SPairValue _itemsCollected;

	[SerializeField]
	private SPairValue _familyMembersCollected;

	[SerializeField]
	private SPairValue _foodCollected;

	[SerializeField]
	private SPairValue _waterCollected;

	[SerializeField]
	private SPairValue _suitcaseStock;

	[SerializeField]
	private SPairValue _expeditionSicknessFalloutChance;

	[SerializeField]
	private SPairValue _expeditionSicknessPostFalloutChance;

	[SerializeField]
	private SPairValue _expeditionRaidersFollowChance;

	[SerializeField]
	private SPairValue _expeditionInventoryDmgChance;

	[SerializeField]
	private int _startDay;

	[SerializeField]
	private List<GameObject> _exactItemsCollected;

	[SerializeField]
	private List<string> _survivalParams;

	[SerializeField]
	private List<SurvivalCondition> _survivalConditions;

	[SerializeField]
	public bool _dayOneDeathsIgnored;

	[SerializeField]
	public bool _lastSurvivorStaysSane;

	public int PrepareTime
	{
		get
		{
			return _prepareTime;
		}
		set
		{
			_prepareTime = value;
		}
	}

	public int ScavengeTime
	{
		get
		{
			return _scavengeTime;
		}
		set
		{
			_scavengeTime = value;
		}
	}

	public SPairValue ItemsCollected
	{
		get
		{
			return _itemsCollected;
		}
		set
		{
			_itemsCollected = value;
		}
	}

	public SPairValue FamilyMembersCollected
	{
		get
		{
			return _familyMembersCollected;
		}
		set
		{
			_familyMembersCollected = value;
		}
	}

	public SPairValue FoodCollected
	{
		get
		{
			return _foodCollected;
		}
		set
		{
			_foodCollected = value;
		}
	}

	public SPairValue WaterCollected
	{
		get
		{
			return _waterCollected;
		}
		set
		{
			_waterCollected = value;
		}
	}

	public SPairValue SuitcaseStock
	{
		get
		{
			return _suitcaseStock;
		}
		set
		{
			_suitcaseStock = value;
		}
	}

	public SPairValue ExpeditionSicknessFalloutChance
	{
		get
		{
			return _expeditionSicknessFalloutChance;
		}
		set
		{
			_expeditionSicknessFalloutChance = value;
		}
	}

	public SPairValue ExpeditionSicknessPostFalloutChance
	{
		get
		{
			return _expeditionSicknessPostFalloutChance;
		}
		set
		{
			_expeditionSicknessPostFalloutChance = value;
		}
	}

	public SPairValue ExpeditionRaidersFollowChance
	{
		get
		{
			return _expeditionRaidersFollowChance;
		}
		set
		{
			_expeditionRaidersFollowChance = value;
		}
	}

	public SPairValue ExpeditionInventoryDmgChance
	{
		get
		{
			return _expeditionInventoryDmgChance;
		}
		set
		{
			_expeditionInventoryDmgChance = value;
		}
	}

	public int StartDay
	{
		get
		{
			return _startDay;
		}
		set
		{
			_startDay = value;
		}
	}

	public List<GameObject> ExactItemsCollected => _exactItemsCollected;

	public List<string> SurvivalParams => _survivalParams;

	public List<SurvivalCondition> SurvivalConditions => _survivalConditions;

	public bool DayOneDeathsIgnored => _dayOneDeathsIgnored;

	public bool LastSurvivorStaysSane => _lastSurvivorStaysSane;

	public string GetText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (_prepareTime > 0)
		{
			stringBuilder.AppendFormat(Settings.Data.LocalizationManager.GetValue("menu_game_diff_exploretime"), _prepareTime);
			stringBuilder.Append("\n");
		}
		if (_scavengeTime > 0)
		{
			stringBuilder.AppendFormat(Settings.Data.LocalizationManager.GetValue("menu_game_diff_scavengetime"), _scavengeTime);
			stringBuilder.Append("\n");
		}
		if (_foodCollected.Available)
		{
			string v = string.Empty;
			string v2 = string.Empty;
			string sep = string.Empty;
			_foodCollected.GetValues(out v, out sep, out v2);
			stringBuilder.AppendFormat(Settings.Data.LocalizationManager.GetValue("menu_game_diff_food"), v, sep, v2);
			stringBuilder.Append("\n");
		}
		if (_waterCollected.Available)
		{
			string v3 = string.Empty;
			string v4 = string.Empty;
			string sep2 = string.Empty;
			_waterCollected.GetValues(out v3, out sep2, out v4);
			stringBuilder.AppendFormat(Settings.Data.LocalizationManager.GetValue("menu_game_diff_water"), v3, sep2, v4);
			stringBuilder.Append("\n");
		}
		if (_itemsCollected.Available)
		{
			string v5 = string.Empty;
			string v6 = string.Empty;
			string sep3 = string.Empty;
			_itemsCollected.GetValues(out v5, out sep3, out v6);
			stringBuilder.AppendFormat(Settings.Data.LocalizationManager.GetValue("menu_game_diff_items"), v5, sep3, v6);
			stringBuilder.Append("\n");
		}
		if (_familyMembersCollected.Available)
		{
			string v7 = string.Empty;
			string v8 = string.Empty;
			string sep4 = string.Empty;
			_familyMembersCollected.GetValues(out v7, out sep4, out v8);
			stringBuilder.AppendFormat(Settings.Data.LocalizationManager.GetValue("menu_game_diff_family"), v7, sep4, v8);
			stringBuilder.Append("\n");
		}
		return stringBuilder.ToString();
	}
}
