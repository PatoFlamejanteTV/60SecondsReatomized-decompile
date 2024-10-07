using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class FlyingVisualsManagerAnimationStateNotifier : MonoBehaviour
{
	[SerializeField]
	private FlyingVisualsManager _manager;

	public void NotifyAnimationEnded()
	{
		_manager.IsAnimationPlaying = false;
	}
}
