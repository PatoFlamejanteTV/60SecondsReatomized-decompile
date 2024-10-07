using UnityEngine;

public class IOCcam : MonoBehaviour
{
	public string tags;

	public LayerMask layerMsk;

	public int occludeeLayer;

	public int samples;

	public float raysFov;

	public bool preCullCheck;

	public float viewDistance;

	public int hideDelay;

	public bool realtimeShadows;

	public float lod1Distance;

	public float lod2Distance;

	public float lodMargin;

	public int lightProbes;

	public float probeRadius;

	private RaycastHit hit;

	private Ray r;

	private int layerMask;

	private IOCcomp iocComp;

	private int haltonIndex;

	private float[] hx;

	private float[] hy;

	private int pixels;

	private Camera cam;

	private Camera rayCaster;

	private void Awake()
	{
		cam = GetComponent<Camera>();
		hit = default(RaycastHit);
		if (viewDistance == 0f)
		{
			viewDistance = 100f;
		}
		cam.farClipPlane = viewDistance;
		haltonIndex = 0;
		if (GetComponent<SphereCollider>() == null)
		{
			SphereCollider sphereCollider = base.gameObject.AddComponent<SphereCollider>();
			sphereCollider.radius = 1f;
			sphereCollider.isTrigger = true;
		}
	}

	private void Start()
	{
		pixels = Mathf.FloorToInt((float)(Screen.width * Screen.height) / 4f);
		hx = new float[pixels];
		hy = new float[pixels];
		for (int i = 0; i < pixels; i++)
		{
			hx[i] = HaltonSequence(i, 2);
			hy[i] = HaltonSequence(i, 3);
		}
		Object[] array = Object.FindObjectsOfType(typeof(GameObject));
		for (int j = 0; j < array.Length; j++)
		{
			GameObject gameObject = (GameObject)array[j];
			if (!tags.Contains(gameObject.tag))
			{
				continue;
			}
			if (gameObject.GetComponent<Light>() != null)
			{
				if (gameObject.GetComponent<IOClight>() == null)
				{
					gameObject.AddComponent<IOClight>();
				}
			}
			else if (gameObject.GetComponent<Terrain>() != null)
			{
				gameObject.AddComponent<IOCterrain>();
			}
			else if (gameObject.GetComponent<IOClod>() == null)
			{
				gameObject.AddComponent<IOClod>();
			}
		}
		GameObject gameObject2 = new GameObject("RayCaster");
		gameObject2.transform.Translate(base.transform.position);
		gameObject2.transform.rotation = base.transform.rotation;
		rayCaster = gameObject2.AddComponent<Camera>();
		rayCaster.enabled = false;
		rayCaster.clearFlags = CameraClearFlags.Nothing;
		rayCaster.cullingMask = 0;
		rayCaster.aspect = cam.aspect;
		rayCaster.nearClipPlane = cam.nearClipPlane;
		rayCaster.farClipPlane = cam.farClipPlane;
		rayCaster.fieldOfView = raysFov;
		gameObject2.transform.parent = base.transform;
	}

	private void Update()
	{
		for (int i = 0; i <= samples; i++)
		{
			r = rayCaster.ViewportPointToRay(new Vector3(hx[haltonIndex], hy[haltonIndex], 0f));
			haltonIndex++;
			if (haltonIndex >= pixels)
			{
				haltonIndex = 0;
			}
			if (Physics.Raycast(r, out hit, viewDistance, layerMsk.value))
			{
				if ((bool)(iocComp = hit.transform.GetComponent<IOCcomp>()))
				{
					iocComp.UnHide(hit);
				}
				else if ((bool)(iocComp = hit.transform.parent.GetComponent<IOCcomp>()))
				{
					iocComp.UnHide(hit);
				}
			}
		}
	}

	private float HaltonSequence(int index, int b)
	{
		float num = 0f;
		float num2 = 1f / (float)b;
		int num3 = index;
		while (num3 > 0)
		{
			num += num2 * (float)(num3 % b);
			num3 = Mathf.FloorToInt(num3 / b);
			num2 /= (float)b;
		}
		return num;
	}
}
