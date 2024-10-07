using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Menu;

public class RuleController : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _ruleTitle;

	[SerializeField]
	private Image _ruleIcon;

	[SerializeField]
	private RectTransform _ruleRect;

	public void SetRule(LocalizedString title, Sprite icon)
	{
		_ruleTitle.text = title;
		_ruleIcon.sprite = icon;
		_ruleRect.sizeDelta = new Vector2(_ruleRect.sizeDelta.x, Mathf.Max(60f, _ruleTitle.preferredHeight));
	}
}
