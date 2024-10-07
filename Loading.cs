using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
	public delegate void ReportLoading(string levelName, float progress, bool complete);

	public static Loading Loader;

	[SerializeField]
	protected string[] _loadLevels;

	[SerializeField]
	protected string _nextLevelName = string.Empty;

	[SerializeField]
	protected string _loadingLevelName = "loading";

	[SerializeField]
	protected bool _autoLoad = true;

	[SerializeField]
	protected bool _waitForFadeOut = true;

	[SerializeField]
	protected float _loadingDelay = 1f;

	[SerializeField]
	protected float _loadingProgressTimeout = 0.25f;

	protected AsyncOperation _loadingProcess;

	protected bool _ready;

	protected bool _signalledReady;

	protected int _contentIndex;

	public string NextLevelName
	{
		get
		{
			return _nextLevelName;
		}
		set
		{
			if (value == "main_menu" && Random.Range(0, 2) > 0)
			{
				_nextLevelName = "main_menu_astro";
			}
			else
			{
				_nextLevelName = value;
			}
		}
	}

	public string LoadingLevelName
	{
		get
		{
			return _loadingLevelName;
		}
		set
		{
			_loadingLevelName = string.Empty;
			for (int i = 0; i < _loadLevels.Length; i++)
			{
				if (_loadLevels[i] == value)
				{
					_loadingLevelName = value;
					break;
				}
			}
		}
	}

	public bool AutoLoad
	{
		get
		{
			return _autoLoad;
		}
		set
		{
			_autoLoad = value;
		}
	}

	public event ReportLoading OnLoadReady;

	public event ReportLoading OnLoadDone;

	private void Awake()
	{
		Loader = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void GoToLoading(bool skipLoadingLevel = false)
	{
		bool flag = !string.IsNullOrEmpty(_nextLevelName);
		bool flag2 = !skipLoadingLevel && !string.IsNullOrEmpty(_loadingLevelName);
		if (flag && flag2)
		{
			StartCoroutine(LoadLoadingLevel());
		}
		else if (flag)
		{
			SceneManager.LoadScene(_nextLevelName);
		}
	}

	protected IEnumerator LoadLoadingLevel()
	{
		SceneManager.LoadScene(_loadingLevelName);
		yield return new WaitForSeconds(_loadingDelay);
		StartLoadingNextLevel();
	}

	public void StartLoadingNextLevel()
	{
		StartCoroutine(LoadNextLevel());
	}

	protected IEnumerator LoadNextLevel()
	{
		_loadingProcess = SceneManager.LoadSceneAsync(_nextLevelName);
		_loadingProcess.allowSceneActivation = _autoLoad;
		while (!_loadingProcess.isDone)
		{
			yield return new WaitForSeconds(_loadingProgressTimeout);
			if (_autoLoad)
			{
				continue;
			}
			if (_ready)
			{
				_loadingProcess.allowSceneActivation = true;
			}
			else
			{
				if (_signalledReady || !(_loadingProcess.progress >= 0.9f))
				{
					continue;
				}
				_signalledReady = true;
				if (_waitForFadeOut)
				{
					if (this.OnLoadDone != null)
					{
						this.OnLoadDone(_nextLevelName, 1f, complete: true);
					}
					SoundManager.Instance.CrossOut(1.5f, SoundManager.Instance.MusicSource);
					yield return new WaitForSeconds(1.25f);
					SignalReady();
				}
				else
				{
					InvokeWaitingForSignal(_loadingProcess.progress, complete: true);
				}
			}
		}
		if (_autoLoad || !_waitForFadeOut)
		{
			if (this.OnLoadDone != null)
			{
				this.OnLoadDone(_nextLevelName, 1f, complete: true);
			}
			yield return new WaitForSeconds(0.75f);
		}
		_ready = false;
		_signalledReady = false;
		_loadingProcess = null;
	}

	protected void InvokeWaitingForSignal(float progress, bool complete)
	{
		if (this.OnLoadReady != null)
		{
			this.OnLoadReady(_nextLevelName, progress, complete);
		}
		GameObject.FindGameObjectWithTag("LoadingProcess").GetComponent<dfControl>().IsVisible = false;
		GameObject obj = GameObject.FindGameObjectWithTag("LoadingReady");
		obj.GetComponent<dfControl>().IsVisible = true;
		dfEventBinding component = obj.GetComponent<dfEventBinding>();
		component.DataTarget.Component = this;
		component.DataTarget.MemberName = "SignalReady";
		component.Bind();
	}

	public void SignalReady()
	{
		_ready = true;
	}

	public void CompleteLoadingNextLevel()
	{
		if (_loadingProcess != null)
		{
			_loadingProcess.allowSceneActivation = true;
		}
	}
}
