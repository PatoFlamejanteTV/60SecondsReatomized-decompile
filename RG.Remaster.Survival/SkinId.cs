using System;
using RG.Core.Base;
using UnityEngine;

namespace RG.Remaster.Survival;

[Serializable]
[CreateAssetMenu(menuName = "60 Seconds Remaster!/Skin Id", fileName = "New SkinId")]
public class SkinId : RGScriptableObject
{
	public string Id => Guid;
}
