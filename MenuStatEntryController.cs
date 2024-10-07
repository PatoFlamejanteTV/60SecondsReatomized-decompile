using I2.Loc;
using RG.Parsecs.EventEditor;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MenuStatEntryController : MonoBehaviour
{
	private TextMeshProUGUI _text;

	[Tooltip("Fill this field if the stat is supposed to be a single or double int value.")]
	[SerializeField]
	private GlobalIntVariable _parameter0;

	[Tooltip("Fill this field if the stat is supposed to be a double int value.")]
	[SerializeField]
	private GlobalIntVariable _parameter1;

	[Tooltip("Fill this field if the stat is supposed to be a single float value.")]
	[SerializeField]
	private GlobalFloatVariable _parameterFloat;

	[SerializeField]
	private LocalizedString _term;

	private void Awake()
	{
		_text = GetComponent<TextMeshProUGUI>();
		_text.SetText(string.Format(_term.ToString(), GetData()));
		LocalizationManager.OnLocalizeEvent += LanguageChanged;
	}

	public void LanguageChanged()
	{
		_text.SetText(string.Format(_term.ToString(), GetData()));
	}

	private object[] GetData()
	{
		if (_parameter0 != null && _parameter1 == null)
		{
			return new object[1] { _parameter0.Value };
		}
		if (_parameter1 != null && _parameter0 != null)
		{
			return new object[2] { _parameter0.Value, _parameter1.Value };
		}
		if (_parameterFloat != null)
		{
			return new object[1] { _parameterFloat.Value };
		}
		Debug.LogError("No parameters set up in MenuStatEntryController!", this);
		return null;
	}
}
