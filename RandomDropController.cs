using FMODUnity;
using UnityEngine;

public class RandomDropController : MonoBehaviour
{
	[SerializeField]
	private Shelter _shelter;

	[EventRef]
	[SerializeField]
	private string _dropSoundName;

	private void OnTriggerEnter(Collider collider)
	{
		if (_shelter != null && IsCollisionAllowed(collider.gameObject))
		{
			_shelter.DropIntoShelter(_dropSoundName);
		}
	}

	private bool IsCollisionAllowed(GameObject collider)
	{
		if (collider.CompareTag("Player"))
		{
			return false;
		}
		return true;
	}
}
