using System.Collections.Generic;
using RG.Parsecs.EventEditor;
using UnityEngine;
using UnityEngine.UI;

public class DisableToggleOnDemo : MonoBehaviour
{
	[SerializeField]
	private GlobalBoolVariable _isDemoVariable;

	[SerializeField]
	private Toggle _toggleToBlockOnDemo;

	[SerializeField]
	private List<GameObject> _additionalObjectsToActivateOnDemo = new List<GameObject>();

	private void Start()
	{
		if (!(_isDemoVariable != null) || !(_toggleToBlockOnDemo != null))
		{
			return;
		}
		_toggleToBlockOnDemo.interactable = !_isDemoVariable.Value;
		foreach (GameObject item in _additionalObjectsToActivateOnDemo)
		{
			item.SetActive(_isDemoVariable.Value);
		}
	}
}
