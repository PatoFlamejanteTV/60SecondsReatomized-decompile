using RG.Parsecs.Survival;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class SpriteJournalContentDisplayer : JournalContentDisplayer<SpriteJournalContent>
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private HorizontalLayoutGroup _layoutGroup;

	public override int LinesAmount => 0;

	public override void SetContentData(JournalContent content)
	{
		if (content.Type == EJournalContentType.SPRITE)
		{
			SpriteJournalContent spriteJournalContent = (SpriteJournalContent)content;
			_image.sprite = spriteJournalContent.Sprite;
			switch (spriteJournalContent.Align)
			{
			case EventContentData.ESpriteAlign.LEFT:
				_layoutGroup.childAlignment = TextAnchor.UpperLeft;
				break;
			case EventContentData.ESpriteAlign.CENTER:
				_layoutGroup.childAlignment = TextAnchor.UpperCenter;
				break;
			case EventContentData.ESpriteAlign.RIGHT:
				_layoutGroup.childAlignment = TextAnchor.UpperRight;
				break;
			}
		}
	}
}
