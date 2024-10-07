using System;
using I2.Loc;
using RG.Core.SaveSystem;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public sealed class TextIconJournalContent : JournalContent
{
	[SerializeField]
	private string _sign;

	[SerializeField]
	private LocalizedString _iconTerm;

	[SerializeField]
	private EventContentData.ETextIconContentType _iconType;

	[SerializeField]
	private int _amount;

	public EventContentData.ETextIconContentType IconType => _iconType;

	public LocalizedString IconTerm => _iconTerm;

	public int Amount => _amount;

	public TextIconJournalContent(LocalizedString iconTerm, int amount, EventContentData.ETextIconContentType iconType, int displayPriority)
		: base(displayPriority)
	{
		type = EJournalContentType.TEXT_ICON;
		_iconType = iconType;
		_iconTerm = iconTerm;
		_amount = amount;
	}

	public TextIconJournalContent(string serializedData, SaveEvent saveEvent)
		: base(saveEvent)
	{
		Deserialize(serializedData, saveEvent);
	}

	public override string Serialize()
	{
		return JsonUtility.ToJson(new TextIconJournalContentWrapper
		{
			DisplayOrder = displayOrder,
			DisplayPriority = displayPriority,
			GroupId = ((groupId != null) ? groupId.Guid : string.Empty),
			Type = type,
			Sign = _sign,
			IconTerm = _iconTerm,
			IconType = _iconType,
			Amount = _amount
		});
	}

	public override void Deserialize(string data, SaveEvent saveEvent)
	{
		TextIconJournalContentWrapper textIconJournalContentWrapper = JsonUtility.FromJson<TextIconJournalContentWrapper>(data);
		DeserializeBaseWrapper(textIconJournalContentWrapper, saveEvent);
		_sign = textIconJournalContentWrapper.Sign;
		_iconTerm = textIconJournalContentWrapper.IconTerm;
		_iconType = textIconJournalContentWrapper.IconType;
		_amount = textIconJournalContentWrapper.Amount;
	}
}
