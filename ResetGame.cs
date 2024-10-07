using RG.Parsecs.Common;

public class ResetGame
{
	private static string GameLevel;

	public static void RestartLevel()
	{
		if (GameSessionData.Instance.Setup.IsScavengeGame())
		{
			if (GameSessionData.Instance.Setup.IsChallengeGame() || GameSessionData.Instance.Setup.IsTutorialGame())
			{
				GameLevel = GameSessionData.Instance.Setup.ForcedLevelStem;
			}
			else if (DemoManager.IS_DEMO_VERSION)
			{
				GameLevel = "level_scavenge_11";
			}
			else
			{
				GameLevel = GameSessionData.Instance.Setup.GetRandomScavengeLevelName();
			}
			if (GameLevel != null)
			{
				DoRestart();
			}
		}
		else
		{
			GameLevel = GameSessionData.Instance.Setup.ForcedLevelStem;
		}
	}

	private static void DoRestart()
	{
		AudioManager.Instance.StopPlayingSfxFadeOut();
		if (ScavengeDataLogger.Instance != null)
		{
			ScavengeDataLogger.Instance.EndLog(gameFinished: false, cloud: false, async: false);
		}
		Singleton<GameManager>.Instance.ScavangeSceneName = GameLevel;
		Singleton<GameManager>.Instance.StartScavenge();
	}
}
