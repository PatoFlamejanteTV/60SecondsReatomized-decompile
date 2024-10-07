using RG.Parsecs.EventEditor;
using RG.Remaster.Survival;
using UnityEngine;

public class ChallengeSkinController : MonoBehaviour
{
	[SerializeField]
	private GlobalBoolVariable _isSkinUnlockedVariable;

	[SerializeField]
	private GlobalBoolVariable _isChallenge01Active;

	[SerializeField]
	private SkinId _forcedSkinId;

	[SerializeField]
	private SkinController _skinController;

	private void Start()
	{
		if (_isChallenge01Active != null && _isChallenge01Active.Value)
		{
			_skinController.ForceSkinUse(_forcedSkinId);
		}
	}

	private void OnDestroy()
	{
		if (_isSkinUnlockedVariable != null && !_isSkinUnlockedVariable.Value)
		{
			_skinController.CurrentSkinIndex.Value = 0;
		}
	}
}
