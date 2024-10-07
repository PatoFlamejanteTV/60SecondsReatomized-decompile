using System.Collections.Generic;
using UnityEngine;

namespace RG.Remaster.Survival;

public class SkinManager : MonoBehaviour
{
	private static SkinManager _instance;

	[SerializeField]
	private List<SkinController> _skinControllers = new List<SkinController>();

	public static SkinManager Instance
	{
		get
		{
			if (_instance == null)
			{
				FindInstanceInScene();
			}
			return _instance;
		}
	}

	private static void FindInstanceInScene()
	{
		_instance = Object.FindObjectOfType<SkinManager>();
		if (_instance == null)
		{
			Debug.LogError("No SkinManager in scene!");
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	public void RegisterSkinController(SkinController controller)
	{
		if (!_skinControllers.Contains(controller))
		{
			_skinControllers.Add(controller);
		}
	}

	public void UnregisterSkinController(SkinController controller)
	{
		if (_skinControllers.Contains(controller))
		{
			_skinControllers.Remove(controller);
		}
	}

	public void AllowChangingSkins(SkinDataList dataList)
	{
		for (int i = 0; i < _skinControllers.Count; i++)
		{
			if (_skinControllers[i].DataList != null && _skinControllers[i].DataList.Equals(dataList))
			{
				_skinControllers[i].AllowSkinChange();
			}
		}
	}

	public void ForceSkinUse(SkinDataList dataList, SkinId skinId)
	{
		for (int i = 0; i < _skinControllers.Count; i++)
		{
			if (_skinControllers[i].DataList != null && _skinControllers[i].DataList.Equals(dataList))
			{
				_skinControllers[i].ForceSkinUse(skinId);
			}
		}
	}
}
