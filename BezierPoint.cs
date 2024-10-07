using System;
using UnityEngine;

[Serializable]
public class BezierPoint : MonoBehaviour
{
	public enum HandleStyle
	{
		Connected,
		Broken,
		None
	}

	[SerializeField]
	private BezierCurve _curve;

	public HandleStyle handleStyle;

	[SerializeField]
	private Vector3 _handle1;

	[SerializeField]
	private Vector3 _handle2;

	private Vector3 lastPosition;

	public BezierCurve curve
	{
		get
		{
			return _curve;
		}
		set
		{
			if ((bool)_curve)
			{
				_curve.RemovePoint(this);
			}
			_curve = value;
			_curve.AddPoint(this);
		}
	}

	public Vector3 position
	{
		get
		{
			return base.transform.position;
		}
		set
		{
			base.transform.position = value;
		}
	}

	public Vector3 localPosition
	{
		get
		{
			return base.transform.localPosition;
		}
		set
		{
			base.transform.localPosition = value;
		}
	}

	public Vector3 handle1
	{
		get
		{
			return _handle1;
		}
		set
		{
			if (!(_handle1 == value))
			{
				_handle1 = value;
				if (handleStyle == HandleStyle.None)
				{
					handleStyle = HandleStyle.Broken;
				}
				else if (handleStyle == HandleStyle.Connected)
				{
					_handle2 = -value;
				}
				_curve.SetDirty();
			}
		}
	}

	public Vector3 globalHandle1
	{
		get
		{
			return base.transform.TransformPoint(handle1);
		}
		set
		{
			handle1 = base.transform.InverseTransformPoint(value);
		}
	}

	public Vector3 handle2
	{
		get
		{
			return _handle2;
		}
		set
		{
			if (!(_handle2 == value))
			{
				_handle2 = value;
				if (handleStyle == HandleStyle.None)
				{
					handleStyle = HandleStyle.Broken;
				}
				else if (handleStyle == HandleStyle.Connected)
				{
					_handle1 = -value;
				}
				_curve.SetDirty();
			}
		}
	}

	public Vector3 globalHandle2
	{
		get
		{
			return base.transform.TransformPoint(handle2);
		}
		set
		{
			handle2 = base.transform.InverseTransformPoint(value);
		}
	}

	private void Update()
	{
		if (!_curve.dirty && base.transform.position != lastPosition)
		{
			_curve.SetDirty();
			lastPosition = base.transform.position;
		}
	}
}
