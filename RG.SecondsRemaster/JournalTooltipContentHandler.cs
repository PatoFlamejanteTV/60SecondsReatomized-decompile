using RG.Parsecs.Common;
using RG.SecondsRemaster.Survival;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster;

public class JournalTooltipContentHandler : TooltipContentHandler
{
	[SerializeField]
	private TextMeshProUGUI _text;

	public override void HandleContent(TooltipContent content)
	{
		if (content is JournalTooltipContent)
		{
			HandleVisitorTooltipContent((JournalTooltipContent)content);
		}
	}

	private void HandleVisitorTooltipContent(JournalTooltipContent content)
	{
		if (_text != null)
		{
			if (string.IsNullOrEmpty(content.Name()))
			{
				_text.text = string.Empty;
			}
			else
			{
				_text.text = content.Name();
			}
		}
	}
}
