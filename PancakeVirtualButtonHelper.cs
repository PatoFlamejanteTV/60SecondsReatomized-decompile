using System.Collections.Generic;
using RG.Parsecs.Survival;
using RG.VirtualInput;
using UnityEngine;

public class PancakeVirtualButtonHelper : MonoBehaviour
{
	[SerializeField]
	private VirtualInputButton _virtualButton;

	[SerializeField]
	private List<GameObject> _pancakeSkins = new List<GameObject>();

	[SerializeField]
	private EndOfDayListenerList _endOfDayListener;

	public void Setup()
	{
		foreach (GameObject pancakeSkin in _pancakeSkins)
		{
			if (pancakeSkin.activeInHierarchy)
			{
				_virtualButton.Selectable.interactable = true;
				_virtualButton.SetPositionTransform(pancakeSkin.transform);
				return;
			}
		}
		_virtualButton.Selectable.interactable = false;
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
