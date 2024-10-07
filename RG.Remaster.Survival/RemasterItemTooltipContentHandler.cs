using System;
using I2.Loc;
using RG.Parsecs.Common;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Core;
using TMPro;
using UnityEngine;

namespace RG.Remaster.Survival;

public class RemasterItemTooltipContentHandler : TooltipContentHandler
{
	[Tooltip("Text handler of tooltip.")]
	[SerializeField]
	private TextMeshProUGUI _text;

	[Tooltip("Additional text handler of tooltip.")]
	[SerializeField]
	private TextMeshProUGUI _additionalText;

	[SerializeField]
	private LocalizedString _color;

	private string _nameInfo;

	private string _itemInfo;

	private string _termString;

	private const string ADDITIONAL_MESSAGE_FORMAT = "<color={1}>{0}</color={1}>";

	public override void HandleContent(TooltipContent content)
	{
		if ((bool)(content as ItemTooltipContent))
		{
			HandleItemTooltipContent((ItemTooltipContent)content);
		}
	}

	private void HandleItemTooltipContent(ItemTooltipContent content)
	{
		if (!(_text != null) || !(content.Item != null))
		{
			return;
		}
		if (content.Item is Item)
		{
			Item item = (Item)content.Item;
			if (item.RuntimeData.IsDamaged && !string.IsNullOrEmpty(content.DamagedItemName))
			{
				_nameInfo = content.DamagedItemName;
			}
			else
			{
				_nameInfo = item.BaseStaticData.Name;
			}
		}
		else if (content.Item is SecondsRemedium)
		{
			SecondsRemedium secondsRemedium = (SecondsRemedium)content.Item;
			if (secondsRemedium.SecondsRemediumRuntimeData.IsDamaged && !string.IsNullOrEmpty(content.DamagedItemName))
			{
				_nameInfo = content.DamagedItemName;
			}
			else
			{
				_nameInfo = secondsRemedium.BaseStaticData.Name;
			}
		}
		else if (content.Item is ConsumableRemedium)
		{
			ConsumableRemedium consumableRemedium = (ConsumableRemedium)content.Item;
			_itemInfo = string.Empty;
			if (content.ItemNamesWithLevel != null && consumableRemedium.RuntimeData.Level - 1 < content.ItemNamesWithLevel.Count)
			{
				_nameInfo = string.Format(content.ItemNamesWithLevel[consumableRemedium.RuntimeData.Level - 1], consumableRemedium.RuntimeData.Amount.ToString());
			}
		}
		else if (content.Item is Remedium)
		{
			Remedium remedium = (Remedium)content.Item;
			_itemInfo = string.Empty;
			if (content.ItemNamesWithLevel != null && remedium.RuntimeData.Level - 1 < content.ItemNamesWithLevel.Count)
			{
				_nameInfo = content.ItemNamesWithLevel[remedium.RuntimeData.Level - 1];
			}
		}
		if (string.IsNullOrEmpty(content.TooltipTerm.Term))
		{
			_termString = string.Empty;
		}
		else
		{
			_termString = content.TooltipTerm.Term;
		}
		if (content.Item is Item || content.Item is Remedium)
		{
			_text.text = _nameInfo;
			_text.gameObject.GetComponent<Localize>().OnLocalize();
			if (!string.IsNullOrEmpty(_termString))
			{
				_additionalText.text = string.Format("<color={1}>{0}</color={1}>", _termString, _color);
				_additionalText.gameObject.SetActive(value: true);
			}
			else
			{
				_additionalText.text = string.Empty;
				_additionalText.gameObject.SetActive(value: false);
			}
		}
		else
		{
			_text.text = $"{_nameInfo} {_itemInfo}{Environment.NewLine}{_termString}";
			_text.gameObject.GetComponent<Localize>().OnLocalize();
		}
	}
}
