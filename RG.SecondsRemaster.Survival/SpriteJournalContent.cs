using System;
using RG.Core.SaveSystem;
using RG.Parsecs.Common;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Serializable]
public sealed class SpriteJournalContent : JournalContent
{
	[SerializeField]
	private Sprite _sprite;

	[SerializeField]
	private EventContentData.ESpriteAlign _align;

	public Sprite Sprite => _sprite;

	public EventContentData.ESpriteAlign Align => _align;

	public SpriteJournalContent(Sprite sprite, EventContentData.ESpriteAlign align, int displayPriority)
		: base(displayPriority)
	{
		type = EJournalContentType.SPRITE;
		_sprite = sprite;
		_align = align;
	}

	public SpriteJournalContent(string serializedData, SaveEvent saveEvent)
		: base(saveEvent)
	{
		Deserialize(serializedData, saveEvent);
	}

	public override string Serialize()
	{
		return JsonUtility.ToJson(new SpriteJournalContentWrapper
		{
			DisplayOrder = displayOrder,
			DisplayPriority = displayPriority,
			GroupId = ((groupId != null) ? groupId.Guid : string.Empty),
			Type = type,
			Sprite = ((_sprite != null) ? _sprite.name : string.Empty),
			Align = _align
		});
	}

	public override void Deserialize(string data, SaveEvent saveEvent)
	{
		SpriteJournalContentWrapper spriteJournalContentWrapper = JsonUtility.FromJson<SpriteJournalContentWrapper>(data);
		DeserializeBaseWrapper(spriteJournalContentWrapper, saveEvent);
		if (!string.IsNullOrEmpty(spriteJournalContentWrapper.Sprite))
		{
			Sprite sprite = ContentManager.GetSprite(spriteJournalContentWrapper.Sprite);
			if (sprite == null)
			{
				Debug.LogError($"Cannot find sprite {spriteJournalContentWrapper.Sprite} in asset bundle");
			}
			else
			{
				_sprite = (string.IsNullOrEmpty(spriteJournalContentWrapper.Sprite) ? null : sprite);
			}
		}
		_align = spriteJournalContentWrapper.Align;
	}
}
