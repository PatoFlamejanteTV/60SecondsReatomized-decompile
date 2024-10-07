using UnityEngine;

public struct SBaseDataLog
{
	public float Time;

	public Vector3 Position;

	public Quaternion Rotation;

	public SBaseDataLog(float time, Vector3 pos, Quaternion rot)
	{
		Time = time;
		Position = pos;
		Rotation = rot;
	}

	public string GetString(char sep)
	{
		return Time.ToString() + sep + Position.ToString() + sep + Rotation.ToString();
	}
}
