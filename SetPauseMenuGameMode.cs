using RG.Parsecs.Menu;
using RG.SecondsRemaster.Menu;
using UnityEngine;

public class SetPauseMenuGameMode : MonoBehaviour
{
	[SerializeField]
	private PauseMenuControl.EGameType _gameType;

	private void Start()
	{
		SetGameType();
	}

	private void SetGameType()
	{
		PauseMenuControl pauseMenuControl = BasePauseMenu.Instance as PauseMenuControl;
		if (pauseMenuControl != null)
		{
			pauseMenuControl.GameType = _gameType;
		}
	}
}
