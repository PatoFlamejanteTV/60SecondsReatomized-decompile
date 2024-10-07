using RG.Core.GameVersion;
using UnityEngine;

public class DebugGameVersion : MonoBehaviour
{
	[SerializeField]
	private GameVersion _gameVersion;

	private void Start()
	{
		Debug.Log(_gameVersion.GetReadableVersion() + " " + Application.platform);
	}
}
