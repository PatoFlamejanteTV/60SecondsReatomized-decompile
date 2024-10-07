using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Scavenge;

public class HandsController : MonoBehaviour
{
	[SerializeField]
	private Image[] _itemImages;

	[SerializeField]
	private Animator[] _handsAnimators;

	[SerializeField]
	private ScavengeTextController _toTheShelterText;

	private const float TO_THE_SHELTER_TEXT_DELAY = 0.5f;

	private int _currentIndex;

	private const string SHOW_ITEM_PARAM_NAME = "Show_Item";

	private const string SHOW_HAND_PARAM_NAME = "Show_Hand";

	private readonly Color SEMI_TRANSPARENT_COLOR = new Color(1f, 1f, 1f, 0.5f);

	private readonly Color WHITE_COLOR = new Color(1f, 1f, 1f);

	private ScavengeItem _lastScavengeItemAdded;

	private Image[] _uiImages;

	public ScavengeItem LastScavengeItemAdded => _lastScavengeItemAdded;

	private void Awake()
	{
		_uiImages = GetComponentsInChildren<Image>(includeInactive: true);
	}

	public bool AddItem(ScavengeItem item)
	{
		int num = 0;
		if (!WillItemFit(item))
		{
			return false;
		}
		num = _currentIndex + item.Weight - 1;
		bool flag = false;
		for (int i = _currentIndex; i <= num; i++)
		{
			_itemImages[i].sprite = item.Icon;
			_itemImages[i].color = (flag ? SEMI_TRANSPARENT_COLOR : WHITE_COLOR);
			_handsAnimators[i].SetTrigger("Show_Item");
			if (!flag)
			{
				flag = true;
			}
		}
		_lastScavengeItemAdded = item;
		_currentIndex = num + 1;
		if (_currentIndex >= _itemImages.Length)
		{
			_toTheShelterText.ShowTextDelayed(0.5f);
		}
		return true;
	}

	public void Clear()
	{
		for (int i = 0; i < _itemImages.Length; i++)
		{
			if (_itemImages[i].gameObject.activeSelf)
			{
				_handsAnimators[i].SetTrigger("Show_Hand");
			}
		}
		_currentIndex = 0;
	}

	public bool AreHandsEmpty()
	{
		return _currentIndex == 0;
	}

	public bool WillItemFit(ScavengeItem item)
	{
		return _currentIndex + item.Weight - 1 < _itemImages.Length;
	}

	public void HideHands()
	{
		for (int i = 0; i < _uiImages.Length; i++)
		{
			_uiImages[i].color = Color.clear;
		}
	}
}
