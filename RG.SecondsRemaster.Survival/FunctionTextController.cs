using I2.Loc;
using RG.Parsecs.EventEditor;
using TMPro;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class FunctionTextController : MonoBehaviour
{
	[SerializeField]
	private NodeFunction _function;

	[SerializeField]
	private TextMeshProUGUI _textField;

	[SerializeField]
	private bool _refreshOnStart;

	private const string FUNCTION_OUTPUT_NAME = "Output";

	private void Start()
	{
		if (_refreshOnStart)
		{
			RefreshText();
		}
	}

	public void RefreshText()
	{
		if (!(_function == null))
		{
			_function.Execute(null);
			LocalizedString localizedString = (LocalizedString)_function.GetOutputValue("Output");
			_textField.text = localizedString.ToString();
		}
	}
}
