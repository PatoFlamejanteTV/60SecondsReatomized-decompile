using System.Collections;
using UnityEngine;

public class Intro : MonoBehaviour
{
	private bool _canProgress;

	private bool _canSkip = true;

	private bool _skip;

	private float _skipInvokeTime;

	private bool _skipPress = true;

	[SerializeField]
	private float _skipTimeout = 0.05f;

	private PlayVideo _video;

	private void Awake()
	{
	}

	private void Start()
	{
		Settings.Data.ShowCursor = false;
		Settings.Data.LockCursor = CursorLockMode.Locked;
		if (Settings.Data.SkipIntro)
		{
			End(menu: true);
		}
		else
		{
			StartCoroutine(IntroProgress());
		}
	}

	private IEnumerator IntroProgress()
	{
		yield return new WaitForSeconds(0.25f);
		_video = Object.FindObjectOfType<PlayVideo>();
		if (_video != null)
		{
			_video.UpdateScaling();
			_video.Play();
		}
	}

	private void Update()
	{
		if (_video != null && !_video.IsPlaying())
		{
			End(menu: true);
		}
		if (_canSkip)
		{
			if (_skip)
			{
				if (_skipInvokeTime <= Time.time)
				{
					End(menu: true);
				}
			}
			else if (_skipPress)
			{
				if (Input.anyKey)
				{
					End(menu: true);
				}
			}
			else
			{
				_skip = true;
				_skipInvokeTime = Time.time + _skipTimeout;
			}
		}
		else
		{
			_skip = false;
		}
	}

	public void End(bool menu)
	{
		if (menu)
		{
			PlayVideo playVideo = Object.FindObjectOfType<PlayVideo>();
			if (playVideo != null && playVideo.IsPlaying())
			{
				playVideo.Stop();
			}
			Application.LoadLevel("main_menu");
		}
	}
}
