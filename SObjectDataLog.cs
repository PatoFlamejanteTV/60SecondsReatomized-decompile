using UnityEngine;

public struct SObjectDataLog
{
	public string Id;

	public SBaseDataLog BaseData;

	public SObjectDataLog(float time, Vector3 pos, Quaternion rot, string id)
	{
		BaseData = new SBaseDataLog(time, pos, rot);
		Id = id;
	}

	public string GetString(char sep)
	{
		return BaseData.GetString(sep) + sep + Id.ToString();
	}
}
