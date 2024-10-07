using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.VirtualInput;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Menu;

public class GammaSettingController : MonoBehaviour
{
	[SerializeField]
	private Slider _slider;

	[SerializeField]
	private GlobalFloatVariable _gammaValue;

	[SerializeField]
	private PostProcessingProfile[] _postProcessingProfiles;

	[SerializeField]
	private bool _applyInstantly = true;

	private bool _blockChangeGammaInvoke;

	private const float EPSILON_FOR_DEFAULTING_TO_MIDDLE = 0.1f;

	private void OnEnable()
	{
		_blockChangeGammaInvoke = true;
		if (_gammaValue.Value < _slider.minValue)
		{
			_slider.value = 0f;
			_gammaValue.Value = 0f;
		}
		else
		{
			_slider.value = _gammaValue.Value;
		}
		_blockChangeGammaInvoke = false;
	}

	public void ChangeGamma(float value)
	{
		if (_blockChangeGammaInvoke)
		{
			return;
		}
		if (!Singleton<VirtualInputManager>.Instance.IsSelectablesMode() && Mathf.Abs(value) < 0.1f)
		{
			_blockChangeGammaInvoke = true;
			_slider.value = 0f;
			_blockChangeGammaInvoke = false;
		}
		_gammaValue.Value = value;
		if (_applyInstantly)
		{
			for (int i = 0; i < _postProcessingProfiles.Length; i++)
			{
				ColorGradingModel.Settings settings = _postProcessingProfiles[i].colorGrading.settings;
				settings.basic.postExposure = _gammaValue.Value;
				_postProcessingProfiles[i].colorGrading.settings = settings;
			}
		}
	}
}
