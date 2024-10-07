using System.Collections.Generic;
using RG.Parsecs.EventEditor;
using UnityEngine;
using UnityEngine.UI;

public class DisableButtonOnDemo : MonoBehaviour
{
	[SerializeField]
	private GlobalBoolVariable _isDemoVariable;

	[SerializeField]
	private Button _buttonToBlockOnDemo;

	[SerializeField]
	private List<GameObject> _additionalObjectsToActivateOnDemo = new List<GameObject>();

	private void Start()
	{
		if (!(_isDemoVariable != null) || !(_buttonToBlockOnDemo != null))
		{
			return;
		}
		_buttonToBlockOnDemo.interactable = !_isDemoVariable.Value;
		foreach (GameObject item in _additionalObjectsToActivateOnDemo)
		{
			item.SetActive(_isDemoVariable.Value);
		}
	}
}
