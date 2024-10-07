using System.Collections.Generic;
using I2.Loc;
using Rewired;
using RG.Parsecs.Common;
using RG.SecondsRemaster.Inputs;
using RG.VirtualInput;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class ControlRemapController : MonoBehaviour
{
	[SerializeField]
	[Tooltip("TMP field that displays current control assignment.")]
	private TextMeshProUGUI _valueText;

	[SerializeField]
	[ActionIdProperty(typeof(RG.SecondsRemaster.Inputs.RewiredConsts.Action))]
	[Tooltip("Rewired Action that will be used for this controller.")]
	private int _actionId;

	[SerializeField]
	[Tooltip("Map Category to which this Action belongs to.")]
	private EMapCategory _mapCategory;

	[SerializeField]
	[Tooltip("Axis Contribution for Axis type Actions. For Button type Actions leave 'Positive'.")]
	private Pole _axisContribution;

	[SerializeField]
	[Tooltip("What types of controllers can be used for mapping the control?")]
	private ControllerType[] _allowedControllerTypes;

	[SerializeField]
	private LocalizedString _waitingForButtonPressTerm;

	[SerializeField]
	private ClosePanelOnCancelPress _controlsPanel;

	[SerializeField]
	[ActionIdProperty(typeof(RG.SecondsRemaster.Inputs.RewiredConsts.Action))]
	[Tooltip("Rewired Action that will be used for this controller.")]
	private List<int> _additionalConfictsActionId = new List<int>();

	[SerializeField]
	private VirtualInputButton _virtualInputButton;

	private static VirtualInputButton _lastUsedButton;

	private Player _player;

	private List<InputMapper> _mappers;

	private ActionElementMap _currentElementMap;

	private bool _assigningInputs;

	private bool _clearingMappers;

	private void Awake()
	{
		_player = ReInput.players.GetPlayer(0);
		ReInput.ControllerConnectedEvent += ReInput_ControllerConnectedEvent;
	}

	public void OnDestroy()
	{
		ReInput.ControllerConnectedEvent -= ReInput_ControllerConnectedEvent;
	}

	private void ReInput_ControllerConnectedEvent(ControllerStatusChangedEventArgs obj)
	{
		_valueText.text = GetElementIdentifierName();
	}

	private void OnEnable()
	{
		_valueText.text = GetElementIdentifierName();
		if (_virtualInputButton != null)
		{
			_virtualInputButton.SelectionPriority = 0;
		}
	}

	private string GetElementIdentifierName()
	{
		ActionElementMap actionElementMapForThisControl = GetActionElementMapForThisControl();
		if (actionElementMapForThisControl == null)
		{
			return "";
		}
		return actionElementMapForThisControl.elementIdentifierName;
	}

	private ActionElementMap GetActionElementMapForThisControl()
	{
		List<ActionElementMap> list = new List<ActionElementMap>();
		_player.controllers.maps.GetElementMapsWithAction(_actionId, skipDisabledMaps: false, list);
		for (int i = 0; i < list.Count; i++)
		{
			ActionElementMap actionElementMap = list[i];
			if (actionElementMap.controllerMap.categoryId == (int)_mapCategory && (actionElementMap.controllerMap.controllerType != 0 || ((Application.systemLanguage != SystemLanguage.French || actionElementMap.controllerMap.layoutId == 1) && (Application.systemLanguage == SystemLanguage.French || actionElementMap.controllerMap.layoutId != 1))))
			{
				if (actionElementMap.elementType == ControllerElementType.Axis)
				{
					return actionElementMap;
				}
				if (actionElementMap.elementType == ControllerElementType.Button && actionElementMap.axisContribution == _axisContribution)
				{
					return actionElementMap;
				}
			}
		}
		return null;
	}

	public void ReAssignButton()
	{
		if (_assigningInputs)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < _allowedControllerTypes.Length; i++)
		{
			if (_allowedControllerTypes[i] != ControllerType.Joystick || _player.controllers.joystickCount != 0)
			{
				num++;
			}
		}
		if (num == 0)
		{
			return;
		}
		_controlsPanel.SetCloseActionBlocked(block: true);
		if (_mappers == null)
		{
			_mappers = new List<InputMapper>();
		}
		_valueText.text = _waitingForButtonPressTerm;
		_currentElementMap = GetActionElementMapForThisControl();
		for (int j = 0; j < _allowedControllerTypes.Length; j++)
		{
			if (_allowedControllerTypes[j] != ControllerType.Joystick || _player.controllers.joystickCount != 0)
			{
				_mappers.Add(StartMapperForControl(_allowedControllerTypes[j]));
			}
		}
		_assigningInputs = true;
		Singleton<VirtualInputManager>.Instance.IsGamepadInputBlocked = true;
	}

	private InputMapper StartMapperForControl(ControllerType controllerType)
	{
		InputMapper inputMapper = new InputMapper();
		inputMapper.options.checkForConflicts = true;
		inputMapper.options.allowButtons = _currentElementMap.elementType == ControllerElementType.Button;
		inputMapper.options.allowAxes = _currentElementMap.elementType == ControllerElementType.Axis;
		inputMapper.options.timeout = 0f;
		inputMapper.InputMappedEvent += OnControlMapped;
		inputMapper.CanceledEvent += OnControlMappingCanceled;
		inputMapper.ConflictFoundEvent += MapperOnConflictFoundEvent;
		InputMapper.Context context = new InputMapper.Context();
		context.actionId = _currentElementMap.actionId;
		if (_currentElementMap.elementType == ControllerElementType.Axis)
		{
			context.actionRange = _currentElementMap.axisRange;
		}
		else if (_currentElementMap.elementType == ControllerElementType.Button)
		{
			context.actionRange = ((_currentElementMap.axisContribution == Pole.Positive) ? AxisRange.Positive : AxisRange.Negative);
		}
		foreach (ControllerMap item in _player.controllers.maps.GetAllMapsInCategory((int)_mapCategory))
		{
			if (item.controllerType != controllerType)
			{
				continue;
			}
			if (controllerType == ControllerType.Keyboard)
			{
				if (item.layoutId == ((Application.systemLanguage == SystemLanguage.French) ? 1 : 0))
				{
					context.controllerMap = item;
				}
			}
			else
			{
				context.controllerMap = item;
			}
		}
		if (_lastUsedButton != null)
		{
			_lastUsedButton.SelectionPriority = 0;
		}
		if (_virtualInputButton != null)
		{
			_virtualInputButton.SelectionPriority = 1;
			_lastUsedButton = _virtualInputButton;
		}
		inputMapper.Start(context);
		return inputMapper;
	}

	private void MapperOnConflictFoundEvent(InputMapper.ConflictFoundEventData obj)
	{
		bool flag = false;
		bool flag2 = false;
		foreach (ElementAssignmentConflictInfo conflict in obj.conflicts)
		{
			if (conflict.controllerMap.categoryId == obj.assignment.controllerMap.categoryId && conflict.controllerMap.layoutId == obj.assignment.controllerMap.layoutId)
			{
				obj.responseCallback(InputMapper.ConflictResponse.Cancel);
				flag = true;
				break;
			}
		}
		if (_additionalConfictsActionId.Count > 0)
		{
			foreach (ElementAssignmentConflictInfo conflict2 in obj.conflicts)
			{
				if (_additionalConfictsActionId.Contains(conflict2.actionId))
				{
					obj.responseCallback(InputMapper.ConflictResponse.Cancel);
					flag2 = true;
					break;
				}
			}
		}
		if (!flag && !flag2)
		{
			obj.responseCallback(InputMapper.ConflictResponse.Add);
		}
	}

	private void ReAllowAssignment()
	{
		_assigningInputs = false;
		_controlsPanel.SetCloseActionBlocked(block: false);
	}

	private void OnControlMappingCanceled(InputMapper.CanceledEventData obj)
	{
		if (!_clearingMappers)
		{
			_valueText.text = GetElementIdentifierName();
			ClearAllMappers();
			Invoke("ReAllowAssignment", 0.1f);
			Singleton<VirtualInputManager>.Instance.IsGamepadInputBlocked = false;
		}
	}

	private void OnControlMapped(InputMapper.InputMappedEventData obj)
	{
		_currentElementMap.controllerMap.DeleteElementMap(_currentElementMap.id);
		_currentElementMap = null;
		_valueText.text = GetElementIdentifierName();
		ClearAllMappers();
		Invoke("ReAllowAssignment", 0.1f);
		Singleton<VirtualInputManager>.Instance.IsGamepadInputBlocked = false;
	}

	private void ClearAllMappers()
	{
		_clearingMappers = true;
		for (int i = 0; i < _mappers.Count; i++)
		{
			_mappers[i].Clear();
		}
		_mappers.Clear();
		_clearingMappers = false;
	}
}
