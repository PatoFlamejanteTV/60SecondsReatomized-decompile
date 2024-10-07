using RG.SecondsRemaster.Survival;
using UnityEngine;

public class SetCurrentJournalTabAsSelectable : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		JournalController journalController = Object.FindObjectOfType<JournalController>();
		if (!(journalController != null))
		{
			return;
		}
		JournalTabsController tabs = journalController.Tabs;
		if (tabs != null && tabs.CurrentTab != null)
		{
			GamepadButton component = tabs.CurrentTab.GetComponent<GamepadButton>();
			if (component != null)
			{
				component.SelectThisSelectable();
			}
		}
	}
}
