using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Survival;

public class JournalTabController : MonoBehaviour
{
	[SerializeField]
	private JournalTabsController _tabsController;

	[SerializeField]
	private BasePagesDisplayController _pagesDisplayController;

	[SerializeField]
	private PageController[] _associatedPages;

	[SerializeField]
	private Button _button;

	[SerializeField]
	private GlobalBoolVariable _isTabEnabled;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private GlobalBoolVariable _isExclamationMarkVisible;

	[SerializeField]
	private GameObject[] _exclamationMarks;

	[SerializeField]
	private GameObject _raycastBlocker;

	[SerializeField]
	private OnUIClickedSoundPlayer _onUiClickedSoundPlayer;

	private PageController _firstPageEnabled;

	public void ResetFirstEnabledPage()
	{
		_firstPageEnabled = null;
	}

	public void RefreshTab()
	{
		if (_isTabEnabled != null && _associatedPages.Length != 0)
		{
			_button.interactable = _isTabEnabled.Value && IsAnyOfPagesEnabled();
		}
		else if (_isTabEnabled != null)
		{
			_button.interactable = _isTabEnabled.Value;
		}
		else if (_associatedPages.Length != 0)
		{
			_button.interactable = IsAnyOfPagesEnabled();
		}
		else
		{
			_button.interactable = false;
		}
		if (_raycastBlocker != null)
		{
			_raycastBlocker.SetActive(_button.interactable);
		}
		SetExclamationMarksVisibility();
	}

	public void SetExclamationMarksVisibility()
	{
		for (int i = 0; i < _exclamationMarks.Length; i++)
		{
			_exclamationMarks[i].SetActive(_isExclamationMarkVisible.Value);
		}
	}

	public bool IsAnyOfPagesEnabled()
	{
		for (int i = 0; i < _associatedPages.Length; i++)
		{
			if (!(_associatedPages[i] == null) && _associatedPages[i].IsEnabled())
			{
				return true;
			}
		}
		return false;
	}

	public void ShowFirstEnabledPage()
	{
		for (int i = 0; i < _associatedPages.Length; i++)
		{
			if (_associatedPages[i] != null && _associatedPages[i].IsEnabled())
			{
				PageController pageController = null;
				pageController = ((!_associatedPages[i].HasSubpages) ? _associatedPages[i] : _associatedPages[i].GetFirstSubpage());
				if (pageController != null)
				{
					_pagesDisplayController.ShowSpecificPage(pageController);
					_firstPageEnabled = pageController;
				}
			}
		}
	}

	private void SetFirstEnabledPage()
	{
		if (_firstPageEnabled != null)
		{
			return;
		}
		for (int i = 0; i < _associatedPages.Length; i++)
		{
			if (_associatedPages[i] != null && _associatedPages[i].IsEnabled())
			{
				PageController pageController = null;
				pageController = ((!_associatedPages[i].HasSubpages) ? _associatedPages[i] : _associatedPages[i].GetFirstSubpage());
				if (pageController != null)
				{
					_firstPageEnabled = pageController;
				}
			}
		}
	}

	public void DisplayPage()
	{
		if (!_tabsController.IsThisTabCurrentTab(this) || !(_pagesDisplayController.GetCurrentPage() == _firstPageEnabled))
		{
			ShowFirstEnabledPage();
			_onUiClickedSoundPlayer.PlaySound();
		}
	}

	public void ActivateTab(bool setFirstPage = true)
	{
		_tabsController.SetTabActive(this);
		if (setFirstPage)
		{
			SetFirstEnabledPage();
		}
		JournalHideButtonController journalHideButtonController = Object.FindObjectOfType<JournalHideButtonController>();
		if (journalHideButtonController != null)
		{
			journalHideButtonController.Show();
		}
	}

	public void SetActiveAnimationParameter(bool value)
	{
		_animator.SetBool("Active", value);
	}
}
