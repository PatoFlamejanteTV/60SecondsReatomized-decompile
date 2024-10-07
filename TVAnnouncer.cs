using System.Collections;
using UnityEngine;

public class TVAnnouncer : Announcer
{
	[SerializeField]
	private MeshRenderer _tvRenderer;

	[SerializeField]
	private Material[] _displayMaterials;

	[SerializeField]
	private Material _offMaterial;

	[SerializeField]
	private float _displayTimeout;

	private int _curMaterialIndex;

	private bool _turnedOn;

	private void Awake()
	{
	}

	public void TurnOn()
	{
		if (_displayMaterials.Length != 0)
		{
			_turnedOn = true;
			if (_displayMaterials.Length > 1)
			{
				StartCoroutine(Display());
			}
			else
			{
				_tvRenderer.material = _displayMaterials[0];
			}
		}
	}

	private IEnumerator Display()
	{
		while (_turnedOn)
		{
			_tvRenderer.material = _displayMaterials[_curMaterialIndex];
			_curMaterialIndex++;
			yield return new WaitForSeconds(_displayTimeout);
		}
	}

	public void TurnOff()
	{
		_tvRenderer.material = _offMaterial;
		_turnedOn = false;
		_curMaterialIndex = 0;
	}

	protected override void OnActivation(string customItem)
	{
		base.OnActivation(customItem);
		TurnOn();
	}

	protected override void OnDeactivation()
	{
		base.OnDeactivation();
		TurnOff();
	}
}
