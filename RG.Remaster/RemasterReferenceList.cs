using System;
using System.Collections.Generic;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.Remaster;

[Serializable]
[CreateAssetMenu(menuName = "60 Seconds Remaster!/New reference object list", fileName = "New Object list")]
public class RemasterReferenceList : ObjectsReferenceList
{
	[SerializeField]
	private List<SurvivalEvent> _survivalEvents;

	[SerializeField]
	private List<ReportEvent> _reportEvents;

	[SerializeField]
	private List<SystemEvent> _systemEvents;

	[SerializeField]
	private List<ExpeditionEvent> _expeditionEvents;

	[SerializeField]
	private List<ExpeditionDestination> _expeditionDestinations;

	[SerializeField]
	private List<VisualId> _visualIds;

	[SerializeField]
	private List<ExpeditionThreat> _expeditionThreats;

	[SerializeField]
	private List<CharacterStatus> _characterStatuses;

	[SerializeField]
	private List<Deck> _decks;

	[SerializeField]
	private List<Character> _characters;

	[SerializeField]
	private List<IItem> _items;

	[SerializeField]
	private List<EventTag> _tags;

	[SerializeField]
	private List<Goal> _goals;

	[SerializeField]
	private List<Resource> _resources;

	[SerializeField]
	private List<ShipSystem> _shipSystems;

	[SerializeField]
	private List<SystemStatusEvent> _systemStatusEvents;

	[SerializeField]
	private List<Mission> _missions;

	[SerializeField]
	private List<Challenge> _challenges;

	[SerializeField]
	private List<CurrentChallengeData> _currentChallengeData;

	[SerializeField]
	private List<TextJournalGroupId> _textJournalGroupId;

	[SerializeField]
	private List<BaseActionCondition> _baseActionConditions;

	private const string DUPLICATE_ID_ERROR_MESSAGE = "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.";

	private Dictionary<string, UnityEngine.Object> _objects;

	public List<TextJournalGroupId> TextJournalGroupId => _textJournalGroupId;

	public List<BaseActionCondition> BaseActionConditions => _baseActionConditions;

	public List<SystemStatusEvent> SystemStatusEvents
	{
		get
		{
			return _systemStatusEvents;
		}
		set
		{
			_systemStatusEvents = value;
		}
	}

	public List<SurvivalEvent> SurvivalEvents
	{
		get
		{
			return _survivalEvents;
		}
		set
		{
			_survivalEvents = value;
		}
	}

	public List<ReportEvent> ReportEvents
	{
		get
		{
			return _reportEvents;
		}
		set
		{
			_reportEvents = value;
		}
	}

	public List<SystemEvent> SystemEvents
	{
		get
		{
			return _systemEvents;
		}
		set
		{
			_systemEvents = value;
		}
	}

	public List<ExpeditionEvent> ExpeditionEvents
	{
		get
		{
			return _expeditionEvents;
		}
		set
		{
			_expeditionEvents = value;
		}
	}

	public List<ExpeditionDestination> ExpeditionDestinations
	{
		get
		{
			return _expeditionDestinations;
		}
		set
		{
			_expeditionDestinations = value;
		}
	}

	public List<VisualId> VisualIds
	{
		get
		{
			return _visualIds;
		}
		set
		{
			_visualIds = value;
		}
	}

	public List<ExpeditionThreat> ExpeditionThreats
	{
		get
		{
			return _expeditionThreats;
		}
		set
		{
			_expeditionThreats = value;
		}
	}

	public List<CharacterStatus> CharacterStatuses
	{
		get
		{
			return _characterStatuses;
		}
		set
		{
			_characterStatuses = value;
		}
	}

	public List<Deck> Decks
	{
		get
		{
			return _decks;
		}
		set
		{
			_decks = value;
		}
	}

	public List<Character> Characters
	{
		get
		{
			return _characters;
		}
		set
		{
			_characters = value;
		}
	}

	public List<IItem> Items
	{
		get
		{
			return _items;
		}
		set
		{
			_items = value;
		}
	}

	public List<EventTag> Tags
	{
		get
		{
			return _tags;
		}
		set
		{
			_tags = value;
		}
	}

	public List<Goal> Goals
	{
		get
		{
			return _goals;
		}
		set
		{
			_goals = value;
		}
	}

	public List<Resource> Resources
	{
		get
		{
			return _resources;
		}
		set
		{
			_resources = value;
		}
	}

	public List<ShipSystem> ShipSystems
	{
		get
		{
			return _shipSystems;
		}
		set
		{
			_shipSystems = value;
		}
	}

	public List<Mission> Missions
	{
		get
		{
			return _missions;
		}
		set
		{
			_missions = value;
		}
	}

	public List<Challenge> Challenges
	{
		get
		{
			return _challenges;
		}
		set
		{
			_challenges = value;
		}
	}

	public List<CurrentChallengeData> CurrentChallengeData
	{
		get
		{
			return _currentChallengeData;
		}
		set
		{
			_currentChallengeData = value;
		}
	}

	public override void Build()
	{
		_objects = new Dictionary<string, UnityEngine.Object>();
		for (int i = 0; i < _visualIds.Count; i++)
		{
			if (_objects.ContainsKey(_visualIds[i].Id))
			{
				Debug.LogErrorFormat(_visualIds[i], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_visualIds[i].Id].name, _visualIds[i].name);
			}
			else
			{
				_objects.Add(_visualIds[i].Id, _visualIds[i]);
			}
		}
		for (int j = 0; j < _expeditionThreats.Count; j++)
		{
			if (_objects.ContainsKey(_expeditionThreats[j].ID))
			{
				Debug.LogErrorFormat(_expeditionThreats[j], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_expeditionThreats[j].ID].name, _expeditionThreats[j].name);
			}
			else
			{
				_objects.Add(_expeditionThreats[j].ID, _expeditionThreats[j]);
			}
		}
		for (int k = 0; k < _characterStatuses.Count; k++)
		{
			if (_objects.ContainsKey(_characterStatuses[k].Id))
			{
				Debug.LogErrorFormat(_characterStatuses[k], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_characterStatuses[k].Id].name, _characterStatuses[k].name);
			}
			else
			{
				_objects.Add(_characterStatuses[k].Id, _characterStatuses[k]);
			}
		}
		for (int l = 0; l < _decks.Count; l++)
		{
			if (_objects.ContainsKey(_decks[l].ID))
			{
				Debug.LogErrorFormat(_decks[l], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_decks[l].ID].name, _decks[l].name);
			}
			else
			{
				_objects.Add(_decks[l].ID, _decks[l]);
			}
		}
		for (int m = 0; m < _characters.Count; m++)
		{
			if (_objects.ContainsKey(_characters[m].ID))
			{
				Debug.LogErrorFormat(_characters[m], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_characters[m].ID].name, _characters[m].name);
			}
			else
			{
				_objects.Add(_characters[m].ID, _characters[m]);
			}
		}
		for (int n = 0; n < _items.Count; n++)
		{
			if (_objects.ContainsKey(_items[n].BaseStaticData.ItemId))
			{
				Debug.LogErrorFormat(_items[n], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_items[n].BaseStaticData.ItemId].name, _items[n].name);
			}
			else
			{
				_objects.Add(_items[n].BaseStaticData.ItemId, _items[n]);
			}
		}
		for (int num = 0; num < _expeditionDestinations.Count; num++)
		{
			if (_objects.ContainsKey(_expeditionDestinations[num].ID))
			{
				Debug.LogErrorFormat(_expeditionDestinations[num], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_expeditionDestinations[num].ID].name, _expeditionDestinations[num].name);
			}
			else
			{
				_objects.Add(_expeditionDestinations[num].ID, _expeditionDestinations[num]);
			}
		}
		for (int num2 = 0; num2 < _tags.Count; num2++)
		{
			if (_objects.ContainsKey("etag_" + _tags[num2].name))
			{
				Debug.LogError(string.Format("Duplicate ID of tag when adding to SurvivalReferenceList dictionary. Key: {0}", "etag_" + _tags[num2].name));
			}
			else
			{
				_objects.Add("etag_" + _tags[num2].name, _tags[num2]);
			}
		}
		for (int num3 = 0; num3 < _survivalEvents.Count; num3++)
		{
			if (_objects.ContainsKey(_survivalEvents[num3].ID))
			{
				Debug.LogErrorFormat(_survivalEvents[num3], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_survivalEvents[num3].ID].name, _survivalEvents[num3].name);
			}
			else
			{
				_objects.Add(_survivalEvents[num3].ID, _survivalEvents[num3]);
			}
		}
		for (int num4 = 0; num4 < _expeditionEvents.Count; num4++)
		{
			if (_objects.ContainsKey(_expeditionEvents[num4].ID))
			{
				Debug.LogErrorFormat(_expeditionEvents[num4], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_expeditionEvents[num4].ID].name, _expeditionEvents[num4].name);
			}
			else
			{
				_objects.Add(_expeditionEvents[num4].ID, _expeditionEvents[num4]);
			}
		}
		for (int num5 = 0; num5 < _reportEvents.Count; num5++)
		{
			if (_objects.ContainsKey(_reportEvents[num5].ID))
			{
				Debug.LogErrorFormat(_reportEvents[num5], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_reportEvents[num5].ID].name, _reportEvents[num5].name);
			}
			else
			{
				_objects.Add(_reportEvents[num5].ID, _reportEvents[num5]);
			}
		}
		for (int num6 = 0; num6 < _systemEvents.Count; num6++)
		{
			if (_objects.ContainsKey(_systemEvents[num6].ID))
			{
				Debug.LogErrorFormat(_systemEvents[num6], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_systemEvents[num6].ID].name, _systemEvents[num6].name);
			}
			else
			{
				_objects.Add(_systemEvents[num6].ID, _systemEvents[num6]);
			}
		}
		for (int num7 = 0; num7 < _goals.Count; num7++)
		{
			if (_objects.ContainsKey(_goals[num7].ID))
			{
				Debug.LogErrorFormat(_goals[num7], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_goals[num7].ID].name, _goals[num7].name);
			}
			else
			{
				_objects.Add(_goals[num7].ID, _goals[num7]);
			}
		}
		for (int num8 = 0; num8 < _resources.Count; num8++)
		{
			if (_objects.ContainsKey(_resources[num8].ID))
			{
				Debug.LogErrorFormat(_resources[num8], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_resources[num8].ID].name, _resources[num8].name);
			}
			else
			{
				_objects.Add(_resources[num8].ID, _resources[num8]);
			}
		}
		for (int num9 = 0; num9 < _shipSystems.Count; num9++)
		{
			if (_objects.ContainsKey(_shipSystems[num9].Id))
			{
				Debug.LogErrorFormat(_shipSystems[num9], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_shipSystems[num9].Id].name, _shipSystems[num9].name);
			}
			else
			{
				_objects.Add(_shipSystems[num9].Id, _shipSystems[num9]);
			}
		}
		for (int num10 = 0; num10 < _systemStatusEvents.Count; num10++)
		{
			if (_objects.ContainsKey(_systemStatusEvents[num10].ID))
			{
				Debug.LogErrorFormat(_systemStatusEvents[num10], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_systemStatusEvents[num10].ID].name, _systemStatusEvents[num10].name);
			}
			else
			{
				_objects.Add(_systemStatusEvents[num10].ID, _systemStatusEvents[num10]);
			}
		}
		for (int num11 = 0; num11 < _missions.Count; num11++)
		{
			if (_objects.ContainsKey(_missions[num11].ID))
			{
				Debug.LogErrorFormat(_missions[num11], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_missions[num11].ID].name, _missions[num11].name);
			}
			else
			{
				_objects.Add(_missions[num11].ID, _missions[num11]);
			}
		}
		for (int num12 = 0; num12 < _textJournalGroupId.Count; num12++)
		{
			if (_objects.ContainsKey(_textJournalGroupId[num12].Guid))
			{
				Debug.LogErrorFormat(_textJournalGroupId[num12], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_textJournalGroupId[num12].Guid].name, _textJournalGroupId[num12].name);
			}
			else
			{
				_objects.Add(_textJournalGroupId[num12].Guid, _textJournalGroupId[num12]);
			}
		}
		for (int num13 = 0; num13 < _baseActionConditions.Count; num13++)
		{
			if (_objects.ContainsKey(_baseActionConditions[num13].Guid))
			{
				Debug.LogErrorFormat(_baseActionConditions[num13], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_baseActionConditions[num13].Guid].name, _baseActionConditions[num13].name);
			}
			else
			{
				_objects.Add(_baseActionConditions[num13].Guid, _baseActionConditions[num13]);
			}
		}
		for (int num14 = 0; num14 < _challenges.Count; num14++)
		{
			if (_objects.ContainsKey(_challenges[num14].Guid))
			{
				Debug.LogErrorFormat(_challenges[num14], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[_challenges[num14].Guid].name, _challenges[num14].name);
			}
			else
			{
				_objects.Add(_challenges[num14].Guid, _challenges[num14]);
			}
		}
		for (int num15 = 0; num15 < CurrentChallengeData.Count; num15++)
		{
			if (_objects.ContainsKey(CurrentChallengeData[num15].Guid))
			{
				Debug.LogErrorFormat(CurrentChallengeData[num15], "Duplicate ID was found in dictionary '{0}'. Object which was attempted to be added: '{1}'.", _objects[CurrentChallengeData[num15].Guid].name, CurrentChallengeData[num15].name);
			}
			else
			{
				_objects.Add(CurrentChallengeData[num15].Guid, CurrentChallengeData[num15]);
			}
		}
	}

	public override void Destroy()
	{
		if (_objects != null)
		{
			_objects.Clear();
		}
	}

	public override UnityEngine.Object GetReferenceObjectByGUID(string id)
	{
		if (_objects.ContainsKey(id))
		{
			return _objects[id];
		}
		throw new Exception($"There is no {id} in dictionary");
	}
}
