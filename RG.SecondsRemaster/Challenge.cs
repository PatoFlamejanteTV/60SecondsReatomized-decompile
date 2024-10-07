using System;
using System.Collections.Generic;
using I2.Loc;
using RG.Core.Base;
using RG.Core.SaveSystem;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Menu;
using RG.SecondsRemaster.Scavenge;
using UnityEngine;

namespace RG.SecondsRemaster;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/Challenges/New Challenge", fileName = "New Challenge")]
public class Challenge : RGScriptableObject, ISaveable
{
	public enum EChallengeType
	{
		NONE,
		SURVIVAL,
		SCAVENGE
	}

	[SerializeField]
	[Header("Save System")]
	private SaveEvent _saveEvent;

	[SerializeField]
	[Header("Main Data")]
	private EChallengeType _challengeType;

	[SerializeField]
	private LocalizedString _name;

	[SerializeField]
	private LocalizedString _description;

	[SerializeField]
	private Sprite _challengeGraphic;

	[SerializeField]
	private List<RewardItem> _rewards;

	[SerializeField]
	private LocalizedString _rewardName;

	[SerializeField]
	[Header("Survival Data")]
	private Mission _mission;

	[SerializeField]
	private List<LocalizedString> _objectives;

	[SerializeField]
	[Header("Scavenge Data")]
	private GameSetup _gameSetup;

	[SerializeField]
	private string _scavengeLevel;

	[SerializeField]
	private List<ScavengeItem> _collectables;

	[SerializeField]
	private GlobalBoolVariable _challengeCompleted;

	private ChallengeRuntimeData _runtimeData;

	public LocalizedString Name => _name;

	public LocalizedString RewardName => _rewardName;

	public Mission Mission => _mission;

	public string ScavengeLevel => _scavengeLevel;

	public LocalizedString Description => _description;

	public List<ScavengeItem> Collectables => _collectables;

	public List<RewardItem> Rewards => _rewards;

	public bool IsUnlocked => _challengeCompleted.Value;

	public float Time => _runtimeData.Time;

	public string UnlockDate => _runtimeData.UnlockDate;

	public GameSetup GameSetup => _gameSetup;

	public string ID => Guid;

	public Sprite ChallengeGraphic => _challengeGraphic;

	public EChallengeType ChallengeType => _challengeType;

	public List<LocalizedString> Objectives => _objectives;

	public void Unlock(float time)
	{
		if (!_challengeCompleted.Value || _runtimeData.Time > time)
		{
			_runtimeData.Time = Mathf.Clamp(time, 0f, 60f);
			_runtimeData.UnlockDate = DateTime.Now.ToString("dd/MM/yyyy");
			_challengeCompleted.Value = true;
		}
	}

	private void OnEnable()
	{
		Register();
	}

	private void OnDestroy()
	{
		Unregister();
	}

	public string Serialize()
	{
		return JsonUtility.ToJson(_runtimeData);
	}

	public void Deserialize(string jsonData)
	{
		_runtimeData = JsonUtility.FromJson<ChallengeRuntimeData>(jsonData);
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
		_runtimeData = new ChallengeRuntimeData();
	}
}
