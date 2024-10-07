using UnityEngine;

public class PauseMenuAnimationControl : MonoBehaviour
{
	[SerializeField]
	private GameObject _pauseMenuObject;

	public void SetTimeScaleToZero()
	{
		Time.timeScale = 0f;
	}

	public void DisablePauseMenuObject()
	{
		_pauseMenuObject.SetActive(value: false);
	}
}
