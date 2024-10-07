using System.Collections;
using UnityEngine;

public class Marker : MonoBehaviour
{
	public enum EMarkerAnimation
	{
		NONE,
		ROTATION,
		PULSE
	}

	public delegate void Report(GameObject entrant, Marker where);

	[SerializeField]
	private GameObject _markerVisual;

	[SerializeField]
	private EMarkerAnimation[] _animations;

	[SerializeField]
	private float _scaleTime = 1f;

	[SerializeField]
	private float _rotationTime = 1f;

	[SerializeField]
	private float _showTime = 0.5f;

	[SerializeField]
	private Vector3 _rotation = Vector3.zero;

	[SerializeField]
	private Vector3 _nominalScale = Vector3.zero;

	[SerializeField]
	private Vector3 _scale = Vector3.zero;

	private GameObject _currentUser;

	public GameObject CurrentUser => _currentUser;

	public event Report OnEnter;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Show(bool show)
	{
		if (_markerVisual != null)
		{
			iTween.Stop(_markerVisual);
			Hashtable hashtable = new Hashtable();
			hashtable.Add("scale", show ? _nominalScale : Vector3.zero);
			hashtable.Add("time", _showTime);
			hashtable.Add("looptype", iTween.LoopType.none);
			hashtable.Add("easeType", "easeInOutSine");
			iTween.ScaleTo(_markerVisual, hashtable);
		}
	}

	public void Animate()
	{
		if (!(_markerVisual != null))
		{
			return;
		}
		iTween.Stop(_markerVisual);
		if (_animations == null)
		{
			return;
		}
		for (int i = 0; i < _animations.Length; i++)
		{
			if (_animations[i] != 0)
			{
				Hashtable hashtable = new Hashtable();
				switch (_animations[i])
				{
				case EMarkerAnimation.ROTATION:
					hashtable = new Hashtable();
					hashtable.Add("amount", _rotation);
					hashtable.Add("time", _rotationTime);
					hashtable.Add("looptype", iTween.LoopType.loop);
					hashtable.Add("easeType", "linear");
					iTween.RotateBy(_markerVisual, hashtable);
					break;
				case EMarkerAnimation.PULSE:
					hashtable.Add("amount", _scale);
					hashtable.Add("time", _scaleTime);
					hashtable.Add("looptype", iTween.LoopType.pingPong);
					hashtable.Add("easeType", "easeInOutSine");
					iTween.ScaleBy(_markerVisual, hashtable);
					break;
				}
			}
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (this.OnEnter != null)
		{
			this.OnEnter(collider.gameObject, this);
		}
		_currentUser = collider.gameObject;
	}

	private void OnTriggerExit(Collider collider)
	{
		if (_currentUser == collider.gameObject)
		{
			_currentUser = null;
		}
	}
}
