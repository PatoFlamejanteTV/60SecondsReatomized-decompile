using UnityEngine;

public class GlobalTools
{
	private static PlayerInventory _inventoryCache;

	private static ThirdPersonController _thirdPersonController;

	private static PlayerInteraction _interactionCache;

	private static GameObject _playerCache;

	private static Shelter _shelterCache;

	private static ShelterInventory _shelterInventoryCache;

	public static void DebugLog(object log)
	{
		if (Debug.isDebugBuild)
		{
			Debug.Log(log);
		}
	}

	public static void DebugLogError(object error)
	{
		if (Debug.isDebugBuild)
		{
			Debug.LogError(error);
		}
	}

	public static PlayerInventory GetPlayerInventory()
	{
		if (_inventoryCache == null)
		{
			_inventoryCache = Object.FindObjectOfType<PlayerInventory>();
		}
		return _inventoryCache;
	}

	public static ThirdPersonController GetThirdPersonController()
	{
		if (_thirdPersonController == null)
		{
			_thirdPersonController = Object.FindObjectOfType<ThirdPersonController>();
		}
		return _thirdPersonController;
	}

	public static PlayerInteraction GetPlayerInteraction()
	{
		if (_interactionCache == null)
		{
			_interactionCache = Object.FindObjectOfType<PlayerInteraction>();
		}
		return _interactionCache;
	}

	public static GameObject GetPlayer()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
		if (gameObject == null)
		{
			GetController<GameFlow>().SpawnPlayer(GameSessionData.Instance.GetPlayerTemplate());
			gameObject = GameObject.FindGameObjectWithTag("Player");
		}
		return gameObject;
	}

	public static Shelter GetShelter()
	{
		if (_shelterCache == null)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Exit");
			if (gameObject != null)
			{
				_shelterCache = gameObject.GetComponent<Shelter>();
			}
		}
		return _shelterCache;
	}

	public static ShelterInventory GetShelterInventory()
	{
		if (_shelterInventoryCache == null)
		{
			_shelterInventoryCache = Object.FindObjectOfType<ShelterInventory>();
		}
		return _shelterInventoryCache;
	}

	public static void HandleErrorMenu()
	{
		if (GameObject.Find("ErrorReportMenu") == null)
		{
			OpenErrorMenu();
		}
		else
		{
			CloseErrorMenu();
		}
	}

	public static void OpenErrorMenu()
	{
		GameObject gameObject = GameObject.Find("ErrorReportMenu");
		if (!(gameObject == null))
		{
			return;
		}
		GameObject gameObject2 = GameObject.FindGameObjectWithTag("DebugPlaceholder");
		if (gameObject2 != null)
		{
			gameObject = Object.Instantiate(Resources.Load("ErrorReportMenu")) as GameObject;
			if (gameObject != null)
			{
				gameObject2.GetComponent<dfControl>().IsInteractive = true;
				gameObject.name = "ErrorReportMenu";
				gameObject.transform.parent = gameObject2.transform;
			}
		}
	}

	public static void CloseErrorMenu()
	{
		GameObject gameObject = GameObject.Find("ErrorReportMenu");
		if (gameObject != null)
		{
			gameObject.transform.parent.GetComponent<dfControl>().IsInteractive = false;
			Object.Destroy(gameObject);
		}
	}

	public static T GetController<T>() where T : Component
	{
		return GameObject.FindGameObjectWithTag("GameController").GetComponent<T>();
	}

	public static AudioSource PlaySound(AudioEntry sound, bool loop = false, float delay = 0f, float volume = float.MaxValue, float pitch = float.MaxValue, Transform location = null)
	{
		return null;
	}

	public static AudioSource PlaySound(AudioClip sound, bool loop = false, float delay = 0f, float volume = float.MaxValue, float pitch = float.MaxValue, Transform location = null)
	{
		return null;
	}

	public static AudioSource PlaySound(string soundName, bool loop = false, float delay = 0f, float volume = float.MaxValue, float pitch = float.MaxValue, Transform location = null)
	{
		return null;
	}

	public static void StopSound(AudioSource sound, bool crossOut = false, float crossTime = 1f)
	{
	}

	public static int GetBitfieldValue(int val)
	{
		return (int)Mathf.Pow(10f, val);
	}

	public static bool TestBitfieldValue(int v1, int v2)
	{
		return v1 == v2;
	}

	public static bool TestBitfield(int testedDigit, int val)
	{
		int num = 1;
		int num2 = 1;
		while (num < val)
		{
			num2++;
			num *= 10;
		}
		if (num != val)
		{
			num2--;
		}
		int num3 = num2 - testedDigit;
		int num4 = 0;
		int num5 = val;
		int num6 = 0;
		while (num5 != 0 && num6 < num3)
		{
			num4 = num5 % 10;
			num5 /= 10;
			num6++;
		}
		if (num5 == 0 && num6 < num3)
		{
			return false;
		}
		return num4 > 0;
	}
}
