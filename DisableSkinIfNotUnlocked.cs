using RG.Parsecs.EventEditor;
using RG.Remaster.Survival;
using UnityEngine;

public class DisableSkinIfNotUnlocked : MonoBehaviour
{
	[SerializeField]
	private GlobalBoolVariable _isSkinUnlockedVariable;

	[SerializeField]
	private SkinController _skinController;

	private void OnDestroy()
	{
		if (_isSkinUnlockedVariable != null && !_isSkinUnlockedVariable.Value)
		{
			_skinController.CurrentSkinIndex.Value = 0;
		}
	}
}
