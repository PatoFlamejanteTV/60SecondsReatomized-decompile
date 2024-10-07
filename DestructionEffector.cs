using FMODUnity;
using RG.Parsecs.Common;
using UnityEngine;

public class DestructionEffector : Effector
{
	[EventRef]
	[SerializeField]
	private string _soundName = string.Empty;

	[SerializeField]
	private int _startHitpoints = 100;

	private int _currentHitpoints;

	private void Awake()
	{
		base.Initialize();
		_currentHitpoints = _startHitpoints;
		base.SpawnPointOffset = new Vector3(0f, 1f, 0f);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Hit(int points = 10)
	{
		_currentHitpoints -= points;
		_currentHitpoints = Mathf.Clamp(_currentHitpoints, 0, _startHitpoints);
		if (_currentHitpoints <= 0)
		{
			Activate();
		}
	}

	public override void Activate()
	{
		base.Activate();
		if (!string.IsNullOrEmpty(_soundName))
		{
			AudioManager.PlaySoundAtPoint(_soundName, base.transform);
		}
		Object.Destroy(base.gameObject);
	}
}
