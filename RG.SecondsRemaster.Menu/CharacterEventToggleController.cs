using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class CharacterEventToggleController : UnityEventToggleController
{
	[SerializeField]
	private Character _character;

	public Character Character => _character;
}
