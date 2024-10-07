using Rewired;
using RG.Parsecs.Common;
using RG.VirtualInput;
using UnityEngine;

namespace RG.Remaster.Survival;

public class HatClickDetector : MonoBehaviour
{
	private HatController _controller;

	[SerializeField]
	private bool _useRightClick = true;

	[SerializeField]
	private bool _useLeftClick;

	private Player _player;

	private bool isMouseOver;

	private void Awake()
	{
		_player = ReInput.players.GetPlayer(0);
	}

	private void Start()
	{
		SetUpControllerReference();
	}

	private void SetUpControllerReference()
	{
		Transform parent = base.transform;
		while (_controller == null && parent != null)
		{
			_controller = parent.GetComponent<HatController>();
			if (_controller == null)
			{
				parent = parent.parent;
			}
		}
	}

	private void OnMouseUpAsButton()
	{
		if (_useLeftClick)
		{
			CheckClick(moveForward: true);
		}
	}

	private void Update()
	{
		if (isMouseOver)
		{
			if (_useRightClick && _player.GetButtonUp(33))
			{
				CheckClick(moveForward: true);
			}
			else if (_useRightClick && _player.GetButtonUp(37))
			{
				CheckClick(moveForward: false);
			}
		}
	}

	private void OnMouseEnter()
	{
		isMouseOver = true;
	}

	private void OnMouseExit()
	{
		isMouseOver = false;
	}

	private void CheckClick(bool moveForward)
	{
		if (!Singleton<VirtualInputManager>.Instance.IsPointerOverGameObject() && _controller != null)
		{
			_controller.HatClicked(moveForward);
		}
	}
}
