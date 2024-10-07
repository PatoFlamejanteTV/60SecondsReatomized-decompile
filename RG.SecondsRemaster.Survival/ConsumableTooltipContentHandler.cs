using I2.Loc;
using RG.Parsecs.Common;
using RG.SecondsRemaster.Core;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class ConsumableTooltipContentHandler : TooltipContentHandler
{
	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private TextMeshProUGUI _additionalText;

	[SerializeField]
	private LocalizedString _color;

	private const string CONSUMABLE_TITLE_FORMAT = "{0}:";

	private const string ADDITIONAL_TEXT_FORMAT = "<color={0}>{1}: {2}</color={0}>";

	public override void HandleContent(TooltipContent content)
	{
		if ((bool)(content as ConsumableTooltipContent))
		{
			HandleConsumableTooltipContent((ConsumableTooltipContent)content);
		}
	}

	private void HandleConsumableTooltipContent(ConsumableTooltipContent content)
	{
		SecondsConsumableRemedium consumable = content.Consumable;
		_text.text = $"{content.GeneralInfo}:";
		_additionalText.text = string.Format("<color={0}>{1}: {2}</color={0}>", _color, content.ContainerName, consumable.RuntimeData.Amount);
		_text.gameObject.GetComponent<Localize>().OnLocalize();
		_additionalText.gameObject.GetComponent<Localize>().OnLocalize();
	}
}
