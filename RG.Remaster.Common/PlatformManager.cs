using RG.Parsecs.Common;
using UnityEngine;

namespace RG.Remaster.Common;

public class PlatformManager : Singleton<PlatformManager>
{
	[SerializeField]
	private RichPresenceManager _richPresenceManager;

	public RichPresenceManager RichPresenceManager => _richPresenceManager;

	private void Start()
	{
		if (_richPresenceManager != null)
		{
			_richPresenceManager.Initialize();
		}
	}
}
