using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class ActionSubPageController : SubPageController
{
	[SerializeField]
	private GameObject _doodlesHolder;

	public void SetDoodlesHolder(GameObject doodlesHolder)
	{
		_doodlesHolder = doodlesHolder;
	}

	public override void Show()
	{
		base.Show();
		if (_doodlesHolder != null)
		{
			_doodlesHolder.SetActive(value: true);
		}
	}

	public override void Hide()
	{
		base.Hide();
		if (_doodlesHolder != null)
		{
			_doodlesHolder.SetActive(value: false);
		}
	}
}
