using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class ApocalypseScreenController : MonoBehaviour
{
	[SerializeField]
	private CharacterEventToggleController[] _characterToggles;

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
		EnableRandomCharacter();
		SetDefaultDifficulty();
	}

	private void SetAllTogglesDisabled()
	{
		for (int i = 0; i < _characterToggles.Length; i++)
		{
			_characterToggles[i].SetToggleWithoutInvokingValueChange(value: false);
		}
		for (int j = 0; j < _difficultyToggles.Length; j++)
		{
			_difficultyToggles[j].SetToggleWithoutInvokingValueChange(value: false);
		}
	}

	private void EnableRandomCharacter()
	{
		int num = Random.Range(0, _characterToggles.Length);
		_characterToggles[num].Toggle.isOn = true;
		SetCharacter(_characterToggles[num].Character);
	}

	private void SetDefaultDifficulty()
	{
		_defaultDifficultyToggle.Toggle.isOn = true;
	}

	public void SetCharacter(Character character)
	{
		_remasterMenuManager.CurrentCharacter = character;
	}

	public void SetDifficulty(DifficultyLevel difficultyLevel)
	{
		_remasterMenuManager.CurrentDifficultyLevel = difficultyLevel;
	}

	public void StartApocalypse()
	{
		if (!_alreadyTriggered)
		{
			_alreadyTriggered = true;
			_remasterMenuManager.StartFullGame();
		}
	}
}
