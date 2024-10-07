using UnityEngine;

[AddComponentMenu("Daikon Forge/Examples/General/FPS Counter")]
public class dfFPSCounter : MonoBehaviour
{
	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private dfLabel label;

	private void Start()
	{
		label = GetComponent<dfLabel>();
		if (label == null)
		{
			Debug.LogError("FPS Counter needs a Label component!");
		}
		timeleft = updateInterval;
		label.Text = "";
	}

	private void Update()
	{
		if (label == null)
		{
			return;
		}
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)timeleft <= 0.0)
		{
			float num = accum / (float)frames;
			string text = $"{num:F0} FPS";
			label.Text = text;
			if (num < 30f)
			{
				label.Color = Color.yellow;
			}
			else if (num < 10f)
			{
				label.Color = Color.red;
			}
			else
			{
				label.Color = Color.green;
			}
			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}
}
