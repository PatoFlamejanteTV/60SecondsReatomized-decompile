using System;
using UnityEngine;

[Serializable]
public struct SDifficultyCollection
{
	[SerializeField]
	private SDifficultyData _easy;

	[SerializeField]
	private SDifficultyData _normal;

	[SerializeField]
	private SDifficultyData _hard;

	public SDifficultyData Easy => _easy;

	public SDifficultyData Normal => _normal;

	public SDifficultyData Hard => _hard;
}
