using RG.Core.Base;
using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/Challenges/New Reward", fileName = "New Reward")]
public class RewardItem : RGScriptableObject
{
	[SerializeField]
	private Sprite _icon;

	[SerializeField]
	private Sprite _conclusionIcon;

	[SerializeField]
	private GlobalBoolVariable _scavengeRewardIsUnlockedVariable;

	public GlobalBoolVariable ScavengeRewardIsUnlockedVariable => _scavengeRewardIsUnlockedVariable;

	public Sprite Icon
	{
		get
		{
			return _icon;
		}
		set
		{
			_icon = value;
		}
	}

	public Sprite ConclusionIcon => _conclusionIcon;
}
