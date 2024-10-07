using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField]
	private GameObject _template;

	[SerializeField]
	private bool _autoRemove;

	[SerializeField]
	private Vector3 _customRange = Vector3.zero;

	[SerializeField]
	private Vector3 _randomRotation = Vector3.zero;

	private bool _used;

	private GameObject SpawnedObject { get; set; }

	public GameObject Template
	{
		get
		{
			return _template;
		}
		set
		{
			_template = value;
		}
	}

	private void Update()
	{
		if (_autoRemove)
		{
			_autoRemove = false;
			Object.Destroy(base.gameObject);
		}
	}

	public bool CanSpawn(GameObject template, bool log)
	{
		if (log)
		{
			Debug.LogFormat("!_used: {0} && (template != null: {1} || _template != null {2})", !_used, template != null, _template != null);
		}
		if (!_used)
		{
			if (!(template != null))
			{
				return _template != null;
			}
			return true;
		}
		return false;
	}

	public bool Spawn(GameObject template = null)
	{
		return SpawnGameObject(template) != null;
	}

	public GameObject SpawnGameObject(GameObject template = null)
	{
		if (CanSpawn(template, log: false))
		{
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			Quaternion quaternion = Quaternion.Euler(new Vector3(eulerAngles.x + Random.Range(0f, _randomRotation.x), eulerAngles.y + Random.Range(0f, _randomRotation.y), eulerAngles.z + Random.Range(0f, _randomRotation.z)));
			GameObject gameObject = Object.Instantiate((template == null) ? _template : template, base.transform.position, quaternion);
			if (!gameObject)
			{
				Debug.LogFormat("Obj after instantiate is null! template: {0}, _template: {1}, transform.position: {2}, randRot: {3}", template, _template, base.transform.position, quaternion);
			}
			return gameObject;
		}
		CanSpawn(template, log: true);
		return null;
	}
}
