using UnityEngine;

namespace RG_GameCamera.Input.Mobile;

public class SimTouch
{
	private enum MouseStatus
	{
		StartDown,
		Down,
		Up,
		None
	}

	public int FingerId;

	public Vector2 Position;

	public Vector2 StartPosition;

	public TouchStatus Status;

	public int TapCount;

	public float DeltaTime;

	public Vector2 DeltaPosition;

	public float TapTimeWindow = 0.3f;

	public float PressTime;

	public float MoveThreshold = 0.5f;

	private Vector2 lastPosition;

	private static KeyCode SimulationKey;

	private float tapTimeout;

	private MouseStatus lastMouseStatus;

	public SimTouch(int fingerID, KeyCode simKey)
	{
		FingerId = fingerID;
		SimulationKey = simKey;
		lastMouseStatus = MouseStatus.None;
	}

	public void ScanInput()
	{
		UpdateTouchSim();
	}

	private MouseStatus GetMouseStatus()
	{
		bool flag = true;
		if (UnityEngine.Input.GetKey(SimulationKey))
		{
			if (FingerId > 0)
			{
				if (lastMouseStatus != MouseStatus.None && lastMouseStatus != 0)
				{
					DeltaPosition = Vector3.zero;
					Position = lastPosition;
					return lastMouseStatus;
				}
				flag = false;
				if (UnityEngine.Input.GetMouseButtonDown(0))
				{
					return MouseStatus.StartDown;
				}
				if (UnityEngine.Input.GetMouseButton(0))
				{
					return MouseStatus.Down;
				}
			}
		}
		else if (FingerId > 0)
		{
			return MouseStatus.None;
		}
		if (flag)
		{
			if (UnityEngine.Input.GetMouseButtonDown(0))
			{
				return MouseStatus.StartDown;
			}
			if (UnityEngine.Input.GetMouseButton(0))
			{
				return MouseStatus.Down;
			}
			if (UnityEngine.Input.GetMouseButtonUp(0))
			{
				return MouseStatus.Up;
			}
		}
		return MouseStatus.None;
	}

	private void UpdateTouchSim()
	{
		DeltaTime = Time.deltaTime;
		Position = UnityEngine.Input.mousePosition;
		DeltaPosition = new Vector2(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y) - lastPosition;
		switch (lastMouseStatus = GetMouseStatus())
		{
		case MouseStatus.StartDown:
			Status = TouchStatus.Start;
			StartPosition = UnityEngine.Input.mousePosition;
			DeltaPosition = Vector2.zero;
			lastPosition = UnityEngine.Input.mousePosition;
			tapTimeout = TapTimeWindow;
			break;
		case MouseStatus.Down:
			Status = TouchStatus.Stationary;
			if (DeltaPosition.sqrMagnitude > MoveThreshold * MoveThreshold)
			{
				Status = TouchStatus.Moving;
				lastPosition = Position;
			}
			break;
		case MouseStatus.Up:
			Status = TouchStatus.End;
			if (tapTimeout > 0f)
			{
				TapCount++;
			}
			break;
		case MouseStatus.None:
			if (Status != 0 && Status != TouchStatus.End)
			{
				Status = TouchStatus.End;
			}
			else if (Status == TouchStatus.End)
			{
				Status = TouchStatus.Invalid;
			}
			break;
		}
		tapTimeout -= Time.deltaTime;
		if (tapTimeout < 0f)
		{
			TapCount = 1;
		}
	}

	private bool GetTouchByID(int id, out Touch touch)
	{
		for (int i = 0; i < UnityEngine.Input.touchCount; i++)
		{
			Touch touch2 = UnityEngine.Input.GetTouch(i);
			if (touch2.fingerId == id)
			{
				touch = touch2;
				return true;
			}
		}
		touch = default(Touch);
		return false;
	}

	private void UpdateTouchInput()
	{
		if (GetTouchByID(FingerId, out var touch))
		{
			DeltaPosition = touch.position - lastPosition;
			DeltaTime = touch.deltaTime;
			Position = touch.position;
			TapCount = touch.tapCount;
			switch (touch.phase)
			{
			case TouchPhase.Began:
				StartPosition = touch.position;
				DeltaPosition = Vector2.zero;
				lastPosition = Position;
				Status = TouchStatus.Start;
				PressTime = 0f;
				break;
			case TouchPhase.Moved:
			case TouchPhase.Stationary:
				Status = TouchStatus.Stationary;
				if (DeltaPosition.sqrMagnitude > MoveThreshold * MoveThreshold)
				{
					Status = TouchStatus.Moving;
					lastPosition = Position;
				}
				PressTime += Time.deltaTime;
				break;
			case TouchPhase.Ended:
				Status = TouchStatus.End;
				break;
			default:
				Status = TouchStatus.Invalid;
				break;
			}
		}
		else if (Status != 0 && Status != TouchStatus.End)
		{
			Status = TouchStatus.End;
		}
		else if (Status == TouchStatus.End)
		{
			Status = TouchStatus.Invalid;
		}
	}
}
