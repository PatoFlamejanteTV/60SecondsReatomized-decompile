using RG.Parsecs.EventEditor;
using UnityEngine;

public class DisableGameObjectOnDemo : MonoBehaviour
{
	[SerializeField]
	private GlobalBoolVariable _isDemoVariable;

	public void Awake()
	{
		if (_isDemoVariable != null && _isDemoVariable.Value)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
