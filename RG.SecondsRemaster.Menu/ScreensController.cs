using System.Collections.Generic;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class ScreensController : MonoBehaviour
{
	[SerializeField]
	private TvButtonsController _tvButtonsController;

	[SerializeField]
	private GameObject _firstScreen;

	[SerializeField]
	private List<GameObject> _screens;

	private Stack<GameObject> _previousScreens = new Stack<GameObject>();

	private GameObject _currentScreen;

	private void OnEnable()
	{
		EnableScreen(_firstScreen);
		_previousScreens.Clear();
	}

	private void EnableScreen(GameObject screen)
	{
		for (int i = 0; i < _screens.Count; i++)
		{
			_screens[i].SetActive(value: false);
		}
		screen.SetActive(value: true);
		_currentScreen = screen;
	}

	public void ShowScreen(GameObject screen)
	{
		if (!_screens.Contains(screen))
		{
			Debug.LogErrorFormat(this, "The 'ScreenController' doesn't contain screen '{0}'.", screen);
		}
		_tvButtonsController.SwitchRandomSelectable();
		_previousScreens.Push(_currentScreen);
		EnableScreen(screen);
	}

	public void GoBack()
	{
		if (_previousScreens.Count != 0)
		{
			_tvButtonsController.SwitchRandomSelectable();
			EnableScreen(_previousScreens.Pop());
		}
	}
}
