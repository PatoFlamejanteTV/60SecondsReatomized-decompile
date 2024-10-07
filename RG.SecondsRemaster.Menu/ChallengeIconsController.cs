using UnityEngine;
using UnityEngine.UI;

namespace RG.SecondsRemaster.Menu;

public class ChallengeIconsController : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _rows;

	[SerializeField]
	private IconList[] _iconLists;

	private int _currentRow;

	private int _currentElement;

	public void DisableAllIcons()
	{
		for (int i = 0; i < _iconLists.Length; i++)
		{
			IconList iconList = _iconLists[i];
			for (int j = 0; j < iconList.Icons.Length; j++)
			{
				iconList.Icons[j].gameObject.SetActive(value: false);
			}
			_rows[i].SetActive(value: false);
		}
		_currentRow = 0;
		_currentElement = 0;
	}

	public void SetNextIcon(Sprite sprite)
	{
		if (_currentRow >= _rows.Length || _currentElement >= _iconLists[_currentRow].Icons.Length)
		{
			Debug.LogErrorFormat("There was not enough space for all Collectables for this Challenge ('{1}')!");
			return;
		}
		if (!_rows[_currentRow].activeSelf)
		{
			_rows[_currentRow].SetActive(value: true);
		}
		IconList iconList = _iconLists[_currentRow];
		Image obj = iconList.Icons[_currentElement];
		obj.sprite = sprite;
		obj.gameObject.SetActive(value: true);
		_currentElement++;
		if (_currentElement >= iconList.Icons.Length)
		{
			_currentRow++;
			_currentElement = 0;
		}
	}
}
