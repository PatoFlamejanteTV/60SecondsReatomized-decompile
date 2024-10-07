using System;
using RG.SecondsRemaster;
using UnityEngine;

[Serializable]
public class ControlMode
{
	[SerializeField]
	private bool _enabled = true;

	[SerializeField]
	private string _name = string.Empty;

	[SerializeField]
	private string _key = string.Empty;

	[SerializeField]
	private bool _mobile;

	[SerializeField]
	private bool _desktop = true;

	[SerializeField]
	private string _icon = string.Empty;

	[SerializeField]
	private EPlayerInput _scavengeControl;

	[SerializeField]
	private float _rotationSensitivity = 1f;

	public string Name => _name;

	public string Key => _key;

	public bool Enabled => _enabled;

	public string Icon => _icon;

	public EPlayerInput ScavengeControl => _scavengeControl;

	public float RotationSensitivity
	{
		get
		{
			return _rotationSensitivity;
		}
		set
		{
			_rotationSensitivity = value;
		}
	}

	public bool Mobile => _mobile;

	public bool IsGamepad()
	{
		return _scavengeControl == EPlayerInput.GAMEPAD;
	}
}
