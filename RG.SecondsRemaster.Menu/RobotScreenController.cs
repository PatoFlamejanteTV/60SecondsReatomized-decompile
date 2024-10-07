using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class RobotScreenController : MonoBehaviour
{
	[SerializeField]
	private float _screenVisibleTime;

	[SerializeField]
	private GameObject _chooseGameModeScreen;

	[SerializeField]
	private ScreensController _screensController;

	private const string SHOW_GAME_MODE_SCREEN = "ShowGameModeScreen";

	private void OnEnable()
	{
		Invoke("ShowGameModeScreen", _screenVisibleTime);
	}

	public void ShowGameModeScreen()
	{
		_screensController.ShowScreen(_chooseGameModeScreen);
	}
}
