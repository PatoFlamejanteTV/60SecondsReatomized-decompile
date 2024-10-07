using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Menu;

public class ChallengeSlotController : MonoBehaviour
{
	[SerializeField]
	private Image[] _missionImages;

	[SerializeField]
	private GameObject _challengeCompleted;

	public Challenge Challenge { get; private set; }

	public void SetChallengeData(Challenge challenge)
	{
		Challenge = challenge;
		for (int i = 0; i < _missionImages.Length; i++)
		{
			_missionImages[i].sprite = challenge.ChallengeGraphic;
		}
		_challengeCompleted.SetActive(challenge.IsUnlocked);
	}
}
