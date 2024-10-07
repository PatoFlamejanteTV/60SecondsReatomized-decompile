using UnityEngine;

public class ShelterEffector : Effector
{
	private GameObject _dropParticlesGameObject;

	private ParticleSystem _dropParticlesSystem;

	private void Awake()
	{
		Initialize();
	}

	private void Start()
	{
		_dropParticlesGameObject = Object.Instantiate(_effector, _spawnPoint.position + _spawnPointOffset, _spawnPoint.rotation) as GameObject;
		_dropParticlesSystem = _dropParticlesGameObject.GetComponent<ParticleSystem>();
		_dropParticlesGameObject.SetActive(value: false);
	}

	public override void Activate()
	{
		if (_dropParticlesSystem.isPlaying)
		{
			_dropParticlesSystem.time = 0f;
			_dropParticlesSystem.Simulate(0f, withChildren: true, restart: true);
			_dropParticlesSystem.Play();
		}
		else
		{
			_dropParticlesGameObject.SetActive(value: true);
		}
	}
}
