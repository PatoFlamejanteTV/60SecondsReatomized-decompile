using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Menu;

public class SoundVolumeSettingController : MonoBehaviour
{
	[SerializeField]
	private Slider _slider;

	[SerializeField]
	private GlobalFloatVariable _masterVolume;

	[SerializeField]
	private GlobalFloatVariable _soundVolume;

	[SerializeField]
	private bool _applyInstantly = true;

	private void OnEnable()
	{
		if (_soundVolume.Value < _slider.minValue)
		{
			_slider.value = _slider.maxValue;
			_soundVolume.Value = _slider.maxValue;
		}
		else
		{
			_slider.value = _soundVolume.Value;
		}
	}

	public void ChangeVolume(float value)
	{
		_soundVolume.Value = value;
		if (_applyInstantly)
		{
			AudioManager.Instance.SetSfxVolume(_soundVolume.Value * _masterVolume.Value);
			AudioManager.Instance.SetUiVolume(_soundVolume.Value * _masterVolume.Value);
		}
	}
}
