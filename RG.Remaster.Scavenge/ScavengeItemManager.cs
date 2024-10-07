using System.Collections.Generic;
using UnityEngine;

namespace RG.Remaster.Scavenge;

public class ScavengeItemManager : MonoBehaviour
{
	private static ScavengeItemManager _instance;

	private List<ScavengeItemController> _scavengeItems = new List<ScavengeItemController>();

	public static ScavengeItemManager Instance
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

	public List<ScavengeItemController> ScavengeItems => _scavengeItems;

	private static void FindInstanceInScene()
	{
		_instance = Object.FindObjectOfType<ScavengeItemManager>();
		if (_instance == null)
		{
			Debug.LogError("No ScavengeItemManager in scene!");
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	public void RegisterController(ScavengeItemController controller)
	{
		if (controller != null && !_scavengeItems.Contains(controller))
		{
			_scavengeItems.Add(controller);
		}
	}

	public void UnregisterController(ScavengeItemController controller)
	{
		if (controller != null && _scavengeItems.Contains(controller))
		{
			_scavengeItems.Remove(controller);
		}
	}
}
