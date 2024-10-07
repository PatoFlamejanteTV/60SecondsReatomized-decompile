using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class SetTooltipLayoutSpacing : MonoBehaviour
{
	[SerializeField]
	private LinesDescription _linesDescription;

	private VerticalLayoutGroup _layoutGroup;

	private void OnEnable()
	{
		if (_layoutGroup == null)
		{
			_layoutGroup = GetComponent<VerticalLayoutGroup>();
		}
		_layoutGroup.spacing = _linesDescription.GetLinesDescriptionForCurrentLanguage().TooltipLayoutSpacing;
	}
}
