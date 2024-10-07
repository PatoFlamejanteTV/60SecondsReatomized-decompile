using UnityEngine;

public class Resizer : MonoBehaviour
{
	[SerializeField]
	private bool _disableWidth;

	[SerializeField]
	private bool _disableHeight;

	[SerializeField]
	private bool _disableDepth;

	private void Start()
	{
		ResolutionHandler resolutionHandler = Object.FindObjectOfType<ResolutionHandler>();
		if (resolutionHandler != null)
		{
			if (!_disableWidth && !_disableHeight && !_disableDepth)
			{
				base.transform.localScale *= resolutionHandler.ResizeRatio;
			}
			else
			{
				float x = (_disableWidth ? base.transform.localScale.x : (base.transform.localScale.x * resolutionHandler.ResizeRatio));
				float y = (_disableHeight ? base.transform.localScale.y : (base.transform.localScale.y * resolutionHandler.ResizeRatio));
				float z = (_disableDepth ? base.transform.localScale.z : (base.transform.localScale.z * resolutionHandler.ResizeRatio));
				base.transform.localScale = new Vector3(x, y, z);
			}
		}
		Object.Destroy(this);
	}

	private void Update()
	{
	}
}
