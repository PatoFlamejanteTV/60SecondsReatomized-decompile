using RG.Parsecs.Survival;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class SwitchButtonController : MonoBehaviour
{
	[SerializeField]
	private SurvivalData _survivalData;

	private Selectable _selectable;

	public void SetSelectable(Button selectable)
	{
		_selectable = selectable;
	}

	public void SetButtonInteractable(bool interactable)
	{
		if (_selectable != null)
		{
			_selectable.interactable = interactable;
		}
		_survivalData.DailyEventResolved = interactable;
	}
}
