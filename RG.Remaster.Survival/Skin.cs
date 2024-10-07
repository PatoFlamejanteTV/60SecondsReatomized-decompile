using UnityEngine;

namespace RG.Remaster.Survival;

public class Skin : MonoBehaviour
{
	[SerializeField]
	private SkinId _id;

	public SkinId Id => _id;
}
