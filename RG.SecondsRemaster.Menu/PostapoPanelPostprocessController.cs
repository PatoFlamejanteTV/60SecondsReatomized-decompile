using System.Collections;
using Cinemachine.PostFX;
using RG.Parsecs.EventEditor;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace RG.SecondsRemaster.Menu;

public class PostapoPanelPostprocessController : MonoBehaviour
{
	[SerializeField]
	private CinemachinePostFX _cameraPostFXComponent;

	[SerializeField]
	private PostProcessingProfile _currentPostProcessingProfile;

	[SerializeField]
	private GlobalBoolVariable _isContinueAvailable;

	[SerializeField]
	private PostProcessingProfile _postapoMainMenuPanelProfile;

	[SerializeField]
	private PostProcessingProfile _mainMenuProfile;

	private float _time;

	private const float SPEED = 1f;

	private const float TRANSITION_DURATION = 0.5f;

	private void OnEnable()
	{
		_time = Time.time;
		if (_isContinueAvailable != null && _isContinueAvailable.Value)
		{
			StartCoroutine(ShiftColorGradingToPanelProfile(_currentPostProcessingProfile.colorGrading.settings));
		}
		if (_isContinueAvailable != null && !_isContinueAvailable.Value)
		{
			SetNormalColorTones();
		}
	}

	public void SetNormalColorTones()
	{
		_currentPostProcessingProfile.colorGrading.settings = _mainMenuProfile.colorGrading.settings;
	}

	private IEnumerator ShiftColorGradingToPanelProfile(ColorGradingModel.Settings colorGradingSettings)
	{
		while (Time.time - _time < 0.5f)
		{
			float t = (Time.time - _time) * 1f / 0.5f;
			colorGradingSettings.basic.tint = Mathf.Lerp(_mainMenuProfile.colorGrading.settings.basic.tint, _postapoMainMenuPanelProfile.colorGrading.settings.basic.tint, t);
			colorGradingSettings.basic.temperature = Mathf.Lerp(_mainMenuProfile.colorGrading.settings.basic.temperature, _postapoMainMenuPanelProfile.colorGrading.settings.basic.temperature, t);
			colorGradingSettings.colorWheels.linear.gain.a = Mathf.Lerp(_mainMenuProfile.colorGrading.settings.colorWheels.linear.gain.a, _postapoMainMenuPanelProfile.colorGrading.settings.colorWheels.linear.gain.a, t);
			colorGradingSettings.colorWheels.linear.gain.r = Mathf.Lerp(_mainMenuProfile.colorGrading.settings.colorWheels.linear.gain.r, _postapoMainMenuPanelProfile.colorGrading.settings.colorWheels.linear.gain.r, t);
			colorGradingSettings.colorWheels.linear.gain.g = Mathf.Lerp(_mainMenuProfile.colorGrading.settings.colorWheels.linear.gain.g, _postapoMainMenuPanelProfile.colorGrading.settings.colorWheels.linear.gain.g, t);
			colorGradingSettings.colorWheels.linear.gain.b = Mathf.Lerp(_mainMenuProfile.colorGrading.settings.colorWheels.linear.gain.b, _postapoMainMenuPanelProfile.colorGrading.settings.colorWheels.linear.gain.b, t);
			_currentPostProcessingProfile.colorGrading.settings = colorGradingSettings;
			yield return null;
		}
		colorGradingSettings.curves = _postapoMainMenuPanelProfile.colorGrading.settings.curves;
	}

	private void OnDisable()
	{
		SetNormalColorTones();
	}

	private void OnApplicationQuit()
	{
		SetNormalColorTones();
	}
}
