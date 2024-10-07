using System.Collections.Generic;
using Rewired;
using RG.Parsecs.Common;
using RG.VirtualInput;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RG.SecondsRemaster.ChallengeConclusion;

public class ConclusionScreenController : MonoBehaviour
{
	[SerializeField]
	private string _survivalSceneName = "Survival";

	[SerializeField]
	private string _challengeConclusionSceneName = "ChallengeConclusion";

	[SerializeField]
	private float _clickAwayDelayTime = 2f;

	[SerializeField]
	private bool _clickAwayAllowed;

	private Player _player;

	private float _startTime;

	private void Awake()
	{
		_player = ReInput.players.GetPlayer(0);
	}

	public void Start()
	{
		Singleton<VirtualInputManager>.Instance.VisualManager.MouseCursorVisible = true;
		_startTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		if (Time.realtimeSinceStartup > _startTime + _clickAwayDelayTime && _player.GetButtonDown(29) && _clickAwayAllowed)
		{
			OnReturnToMenuButtonClick();
		}
	}

	public void OnReturnToMenuButtonClick()
	{
		List<Scene> list = new List<Scene>();
		SceneManager.GetSceneByName(_survivalSceneName);
		list.Add(SceneManager.GetSceneByName(_survivalSceneName));
		list.Add(SceneManager.GetSceneByName(_challengeConclusionSceneName));
		Singleton<GameManager>.Instance.LoadMenu(list);
	}
}
