using I2.Loc;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class JournalExpeditionTooltipContent : JournalTooltipContent
{
	[SerializeField]
	private ExpeditionData _expeditionData;

	[SerializeField]
	private LocalizedString _expeditionOngoing;

	[SerializeField]
	private CharacterList _defaultCharacterList;

	[SerializeField]
	private CharacterStatus _onExpeditionStatus;

	public override LocalizedString Name()
	{
		if (_defaultCharacterList.GetCharactersWithStatus(_onExpeditionStatus.Guid).Count > 0)
		{
			return _expeditionOngoing;
		}
		if (_expeditionData.RuntimeData.IsOngoing)
		{
			return _expeditionOngoing;
		}
		return base.Name();
	}

	public override bool IsValid()
	{
		if (base.IsValid() && _expeditionData != null)
		{
			return !string.IsNullOrEmpty(_expeditionOngoing);
		}
		return false;
	}
}
