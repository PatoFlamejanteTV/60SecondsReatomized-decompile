using UnityEngine;

namespace RG_GameCamera.Utils;

public class CursorLocking : MonoBehaviour
{
	public bool LockCursor;

	public KeyCode LockKey;

	public KeyCode UnlockKey;

	public static bool IsLocked;

	private static CursorLocking instance;

	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		if (LockCursor)
		{
			Lock();
		}
		else
		{
			Unlock();
		}
		IsLocked = Screen.lockCursor;
		if (UnityEngine.Input.GetKeyDown(LockKey))
		{
			Lock();
		}
		if (UnityEngine.Input.GetKeyDown(UnlockKey))
		{
			Unlock();
		}
		if (!Screen.lockCursor)
		{
			Cursor.visible = true;
		}
	}

	public static void Lock()
	{
		Screen.lockCursor = true;
		Cursor.visible = false;
		instance.LockCursor = true;
	}

	public static void Unlock()
	{
		Screen.lockCursor = false;
		Cursor.visible = true;
		instance.LockCursor = false;
	}
}
