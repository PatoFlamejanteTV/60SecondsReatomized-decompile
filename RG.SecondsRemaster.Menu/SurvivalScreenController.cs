using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class SurvivalScreenController : MonoBehaviour
{
	[SerializeField]
	private UnityEventToggleController[] _difficultyToggles;

	[SerializeField]
	private UnityEventToggleController _defaultDifficultyToggle;

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
		SetAllTogglesDisabled();
		SetDefaultDifficulty();
	}

	private void SetAllTogglesDisabled()
	{
		for (int i = 0; i < _difficultyToggles.Length; i++)
		{
			_difficultyToggles[i].SetToggleWithoutInvokingValueChange(value: false);
		}
	}

	private void SetDefaultDifficulty()
	{
		_defaultDifficultyToggle.Toggle.isOn = true;
	}

	public void SetDifficulty(DifficultyLevel difficultyLevel)
	{
		_remasterMenuManager.CurrentDifficultyLevel = difficultyLevel;
	}

	public void StartSurvival()
	{
		if (!_alreadyTriggered)
		{
			_alreadyTriggered = true;
			_remasterMenuManager.StartSurvival();
		}
	}
}
