using RG.Core.GameVersion;
using TMPro;
using UnityEngine;

public class VersionSettingController : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _valueField;

	[SerializeField]
	private GameVersion _versionData;

	private void OnEnable()
	{
		_valueField.text = GetVersionString(_versionData);
	}

	private string GetVersionString(GameVersion gv)
	{
		if (gv != null)
		{
			int num = ((gv.Major > 0) ? gv.Major : 0);
			int num2 = ((gv.Minor > 0) ? gv.Minor : 0);
			int num3 = ((gv.Build > 0) ? gv.Build : 0);
			int num4 = ((gv.Patch > 0) ? gv.Patch : 0);
			return $"{num}.{num2}.{num4}.{num3}";
		}
		return string.Empty;
	}
}
