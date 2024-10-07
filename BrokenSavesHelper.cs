using RG.Core.SaveSystem;
using RG.Parsecs.EventEditor;
using UnityEngine;

public class BrokenSavesHelper : MonoBehaviour
{
	[SerializeField]
	private GlobalBoolVariable _continueAvailable;

	private const string GlobalGameDataSaveName = "GlobalGameData";

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
		if (StorageDataManager.SURVIVAL_SAVE_CORRUPTED)
		{
			StorageDataManager.SURVIVAL_SAVE_CORRUPTED = false;
			Debug.Log("Survival save was corrupted and deleted, need to change continue available");
			_continueAvailable.Value = false;
			StorageDataManager.TheInstance.Save("GlobalGameData", delegate
			{
				Debug.Log("Saved Global Game Data");
			}, delegate
			{
				Debug.Log("Failed to save Global Game Data");
			});
		}
	}
}
