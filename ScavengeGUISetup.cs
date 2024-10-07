using UnityEngine;

public class ScavengeGUISetup : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _43objects;

	[SerializeField]
	private dfTweenPlayableBase[] _43tweens;

	[SerializeField]
	private GameObject[] _nativeObjects;

	[SerializeField]
	private dfTweenPlayableBase[] _nativeTweens;

	[SerializeField]
	private GameObject[] _1610objects;

	[SerializeField]
	private dfTweenPlayableBase[] _1610tweens;

	private bool _processed;

	private void Awake()
	{
		if (!_processed)
		{
			Process();
		}
	}

	public void Process()
	{
		_processed = true;
		ResolutionHandler resolutionHandler = Object.FindObjectOfType<ResolutionHandler>();
		GlobalTools.GetController<GameFlow>();
		bool flag = ResolutionHandler.Is169(resolutionHandler.SelectedAspectRatio.AspectRatio);
		bool flag2 = ResolutionHandler.Is1610(resolutionHandler.SelectedAspectRatio.AspectRatio);
		bool flag3 = ResolutionHandler.Is43(resolutionHandler.SelectedAspectRatio.AspectRatio);
		if (!flag && !flag2 && !flag3)
		{
			flag = true;
		}
		if (flag3 && resolutionHandler.SelectedAspectRatio.AspectRatio == 1.25f)
		{
			for (int i = 0; i < _43objects.Length; i++)
			{
				dfSprite component = _43objects[i].GetComponent<dfSprite>();
				component.Height = component.Width / 1.25f;
			}
		}
		ActivateObjects(_nativeObjects, flag);
		ActivateTweens(_nativeTweens, flag);
		ActivateObjects(_1610objects, flag2);
		ActivateTweens(_1610tweens, flag2);
		ActivateObjects(_43objects, flag3);
		ActivateTweens(_43tweens, flag3);
		Object.Destroy(this);
	}

	private static void ActivateObjects(GameObject[] objects = null, bool activate = false)
	{
		if (objects != null)
		{
			for (int i = 0; i < objects.Length; i++)
			{
				objects[i].SetActive(activate);
			}
		}
	}

	private static void ActivateTweens(dfTweenPlayableBase[] tweens = null, bool activate = false)
	{
		if (tweens != null)
		{
			for (int i = 0; i < tweens.Length; i++)
			{
				tweens[i].enabled = activate;
			}
		}
	}
}
