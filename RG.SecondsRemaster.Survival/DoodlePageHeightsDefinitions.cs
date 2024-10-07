using RG.Core.Base;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/Doodle Page Heights Definition")]
public class DoodlePageHeightsDefinitions : RGScriptableObject
{
	[SerializeField]
	private VisualId[] _visualIds;

	[SerializeField]
	[Range(0f, 1f)]
	private float[] _percentagePageHeights;

	private const float DEFAULT_PAGE_HEIGHT_ALLOWANCE = 0.5f;

	private const string ARRAYS_NOT_EQUAL_ERROR_MESSAGE = "VisualsIds and PercentagePageHeights arrays needs to be of an equal lengths.";

	public float GetPageHeight(VisualId visualId)
	{
		for (int i = 0; i < _visualIds.Length; i++)
		{
			if (_visualIds[i] == visualId)
			{
				return _percentagePageHeights[i];
			}
		}
		return 0.5f;
	}

	private void OnValidate()
	{
		if (_visualIds.Length != _percentagePageHeights.Length)
		{
			Debug.LogError("VisualsIds and PercentagePageHeights arrays needs to be of an equal lengths.", this);
		}
	}
}
