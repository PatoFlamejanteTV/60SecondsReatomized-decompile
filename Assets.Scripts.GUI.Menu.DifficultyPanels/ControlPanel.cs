using UnityEngine;

namespace Assets.Scripts.GUI.Menu.DifficultyPanels;

[RequireComponent(typeof(dfControl))]
internal abstract class ControlPanel : MonoBehaviour
{
	[SerializeField]
	private dfControl control;

	public bool IsVisible
	{
		get
		{
			return control.IsVisible;
		}
		set
		{
			control.IsVisible = value;
		}
	}

	public abstract string SetDifficulty(EGameDifficulty difficulty);
}
