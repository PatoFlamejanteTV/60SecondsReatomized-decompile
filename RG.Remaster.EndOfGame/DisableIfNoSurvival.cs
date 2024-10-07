using RG.Parsecs.EventEditor;
using RG.SecondsRemaster;
using UnityEngine;

namespace RG.Remaster.EndOfGame;

public class DisableIfNoSurvival : MonoBehaviour
{
	[SerializeField]
	private CurrentChallengeData _challengeData;

	[SerializeField]
	private GlobalBoolVariable _scavengeOnlyBoolVariable;

	private void OnEnable()
	{
		if (_challengeData != null && (bool)_scavengeOnlyBoolVariable && (_challengeData.RuntimeData.Challenge != null || _scavengeOnlyBoolVariable.Value))
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
