using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class AtomicDrillScreenController : MonoBehaviour
{
	[SerializeField]
	private RemasterMenuManager _remasterMenuManager;

	private bool _alreadyTriggered;

	public void Awake()
	{
		_alreadyTriggered = false;
	}

	private void OnEnable()
	{
		if (_remasterMenuManager == null)
		{
			_remasterMenuManager = Object.FindObjectOfType<RemasterMenuManager>();
		}
	}

	public void StartTutorial()
	{
		if (!_alreadyTriggered)
		{
			_alreadyTriggered = true;
			_remasterMenuManager.StartTutorial();
		}
	}
}
