using RG.Parsecs.Common;
using UnityEngine;
using UnityEngine.UI;

public class RemasterAchievementTooltipContent : TooltipContent
{
	[SerializeField]
	private Achievement _achievement;

	[SerializeField]
	private Image _icon;

	public Achievement Achievement => _achievement;

	private void Start()
	{
		if (!_achievement.IsAchieved)
		{
			_icon.color = Color.clear;
		}
	}

	public override bool IsValid()
	{
		return _achievement != null;
	}
}
