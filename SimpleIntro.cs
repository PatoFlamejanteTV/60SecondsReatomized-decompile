using UnityEngine;

public class SimpleIntro : MonoBehaviour
{
	private bool _canProgress;

	private bool _canSkip = true;

	private bool _skip;

	private float _skipInvokeTime;

	private bool _skipPress = true;

	[SerializeField]
	private float _skipTimeout = 0.05f;

	[SerializeField]
	private dfTweenPlayableBase _introAnimation;

	private void Awake()
	{
	}

	private void Start()
	{
		Settings.Data.ShowCursor = false;
		Settings.Data.LockCursor = CursorLockMode.Locked;
		if (Settings.Data.SkipIntro)
		{
			End();
		}
		else if (_introAnimation != null)
		{
			_introAnimation.Play();
		}
	}

	private void Update()
	{
		if (_canSkip)
		{
			if (_skip)
			{
				if (_skipInvokeTime <= Time.time)
				{
					End();
				}
			}
			else if (_skipPress)
			{
				if (Input.anyKey)
				{
					End();
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

	public void End()
	{
		Loading.Loader.NextLevelName = "main_menu";
		Loading.Loader.GoToLoading();
	}
}
