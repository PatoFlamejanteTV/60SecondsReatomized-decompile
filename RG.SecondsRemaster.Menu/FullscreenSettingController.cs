using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class FullscreenSettingController : MonoBehaviour
{
	[SerializeField]
	private Animator _knobAnimator;

	[SerializeField]
	private GlobalBoolVariable _isFullScreen;

	[SerializeField]
	private GlobalIntVariable _widthVariable;

	[SerializeField]
	private GlobalIntVariable _heightVariable;

	[SerializeField]
	private bool _applyInstantly;

	private static readonly int Right = Animator.StringToHash("Right");

	private const string NO_KNOB_STATE = "Value_Knob_Left";

	private const string YES_KNOB_STATE = "Value_Knob_Right";

	private const string RIGHT_PARAM_NAME = "Right";

	private void OnEnable()
	{
		_knobAnimator.Play(_isFullScreen.Value ? "Value_Knob_Right" : "Value_Knob_Left");
		_knobAnimator.SetBool(Right, _isFullScreen.Value);
	}

	public void SwitchFullscreen()
	{
		SetFullscreen(!_isFullScreen.Value);
	}

	public void SetFullscreen(bool fullscreen)
	{
		_isFullScreen.Value = fullscreen;
		_knobAnimator.SetBool(Right, fullscreen);
		if (_applyInstantly)
		{
			Screen.SetResolution(_widthVariable.Value, _heightVariable.Value, _isFullScreen.Value);
		}
	}
}
