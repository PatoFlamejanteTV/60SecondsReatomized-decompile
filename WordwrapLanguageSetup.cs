using I2.Loc;
using UnityEngine;

public class WordwrapLanguageSetup : MonoBehaviour
{
	public static void StaticAsianWordWrap()
	{
	}

	public void AsianWordWrap()
	{
		StaticAsianWordWrap();
	}

	public static bool IsAsianLanguage()
	{
		if (!(LocalizationManager.CurrentLanguageCode == "zh-CN") && !(LocalizationManager.CurrentLanguageCode == "ja"))
		{
			return LocalizationManager.CurrentLanguageCode == "ko";
		}
		return true;
	}
}
