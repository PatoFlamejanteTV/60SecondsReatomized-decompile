using RG.Parsecs.EventEditor;
using UnityEngine;

public class PanelStyleController : MonoBehaviour
{
	[SerializeField]
	private GlobalBoolVariable _isContinueAvailable;

	[Tooltip("GameObject of given panel (Probably the root object that has 'normal' sprite component)")]
	[SerializeField]
	private GameObject _normalPanel;

	[Tooltip("GameObject of postapo overlay for 'normalPanel'")]
	[SerializeField]
	private GameObject _postapoPanel;

	private void Start()
	{
		if (_isContinueAvailable != null && _isContinueAvailable.Value)
		{
			_normalPanel.SetActive(value: true);
			_postapoPanel.SetActive(value: true);
		}
		if (_isContinueAvailable != null && !_isContinueAvailable.Value)
		{
			_normalPanel.SetActive(value: true);
			_postapoPanel.SetActive(value: false);
		}
	}

	private void Update()
	{
	}
}
