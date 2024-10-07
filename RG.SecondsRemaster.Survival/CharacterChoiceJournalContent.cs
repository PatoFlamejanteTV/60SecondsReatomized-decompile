using System;
using System.Collections.Generic;
using I2.Loc;
using RG.Core.SaveSystem;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public sealed class CharacterChoiceJournalContent : JournalContent
{
	[SerializeField]
	private List<Character> _characters;

	[SerializeField]
	private LocalizedString _callToActionTerm;

	public List<Character> Characters => _characters;

	public LocalizedString CallToActionTerm => _callToActionTerm;

	public CharacterChoiceJournalContent(List<Character> characters, LocalizedString callToActionTerm)
		: base(int.MinValue)
	{
		type = EJournalContentType.CHARACTER_CHOICE;
		_characters = characters;
		_callToActionTerm = callToActionTerm;
	}

	public CharacterChoiceJournalContent(string serializedData, SaveEvent saveEvent)
		: base(saveEvent)
	{
		Deserialize(serializedData, saveEvent);
	}

	public override string Serialize()
	{
		CharacterChoiceJournalContentWrapper characterChoiceJournalContentWrapper = new CharacterChoiceJournalContentWrapper
		{
			DisplayOrder = displayOrder,
			DisplayPriority = displayPriority,
			GroupId = ((groupId != null) ? groupId.Guid : string.Empty),
			Type = type,
			Characters = new List<string>(),
			CallToActionTerm = _callToActionTerm
		};
		for (int i = 0; i < _characters.Count; i++)
		{
			characterChoiceJournalContentWrapper.Characters.Add(_characters[i].Guid);
		}
		return JsonUtility.ToJson(characterChoiceJournalContentWrapper);
	}

	public override void Deserialize(string data, SaveEvent saveEvent)
	{
		CharacterChoiceJournalContentWrapper characterChoiceJournalContentWrapper = JsonUtility.FromJson<CharacterChoiceJournalContentWrapper>(data);
		DeserializeBaseWrapper(characterChoiceJournalContentWrapper, saveEvent);
		for (int i = 0; i < characterChoiceJournalContentWrapper.Characters.Count; i++)
		{
			_characters.Add((Character)saveEvent.GetReferenceObjectByID(characterChoiceJournalContentWrapper.Characters[i]));
		}
		_callToActionTerm = characterChoiceJournalContentWrapper.CallToActionTerm;
	}
}
