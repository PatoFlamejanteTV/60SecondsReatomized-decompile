using System;
using UnityEngine;

namespace RG.Remaster.Common;

[Serializable]
public class RichPresence
{
	[SerializeField]
	private ERichPresenceStatus _statusId;

	[SerializeField]
	private string _content = string.Empty;

	[SerializeField]
	private SParametrizedRichPresence[] _parameters;

	public ERichPresenceStatus StatusId => _statusId;

	public string Content => _content;

	public string GetParametrizedRichPresence(ERichPresenceParameter paramId)
	{
		if (_parameters != null)
		{
			for (int i = 0; i < _parameters.Length; i++)
			{
				if (_parameters[i].Id == paramId)
				{
					return _parameters[i].Content;
				}
			}
		}
		return null;
	}
}
