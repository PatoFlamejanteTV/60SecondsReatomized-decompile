using UnityEngine;

namespace RG.SecondsRemaster.Scavenge;

[RequireComponent(typeof(Animator))]
public class ScavengeTextController : MonoBehaviour
{
	private Animator _animator;

	private const string SHOW_TEXT_PROPERTY_NAME = "Show";

	private void Awake()
	{
		_animator = GetComponent<Animator>();
	}

	public void ShowText()
	{
		_animator.SetTrigger("Show");
	}

	internal void ShowTextDelayed(float delay)
	{
		Invoke("ShowText", delay);
	}
}
