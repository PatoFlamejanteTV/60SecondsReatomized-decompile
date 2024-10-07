using RG.Parsecs.Common;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class RemasterTextTooltipContentHandler : TooltipContentHandler
{
	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private Color _tooltipColor;

	private const string TOOLTIP_FORMAT = "<color=#{0}>{1}</color>";

	private string _tooltipColorHex;

	private void Awake()
	{
		_tooltipColorHex = ColorUtility.ToHtmlStringRGB(_tooltipColor);
	}

	public override void HandleContent(TooltipContent content)
	{
		TextTooltipContent textTooltipContent = content as TextTooltipContent;
		if (textTooltipContent != null)
		{
			_text.text = $"<color=#{_tooltipColorHex}>{textTooltipContent.TextTerm}</color>";
		}
	}
}
