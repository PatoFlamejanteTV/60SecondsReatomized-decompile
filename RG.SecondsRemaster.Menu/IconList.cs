using System;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Menu;

[Serializable]
public class IconList
{
	[SerializeField]
	private Image[] _icons;

	public Image[] Icons => _icons;
}
