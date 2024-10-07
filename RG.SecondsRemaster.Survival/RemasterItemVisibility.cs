using RG.Parsecs.Survival;
using RG.SecondsRemaster.Core;

namespace RG.SecondsRemaster.Survival;

public class RemasterItemVisibility : ItemVisibility
{
	public override void RefreshDisplay()
	{
		SetItemActive(_item.BaseRuntimeData.IsAvailable);
	}

	private void SetItemActive(bool active)
	{
		bool damaged = false;
		if (_item is Item)
		{
			damaged = ((Item)_item).RuntimeData.IsDamaged;
		}
		else if (_item is SecondsRemedium)
		{
			damaged = ((SecondsRemedium)_item).SecondsRemediumRuntimeData.IsDamaged;
		}
		for (int i = 0; i < _itemContainers.Length; i++)
		{
			_itemContainers[i].SetActive(i + 1 == _item.BaseRuntimeData.Level && active, damaged);
		}
		_virtualButton.Selectable.interactable = active;
	}
}
