using Rewired;
using RG.Parsecs.Common;
using UnityEngine;
using UnityEngine.Playables;

namespace RG.SecondsRemaster.Menu;

public class IntroController : MonoBehaviour
{
	[SerializeField]
	private bool _disableIntroInEditor = true;

	[SerializeField]
	private bool _introEnabled;

	[SerializeField]
	private PlayableDirector _director;

	[SerializeField]
	private float _playableDelay = 4f;

	private Player _player;

	private bool _isIntroVisible;

	private void Awake()
	{
		if (!Singleton<GameManager>.Instance.FirstMenuEnter || !_introEnabled)
		{
			DisableIntro();
		}
		else
		{
			_player = ReInput.players.GetPlayer(0);
		}
	}

	private void Start()
	{
		if (Singleton<GameManager>.Instance.FirstMenuEnter && _introEnabled)
		{
			Singleton<GameManager>.Instance.FirstMenuEnter = false;
			Invoke("Play", _playableDelay);
		}
	}

	private void Play()
	{
		_director.Play();
		_isIntroVisible = true;
	}

	private void OnDisable()
	{
		_isIntroVisible = false;
	}

	private void Update()
	{
		if ((_isIntroVisible && _player.GetButtonDown(29)) || _player.GetButtonDown(30))
		{
			DisableIntro();
		}
	}

	private void DisableIntro()
	{
		_isIntroVisible = false;
		base.gameObject.SetActive(value: false);
		_director.Stop();
	}
}
