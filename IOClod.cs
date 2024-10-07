using System;
using System.Linq;
using UnityEngine;

public class IOClod : IOCcomp
{
	public bool Occludee;

	public float Lod1;

	public float Lod2;

	public float LodMargin;

	public bool LodOnly;

	private int currentLayer;

	private Vector3 hitPoint;

	private float lod_1;

	private float lod_2;

	private float lodMargin;

	private bool realtimeShadows;

	private IOCcam iocCam;

	private int counter;

	private Renderer[] rs0;

	private Renderer[] rs1;

	private Renderer[] rs2;

	private Renderer[] rs;

	private bool hidden;

	private int currentLod;

	private float prevDist;

	private float distOffset;

	private int lods;

	private float dt;

	private float hitTimeOffset;

	private float prevHitTime;

	private bool sleeping;

	private Shader shInvisible;

	private Shader[][] sh;

	private Shader[][] sh0;

	private float distanceFromCam;

	private float shadowDistance;

	private int frameInterval;

	private RaycastHit h;

	private Ray r;

	private bool visible;

	private Vector3 p;

	private void Awake()
	{
		Init();
	}

	public override void Init()
	{
		try
		{
			iocCam = Camera.main.GetComponent<IOCcam>();
			shadowDistance = QualitySettings.shadowDistance * 2f;
			currentLayer = base.gameObject.layer;
			prevDist = 0f;
			prevHitTime = Time.time;
			sleeping = true;
			h = default(RaycastHit);
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
		UpdateValues();
		if ((bool)base.transform.Find("Lod_0"))
		{
			lods = 1;
			rs0 = (from x in base.transform.Find("Lod_0").GetComponentsInChildren<Renderer>(includeInactive: false)
				where x.gameObject.GetComponent<Light>() == null
				select x).ToArray();
			sh0 = new Shader[rs0.Length][];
			for (int i = 0; i < rs0.Length; i++)
			{
				sh0[i] = new Shader[rs0[i].sharedMaterials.Length];
				for (int j = 0; j < rs0[i].sharedMaterials.Length; j++)
				{
					sh0[i][j] = rs0[i].sharedMaterials[j].shader;
				}
			}
			if ((bool)base.transform.Find("Lod_1"))
			{
				lods++;
				rs1 = (from x in base.transform.Find("Lod_1").GetComponentsInChildren<Renderer>(includeInactive: false)
					where x.gameObject.GetComponent<Light>() == null
					select x).ToArray();
				if ((bool)base.transform.Find("Lod_2"))
				{
					lods++;
					rs2 = (from x in base.transform.Find("Lod_2").GetComponentsInChildren<Renderer>(includeInactive: false)
						where x.gameObject.GetComponent<Light>() == null
						select x).ToArray();
				}
			}
		}
		else
		{
			lods = 0;
		}
		rs = (from x in GetComponentsInChildren<Renderer>(includeInactive: false)
			where x.gameObject.GetComponent<Light>() == null
			select x).ToArray();
		sh = new Shader[rs.Length][];
		for (int k = 0; k < rs.Length; k++)
		{
			sh[k] = new Shader[rs[k].sharedMaterials.Length];
			for (int l = 0; l < rs[k].sharedMaterials.Length; l++)
			{
				sh[k][l] = rs[k].sharedMaterials[l].shader;
			}
		}
		shInvisible = Shader.Find("Custom/Invisible");
		Initialize();
	}

	public void Initialize()
	{
		if (iocCam.enabled)
		{
			HideAll();
		}
		else
		{
			base.enabled = false;
			ShowLod(1f);
		}
		base.gameObject.layer = currentLayer;
	}

	private void Update()
	{
		frameInterval = Time.frameCount % 4;
		if (frameInterval == 0)
		{
			if (!LodOnly)
			{
				if (hidden || Time.frameCount - counter <= iocCam.hideDelay)
				{
					return;
				}
				switch (currentLod)
				{
				case 0:
					visible = rs0[0].isVisible;
					break;
				case 1:
					visible = rs1[0].isVisible;
					break;
				case 2:
					visible = rs2[0].isVisible;
					break;
				default:
					visible = rs[0].isVisible;
					break;
				}
				if (((iocCam.preCullCheck && visible) | Occludee) && visible)
				{
					p = base.transform.localToWorldMatrix.MultiplyPoint(hitPoint);
					r = new Ray(p, iocCam.transform.position - p);
					if (!Physics.Raycast(r, out h, iocCam.viewDistance))
					{
						return;
					}
					if (!h.collider.CompareTag(iocCam.tag))
					{
						Hide();
						return;
					}
					counter = Time.frameCount;
					if (Occludee & (lods > 0))
					{
						ShowLod(h.distance);
					}
				}
				else
				{
					Hide();
				}
			}
			else
			{
				if (sleeping || Time.frameCount - counter <= iocCam.hideDelay)
				{
					return;
				}
				if (Occludee)
				{
					base.gameObject.layer = currentLayer;
					Vector3 vector = base.transform.localToWorldMatrix.MultiplyPoint(hitPoint);
					r = new Ray(vector, iocCam.transform.position - vector);
					if (Physics.Raycast(r, out h, iocCam.viewDistance) && !h.collider.CompareTag(iocCam.tag))
					{
						ShowLod(3000f);
						sleeping = true;
					}
				}
				else
				{
					ShowLod(3000f);
					sleeping = true;
				}
			}
		}
		else
		{
			if (!realtimeShadows || frameInterval != 2)
			{
				return;
			}
			distanceFromCam = Vector3.Distance(base.transform.position, iocCam.transform.position);
			if (!hidden)
			{
				return;
			}
			if (lods == 0)
			{
				if (distanceFromCam > shadowDistance)
				{
					if (!rs[0].enabled)
					{
						return;
					}
					for (int i = 0; i < rs.Length; i++)
					{
						rs[i].enabled = false;
						for (int j = 0; j < rs[i].materials.Length; j++)
						{
							rs[i].materials[j].shader = sh[i][j];
						}
					}
				}
				else
				{
					if (rs[0].enabled)
					{
						return;
					}
					for (int k = 0; k < rs.Length; k++)
					{
						Material[] materials = rs[k].materials;
						for (int l = 0; l < materials.Length; l++)
						{
							materials[l].shader = shInvisible;
						}
						rs[k].enabled = true;
					}
				}
			}
			else if (distanceFromCam > shadowDistance)
			{
				if (!rs0[0].enabled)
				{
					return;
				}
				for (int m = 0; m < rs0.Length; m++)
				{
					rs0[m].enabled = false;
					for (int n = 0; n < rs0[m].materials.Length; n++)
					{
						rs0[m].materials[n].shader = sh0[m][n];
					}
				}
			}
			else
			{
				if (rs0[0].enabled)
				{
					return;
				}
				for (int num = 0; num < rs0.Length; num++)
				{
					Material[] materials = rs0[num].materials;
					for (int l = 0; l < materials.Length; l++)
					{
						materials[l].shader = shInvisible;
					}
					rs0[num].enabled = true;
				}
			}
		}
	}

	public void UpdateValues()
	{
		if (Lod1 != 0f)
		{
			lod_1 = Lod1;
		}
		else
		{
			lod_1 = iocCam.lod1Distance;
		}
		if (Lod2 != 0f)
		{
			lod_2 = Lod2;
		}
		else
		{
			lod_2 = iocCam.lod2Distance;
		}
		if (LodMargin != 0f)
		{
			lodMargin = LodMargin;
		}
		else
		{
			lodMargin = iocCam.lodMargin;
		}
		realtimeShadows = iocCam.realtimeShadows;
	}

	public override void UnHide(RaycastHit h)
	{
		counter = Time.frameCount;
		hitPoint = base.transform.worldToLocalMatrix.MultiplyPoint(h.point);
		if (hidden)
		{
			hidden = false;
			ShowLod(h.distance);
			if (Occludee)
			{
				base.gameObject.layer = iocCam.occludeeLayer;
			}
		}
		else
		{
			if (lods <= 0)
			{
				return;
			}
			distOffset = prevDist - h.distance;
			hitTimeOffset = Time.time - prevHitTime;
			prevHitTime = Time.time;
			if ((Mathf.Abs(distOffset) > lodMargin) | (hitTimeOffset > 1f))
			{
				ShowLod(h.distance);
				prevDist = h.distance;
				sleeping = false;
				if (Occludee)
				{
					base.gameObject.layer = iocCam.occludeeLayer;
				}
			}
		}
	}

	public void ShowLod(float d)
	{
		int num = 0;
		switch (lods)
		{
		case 0:
			currentLod = -1;
			break;
		case 2:
			if (d < lod_1)
			{
				currentLod = 0;
			}
			else
			{
				currentLod = 1;
			}
			break;
		case 3:
			if (d < lod_1)
			{
				currentLod = 0;
			}
			else if ((d > lod_1) & (d < lod_2))
			{
				currentLod = 1;
			}
			else
			{
				currentLod = 2;
			}
			break;
		}
		switch (currentLod)
		{
		case 0:
			if (!LodOnly && rs0[0].enabled)
			{
				for (num = 0; num < rs0.Length; num++)
				{
					for (int k = 0; k < rs0[num].materials.Length; k++)
					{
						rs0[num].materials[k].shader = sh0[num][k];
					}
				}
			}
			else
			{
				for (num = 0; num < rs0.Length; num++)
				{
					rs0[num].enabled = true;
				}
			}
			for (num = 0; num < rs1.Length; num++)
			{
				rs1[num].enabled = false;
			}
			if (lods == 3)
			{
				for (num = 0; num < rs2.Length; num++)
				{
					rs2[num].enabled = false;
				}
			}
			return;
		case 1:
			for (num = 0; num < rs1.Length; num++)
			{
				rs1[num].enabled = true;
			}
			for (num = 0; num < rs0.Length; num++)
			{
				rs0[num].enabled = false;
				if (!LodOnly && realtimeShadows)
				{
					for (int i = 0; i < rs0[num].materials.Length; i++)
					{
						rs0[num].materials[i].shader = sh0[num][i];
					}
				}
			}
			if (lods == 3)
			{
				for (num = 0; num < rs2.Length; num++)
				{
					rs2[num].enabled = false;
				}
			}
			return;
		case 2:
			for (num = 0; num < rs2.Length; num++)
			{
				rs2[num].enabled = true;
			}
			for (num = 0; num < rs0.Length; num++)
			{
				rs0[num].enabled = false;
				if (!LodOnly && realtimeShadows)
				{
					for (int j = 0; j < rs0[num].materials.Length; j++)
					{
						rs0[num].materials[j].shader = sh0[num][j];
					}
				}
			}
			for (num = 0; num < rs1.Length; num++)
			{
				rs1[num].enabled = false;
			}
			return;
		}
		if (!LodOnly && rs[0].enabled)
		{
			for (num = 0; num < rs.Length; num++)
			{
				for (int l = 0; l < rs[num].materials.Length; l++)
				{
					rs[num].materials[l].shader = sh[num][l];
				}
			}
		}
		else
		{
			for (num = 0; num < rs.Length; num++)
			{
				rs[num].enabled = true;
			}
		}
	}

	public void Hide()
	{
		int num = 0;
		hidden = true;
		switch (currentLod)
		{
		case 0:
			if (realtimeShadows && distanceFromCam <= shadowDistance)
			{
				for (num = 0; num < rs0.Length; num++)
				{
					Material[] materials = rs0[num].materials;
					for (int i = 0; i < materials.Length; i++)
					{
						materials[i].shader = shInvisible;
					}
				}
			}
			else
			{
				for (num = 0; num < rs0.Length; num++)
				{
					rs0[num].enabled = false;
				}
			}
			break;
		case 1:
			for (num = 0; num < rs1.Length; num++)
			{
				rs1[num].enabled = false;
			}
			break;
		case 2:
			for (num = 0; num < rs2.Length; num++)
			{
				rs2[num].enabled = false;
			}
			break;
		default:
			if (realtimeShadows && distanceFromCam <= shadowDistance)
			{
				for (num = 0; num < rs.Length; num++)
				{
					Material[] materials = rs[num].materials;
					for (int i = 0; i < materials.Length; i++)
					{
						materials[i].shader = shInvisible;
					}
				}
			}
			else
			{
				for (num = 0; num < rs.Length; num++)
				{
					rs[num].enabled = false;
				}
			}
			break;
		}
		if (Occludee)
		{
			base.gameObject.layer = currentLayer;
		}
	}

	public void HideAll()
	{
		int num = 0;
		if (!LodOnly)
		{
			hidden = true;
			if (lods == 0 && rs != null)
			{
				for (num = 0; num < rs.Length; num++)
				{
					rs[num].enabled = false;
					if (realtimeShadows)
					{
						for (int i = 0; i < rs[num].materials.Length; i++)
						{
							rs[num].materials[i].shader = sh[num][i];
						}
					}
				}
				return;
			}
			for (num = 0; num < rs0.Length; num++)
			{
				rs0[num].enabled = false;
				if (realtimeShadows)
				{
					for (int j = 0; j < rs0[num].materials.Length; j++)
					{
						rs0[num].materials[j].shader = sh0[num][j];
					}
				}
			}
			for (num = 0; num < rs1.Length; num++)
			{
				rs1[num].enabled = false;
			}
			if (rs2 != null)
			{
				for (num = 0; num < rs2.Length; num++)
				{
					rs2[num].enabled = false;
				}
			}
		}
		else
		{
			prevHitTime -= 3f;
			ShowLod(3000f);
		}
	}
}
