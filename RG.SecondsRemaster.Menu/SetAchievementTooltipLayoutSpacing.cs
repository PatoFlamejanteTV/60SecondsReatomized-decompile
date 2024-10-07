using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Menu;

public class SetAchievementTooltipLayoutSpacing : MonoBehaviour
{
	[SerializeField]
	private LinesDescription _linesDescription;

	[SerializeField]
	private VerticalLayoutGroup _layoutGroup;

	private void OnEnable()
	{
		if (_layoutGroup != null)
		{
			_layoutGroup.spacing = _linesDescription.GetLinesDescriptionForCurrentLanguage().AchievementTooltipLayoutSpacing;
		}
	}
}
