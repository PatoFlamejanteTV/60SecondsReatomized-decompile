using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class SubPageController : PageController
{
	[SerializeField]
	private PageController _parentPage;

	public override PageController ParentPage
	{
		get
		{
			return _parentPage;
		}
		set
		{
			_parentPage = value;
		}
	}

	public override bool IsSubpage()
	{
		return true;
	}

	public override void Show()
	{
		if (ParentPage != null)
		{
			ParentPage.Show();
		}
		base.Show();
	}

	public override void Hide()
	{
		if (ParentPage != null)
		{
			ParentPage.Hide();
		}
		base.Hide();
	}
}
