using System.Collections.Generic;
using RG.Parsecs.Common;
using RG.VirtualInput;
using UnityEngine;

namespace RG.Remaster.Common;

public class OnSpriteClickAnimationPlayer : MonoBehaviour
{
	private const int RIGHT_MOUSE_BUTTON = 1;

	[SerializeField]
	private bool _useRightClick;

	[SerializeField]
	private bool _useLeftClick = true;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private List<string> _animationTriggerNames;

	private void Start()
	{
		ValidateSetup();
	}

	private void ValidateSetup()
	{
		if (_animator == null)
		{
			Debug.LogError("Animator not set up properly in " + base.gameObject.name);
		}
		if (_animationTriggerNames == null || _animationTriggerNames.Count < 1 || _animationTriggerNames.Contains(null))
		{
			Debug.LogError("Animation Trigger Names not set up properly in " + base.gameObject.name);
		}
	}

	private void OnMouseUpAsButton()
	{
		if (_useLeftClick && !Singleton<VirtualInputManager>.Instance.IsPointerOverGameObject())
		{
			PlayRandomAnimation();
		}
	}

	private void OnMouseOver()
	{
		if (_useRightClick && Input.GetMouseButtonUp(1) && !Singleton<VirtualInputManager>.Instance.IsPointerOverGameObject())
		{
			PlayRandomAnimation();
		}
	}

	private void PlayRandomAnimation()
	{
		int index = ((_animationTriggerNames.Count > 1) ? Random.Range(0, _animationTriggerNames.Count) : 0);
		_animator.SetTrigger(_animationTriggerNames[index]);
	}
}
