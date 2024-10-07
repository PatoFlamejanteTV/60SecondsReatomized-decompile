using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class JournalTabsController : MonoBehaviour
{
	[SerializeField]
	private JournalTabController[] _tabs;

	[SerializeField]
	private Transform _coverObject;

	private JournalTabController _currentTab;

	public JournalTabController CurrentTab => _currentTab;

	private void Awake()
	{
		SetCoverAsLastSibling();
	}

	private void SetCoverAsLastSibling()
	{
		_coverObject.SetAsLastSibling();
	}

	public bool IsThisTabCurrentTab(JournalTabController tab)
	{
		return _currentTab == tab;
	}

	public void SetTabActive(JournalTabController tab)
	{
		if (_currentTab != null)
		{
			_currentTab.transform.SetAsFirstSibling();
			_currentTab.SetActiveAnimationParameter(value: false);
		}
		tab.transform.SetSiblingIndex(_coverObject.GetSiblingIndex() + 1);
		tab.SetActiveAnimationParameter(value: true);
		_currentTab = tab;
	}

	public void RefreshAllTabs()
	{
		for (int i = 0; i < _tabs.Length; i++)
		{
			_tabs[i].RefreshTab();
		}
	}

	public void ResetFirstEnabledPage()
	{
		for (int i = 0; i < _tabs.Length; i++)
		{
			_tabs[i].ResetFirstEnabledPage();
		}
	}
}
