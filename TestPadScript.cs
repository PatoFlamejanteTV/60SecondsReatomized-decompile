using UnityEngine;

public class TestPadScript : MonoBehaviour
{
	[SerializeField]
	private GamepadPanelCloseable _visible;

	[SerializeField]
	private GamepadPanelCloseable _hide;

	private bool _stateVisible;

	public void Show()
	{
		_stateVisible = !_stateVisible;
		if (_stateVisible)
		{
			_visible.Hide();
			_hide.Show();
		}
		else
		{
			_hide.Hide();
			_visible.Show();
		}
	}
}
