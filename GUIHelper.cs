using System.Collections;
using UnityEngine;

public class GUIHelper
{
	public delegate void Concluder(bool flag);

	public static GameObject Obscurer;

	public static GameObject CircleFader;

	public static void MakeObscurerVisible(bool visible)
	{
		dfPanel dfPanel2 = ((Obscurer != null) ? Obscurer.GetComponent<dfPanel>() : null);
		if (dfPanel2 != null)
		{
			dfPanel2.BackgroundColor = (visible ? new Color32(0, 0, 0, byte.MaxValue) : new Color32(0, 0, 0, 0));
			dfPanel2.Opacity = (visible ? 255f : 0f);
		}
		dfSprite dfSprite2 = ((Obscurer != null) ? Obscurer.GetComponent<dfSprite>() : null);
		if (dfSprite2 != null)
		{
			dfSprite2.Color = (visible ? new Color32(0, 0, 0, byte.MaxValue) : new Color32(0, 0, 0, 0));
			dfSprite2.Opacity = (visible ? 255f : 0f);
		}
	}

	private static Vector3 GetCircleFadeSize(bool max)
	{
		if (max)
		{
			ResolutionHandler resolutionHandler = Object.FindObjectOfType<ResolutionHandler>();
			float num = ((resolutionHandler == null) ? 1f : resolutionHandler.ResizeRatio) * 6.25f;
			return new Vector3(num, num, 1f);
		}
		return new Vector3(0f, 0f, 1f);
	}

	public static IEnumerator DoCircleFade(bool fadeIn, float time, float initalDelay = 0f, bool destroyCircleFader = false, bool obscurerEndDeactivate = true, Concluder concluder = null, bool concluderFlag = false)
	{
		if (!Mathf.Approximately(initalDelay, 0f))
		{
			yield return new WaitForSeconds(initalDelay);
		}
		if (CircleFader == null)
		{
			CircleFader = GameObject.Find("CircleFader");
			if (CircleFader != null && CircleFader.transform.parent != null)
			{
				Obscurer = CircleFader.transform.parent.gameObject;
			}
		}
		if (CircleFader != null)
		{
			CircleFader.GetComponent<Renderer>().enabled = true;
			float startVal;
			float endVal;
			if (fadeIn)
			{
				startVal = 0f;
				endVal = 1f;
			}
			else
			{
				startVal = 1f;
				endVal = 0f;
			}
			Material mat = CircleFader.GetComponent<Renderer>().material;
			float endTime = Time.time + time;
			float currentTime = 0f;
			while (Time.time < endTime)
			{
				float value = Mathf.Lerp(startVal, endVal, currentTime / time);
				mat.SetFloat("_Cutoff", value);
				currentTime += Time.deltaTime;
				yield return null;
			}
			mat.SetFloat("_Cutoff", endVal);
			concluder?.Invoke(concluderFlag);
			if (destroyCircleFader)
			{
				Object.Destroy(CircleFader);
				CircleFader = null;
			}
			else if (fadeIn)
			{
				CircleFader.GetComponent<Renderer>().enabled = false;
			}
		}
	}

	public static void ScaleObject(GameObject obj, Vector3 scale, float time)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("scale", scale);
		hashtable.Add("time", time);
		hashtable.Add("looptype", iTween.LoopType.none);
		hashtable.Add("easeType", "linear");
		iTween.ScaleTo(obj, hashtable);
	}
}
