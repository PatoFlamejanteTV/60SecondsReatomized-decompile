using Rewired;
using RG.Parsecs.Survival;
using UnityEngine;

public class SurvivalShowPromptsButton : MonoBehaviour
{
	[SerializeField]
	private RectTransform _showPromptsRect;

	[SerializeField]
	[Tooltip("Reference to Survival Data scriptable object.")]
	private SurvivalData _survivalData;

	[SerializeField]
	private EndGameData _endGameData;

	private Player _player;

	private Controller _lastController;

	public void Awake()
	{
		_player = ReInput.players.GetPlayer(0);
	}

	private void Update()
	{
		if (_player != null && _player.controllers.GetLastActiveController() != _lastController)
		{
			_lastController = _player.controllers.GetLastActiveController();
		}
		if (_lastController != null)
		{
			_showPromptsRect.gameObject.SetActive(_lastController is Joystick && _survivalData.CurrentDay <= 3 && !_endGameData.RuntimeData.ShouldEndGame);
		}
		else
		{
			_showPromptsRect.gameObject.SetActive(value: false);
		}
	}
}
