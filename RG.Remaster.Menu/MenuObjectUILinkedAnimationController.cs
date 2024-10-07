using System.Collections.Generic;
using RG.Parsecs.Common;
using RG.VirtualInput;
using UnityEngine;
using UnityEngine.Events;

namespace RG.Remaster.Menu;

[RequireComponent(typeof(Animator))]
public class MenuObjectUILinkedAnimationController : MonoBehaviour
{
	private Animator _animator;

	private string SHOULDBEACTIVE_PARAMETER_NAME = "ShouldBeActive";

	[SerializeField]
	private UnityEvent _onSelect;

	[SerializeField]
	private UnityEvent _onDeselect;

	[SerializeField]
	private UnityEvent _onClick;

	[SerializeField]
	private bool _usesRandomizedAnimations;

	[Tooltip("If UsesRandomizedAnimations is set to true, the parameter to set will be chosen randomly from this list. In that case the list cannot be null nor empty and has to match with the AnimatorController.")]
	[SerializeField]
	private List<string> _parameterNames;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
	}

	public void SetShouldBeActive(bool value)
	{
		if (!(_animator != null))
		{
			return;
		}
		if (_usesRandomizedAnimations)
		{
			if (_parameterNames == null || _parameterNames.Count <= 0)
			{
				return;
			}
			if (value)
			{
				_animator.SetBool(_parameterNames[Random.Range(0, _parameterNames.Count)], value: true);
				return;
			}
			for (int i = 0; i < _parameterNames.Count; i++)
			{
				_animator.SetBool(_parameterNames[i], value: false);
			}
		}
		else
		{
			_animator.SetBool(SHOULDBEACTIVE_PARAMETER_NAME, value);
		}
	}

	private void OnMouseEnter()
	{
		if (!Singleton<VirtualInputManager>.Instance.IsPointerOverGameObject() && _onSelect != null)
		{
			_onSelect.Invoke();
		}
	}

	private void OnMouseExit()
	{
		if (!Singleton<VirtualInputManager>.Instance.IsPointerOverGameObject() && _onSelect != null)
		{
			_onDeselect.Invoke();
		}
	}

	private void OnMouseDown()
	{
		if (!Singleton<VirtualInputManager>.Instance.IsPointerOverGameObject())
		{
			_onClick.Invoke();
		}
	}
}
