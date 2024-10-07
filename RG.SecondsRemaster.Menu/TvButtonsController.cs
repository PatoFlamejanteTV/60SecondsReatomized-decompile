using UnityEngine;
using UnityEngine.EventSystems;

namespace RG.SecondsRemaster.Menu;

public class TvButtonsController : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _tvButtons;

	public void SwitchRandomSelectable()
	{
		int num = Random.Range(0, _tvButtons.Length);
		ExecuteEvents.Execute(_tvButtons[num].gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
	}
}
