using System.Collections;
using FMODUnity;
using RG.Parsecs.Common;
using UnityEngine;

public class DamageEffector : MonoBehaviour
{
	[SerializeField]
	private GameObject _target;

	[SerializeField]
	private ParticleSystem[] _effector;

	[SerializeField]
	private LayerMask _layerMask;

	[EventRef]
	[SerializeField]
	private string _soundName = string.Empty;

	[SerializeField]
	private Achievement _achievementToUnlock;

	[SerializeField]
	private Transform _transformTarget;

	[SerializeField]
	private bool _useRaycastsForDamageDetection = true;

	[SerializeField]
	private bool _useCollisionsForDamageDetection;

	private const string PLAYER_TAG = "Player";

	public event DamageHandler OnDamage;

	private void Start()
	{
		for (int i = 0; i < _effector.Length; i++)
		{
			_effector[i].gameObject.SetActive(value: false);
			_effector[i].Stop();
		}
		if (_useRaycastsForDamageDetection)
		{
			StartCoroutine(DetectDamage());
		}
	}

	private IEnumerator DetectDamage()
	{
		yield return new WaitForSeconds(1f);
		do
		{
			yield return new WaitForSeconds(0.5f);
		}
		while (Physics.Raycast(_target.transform.position + new Vector3(0f, 0.5f, 0f), -_target.transform.up, 100f, _layerMask.value));
		Activate();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_useCollisionsForDamageDetection && other.transform.CompareTag("Player"))
		{
			Activate();
			_useCollisionsForDamageDetection = false;
		}
	}

	private void Activate()
	{
		if (_achievementToUnlock != null)
		{
			AchievementsSystem.UnlockAchievement(_achievementToUnlock);
		}
		for (int i = 0; i < _effector.Length; i++)
		{
			_effector[i].gameObject.SetActive(value: true);
			_effector[i].Play();
		}
		if (!string.IsNullOrEmpty(_soundName))
		{
			if (_transformTarget != null)
			{
				AudioManager.PlaySoundAtPoint(_soundName, _transformTarget);
			}
			else
			{
				AudioManager.PlaySoundAtPoint(_soundName, base.transform);
			}
		}
		if (this.OnDamage != null)
		{
			this.OnDamage();
		}
	}
}
