using UnityEngine;

public class SetAsSelectable : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GamepadButton component = animator.gameObject.GetComponent<GamepadButton>();
		if (component != null)
		{
			component.SelectThisSelectable();
		}
	}
}
