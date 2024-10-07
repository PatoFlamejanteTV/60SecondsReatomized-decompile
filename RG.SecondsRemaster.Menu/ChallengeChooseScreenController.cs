using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class ChallengeChooseScreenController : MonoBehaviour
{
	[SerializeField]
	private ChallengeSlotController[] _challengesSlotController;

	[SerializeField]
	private TextMeshProUGUI _challengeTitle;

	[SerializeField]
	private ChallengeList _challenges;

	[SerializeField]
	private RemasterMenuManager _remasterMenuManager;

	private int _currentIndex;

	private List<Challenge> _allChallenges;

	private const int CENTER_INDEX = 2;

	public void SetChallengeOnCenter(ChallengeSlotController challengeSlot)
	{
		for (int i = 0; i < _allChallenges.Count; i++)
		{
			if (challengeSlot.Challenge == _allChallenges[i])
			{
				_currentIndex = RemasterMath.Modulo(i - 2, _allChallenges.Count);
				UpdateChallengeDisplay();
				break;
			}
		}
	}

	public void SetCurrentChallenge()
	{
		_remasterMenuManager.CurrentChallenge = _allChallenges[GetCycledIndexOfNumber(2)];
	}

	private void GetAllChallenges()
	{
		if (_allChallenges == null)
		{
			_allChallenges = new List<Challenge>();
			List<Challenge> challenges = _challenges.Challenges;
			for (int i = 0; i < challenges.Count; i++)
			{
				_allChallenges.Add(challenges[i]);
			}
		}
	}

	private void OnEnable()
	{
		if (_remasterMenuManager == null)
		{
			_remasterMenuManager = Object.FindObjectOfType<RemasterMenuManager>();
		}
		GetAllChallenges();
		if (_remasterMenuManager.CurrentChallenge == null)
		{
			SetIndexOnFirstActiveChallenge();
		}
		else
		{
			SetIndexOnCurrentChallenge();
		}
		UpdateChallengeDisplay();
	}

	public void SetIndexOnFirstActiveChallenge()
	{
		_currentIndex = 0;
	}

	public void SetIndexOnCurrentChallenge()
	{
		Challenge currentChallenge = _remasterMenuManager.CurrentChallenge;
		for (int i = 0; i < _allChallenges.Count; i++)
		{
			if (_allChallenges[i] == currentChallenge)
			{
				_currentIndex = RemasterMath.Modulo(i - 2, _allChallenges.Count);
				return;
			}
		}
		_currentIndex = 0;
	}

	public int GetCycledIndexOfNumber(int number)
	{
		return (_currentIndex + number) % _allChallenges.Count;
	}

	public void CycleChallengeForth()
	{
		_currentIndex++;
		if (_currentIndex >= _allChallenges.Count)
		{
			_currentIndex = 0;
		}
		UpdateChallengeDisplay();
	}

	public void CycleChallengeBack()
	{
		_currentIndex--;
		if (_currentIndex < 0)
		{
			_currentIndex = _allChallenges.Count - 1;
		}
		UpdateChallengeDisplay();
	}

	public void UpdateChallengeDisplay()
	{
		_challengesSlotController[0].SetChallengeData(_allChallenges[GetCycledIndexOfNumber(0)]);
		_challengesSlotController[1].SetChallengeData(_allChallenges[GetCycledIndexOfNumber(1)]);
		_challengesSlotController[2].SetChallengeData(_allChallenges[GetCycledIndexOfNumber(2)]);
		_challengesSlotController[3].SetChallengeData(_allChallenges[GetCycledIndexOfNumber(3)]);
		_challengesSlotController[4].SetChallengeData(_allChallenges[GetCycledIndexOfNumber(4)]);
		_challengeTitle.text = _allChallenges[GetCycledIndexOfNumber(2)].Name;
	}
}
