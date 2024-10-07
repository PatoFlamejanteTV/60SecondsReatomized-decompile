using I2.Loc;
using RG.Core.Base;
using UnityEngine;

namespace RG.SecondsRemaster;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/New Difficulty Level", fileName = "New Difficulty Level")]
public class DifficultyLevel : RGScriptableObject
{
	[Tooltip("This field allows to choose difficulty level in Scavenge from original 60 Seconds data.")]
	[SerializeField]
	private EGameDifficulty _scavengeDifficulty;

	[Tooltip("This field allows to set difficulty level in Survival which base on 60 Parsecs style. Design control the system in events based on the value in the global int variable.")]
	[SerializeField]
	private uint _survivalDifficulty;

	[Tooltip("Reference to original settings for game - from 60 Seconds.")]
	[SerializeField]
	private GameSetup _setup;

	[SerializeField]
	private LocalizedString _name;

	[SerializeField]
	private LocalizedString _description;

	public EGameDifficulty ScavengeDifficulty => _scavengeDifficulty;

	public LocalizedString Name => _name;

	public int SurvivalDifficulty => (int)_survivalDifficulty;

	public LocalizedString Description => _description;

	public GameSetup Setup => _setup;
}
