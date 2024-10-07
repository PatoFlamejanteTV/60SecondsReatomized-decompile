using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class CharacterScratchController : MonoBehaviour
{
	[SerializeField]
	private GameObject _scratch;

	[SerializeField]
	private Image _icon;

	public void SetScratch(bool characterAvaialable)
	{
		_scratch.SetActive(!characterAvaialable);
		_icon.color = (characterAvaialable ? Color.white : new Color(1f, 1f, 1f, 1f));
	}
}
