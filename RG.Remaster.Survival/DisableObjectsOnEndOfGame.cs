using System.Collections.Generic;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.Remaster.Survival;

public class DisableObjectsOnEndOfGame : MonoBehaviour
{
	[SerializeField]
	private bool _disableObjectsOnEndgame;

	[SerializeField]
	private List<GameObject> _objectsToDisableOnEndgameList;

	[SerializeField]
	private EndOfDayListenerList _eodListenerList;

	[SerializeField]
	private EndGameData _endGameData;

	[SerializeField]
	private GlobalBoolVariable _isAbsence;

	[SerializeField]
	private GlobalBoolVariable _isHatchVisible;

	[SerializeField]
	private GlobalBoolVariable _isScavengeOnly;

	private void Awake()
	{
		if (_disableObjectsOnEndgame && _objectsToDisableOnEndgameList.Count != 0)
		{
			if (_isScavengeOnly != null && _isScavengeOnly.Value)
			{
				DeactivateObjectsOnEndgame(_objectsToDisableOnEndgameList);
			}
			else
			{
				_eodListenerList.RegisterOnEndOfDay(OnEndOfDay, "PrepareSystem", 999, this);
			}
		}
	}

	private void OnDestroy()
	{
		_eodListenerList.UnregisterOnEndOfDay(OnEndOfDay, "PrepareSystem");
	}

	private void DeactivateObjectsOnEndgame(List<GameObject> objects)
	{
		if (objects != null)
		{
			for (int i = 0; i < objects.Count; i++)
			{
				objects[i].SetActive(value: false);
			}
		}
	}

	private void OnEndOfDay()
	{
		if ((_endGameData != null && _endGameData.RuntimeData.ShouldEndGame) || (_isAbsence != null && _isAbsence.Value) || (_isHatchVisible != null && _isHatchVisible.Value))
		{
			DeactivateObjectsOnEndgame(_objectsToDisableOnEndgameList);
		}
	}
}
