using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Scavenge;

public class ChallengeItemController : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private Animator _animator;

	private const string HIDE_PARAM_NAME = "Hide";

	private const string CHANGE_ROWS = "HideAndShow";

	public bool Enabled { get; private set; }

	public ScavengeItem Item { get; private set; }

	public RectTransform ParentRectTransform { get; private set; }

	public void SetRect(RectTransform parentCanvas)
	{
		ParentRectTransform = parentCanvas;
	}

	public void SetIcon(ScavengeItem item)
	{
		Enabled = true;
		Item = item;
		_image.sprite = item.Icon;
	}

	public void DisableIcon()
	{
		Enabled = false;
		_animator.SetTrigger("Hide");
	}

	public void ChangeIconRow()
	{
		_animator.SetTrigger("HideAndShow");
	}

	public void DisableObject()
	{
		base.gameObject.SetActive(value: false);
	}
}
