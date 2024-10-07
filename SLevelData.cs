using System;
using DunGen.Graph;
using UnityEngine;

[Serializable]
public struct SLevelData
{
	[SerializeField]
	private GameObject _housePrefab;

	[SerializeField]
	private DungeonFlow _flowData;

	[SerializeField]
	private GameObject _playerTemplate;

	[SerializeField]
	private GameObject _doloresTemplate;

	[SerializeField]
	private string _levelPrefabsPath;

	[SerializeField]
	private GameObject[] _levelPrefabs;

	public GameObject HousePrefab
	{
		get
		{
			return _housePrefab;
		}
		set
		{
			_housePrefab = value;
		}
	}

	public DungeonFlow FlowData
	{
		get
		{
			return _flowData;
		}
		set
		{
			_flowData = value;
		}
	}

	public GameObject PlayerTemplate
	{
		get
		{
			return _playerTemplate;
		}
		set
		{
			_playerTemplate = value;
		}
	}

	public GameObject DoloresTemplate
	{
		get
		{
			return _doloresTemplate;
		}
		set
		{
			_doloresTemplate = value;
		}
	}

	public GameObject GetLevelPrefab(int index)
	{
		if (_levelPrefabs != null && _levelPrefabs.Length != 0 && _levelPrefabs.Length > index)
		{
			return _levelPrefabs[index];
		}
		return null;
	}

	public GameObject GetRandomLevelPrefab()
	{
		if (_levelPrefabs != null && _levelPrefabs.Length != 0)
		{
			return _levelPrefabs[UnityEngine.Random.Range(0, _levelPrefabs.Length)];
		}
		return null;
	}
}
