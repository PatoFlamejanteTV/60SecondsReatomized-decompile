using System.Collections.Generic;
using I2.Loc;
using RG.Parsecs.Survival;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Menu;

public class ChallengeDescriptionSurvivalScreenController : MonoBehaviour
{
	[SerializeField]
	private ChallengeList _challengeList;

	[SerializeField]
	private Image _challengeImage;

	[SerializeField]
	private GameObject _challengeCompleted;

	[SerializeField]
	private TextMeshProUGUI _challengeTitle;

	[SerializeField]
	private TextMeshProUGUI _challengeDescription;

	[SerializeField]
	private TextMeshProUGUI _rewardName;

	[SerializeField]
	private RuleController _rulePrefab;

	[SerializeField]
	private Transform _rulesHolder;

	[SerializeField]
	private Sprite _goalRuleSprite;

	[SerializeField]
	private Sprite _objectiveRuleSprite;

	[SerializeField]
	private ChallengeIconsController _rewardIcons;

	[SerializeField]
	private RemasterMenuManager _remasterMenuManager;

	[SerializeField]
	private LocalizedString _rewardForTerm;

	[SerializeField]
	private TvButtonsController _tvButtonsController;

	private int _currentChallengeIndex;

	private List<RuleController> _currentRules;

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
		_currentChallengeIndex = _challengeList.Challenges.IndexOf(_remasterMenuManager.CurrentChallenge);
		SetChallengeData();
	}

	public void SetNextChallenge()
	{
		if (_currentChallengeIndex + 1 >= _challengeList.Challenges.Count)
		{
			_currentChallengeIndex = 0;
		}
		else
		{
			_currentChallengeIndex++;
		}
		_remasterMenuManager.CurrentChallenge = _challengeList.Challenges[_currentChallengeIndex];
		_tvButtonsController.SwitchRandomSelectable();
		SetChallengeData();
	}

	public void SetPreviousChallenge()
	{
		if (_currentChallengeIndex - 1 < 0)
		{
			_currentChallengeIndex = _challengeList.Challenges.Count - 1;
		}
		else
		{
			_currentChallengeIndex--;
		}
		_remasterMenuManager.CurrentChallenge = _challengeList.Challenges[_currentChallengeIndex];
		_tvButtonsController.SwitchRandomSelectable();
		SetChallengeData();
	}

	private void SetChallengeData()
	{
		SetChallengeImage();
		SetChallengeTitle();
		SetChallengeDescription();
		SetRules();
		SetRewardIcons();
		SetRewardTerm();
		SetChallengeCompleted();
	}

	private void SetChallengeImage()
	{
		_challengeImage.sprite = _remasterMenuManager.CurrentChallenge.ChallengeGraphic;
	}

	private void SetChallengeTitle()
	{
		_challengeTitle.text = _remasterMenuManager.CurrentChallenge.Name;
	}

	private void SetChallengeDescription()
	{
		_challengeDescription.text = _remasterMenuManager.CurrentChallenge.Description;
	}

	private void SetRules()
	{
		if (_currentRules == null)
		{
			_currentRules = new List<RuleController>();
		}
		List<MedalReward> goals = _remasterMenuManager.CurrentChallenge.Mission.Goals;
		int num = 0;
		for (int i = 0; i < goals.Count; i++)
		{
			if (num < _currentRules.Count)
			{
				_currentRules[num].SetRule(goals[i].Goal.Description, _goalRuleSprite);
				_currentRules[num].gameObject.SetActive(value: true);
			}
			else
			{
				RuleController component = Object.Instantiate(_rulePrefab, _rulesHolder).GetComponent<RuleController>();
				component.SetRule(goals[i].Goal.Description, _goalRuleSprite);
				_currentRules.Add(component);
			}
			num++;
		}
		List<LocalizedString> objectives = _remasterMenuManager.CurrentChallenge.Objectives;
		for (int j = 0; j < objectives.Count; j++)
		{
			if (num < _currentRules.Count)
			{
				_currentRules[num].SetRule(objectives[j], _objectiveRuleSprite);
				_currentRules[num].gameObject.SetActive(value: true);
			}
			else
			{
				RuleController component2 = Object.Instantiate(_rulePrefab, _rulesHolder).GetComponent<RuleController>();
				component2.SetRule(objectives[j], _objectiveRuleSprite);
				_currentRules.Add(component2);
			}
			num++;
		}
		for (int k = num; k < _currentRules.Count; k++)
		{
			_currentRules[k].gameObject.SetActive(value: false);
		}
	}

	private void SetRewardIcons()
	{
		_rewardIcons.DisableAllIcons();
		for (int i = 0; i < _remasterMenuManager.CurrentChallenge.Rewards.Count; i++)
		{
			RewardItem rewardItem = _remasterMenuManager.CurrentChallenge.Rewards[i];
			_rewardIcons.SetNextIcon(rewardItem.Icon);
		}
	}

	private void SetRewardTerm()
	{
		_rewardName.text = string.Format(_rewardForTerm.ToString(), ((string)_remasterMenuManager.CurrentChallenge.RewardName != null) ? _remasterMenuManager.CurrentChallenge.RewardName.ToString() : string.Empty);
	}

	private void SetChallengeCompleted()
	{
		_challengeCompleted.SetActive(_remasterMenuManager.CurrentChallenge.IsUnlocked);
	}

	public void StartChallenge()
	{
		if (!_alreadyTriggered)
		{
			_alreadyTriggered = true;
			_remasterMenuManager.StartChallenge();
		}
	}
}
