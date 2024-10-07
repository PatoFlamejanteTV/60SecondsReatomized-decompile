using RG.Parsecs.Common;
using UnityEngine;

namespace RG.Remaster.Survival;

public class AchievementOnEnableUnlocker : MonoBehaviour
{
	[SerializeField]
	private Achievement _achievement;

	private void OnEnable()
	{
		if (!AchievementsSystem.IsAchievementUnlocked(_achievement))
		{
			AchievementsSystem.UnlockAchievement(_achievement);
		}
	}
}
