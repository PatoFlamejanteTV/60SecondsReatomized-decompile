using RG.Core.Base;
using RG.Core.SaveSystem;
using UnityEngine;

namespace RG.SecondsRemaster;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/Challenges/New Current Challenge Data", fileName = "New Current Challenge Data")]
public class CurrentChallengeData : RGScriptableObject, ISaveable
{
	[SerializeField]
	private SaveEvent _saveEvent;

	[SerializeField]
	private CurrentChallengeRuntimeData _runtimeData;

	public string ID => Guid;

	public CurrentChallengeRuntimeData RuntimeData => _runtimeData;

	private void OnEnable()
	{
		Register();
	}

	private void OnDisable()
	{
		Unregister();
	}

	public string Serialize()
	{
		return JsonUtility.ToJson(new CurrentChallengeRuntimeDataWrapper
		{
			Challenge = (_runtimeData.Challenge ? _runtimeData.Challenge.Guid : string.Empty)
		});
	}

	public void Deserialize(string jsonData)
	{
		CurrentChallengeRuntimeDataWrapper currentChallengeRuntimeDataWrapper = JsonUtility.FromJson<CurrentChallengeRuntimeDataWrapper>(jsonData);
		_runtimeData.Challenge = ((!string.IsNullOrEmpty(currentChallengeRuntimeDataWrapper.Challenge)) ? (_saveEvent.GetReferenceObjectByID(currentChallengeRuntimeDataWrapper.Challenge) as Challenge) : null);
	}

	public void Register()
	{
		if (_saveEvent != null)
		{
			_saveEvent.RegisterListener(this);
		}
	}

	public void Unregister()
	{
		if (_saveEvent != null)
		{
			_saveEvent.UnregisterListener(this);
		}
	}

	public void ResetData()
	{
		_runtimeData.Challenge = null;
	}
}
