using I2.Loc;
using RG.Parsecs.Common;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class DifficultyTooltipContentHandler : TooltipContentHandler
{
	[SerializeField]
	private Color _tooltipHeaderColor;

	[SerializeField]
	private Color _tooltipParamColor;

	[SerializeField]
	private TextMeshProUGUI[] _headersTextFields;

	[SerializeField]
	private TextMeshProUGUI[] _paramTextsFields;

	[SerializeField]
	[Tooltip("Tooltip headers, the number of elements must match the number of content strings!")]
	private LocalizedString[] _difficultyHeaders;

	private string _tooltipHeaderColorHex;

	private string _tooltipParamColorHex;

	private const string DIFFICULTY_TOOLTIP_HEADER_FORMAT = "<uppercase><color=#{0}>{1}:</uppercase></color>";

	private const string DIFFICULTY_TOOLTIP_PARAM_FORMAT = "<color=#{0}>{1}</color>";

	public override void HandleContent(TooltipContent content)
	{
		DifficultyTooltipContent difficultyTooltipContent = content as DifficultyTooltipContent;
		if (difficultyTooltipContent != null && _difficultyHeaders.Length == difficultyTooltipContent.DifficultyLevelTexts.Length)
		{
			AppendTexts(_headersTextFields, _difficultyHeaders, "<uppercase><color=#{0}>{1}:</uppercase></color>", _tooltipHeaderColorHex);
			AppendTexts(_paramTextsFields, difficultyTooltipContent.DifficultyLevelTexts, "<color=#{0}>{1}</color>", _tooltipParamColorHex);
		}
	}

	private void Awake()
	{
		_tooltipHeaderColorHex = ColorUtility.ToHtmlStringRGB(_tooltipHeaderColor);
		_tooltipParamColorHex = ColorUtility.ToHtmlStringRGB(_tooltipParamColor);
	}

	private void AppendTexts(TextMeshProUGUI[] textFields, LocalizedString[] stringsToAppend, string format, string hexColor)
	{
		for (int i = 0; i < textFields.Length; i++)
		{
			textFields[i].text = string.Format(format, hexColor, stringsToAppend[i]);
		}
	}
}
