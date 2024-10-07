using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Gamma Correction")]
public class GammaCorrectionEffect : ImageEffectBase
{
	public float gamma;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.material.SetFloat("_Gamma", 1f / gamma);
		Graphics.Blit(source, destination, base.material);
	}
}
