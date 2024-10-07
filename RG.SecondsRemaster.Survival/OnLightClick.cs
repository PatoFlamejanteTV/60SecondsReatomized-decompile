using RG.Parsecs.Common;
using RG.VirtualInput;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class OnLightClick : MonoBehaviour
{
	[SerializeField]
	private FlickerLights _flickerLights;

	private void OnMouseDown()
	{
	}

	private void OnMouseUp()
	{
	}

	private void OnMouseOver()
	{
	}

	private void OnMouseEnter()
	{
	}

	private void OnMouseExit()
	{
	}

	private void OnMouseUpAsButton()
	{
		if (!Singleton<VirtualInputManager>.Instance.IsPointerOverGameObject())
		{
			_flickerLights.StartFlicking();
		}
	}
}
