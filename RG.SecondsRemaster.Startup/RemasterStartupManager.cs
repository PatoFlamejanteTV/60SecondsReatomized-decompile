using System.Collections;
using I2.Loc;
using RG.Parsecs.Common;
using UnityEngine;

namespace RG.SecondsRemaster.Startup;

public class RemasterStartupManager : MonoBehaviour
{
	public void StartLoadingGame()
	{
		StartCoroutine(LoadGame());
	}

	private IEnumerator LoadGame()
	{
		Singleton<GameManager>.Instance.FirstMenuEnter = true;
		LocalizationManager.InitializeIfNeeded();
		yield return Singleton<GameManager>.Instance.ContentManager.LoadCommonAsset();
		Singleton<GameManager>.Instance.LoadMenuWithOpening();
	}
}
