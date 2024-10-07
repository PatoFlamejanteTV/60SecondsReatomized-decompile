using RG.Parsecs.Survival;

namespace RG.SecondsRemaster.Survival;

public class RemasterTooltipsUIManager : TooltipsUIManager
{
	public void HideTooltipsUI()
	{
		for (int i = 0; i < _handlers.Count; i++)
		{
			_handlers[i].gameObject.SetActive(value: false);
		}
	}
}
