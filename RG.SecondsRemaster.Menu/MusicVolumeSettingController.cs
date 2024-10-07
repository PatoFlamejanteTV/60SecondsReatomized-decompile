using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Menu;

public class MusicVolumeSettingController : MonoBehaviour
{
	[SerializeField]
	private Slider _slider;

	[SerializeField]
	private GlobalFloatVariable _masterVolume;

	[SerializeField]
	private GlobalFloatVariable _musicVolume;

	[SerializeField]
	private bool _applyInstantly = true;

	private void OnEnable()
	{
		if (_musicVolume.Value < _slider.minValue)
		{
			_slider.value = _slider.maxValue;
			_musicVolume.Value = _slider.maxValue;
		}
		else
		{
			_slider.value = _musicVolume.Value;
		}
	}

	public void ChangeVolume(float value)
	{
		_musicVolume.Value = value;
		if (_applyInstantly)
		{
			AudioManager.Instance.SetMusicVolume(_musicVolume.Value * _masterVolume.Value);
		}
	}
}
