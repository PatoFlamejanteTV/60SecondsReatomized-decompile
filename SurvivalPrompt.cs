using UnityEngine;
using UnityEngine.UI;

public class SurvivalPrompt : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup _promptCanvasGroup;

	[SerializeField]
	private bool _checkButtonInteractable;

	[SerializeField]
	private Button _buttonToCheck;

	public void OnEnable()
	{
		SurvivalPromptsController.Instance.RegisterPrompt(this);
	}

	public void OnDisable()
	{
		SurvivalPromptsController.Instance.UnregisterPrompt(this);
	}

	public void Show()
	{
		if (!_checkButtonInteractable || !(_buttonToCheck != null) || _buttonToCheck.interactable)
		{
			_promptCanvasGroup.alpha = 1f;
		}
	}

	public void Hide()
	{
		_promptCanvasGroup.alpha = 0f;
	}
}
