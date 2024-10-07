using FMODUnity;
using RG.Parsecs.Common;
using RG.VirtualInput;
using UnityEngine;

public class FlyingObjectGrab : MonoBehaviour
{
	[EventRef]
	[SerializeField]
	private string _grabSound;

	[SerializeField]
	private Achievement _achievementToBeUnlocked;

	private void OnMouseDown()
	{
		if (!Singleton<VirtualInputManager>.Instance.IsPointerOverGameObject())
		{
			if (_achievementToBeUnlocked != null && !AchievementsSystem.IsAchievementUnlocked(_achievementToBeUnlocked))
			{
				AchievementsSystem.UnlockAchievement(_achievementToBeUnlocked);
			}
			AudioManager.PlaySound(_grabSound);
			base.gameObject.SetActive(value: false);
		}
	}
}
