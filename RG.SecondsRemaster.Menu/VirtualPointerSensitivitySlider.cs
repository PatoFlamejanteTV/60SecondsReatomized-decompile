using RG.Parsecs.EventEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Menu;

public class VirtualPointerSensitivitySlider : MonoBehaviour
{
	[SerializeField]
	private Slider _slider;

	[SerializeField]
	private GlobalFloatVariable _pointerSensitivity;

	private void OnEnable()
	{
		_slider.value = _pointerSensitivity.Value;
	}

	public void ChangeSensitivity(float value)
	{
		_pointerSensitivity.Value = value;
	}
}
