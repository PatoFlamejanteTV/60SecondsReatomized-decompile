using System.Collections;
using UnityEngine;

public class CFX_ShurikenThreadFix : MonoBehaviour
{
	private ParticleSystem[] systems;

	private void OnEnable()
	{
		systems = GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = systems;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = false;
		}
		StartCoroutine("WaitFrame");
	}

	private IEnumerator WaitFrame()
	{
		yield return null;
		ParticleSystem[] array = systems;
		foreach (ParticleSystem obj in array)
		{
			obj.enableEmission = true;
			obj.Play(withChildren: true);
		}
	}
}
