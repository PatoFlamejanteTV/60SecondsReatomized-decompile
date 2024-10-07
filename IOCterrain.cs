using System;
using UnityEngine;

public class IOCterrain : IOCcomp
{
	private IOCcam iocCam;

	private bool hidden;

	private int counter;

	private int frameInterval;

	private Terrain terrain;

	private void Awake()
	{
		Init();
	}

	public override void Init()
	{
		try
		{
			iocCam = Camera.main.GetComponent<IOCcam>();
			terrain = GetComponent<Terrain>();
			base.enabled = true;
		}
		catch (Exception ex)
		{
			base.enabled = false;
			Debug.Log(ex.Message);
		}
	}

	private void Start()
	{
		terrain.enabled = false;
	}

	private void Update()
	{
		frameInterval = Time.frameCount % 4;
		if (frameInterval == 0 && !hidden && Time.frameCount - counter > iocCam.hideDelay)
		{
			Hide();
		}
	}

	public void Hide()
	{
		terrain.enabled = false;
		hidden = true;
	}

	public override void UnHide(RaycastHit hit)
	{
		counter = Time.frameCount;
		terrain.enabled = true;
		hidden = false;
	}
}
