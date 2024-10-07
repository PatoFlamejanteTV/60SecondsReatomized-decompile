using System;
using UnityEngine;
using UnityEngine.Profiling;

[AddComponentMenu("Daikon Forge/Examples/General/Debug Render Info")]
public class DebugRenderInfo : MonoBehaviour
{
	public float interval = 0.5f;

	private dfLabel info;

	private dfGUIManager view;

	private float lastUpdate;

	private int frameCount;

	private void Start()
	{
		info = GetComponent<dfLabel>();
		if (info == null)
		{
			base.enabled = false;
			throw new Exception("No Label component found");
		}
		info.Text = "";
	}

	private void Update()
	{
		if (view == null)
		{
			view = info.GetManager();
		}
		frameCount++;
		float num = Time.realtimeSinceStartup - lastUpdate;
		if (!(num < interval))
		{
			lastUpdate = Time.realtimeSinceStartup;
			float num2 = 1f / (num / (float)frameCount);
			Vector2 vector = new Vector2(Screen.width, Screen.height);
			string text = $"{(int)vector.x}x{(int)vector.y}";
			float num3 = (Profiler.supported ? ((float)Profiler.GetMonoUsedSize() / 1048576f) : ((float)GC.GetTotalMemory(forceFullCollection: false) / 1048576f));
			string text2 = $"Screen : {text}, DrawCalls: {view.TotalDrawCalls}, Triangles: {view.TotalTriangles}, Mem: {num3:F0}MB, FPS: {num2:F0}";
			info.Text = text2.Trim();
			frameCount = 0;
		}
	}
}
