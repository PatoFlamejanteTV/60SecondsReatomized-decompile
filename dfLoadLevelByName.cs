using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("Daikon Forge/Examples/General/Load Level On Click")]
public class dfLoadLevelByName : MonoBehaviour
{
	public string LevelName;

	private void OnClick()
	{
		if (!string.IsNullOrEmpty(LevelName))
		{
			Application.LoadLevel(LevelName);
		}
	}
}
