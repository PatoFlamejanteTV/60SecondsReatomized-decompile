using System;

[Serializable]
public struct SKeyValuePair
{
	public string Id;

	public string Val;

	public SKeyValuePair(string id, string val)
	{
		Id = id;
		Val = val;
	}
}
