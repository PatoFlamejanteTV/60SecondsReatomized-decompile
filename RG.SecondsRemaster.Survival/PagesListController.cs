using System.Collections.Generic;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

public class PagesListController : MonoBehaviour
{
	[SerializeField]
	private List<PageController> _pages;

	public List<PageController> Pages => _pages;

	public void ClearPages()
	{
		for (int i = 0; i < _pages.Count; i++)
		{
			Object.Destroy(_pages[i].gameObject);
		}
		_pages.Clear();
	}
}
