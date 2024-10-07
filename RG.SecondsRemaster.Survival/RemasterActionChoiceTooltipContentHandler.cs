using RG.Parsecs.Common;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class RemasterActionChoiceTooltipContentHandler : TooltipContentHandler
{
	[Tooltip("Text handler of tooltip.")]
	[SerializeField]
	private TextMeshProUGUI _text;

	public override void HandleContent(TooltipContent content)
	{
		ActionChoiceTooltipContent actionChoiceTooltipContent = content as ActionChoiceTooltipContent;
		if (actionChoiceTooltipContent != null)
		{
			HandleTooltipContent(actionChoiceTooltipContent);
		}
	}

	private void HandleTooltipContent(ActionChoiceTooltipContent content)
	{
		if (content.Character != null)
		{
			_text.text = content.Character.StaticData.Name;
		}
		else if (content.Item != null)
		{
			_text.text = content.Item.BaseStaticData.Name;
		}
		else if (!string.IsNullOrEmpty(content.Term) && (string)content.Term != null)
		{
			_text.text = content.Term;
		}
	}
}
