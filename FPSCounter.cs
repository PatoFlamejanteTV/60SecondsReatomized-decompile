using System.Collections;
using UnityEngine;

[AddComponentMenu("Utilities/HUDFPS")]
public class FPSCounter : MonoBehaviour
{
	public Rect startRect;

	public bool updateColor = true;

	public bool allowDrag = true;

	public float frequency = 0.5f;

	public int nbDecimal = 1;

	private float accum;

	private int frames;

	private Color color = Color.white;

	private string sFPS = "";

	private GUIStyle style;

	private void Start()
	{
		startRect = new Rect((float)(Screen.width / 2) - 37f, 10f, 75f, 50f);
		StartCoroutine(FPS());
	}

	private void Update()
	{
		accum += Time.timeScale / Time.deltaTime;
		frames++;
	}

	private IEnumerator FPS()
	{
		while (true)
		{
			float num = accum / (float)frames;
			sFPS = num.ToString("f" + Mathf.Clamp(nbDecimal, 0, 10));
			color = ((num >= 30f) ? Color.green : ((num > 10f) ? Color.yellow : Color.red));
			accum = 0f;
			frames = 0;
			yield return new WaitForSeconds(frequency);
		}
	}

	private void OnGUI()
	{
		if (style == null)
		{
			style = new GUIStyle(GUI.skin.label);
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleCenter;
		}
		GUI.color = (updateColor ? color : Color.white);
		startRect = GUI.Window(0, startRect, DoMyWindow, "");
	}

	private void DoMyWindow(int windowID)
	{
		GUI.Label(new Rect(0f, 0f, startRect.width, startRect.height), sFPS + " FPS", style);
		if (allowDrag)
		{
			GUI.DragWindow(new Rect(0f, 0f, Screen.width, Screen.height));
		}
	}
}
