using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Core;
using RG.VirtualInput;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawCrossOnClick : MonoBehaviour
{
	[SerializeField]
	private GameObject _cross;

	private Camera _camera;

	[SerializeField]
	private GlobalBoolVariable _secretBonusVariable;

	[SerializeField]
	private EndOfDayListenerList _endOfDay;

	[SerializeField]
	private SecondsItem _map;

	private void Awake()
	{
		_camera = Camera.main;
		_endOfDay.RegisterOnEndOfDay(OnEndOfDay, "Reset", 1, this);
	}

	private void OnDestroy()
	{
		_endOfDay.UnregisterOnEndOfDay(OnEndOfDay, "Reset");
	}

	private void OnDisable()
	{
		_cross.gameObject.SetActive(value: false);
	}

	private void OnEndOfDay()
	{
		if (_map.IsDamaged() || !_map.RuntimeData.IsAvailable)
		{
			_secretBonusVariable.Value = false;
			_cross.gameObject.SetActive(value: false);
		}
	}

	private void OnMouseDown()
	{
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			Vector3 position = _camera.ScreenToWorldPoint(Singleton<VirtualInputManager>.Instance.GetMousePosition());
			position.z = 0f;
			_cross.transform.position = position;
			_cross.gameObject.SetActive(value: true);
			_secretBonusVariable.Value = true;
		}
	}
}
