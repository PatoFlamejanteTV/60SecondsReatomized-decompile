using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Menu;

public class MasterVolumeSettingController : MonoBehaviour
{
	[SerializeField]
	private Slider _slider;

	[SerializeField]
	private GlobalFloatVariable _masterVolume;

	[SerializeField]
	private GlobalFloatVariable _soundVolume;

	[SerializeField]
	private GlobalFloatVariable _musicVolume;

	[SerializeField]
	private bool _applyInstantly = true;

	private void OnEnable()
	{
		if (_masterVolume.Value < _slider.minValue)
		{
			_slider.value = _slider.maxValue;
			_masterVolume.Value = _slider.maxValue;
		}
		else
		{
			_slider.value = _masterVolume.Value;
		}
	}

	public void ChangeVolume(float value)
	{
		_masterVolume.Value = value;
		if (_applyInstantly)
		{
			AudioManager.Instance.SetMusicVolume(_musicVolume.Value * _masterVolume.Value);
			AudioManager.Instance.SetSfxVolume(_soundVolume.Value * _masterVolume.Value);
			AudioManager.Instance.SetUiVolume(_soundVolume.Value * _masterVolume.Value);
		}
	}
}
