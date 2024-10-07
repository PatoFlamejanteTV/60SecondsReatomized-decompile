using System;
using UnityEngine;

[Serializable]
public class AudioEntry
{
	[SerializeField]
	private AudioClip _clip;

	[SerializeField]
	private string _group = string.Empty;

	[SerializeField]
	private float _baseVolume = 1f;

	[SerializeField]
	private float _volumeVariation;

	[SerializeField]
	private float _pitchVariation;

	public string Name
	{
		get
		{
			if (!(_clip == null))
			{
				return _clip.name;
			}
			return string.Empty;
		}
	}

	public float BaseVolume => _baseVolume;

	public float VolumeVariation => _volumeVariation;

	public float PitchVariation => _pitchVariation;

	public AudioClip Clip => _clip;

	public string Group
	{
		get
		{
			return _group;
		}
		set
		{
			_group = value;
		}
	}

	public float GetRandomisedPitch()
	{
		return UnityEngine.Random.Range(1f - _pitchVariation, 1f + _pitchVariation);
	}

	public float GetRandomisedVolume()
	{
		return UnityEngine.Random.Range(1f - _volumeVariation, 1f + _volumeVariation);
	}
}
