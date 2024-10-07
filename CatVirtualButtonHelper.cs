using RG.Parsecs.Survival;
using RG.VirtualInput;
using UnityEngine;

public class CatVirtualButtonHelper : MonoBehaviour
{
	[SerializeField]
	private VirtualInputButton _virtualButton;

	[SerializeField]
	private EndOfDayListenerList _endOfDayListener;

	[SerializeField]
	private GameObject _catVisualGameObject;

	public void Setup()
	{
		if (_catVisualGameObject.activeInHierarchy)
		{
			_virtualButton.Selectable.interactable = true;
		}
		else
		{
			_virtualButton.Selectable.interactable = false;
		}
	}

	private void Awake()
	{
		_endOfDayListener.RegisterOnEndOfDay(Setup, "Visuals", 10, this, forceOrder: true);
	}

	private void OnDestroy()
	{
		_endOfDayListener.UnregisterOnEndOfDay(Setup, "Visuals");
	}
}
