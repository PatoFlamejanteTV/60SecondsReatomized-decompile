using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class CreditsEyeGroup : MonoBehaviour
{
	[HideInInspector]
	public EEyesType GroupEyesType;

	[SerializeField]
	private CreditsEyesController _tedEyesController;

	[SerializeField]
	private CreditsEyesController _doloresEyesController;

	[SerializeField]
	private RectTransform _rectTransform;

	private Vector3[] _corners;

	private Vector2 GetRandomPointInRect(Vector3[] corners)
	{
		return new Vector2(Random.Range(corners[0].x, corners[2].x), Random.Range(corners[0].y, corners[2].y));
	}

	public void Initialize()
	{
		if (_corners == null)
		{
			_corners = new Vector3[4];
			_rectTransform.GetLocalCorners(_corners);
		}
	}

	public CreditsEyesController StartAnimationAtRandomPointAndReturnInstance()
	{
		switch (GroupEyesType)
		{
		case EEyesType.TED:
			_tedEyesController.RectTransform.anchoredPosition = GetRandomPointInRect(_corners);
			_tedEyesController.ShowAnimation();
			return _tedEyesController;
		case EEyesType.DOLORES:
			_doloresEyesController.RectTransform.anchoredPosition = GetRandomPointInRect(_corners);
			_doloresEyesController.ShowAnimation();
			return _doloresEyesController;
		default:
			return null;
		}
	}
}
