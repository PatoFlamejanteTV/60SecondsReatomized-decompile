using I2.Loc;
using RG.Parsecs.Common;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class ActionChoiceTooltipContent : TooltipContent
{
	[SerializeField]
	private Character _character;

	[SerializeField]
	private IItem _item;

	[SerializeField]
	private LocalizedString _term;

	public Character Character => _character;

	public IItem Item => _item;

	public LocalizedString Term => _term;

	public override bool IsValid()
	{
		return true;
	}

	public void SetCharacterContent(Character character)
	{
		_character = character;
		_item = null;
		_term = null;
	}

	public void SetItemContent(IItem item)
	{
		_item = item;
		_character = null;
		_term = null;
	}

	public void SetTermContent(LocalizedString term)
	{
		_term = term;
		_character = null;
		_item = null;
	}
}
