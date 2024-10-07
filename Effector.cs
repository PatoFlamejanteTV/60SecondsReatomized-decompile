using UnityEngine;

public class Effector : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Particles prefab object to spawn")]
	protected Object _effector;

	[SerializeField]
	protected Transform _spawnPoint;

	[SerializeField]
	protected Vector3 _spawnPointOffset = Vector3.zero;

	public Vector3 SpawnPointOffset
	{
		get
		{
			return _spawnPointOffset;
		}
		set
		{
			_spawnPointOffset = value;
		}
	}

	private void Awake()
	{
		Initialize();
	}

	protected virtual void Initialize()
	{
		if (_spawnPoint == null)
		{
			_spawnPoint = base.transform;
		}
	}

	private void Start()
	{
	}

	public virtual void Activate()
	{
		Object.Instantiate(_effector, _spawnPoint.position + _spawnPointOffset, _spawnPoint.rotation);
	}
}
