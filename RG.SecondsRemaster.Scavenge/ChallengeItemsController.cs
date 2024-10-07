using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Scavenge;

public class ChallengeItemsController : MonoBehaviour
{
	[SerializeField]
	private Transform _iconHolder;

	[SerializeField]
	private Transform _iconRectTransformPrefab;

	private const float TIME_OF_TRAVEL_MULTIPLIER = 4.5f;

	private const float TIME_OF_SWITCHING_ROWS_MULTIPLIER = 5f;

	private const float TIME_OF_ICON_ANIMATION = 0.5f;

	private const float FIRST_ROW_HEIGHT = -125f;

	private const float FIRST_COLUMN_WIDTH = 300f;

	private const float DEFAULT_PREFERRED_WIDTH = 200f;

	private const float MIN_PREFERRED_WIDTH_FOR_PULSE = 1f;

	private readonly WaitForSeconds _pulseWaitForSeconds = new WaitForSeconds(0.5f);

	private List<ScavengeItem> _items;

	private List<ChallengeItemController> _icons;

	private void Start()
	{
		if (GameSessionData.Instance.CurrentChallenge == null)
		{
			_iconHolder.gameObject.SetActive(value: false);
			return;
		}
		_items = GameSessionData.Instance.CurrentChallenge.Collectables;
		SpawnChallengeItems();
	}

	private void SpawnChallengeItems()
	{
		if (_items != null)
		{
			_icons = new List<ChallengeItemController>(_items.Count);
			for (int i = 0; i < _items.Count; i++)
			{
				ScavengeItem icon = _items[i];
				RectTransform rectTransform = (RectTransform)Object.Instantiate(_iconRectTransformPrefab, _iconHolder);
				ChallengeItemController componentInChildren = rectTransform.GetComponentInChildren<ChallengeItemController>();
				componentInChildren.SetIcon(icon);
				componentInChildren.SetRect(rectTransform);
				_icons.Add(componentInChildren);
			}
		}
	}

	public void HideChallengeUI()
	{
		for (int i = 0; i < _icons.Count; i++)
		{
			_icons[i].gameObject.SetActive(value: false);
		}
	}

	public void DisableScavengeItem(ScavengeItem item)
	{
		if (_icons == null)
		{
			return;
		}
		for (int i = 0; i < _icons.Count; i++)
		{
			ChallengeItemController challengeItemController = _icons[i];
			if (challengeItemController.Item == item && challengeItemController.Enabled)
			{
				challengeItemController.DisableIcon();
				StartCoroutine(ShrinkIconRect(challengeItemController.ParentRectTransform));
				break;
			}
		}
		ChangeIconRow();
	}

	private void ChangeIconRow()
	{
		int num = _icons.Count - 1;
		while (num >= 0)
		{
			ChallengeItemController challengeItemController = _icons[num];
			if (!(challengeItemController.ParentRectTransform.anchoredPosition.y >= -125f))
			{
				if (challengeItemController.ParentRectTransform.anchoredPosition.x < 300f)
				{
					challengeItemController.ChangeIconRow();
					StartCoroutine(PulseIconRectWidth(challengeItemController.ParentRectTransform));
					break;
				}
				num--;
				continue;
			}
			break;
		}
	}

	private IEnumerator ShrinkIconRect(RectTransform iconRect)
	{
		float currentTime = 0f;
		float elementPreferredWidth = iconRect.GetComponent<LayoutElement>().preferredWidth;
		float timeOfTravelMultiplier = 4.5f;
		yield return _pulseWaitForSeconds;
		while (elementPreferredWidth > 0f)
		{
			float num2 = (iconRect.GetComponent<LayoutElement>().preferredWidth = Mathf.Lerp(elementPreferredWidth, 0f, currentTime));
			elementPreferredWidth = num2;
			currentTime += Time.deltaTime * timeOfTravelMultiplier;
			yield return new WaitForEndOfFrame();
		}
		iconRect.gameObject.SetActive(value: false);
	}

	private IEnumerator PulseIconRectWidth(RectTransform iconRect)
	{
		float currentTime = 0f;
		float elementPreferredWidth = iconRect.GetComponent<LayoutElement>().preferredWidth;
		float timeOfTravelMultiplier = 5f;
		yield return _pulseWaitForSeconds;
		while (elementPreferredWidth > 1f)
		{
			float num2 = (iconRect.GetComponent<LayoutElement>().preferredWidth = Mathf.Lerp(elementPreferredWidth, 1f, currentTime));
			elementPreferredWidth = num2;
			currentTime += Time.deltaTime * timeOfTravelMultiplier;
			yield return new WaitForEndOfFrame();
		}
		currentTime = 0f;
		while (elementPreferredWidth < 200f)
		{
			float num2 = (iconRect.GetComponent<LayoutElement>().preferredWidth = Mathf.Lerp(elementPreferredWidth, 200f, currentTime));
			elementPreferredWidth = num2;
			currentTime += Time.deltaTime * timeOfTravelMultiplier;
			yield return new WaitForEndOfFrame();
		}
	}
}
