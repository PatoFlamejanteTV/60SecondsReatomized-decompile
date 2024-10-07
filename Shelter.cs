using System.Collections;
using RG.Parsecs.Common;
using UnityEngine;

public class Shelter : MonoBehaviour
{
	[SerializeField]
	private Transform _hatch;

	[SerializeField]
	private float _guiderLoopTime = 1f;

	[SerializeField]
	private Vector3 _guiderScaleDelta = Vector3.zero;

	[SerializeField]
	private GameObject _rangeMarkerPositioner;

	[SerializeField]
	private GameObject _rangeMarkerTemplate;

	[SerializeField]
	private float _rangeMarkerFlashTime = 2f;

	[SerializeField]
	private Color _rangeMarkerFlashColorLow = new Color(255f, 255f, 255f, 0f);

	[SerializeField]
	private Color _rangeMarkerFlashColorHigh = new Color(255f, 255f, 255f, 100f);

	[SerializeField]
	private float _swingFactor = 0.275f;

	private Transform _rangeMarker;

	private GameObject _guider;

	private Transform _playerTransform;

	private Transform _cameraTransform;

	private Transform _guiderTransform;

	private void Start()
	{
		if (_rangeMarkerPositioner != null && _rangeMarkerTemplate != null)
		{
			GameObject gameObject = Object.Instantiate(_rangeMarkerTemplate);
			_rangeMarker = gameObject.transform;
			_rangeMarker.transform.parent = _rangeMarkerPositioner.transform.parent;
			_rangeMarker.position = _rangeMarkerPositioner.transform.position;
			_rangeMarker.rotation = _rangeMarkerPositioner.transform.rotation;
			Object.Destroy(_rangeMarkerPositioner);
		}
		_playerTransform = GlobalTools.GetPlayer().transform;
		_cameraTransform = Camera.main.transform;
	}

	public void OpenHatch(bool open, float time)
	{
		iTween.RotateBy(_hatch.gameObject, new Vector3(open ? (0f - _swingFactor) : _swingFactor, 0f, 0f), time);
	}

	public void SetGuider()
	{
		_guider = GameObject.Find("pointer");
		_guiderTransform = _guider.transform;
		ShowGuider(show: false);
	}

	public void ShowGuider(bool show)
	{
		if (_guider != null)
		{
			_guider.SetActive(show);
		}
	}

	public void ShowRange(bool show)
	{
		if (_rangeMarker != null)
		{
			_rangeMarker.GetComponent<Renderer>().material.color = (show ? _rangeMarkerFlashColorHigh : _rangeMarkerFlashColorLow);
			if (!show)
			{
				iTween.Stop(base.gameObject);
			}
		}
	}

	public void Flash()
	{
		if (_guider != null)
		{
			ShowGuider(show: true);
			Hashtable hashtable = new Hashtable();
			hashtable.Add("amount", _guiderScaleDelta);
			hashtable.Add("time", _guiderLoopTime);
			hashtable.Add("easetype", iTween.EaseType.spring);
			hashtable.Add("looptype", iTween.LoopType.pingPong);
			iTween.ScaleBy(_guider.gameObject, hashtable);
		}
		Hashtable hashtable2 = new Hashtable();
		hashtable2.Add("from", _rangeMarkerFlashColorLow);
		hashtable2.Add("to", _rangeMarkerFlashColorHigh);
		hashtable2.Add("time", _rangeMarkerFlashTime);
		hashtable2.Add("looptype", iTween.LoopType.pingPong);
		hashtable2.Add("onupdate", "OnRangeUpdated");
		iTween.ValueTo(base.gameObject, hashtable2);
	}

	private void OnRangeUpdated(Color color)
	{
		_rangeMarker.GetComponent<Renderer>().material.color = color;
	}

	private void Update()
	{
		if (_guider != null && _guider.activeSelf)
		{
			Vector3 forward = _cameraTransform.position - _guiderTransform.position;
			forward.y = 0f;
			Quaternion rotation = Quaternion.LookRotation(forward);
			_guiderTransform.rotation = rotation;
		}
	}

	public void DropIntoShelter(string dropSound)
	{
		GetComponent<Effector>().Activate();
		AudioManager.PlaySound(dropSound);
	}
}
