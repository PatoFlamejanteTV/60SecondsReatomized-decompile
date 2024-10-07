using Rewired;
using RG.Parsecs.Common;
using RG.VirtualInput;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandHintController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public static bool ShouldShowHint = true;

	[SerializeField]
	private RectTransform _hintRect;

	[SerializeField]
	private Button _button;

	private bool _isShowed;

	private Player _player;

	public void Awake()
	{
		_player = ReInput.players.GetPlayer(0);
	}

	public void Update()
	{
		if (_isShowed && (!ShouldShowHint || _player.GetButtonDown(41)))
		{
			HideHint();
		}
	}

	public void TryToShowHint()
	{
		if (ShouldShowHint && _button.interactable && (Singleton<VirtualInputManager>.Instance.IsPointerMode() || Singleton<VirtualInputManager>.Instance.IsSelectablesMode()) && (bool)_hintRect)
		{
			_hintRect.gameObject.SetActive(value: true);
			_isShowed = true;
		}
	}

	public void HideHint()
	{
		if ((bool)_hintRect)
		{
			_hintRect.gameObject.SetActive(value: false);
			_isShowed = false;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (Singleton<VirtualInputManager>.Instance.IsPointerMode())
		{
			TryToShowHint();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HideHint();
	}
}
