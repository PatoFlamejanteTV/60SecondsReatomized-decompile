using System.Collections.Generic;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.Remaster.Menu;

public class MenuVisualsManager : MonoBehaviour
{
	[SerializeField]
	private GlobalBoolVariable _isContinueAvailable;

	[SerializeField]
	private NodeFunction _regularVisualsSetupFuntion;

	[SerializeField]
	private NodeFunction _postapoVisualsSetupFuntion;

	[SerializeField]
	private List<GameObject> _objectsToActivateInPostapo;

	public void Start()
	{
		if (_isContinueAvailable != null && _isContinueAvailable.Value && _postapoVisualsSetupFuntion != null)
		{
			if (_objectsToActivateInPostapo != null)
			{
				for (int i = 0; i < _objectsToActivateInPostapo.Count; i++)
				{
					_objectsToActivateInPostapo[i].SetActive(value: true);
				}
			}
			_postapoVisualsSetupFuntion.Execute(null);
		}
		else if (_regularVisualsSetupFuntion != null)
		{
			_regularVisualsSetupFuntion.Execute(null);
		}
		VisualsManager.Instance.RefreshVisualsStateCoroutine();
	}
}
