using I2.Loc;
using RG.Parsecs.Loading;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Loading;

public class LoadingController : PosterController
{
	[SerializeField]
	private TextMeshProUGUI _middleText;

	public void SetLoadingScreen(LoadingScreen screen)
	{
		SetPoster(screen);
		if (_middleText != null && !string.IsNullOrEmpty(screen.MiddleText.mTerm))
		{
			_middleText.text = screen.MiddleText;
			_middleText.gameObject.GetComponent<Localize>().OnLocalize();
		}
	}
}
