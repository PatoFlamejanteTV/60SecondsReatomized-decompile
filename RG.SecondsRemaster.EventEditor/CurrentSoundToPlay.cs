using RG.Core.Base;
using RG.Core.SaveSystem;
using UnityEngine;

namespace RG.SecondsRemaster.EventEditor;

[CreateAssetMenu(fileName = "Current Sound To Play", menuName = "60 Parsecs!/Current Sound To Play")]
public class CurrentSoundToPlay : RGScriptableObject, ISaveable
{
	[SerializeField]
	private SaveEvent _saveEvent;

	[SerializeField]
	private string _eventName = string.Empty;

	[SerializeField]
	private int _eventPriority = -1;

	[SerializeField]
	[Range(0f, 1f)]
	private float _volume = 0.5f;

	[SerializeField]
	[Range(-1f, 1f)]
	private float _pan;

	[SerializeField]
	[Range(-1f, 1f)]
	private float _pitch;

	[SerializeField]
	private int _offset;

	[SerializeField]
	private bool _offsetCheck;

	public string EventName
	{
		get
		{
			return _eventName;
		}
		set
		{
			_eventName = value;
		}
	}

	public int EventPriority
	{
		get
		{
			return _eventPriority;
		}
		set
		{
			_eventPriority = value;
		}
	}

	public float Volume
	{
		get
		{
			return _volume;
		}
		set
		{
			_volume = value;
		}
	}

	public float Pan
	{
		get
		{
			return _pan;
		}
		set
		{
			_pan = value;
		}
	}

	public float Pitch
	{
		get
		{
			return _pitch;
		}
		set
		{
			_pitch = value;
		}
	}

	public int Offset
	{
		get
		{
			return _offset;
		}
		set
		{
			_offset = value;
		}
	}

	public bool OffsetCheck
	{
		get
		{
			return _offsetCheck;
		}
		set
		{
			_offsetCheck = value;
		}
	}

	public string ID => Guid;

	private void OnEnable()
	{
		Register();
	}

	public string Serialize()
	{
		CurrentSoundToPlayWrapper currentSoundToPlayWrapper = default(CurrentSoundToPlayWrapper);
		currentSoundToPlayWrapper.EventName = EventName;
		currentSoundToPlayWrapper.EventPriority = EventPriority;
		currentSoundToPlayWrapper.Volume = Volume;
		currentSoundToPlayWrapper.Pan = Pan;
		currentSoundToPlayWrapper.Pitch = Pitch;
		currentSoundToPlayWrapper.Offset = Offset;
		currentSoundToPlayWrapper.OffsetCheck = OffsetCheck;
		return JsonUtility.ToJson(currentSoundToPlayWrapper);
	}

	public void Deserialize(string jsonData)
	{
		CurrentSoundToPlayWrapper currentSoundToPlayWrapper = JsonUtility.FromJson<CurrentSoundToPlayWrapper>(jsonData);
		EventName = currentSoundToPlayWrapper.EventName;
		EventPriority = currentSoundToPlayWrapper.EventPriority;
		Volume = currentSoundToPlayWrapper.Volume;
		Pan = currentSoundToPlayWrapper.Pan;
		Pitch = currentSoundToPlayWrapper.Pitch;
		Offset = currentSoundToPlayWrapper.Offset;
		OffsetCheck = currentSoundToPlayWrapper.OffsetCheck;
	}

	public void Register()
	{
		if ((bool)_saveEvent)
		{
			_saveEvent.RegisterListener(this);
			return;
		}
		Debug.LogFormat(this, "{0} has not set Save Event", base.name);
	}

	public void Unregister()
	{
		if ((bool)_saveEvent)
		{
			_saveEvent.UnregisterListener(this);
		}
	}

	public void ResetData()
	{
		EventName = string.Empty;
		EventPriority = -1;
		Volume = 0.5f;
		Pan = 0f;
		Pitch = 0f;
		Offset = 0;
		OffsetCheck = false;
	}
}
