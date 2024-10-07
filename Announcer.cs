using System.Collections;
using FMOD.Studio;
using FMODUnity;
using RG.Parsecs.Common;
using UnityEngine;

public class Announcer : MonoBehaviour
{
	[SerializeField]
	private GameObject _guiTemplate;

	[SerializeField]
	private float _timeout;

	[EventRef]
	[SerializeField]
	private string _soundName = string.Empty;

	private EventInstance _audio;

	private ScavengeItemController _announcedScavengeItemController;

	private dfFollowObject _annoucerFollowerGUI;

	private dfTweenPlayableBase _hideTween;

	private bool _terminated;

	private bool _running;

	public ScavengeItemController AnnouncedScavengeItemController => _announcedScavengeItemController;

	public bool Terminated => _terminated;

	public bool Running => _running;

	public float Timeout
	{
		get
		{
			return _timeout;
		}
		set
		{
			_timeout = value;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
		DamageEffector component = GetComponent<DamageEffector>();
		if (component != null)
		{
			component.OnDamage += Terminate;
		}
	}

	private void Update()
	{
	}

	private void SelectItemToAnnounce(string customItem)
	{
		GameFlow component = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameFlow>();
		if (customItem != null)
		{
			for (int i = 0; i < component.SpecialLevelItems.Length; i++)
			{
				ScavengeItemController component2 = component.SpecialLevelItems[i].GetComponent<ScavengeItemController>();
				if (component2.SurvivalName == customItem)
				{
					_announcedScavengeItemController = component2;
					component.ReportSpecialItem(component2);
					break;
				}
			}
			return;
		}
		while (_announcedScavengeItemController == null)
		{
			ScavengeItemController scavengeItemController = null;
			while (scavengeItemController == null || (scavengeItemController != null && !scavengeItemController.gameObject.activeSelf))
			{
				scavengeItemController = component.SpecialLevelItems[Random.Range(0, component.SpecialLevelItems.Length)].GetComponent<ScavengeItemController>();
			}
			if (component.ReportSpecialItem(scavengeItemController))
			{
				_announcedScavengeItemController = scavengeItemController;
			}
		}
	}

	private IEnumerator Run()
	{
		_running = true;
		yield return new WaitForSeconds(_timeout);
		if (!_terminated)
		{
			StartCoroutine(Cleanup());
		}
	}

	private IEnumerator Cleanup()
	{
		_terminated = true;
		if (_running)
		{
			_running = false;
			if (_hideTween != null)
			{
				_hideTween.Play();
			}
			yield return new WaitForSeconds(0.25f);
			Activate(activate: false, null);
		}
	}

	public void Activate(bool activate, string customItem)
	{
		if (activate)
		{
			OnActivation(customItem);
		}
		else
		{
			OnDeactivation();
		}
	}

	protected virtual void OnActivation(string customITem)
	{
		SelectItemToAnnounce(customITem);
		if (_annoucerFollowerGUI == null)
		{
			GameObject gameObject = GameObject.Find("InGame");
			dfControl dfControl2 = ((gameObject == null) ? null : gameObject.GetComponent<dfControl>());
			if (dfControl2 != null)
			{
				dfControl dfControl3 = dfControl2.GetComponent<dfControl>().AddPrefab(_guiTemplate);
				_annoucerFollowerGUI = dfControl3.GetComponent<dfFollowObject>();
				dfTweenPlayableBase[] components = dfControl3.GetComponents<dfTweenPlayableBase>();
				for (int i = 0; i < components.Length; i++)
				{
					if (components[i].TweenName == "PopOut")
					{
						_hideTween = components[i];
						break;
					}
				}
				dfDataObjectProxy component = dfControl3.GetComponent<dfDataObjectProxy>();
				if (component != null)
				{
					component.Data = this;
				}
				dfPivotPoint pivot = dfPivotPoint.MiddleCenter;
				dfPivotPoint anchor = dfPivotPoint.MiddleCenter;
				ResolutionHandler resolutionHandler = Object.FindObjectOfType<ResolutionHandler>();
				if (resolutionHandler != null)
				{
					if (ResolutionHandler.Is169(resolutionHandler.SelectedAspectRatio.AspectRatio))
					{
						pivot = dfPivotPoint.TopLeft;
						anchor = dfPivotPoint.TopLeft;
						_annoucerFollowerGUI.offset = new Vector3(0.5f, 0.5f, 0.5f);
					}
					else if (ResolutionHandler.Is1610(resolutionHandler.SelectedAspectRatio.AspectRatio))
					{
						pivot = dfPivotPoint.TopLeft;
						anchor = dfPivotPoint.TopLeft;
						_annoucerFollowerGUI.offset = new Vector3(0.5f, 1f, 1f);
					}
					else if (ResolutionHandler.Is43(resolutionHandler.SelectedAspectRatio.AspectRatio))
					{
						pivot = dfPivotPoint.TopRight;
						anchor = dfPivotPoint.TopRight;
						_annoucerFollowerGUI.offset = new Vector3(0f, 1f, 0f);
					}
					else if (ResolutionHandler.Is54(resolutionHandler.SelectedAspectRatio.AspectRatio))
					{
						pivot = dfPivotPoint.TopRight;
						anchor = dfPivotPoint.TopRight;
						_annoucerFollowerGUI.offset = new Vector3(0f, 1f, 0f);
					}
				}
				dfControl2.Pivot = pivot;
				_annoucerFollowerGUI.anchor = anchor;
				_annoucerFollowerGUI.gameObject.SetActive(value: true);
				_annoucerFollowerGUI.mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
				_annoucerFollowerGUI.attach = base.gameObject;
				_annoucerFollowerGUI.enabled = true;
			}
		}
		if (!string.IsNullOrEmpty(_soundName))
		{
			_audio = AudioManager.PlaySoundAndReturnInstance(_soundName);
		}
		StartCoroutine(Run());
	}

	protected virtual void OnDeactivation()
	{
		if (_annoucerFollowerGUI != null && _annoucerFollowerGUI.gameObject != null)
		{
			_annoucerFollowerGUI.gameObject.SetActive(value: false);
			_annoucerFollowerGUI.GetComponent<dfControl>().IsVisible = false;
			_annoucerFollowerGUI.enabled = false;
		}
		if (_audio.isValid())
		{
			AudioManager.StopSound(_audio, FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
	}

	public void Terminate()
	{
		StartCoroutine(Cleanup());
	}
}
