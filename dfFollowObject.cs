using UnityEngine;

[AddComponentMenu("Daikon Forge/Examples/General/Follow Object")]
public class dfFollowObject : MonoBehaviour
{
	public Camera mainCamera;

	public GameObject attach;

	public dfPivotPoint anchor = dfPivotPoint.MiddleCenter;

	public Vector3 offset;

	public float hideDistance = 20f;

	public float fadeDistance = 15f;

	public bool constantScale;

	public bool stickToScreenEdge;

	private Transform controlTransform;

	private Transform followTransform;

	private dfControl myControl;

	private dfGUIManager manager;

	private Vector2 lastPosition = Vector2.one * float.MinValue;

	private void OnEnable()
	{
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
			if (mainCamera == null)
			{
				Debug.Log("dfFollowObject component is unable to determine which camera is the MainCamera", base.gameObject);
				base.enabled = false;
				return;
			}
		}
		myControl = GetComponent<dfControl>();
		if (myControl == null)
		{
			Debug.LogError("No dfControl component on this GameObject: " + base.gameObject.name, base.gameObject);
			base.enabled = false;
		}
		if (myControl == null || attach == null)
		{
			Debug.LogWarning("Configuration incomplete: " + base.name);
			base.enabled = false;
			return;
		}
		controlTransform = myControl.transform;
		followTransform = attach.transform;
		manager = myControl.GetManager();
		dfFollowObjectSorter.Register(this);
	}

	private void OnDisable()
	{
		dfFollowObjectSorter.Unregister(this);
	}

	private void Update()
	{
		Vector3 position = followTransform.position;
		float num = Vector3.Distance(mainCamera.transform.position, position);
		if (num > hideDistance)
		{
			myControl.Opacity = 0f;
			return;
		}
		if (num > fadeDistance)
		{
			myControl.Opacity = 1f - (num - fadeDistance) / (hideDistance - fadeDistance);
		}
		else
		{
			myControl.Opacity = 1f;
		}
		Vector3 position2 = followTransform.position + offset;
		Vector2 vector = manager.ScreenToGui(mainCamera.WorldToScreenPoint(position2));
		if (!manager.PixelPerfectMode)
		{
			if (constantScale)
			{
				controlTransform.localScale = Vector3.one * (manager.FixedHeight / mainCamera.pixelHeight);
			}
			else
			{
				controlTransform.localScale = Vector3.one;
			}
		}
		Vector2 anchoredControlPosition = getAnchoredControlPosition();
		vector.x -= anchoredControlPosition.x * controlTransform.localScale.x;
		vector.y -= anchoredControlPosition.y * controlTransform.localScale.y;
		if (stickToScreenEdge)
		{
			Vector2 screenSize = manager.GetScreenSize();
			vector.x = Mathf.Clamp(vector.x, 0f, screenSize.x - myControl.Width);
			vector.y = Mathf.Clamp(vector.y, 0f, screenSize.y - myControl.Height);
		}
		if (!(Vector2.Distance(vector, lastPosition) <= 0.5f))
		{
			lastPosition = vector;
			Vector3 normalized = (attach.transform.position - mainCamera.transform.position).normalized;
			if (!(Vector3.Dot(mainCamera.transform.forward, normalized) > float.Epsilon))
			{
				myControl.IsVisible = false;
				return;
			}
			myControl.IsVisible = true;
			myControl.RelativePosition = vector;
		}
	}

	private Vector2 getAnchoredControlPosition()
	{
		float height = myControl.Height;
		float x = myControl.Width / 2f;
		float width = myControl.Width;
		float y = myControl.Height / 2f;
		Vector2 result = default(Vector2);
		switch (anchor)
		{
		case dfPivotPoint.TopLeft:
			result.x = width;
			result.y = height;
			break;
		case dfPivotPoint.TopCenter:
			result.x = x;
			result.y = height;
			break;
		case dfPivotPoint.TopRight:
			result.x = 0f;
			result.y = height;
			break;
		case dfPivotPoint.MiddleLeft:
			result.x = width;
			result.y = y;
			break;
		case dfPivotPoint.MiddleCenter:
			result.x = x;
			result.y = y;
			break;
		case dfPivotPoint.MiddleRight:
			result.x = 0f;
			result.y = y;
			break;
		case dfPivotPoint.BottomLeft:
			result.x = width;
			result.y = 0f;
			break;
		case dfPivotPoint.BottomCenter:
			result.x = x;
			result.y = 0f;
			break;
		case dfPivotPoint.BottomRight:
			result.x = 0f;
			result.y = 0f;
			break;
		}
		return result;
	}
}
