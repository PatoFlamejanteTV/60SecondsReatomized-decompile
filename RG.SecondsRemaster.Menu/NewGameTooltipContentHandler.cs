using RG.Parsecs.Common;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class NewGameTooltipContentHandler : TooltipContentHandler
{
	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private Color _textColor;

	private string _textColorHex;

	private const string TEXT_FORMAT = "<color=#{0}>{1}</color>";

	public override void HandleContent(TooltipContent content)
	{
		NewGameTooltipContent newGameTooltipContent = content as NewGameTooltipContent;
		if (newGameTooltipContent != null)
		{
			_text.text = $"<color=#{_textColorHex}>{newGameTooltipContent.TextTerm}</color>";
		}
	}

	private void Awake()
	{
		_textColorHex = ColorUtility.ToHtmlStringRGB(_textColor);
	}
}
