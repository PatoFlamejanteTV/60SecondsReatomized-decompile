using System;
using System.Collections.Generic;
using I2.Loc;
using RG.Core.SaveSystem;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public sealed class TextJournalContent : JournalContent, ILocalizationParamsManager
{
	[SerializeField]
	private string _pureText;

	[SerializeField]
	private LocalizedString _term;

	[SerializeField]
	private List<Character> _characters;

	[SerializeField]
	private Character _expeditionCharacter;

	[SerializeField]
	private List<IItem> _items;

	[SerializeField]
	private List<int> _localVariablesInts;

	[SerializeField]
	private List<LocalizedString> _terms;

	[SerializeField]
	private EParsecsEventPhase _eventPhase;

	private const string EXPEDITION_PREFIX = "EXPEDITION";

	private const string EXPEDITION_CHARACTER_NAME_I2_LOC_PARAM = "EXPEDITION_CHARACTER_NAME";

	private const string EXPEDITION_CHARACTER_SURNAME_I2_LOC_PARAM = "EXPEDITION_CHARACTER_SURNAME";

	private const string EXPEDITION_CHARACTER_FULL_NAME_I2_LOC_PARAM = "EXPEDITION_CHARACTER_FULL_NAME";

	private const string EXPEDITION_CHARACTER_SEX = "EXPEDITION_CHARACTER_SEX";

	private const string TERM_PARAM = "TERM_";

	private const string DIVIDE_CHAR = "|";

	public List<LocalizedString> Terms
	{
		get
		{
			return _terms;
		}
		set
		{
			_terms = value;
		}
	}

	public LocalizedString Term => _term;

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

	public Character ExpeditionCharacter
	{
		get
		{
			return _expeditionCharacter;
		}
		set
		{
			_expeditionCharacter = value;
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

	public List<int> LocalVariablesInts
	{
		get
		{
			return _localVariablesInts;
		}
		set
		{
			_localVariablesInts = value;
		}
	}

	public string PureText => _pureText;

	public EParsecsEventPhase EventPhase
	{
		get
		{
			return _eventPhase;
		}
		set
		{
			_eventPhase = value;
		}
	}

	public TextJournalContent(string serializedData, SaveEvent saveEvent)
		: base(saveEvent)
	{
		Deserialize(serializedData, saveEvent);
	}

	public TextJournalContent(LocalizedString term, int displayPriority)
		: base(displayPriority)
	{
		type = EJournalContentType.TEXT;
		_term = term;
	}

	public TextJournalContent(string text, int displayPriority)
		: base(displayPriority)
	{
		type = EJournalContentType.TEXT;
		_pureText = text;
	}

	public override string Serialize()
	{
		TextJournalContentWrapper textJournalContentWrapper = new TextJournalContentWrapper
		{
			DisplayOrder = displayOrder,
			DisplayPriority = displayPriority,
			GroupId = ((groupId != null) ? groupId.Guid : string.Empty),
			Type = type,
			PureText = _pureText,
			Term = _term,
			Characters = new List<string>(),
			Items = new List<string>(),
			ExpeditionCharacter = ((_expeditionCharacter != null) ? _expeditionCharacter.Guid : string.Empty),
			LocalVariablesInts = _localVariablesInts,
			Terms = _terms
		};
		if (_characters != null)
		{
			for (int i = 0; i < _characters.Count; i++)
			{
				if (_characters[i] != null)
				{
					textJournalContentWrapper.Characters.Add(_characters[i].Guid);
				}
			}
		}
		if (_items != null)
		{
			for (int j = 0; j < _items.Count; j++)
			{
				if (_items[j] != null)
				{
					textJournalContentWrapper.Items.Add(_items[j].Guid);
				}
			}
		}
		return JsonUtility.ToJson(textJournalContentWrapper);
	}

	public override void Deserialize(string data, SaveEvent saveEvent)
	{
		TextJournalContentWrapper textJournalContentWrapper = JsonUtility.FromJson<TextJournalContentWrapper>(data);
		DeserializeBaseWrapper(textJournalContentWrapper, saveEvent);
		_pureText = textJournalContentWrapper.PureText;
		_term = textJournalContentWrapper.Term;
		_expeditionCharacter = ((!string.IsNullOrEmpty(textJournalContentWrapper.ExpeditionCharacter)) ? ((Character)saveEvent.GetReferenceObjectByID(textJournalContentWrapper.ExpeditionCharacter)) : null);
		_terms = textJournalContentWrapper.Terms;
		_localVariablesInts = textJournalContentWrapper.LocalVariablesInts;
		for (int i = 0; i < textJournalContentWrapper.Characters.Count; i++)
		{
			if (!string.IsNullOrEmpty(textJournalContentWrapper.Characters[i]))
			{
				_characters.Add((Character)saveEvent.GetReferenceObjectByID(textJournalContentWrapper.Characters[i]));
			}
		}
		for (int j = 0; j < textJournalContentWrapper.Items.Count; j++)
		{
			if (!string.IsNullOrEmpty(textJournalContentWrapper.Items[j]))
			{
				_items.Add((IItem)saveEvent.GetReferenceObjectByID(textJournalContentWrapper.Items[j]));
			}
		}
	}

	private void RegisterInI2Loc()
	{
		if (!LocalizationManager.ParamManagers.Contains(this))
		{
			LocalizationManager.ParamManagers.Add(this);
			LocalizationManager.LocalizeAll(Force: true);
		}
	}

	private void UnregisterFromI2Loc()
	{
		LocalizationManager.ParamManagers.Remove(this);
	}

	public void RegisterManagers()
	{
		if (_characters != null && _characters.Count > 0 && _characters[0] != null)
		{
			_characters[0].RegisterInI2Loc();
		}
		if (_items != null && _items.Count > 0 && _items[0] != null)
		{
			_items[0].RegisterToI2Loc();
		}
		if (_expeditionCharacter != null || _terms != null || _localVariablesInts != null)
		{
			RegisterInI2Loc();
		}
	}

	public void UnregisterManagers()
	{
		if (_characters != null && _characters.Count > 0 && _characters[0] != null)
		{
			_characters[0].UnregisterFromI2Loc();
		}
		if (_items != null && _items.Count > 0 && _items[0] != null)
		{
			_items[0].UnregisterFromI2Loc();
		}
		if (_expeditionCharacter != null || _terms != null || _localVariablesInts != null)
		{
			UnregisterFromI2Loc();
		}
	}

	public string GetParameterValue(string param)
	{
		if (char.IsNumber(param[0]) && param.Contains("|"))
		{
			int num = Convert.ToInt32(param.Substring(0, param.IndexOf("|", StringComparison.Ordinal)));
			string[] array = param.Substring(param.IndexOf("|", StringComparison.Ordinal) + 1).Split('/');
			if (_localVariablesInts == null)
			{
				Debug.LogErrorFormat("This term {0} is trying to set int variable {1} but Variables connection is not connected in node.", _term.mTerm, num);
				return null;
			}
			if (_localVariablesInts.Count <= num)
			{
				Debug.LogErrorFormat("This term {0} is trying to set int variable {1} which was not provided in node", _term.mTerm, num);
				return null;
			}
			if (array.Length <= _localVariablesInts[num])
			{
				Debug.LogErrorFormat("This term {0} is trying to set option {1} which was not provided in node", _term.mTerm, _localVariablesInts[num]);
				return null;
			}
			return array[_localVariablesInts[num]];
		}
		if (char.IsNumber(param[0]) && !param.Contains("|"))
		{
			int num2 = Convert.ToInt32(param);
			if (_localVariablesInts == null)
			{
				Debug.LogErrorFormat("This term {0} is trying to set int variable {1} but Variables connection is not connected in node.", _term.mTerm, num2);
				return null;
			}
			if (_localVariablesInts.Count <= num2)
			{
				Debug.LogErrorFormat("This term {0} is trying to set int variable {1} which was not provided in node", _term.mTerm, num2);
				return null;
			}
			return _localVariablesInts[num2].ToString();
		}
		if (param.StartsWith("EXPEDITION"))
		{
			switch (param)
			{
			case "EXPEDITION_CHARACTER_NAME":
				return _expeditionCharacter.StaticData.Name;
			case "EXPEDITION_CHARACTER_SURNAME":
				return _expeditionCharacter.StaticData.Surname;
			case "EXPEDITION_CHARACTER_FULL_NAME":
				return string.Concat(_expeditionCharacter.StaticData.Name, " ", _expeditionCharacter.StaticData.Surname);
			}
			if (param.StartsWith("EXPEDITION_CHARACTER_SEX"))
			{
				return GetOptionBySex(param);
			}
		}
		else if (param.StartsWith("TERM_"))
		{
			if (_terms.Count == 0)
			{
				return null;
			}
			int num3 = param.IndexOf("_", StringComparison.Ordinal) + 1;
			int num4 = Convert.ToInt32(param.Substring(num3, param.Length - num3));
			if (_terms == null)
			{
				Debug.LogErrorFormat("This term {0} is trying to set term {1} but Terms connection is not connected in node", _term.mTerm, num4);
				return null;
			}
			if (_terms.Count <= num4)
			{
				Debug.LogErrorFormat("This term {0} is trying to set term {1} which was not provided in node", _term.mTerm, num4);
				return null;
			}
			return _terms[num4];
		}
		return null;
	}

	private string GetOptionBySex(string text)
	{
		string[] array = text.Substring(text.IndexOf("|", StringComparison.Ordinal) + 1).Split('/');
		if (_expeditionCharacter.StaticData.Female)
		{
			return array[0];
		}
		return array[1];
	}
}
