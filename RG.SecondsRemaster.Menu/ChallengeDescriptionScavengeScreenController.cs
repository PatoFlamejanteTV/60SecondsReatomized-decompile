using I2.Loc;
using RG.SecondsRemaster.Scavenge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Menu;

public class ChallengeDescriptionScavengeScreenController : MonoBehaviour
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
	private ChallengeIconsController _collectIcons;

	[SerializeField]
	private ChallengeIconsController _rewardIcons;

	[SerializeField]
	private RemasterMenuManager _remasterMenuManager;

	[SerializeField]
	private LocalizedString _rewardForTerm;

	[SerializeField]
	private TvButtonsController _tvButtonsController;

	[SerializeField]
	private TextMeshProUGUI _challengeCompletedTime;

	[SerializeField]
	private LocalizedString _challengeCompletedTerm;

	private int _currentChallengeIndex;

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
		SetCollectIcons();
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

	private void SetCollectIcons()
	{
		_collectIcons.DisableAllIcons();
		for (int i = 0; i < _remasterMenuManager.CurrentChallenge.Collectables.Count; i++)
		{
			ScavengeItem scavengeItem = _remasterMenuManager.CurrentChallenge.Collectables[i];
			_collectIcons.SetNextIcon(scavengeItem.MenuIcon);
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
		_rewardName.text = string.Format(_rewardForTerm.ToString(), _remasterMenuManager.CurrentChallenge.RewardName);
	}

	private void SetChallengeCompleted()
	{
		if (_remasterMenuManager.CurrentChallenge.IsUnlocked)
		{
			_challengeCompletedTime.gameObject.SetActive(value: true);
			_challengeCompletedTime.text = string.Format(_challengeCompletedTerm.ToString(), $"{_remasterMenuManager.CurrentChallenge.Time:0.00}");
			_challengeCompleted.SetActive(value: true);
		}
		else
		{
			_challengeCompletedTime.gameObject.SetActive(value: false);
			_challengeCompleted.SetActive(value: false);
		}
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
