using RG_GameCamera.Modes;
using UnityEngine;

namespace RG_GameCamera.Examples;

public class SpawnDespawn : MonoBehaviour
{
	public GameObject CharacterControllerPrefab;

	public GameObject CharacterControllerCurrent;

	private CameraManager cameraManager;

	private Vector3 lastPos;

	private bool spawned;

	private void Start()
	{
		spawned = true;
		cameraManager = CameraManager.Instance;
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(10f, 100f, 300f, 30f), spawned ? "Despawn" : "Spawn"))
		{
			spawned = !spawned;
			if (spawned)
			{
				Spawn();
			}
			else
			{
				Despawn();
			}
		}
	}

	private void Spawn()
	{
		CharacterControllerCurrent = Object.Instantiate(CharacterControllerPrefab, lastPos, Quaternion.identity);
		cameraManager.SetCameraTarget(CharacterControllerCurrent.transform);
		cameraManager.SetMode(Type.ThirdPerson);
	}

	private void Despawn()
	{
		lastPos = CharacterControllerCurrent.transform.position;
		Object.Destroy(CharacterControllerCurrent.gameObject);
		cameraManager.SetMode(Type.None);
	}
}
