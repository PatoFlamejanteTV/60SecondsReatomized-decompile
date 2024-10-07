using RG.Parsecs.Loading;
using RG.Parsecs.Survival;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Loading;

public class DisplayLoading : MonoBehaviour
{
	[SerializeField]
	private PosterList _loadingScreens;

	[SerializeField]
	private LoadingController _loadingScreenController;

	[SerializeField]
	private MissionManagerData _missionManagerData;

	[SerializeField]
	private Image[] _backgroundImages;

	[SerializeField]
	private Image[] _spikesImages;

	private void Start()
	{
		if (MissionManager.Instance.IsMissionOngoing() && _missionManagerData.CurrentMission.Poster != null)
		{
			SetMissionPoster();
		}
		else
		{
			SetRandomPoster();
		}
	}

	private void SetBackgroundColors(LoadingScreen screen)
	{
		SetColorToImages(_backgroundImages, screen.BackgroundColor);
		SetColorToImages(_spikesImages, screen.SpikesColor);
	}

	private void SetColorToImages(Image[] images, Color color)
	{
		if (images == null)
		{
			return;
		}
		for (int i = 0; i < images.Length; i++)
		{
			if (images[i] != null)
			{
				images[i].color = color;
			}
		}
	}

	private void SetRandomPoster()
	{
		int num = Random.Range(0, _loadingScreens.Posters.Length);
		LoadingScreen loadingScreen = _loadingScreens.Posters[num] as LoadingScreen;
		if (!(loadingScreen == null))
		{
			_loadingScreenController.SetLoadingScreen(loadingScreen);
			SetBackgroundColors(loadingScreen);
		}
	}

	private void SetMissionPoster()
	{
		LoadingScreen loadingScreen = ((_missionManagerData.CurrentMission.Poster != null) ? (_missionManagerData.CurrentMission.Poster as LoadingScreen) : null);
		if (!(loadingScreen == null))
		{
			_loadingScreenController.SetLoadingScreen(loadingScreen);
			SetBackgroundColors(loadingScreen);
		}
	}
}
