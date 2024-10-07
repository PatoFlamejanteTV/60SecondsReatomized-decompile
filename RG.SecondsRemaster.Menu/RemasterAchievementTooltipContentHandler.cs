using RG.Parsecs.Common;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class RemasterAchievementTooltipContentHandler : TooltipContentHandler
{
	[SerializeField]
	private TextMeshProUGUI _titleText;

	[SerializeField]
	private TextMeshProUGUI _descriptionText;

	[SerializeField]
	private Color _titleColor;

	[SerializeField]
	private Color _descriptionColor;

	private string _titleColorHex;

	private string _descriptionColorHex;

	private const string TEXT_FORMAT = "<color=#{1}>{0}</color>";

	public override void HandleContent(TooltipContent content)
	{
		RemasterAchievementTooltipContent remasterAchievementTooltipContent = content as RemasterAchievementTooltipContent;
		if (remasterAchievementTooltipContent != null)
		{
			_titleText.text = string.Format("<color=#{1}>{0}</color>", remasterAchievementTooltipContent.Achievement.title, _titleColorHex);
			_descriptionText.text = string.Format("<color=#{1}>{0}</color>", remasterAchievementTooltipContent.Achievement.description, _descriptionColorHex);
		}
	}

	private void Awake()
	{
		_titleColorHex = ColorUtility.ToHtmlStringRGB(_titleColor);
		_descriptionColorHex = ColorUtility.ToHtmlStringRGB(_descriptionColor);
	}
}
