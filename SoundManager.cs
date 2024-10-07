using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	private class PlayInstance
	{
		private AudioSource _source;

		private float _timeStarted;

		private float _length;

		private float _endTime;

		public AudioSource Source => _source;

		public float TimeStarted => _timeStarted;

		public float Length => _length;

		public PlayInstance(AudioSource source, float timeStarted, float length)
		{
			_source = source;
			_timeStarted = timeStarted;
			_length = length;
			_endTime = timeStarted + length;
		}

		public bool ShouldStop()
		{
			return Time.time >= _endTime;
		}
	}

	public static SoundManager Instance;

	[SerializeField]
	private AudioSource _musicSource;

	[Range(1f, 32f)]
	[SerializeField]
	private int _sfxSourceCount = 1;

	private AudioSource[] _sfxSources;

	private List<PlayInstance> _sfxPlayback = new List<PlayInstance>();

	private AudioCollection _audioCollection;

	private AudioEntry _currentMusicTrack;

	private bool _terminate;

	private bool _paused;

	private bool _sfxMute;

	private bool _musicMute;

	private bool _musicStop;

	private bool _musicForcedChange = true;

	private bool _musicWaitTrigger;

	private EPlaybackStyle _musicPlaybackStyle;

	private float _sfxVolume = 1f;

	private float _musicVolume = 1f;

	private float _lastPauseTime;

	private float _musicEndMark;

	public AudioSource MusicSource => _musicSource;

	public float VolumeSfx
	{
		get
		{
			return _sfxVolume;
		}
		set
		{
			_sfxVolume = value;
		}
	}

	public float VolumeMusic
	{
		get
		{
			return _musicVolume;
		}
		set
		{
			_musicVolume = value;
			if (_musicSource != null && _currentMusicTrack != null)
			{
				_musicSource.volume = _musicVolume * _currentMusicTrack.GetRandomisedVolume();
			}
		}
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		_sfxSources = new AudioSource[Mathf.Clamp(_sfxSourceCount, 1, 32)];
		for (int i = 0; i < _sfxSourceCount; i++)
		{
			_sfxSources[i] = base.gameObject.AddComponent<AudioSource>();
		}
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		StartCoroutine(ControlMusic());
		StartCoroutine(ControlSfx());
	}

	private void Update()
	{
	}

	public void LoadAudioCollection(AudioCollection ac, EPlaybackStyle playback = EPlaybackStyle.SEQUENCE, bool musicAutoPlay = true)
	{
		_audioCollection = ac;
		if (_audioCollection != null)
		{
			_audioCollection.Initialize();
			_musicForcedChange = true;
			_musicPlaybackStyle = playback;
			_musicWaitTrigger = !musicAutoPlay;
		}
		else
		{
			Debug.LogError("Invalid audio collection.");
		}
	}

	public void UnloadAudioCollection()
	{
		_audioCollection = null;
	}

	public void MuteMusic(bool mute = true)
	{
		if (_musicSource != null)
		{
			_musicSource.mute = mute;
			_musicMute = mute;
		}
	}

	public void PauseMusic(bool pause)
	{
		if (pause)
		{
			_musicSource.Pause();
			_lastPauseTime = Time.time;
		}
		else
		{
			_musicSource.UnPause();
			float num = Time.time - _lastPauseTime;
			_musicEndMark += num;
		}
	}

	public void RestartMusic()
	{
		if (_musicSource != null)
		{
			_musicSource.Play();
		}
	}

	public void PauseToggle()
	{
		_paused = !_paused;
		PauseMusic(_paused);
		for (int i = 0; i < _sfxPlayback.Count; i++)
		{
			if (_paused)
			{
				_sfxPlayback[i].Source.Pause();
			}
			else
			{
				_sfxPlayback[i].Source.UnPause();
			}
		}
	}

	public void Stop()
	{
		StopSFX();
		StopMusic();
	}

	public void StopMusic(bool immediate = false)
	{
		_currentMusicTrack = null;
		_musicSource.Stop();
		_musicStop = true;
	}

	public void StartMusic()
	{
		_musicSource.Play();
		_musicStop = false;
	}

	public void StopSFX()
	{
		for (int num = _sfxPlayback.Count - 1; num >= 0; num--)
		{
			StopSFXObject(_sfxPlayback[num].Source);
		}
	}

	public void StopSFXObject(AudioSource src)
	{
		if (!(src != null))
		{
			return;
		}
		src.Stop();
		for (int i = 0; i < _sfxPlayback.Count; i++)
		{
			if (_sfxPlayback[i].Source == src)
			{
				_sfxPlayback.RemoveAt(i);
				break;
			}
		}
	}

	private AudioSource GetFreeSfxSource()
	{
		int i = 0;
		while (i < _sfxSourceCount)
		{
			if (_sfxPlayback.Find((PlayInstance x) => x.Source == _sfxSources[i]) == null)
			{
				return _sfxSources[i];
			}
			int num = i + 1;
			i = num;
		}
		Debug.LogError("Not enough sfx audio sources for playback.");
		return null;
	}

	public void StopSFXObject(GameObject obj)
	{
		if (obj != null)
		{
			StopSFXObject(obj.GetComponent<AudioSource>());
		}
	}

	private void PlaySoundClip(AudioSource src, AudioClip audio, float volume)
	{
		src.volume = volume;
		src.pitch = 1f;
		src.PlayOneShot(audio);
	}

	private void PlaySoundClip(AudioSource src, AudioEntry audio, float volume)
	{
		src.volume = audio.BaseVolume * audio.GetRandomisedVolume() * volume;
		src.pitch = 1f * audio.GetRandomisedPitch();
		src.PlayOneShot(audio.Clip);
	}

	private void PlaySound(AudioSource src, AudioEntry audio, float volume, bool loop = false, float delay = 0f)
	{
		src.clip = audio.Clip;
		src.volume = audio.BaseVolume * audio.GetRandomisedVolume() * volume;
		src.pitch = 1f * audio.GetRandomisedPitch();
		src.loop = loop;
		if (Mathf.Approximately(delay, 0f))
		{
			src.Play();
		}
		else
		{
			src.PlayDelayed(delay);
		}
	}

	private IEnumerator ControlSfx()
	{
		while (!_terminate)
		{
			for (int num = _sfxPlayback.Count - 1; num >= 0; num--)
			{
				if ((!_sfxPlayback[num].Source.isPlaying || _sfxPlayback[num].ShouldStop()) && !_sfxPlayback[num].Source.loop)
				{
					_sfxPlayback[num].Source.clip = null;
					_sfxPlayback.RemoveAt(num);
				}
			}
			yield return new WaitForSeconds(1f);
		}
		StopSFX();
		_sfxPlayback.Clear();
	}

	private IEnumerator ControlMusic()
	{
		AudioEntry[] musicTracks = null;
		bool ended = false;
		int nextTrackToPlayIndex = 0;
		List<int> tracksAlreadyPlayed = new List<int>();
		while (!_terminate)
		{
			if (_audioCollection != null)
			{
				if (_musicForcedChange)
				{
					StopMusic();
					musicTracks = _audioCollection.GetMusicGroup();
					_musicForcedChange = false;
					_musicStop = false;
					_musicEndMark = 0f;
					ended = false;
					nextTrackToPlayIndex = 0;
					tracksAlreadyPlayed.Clear();
				}
				if (!ended && !_paused && _musicEndMark <= Time.time)
				{
					int num = -1;
					switch (_musicPlaybackStyle)
					{
					case EPlaybackStyle.SEQUENCE:
					{
						if (nextTrackToPlayIndex >= musicTracks.Length)
						{
							ended = true;
							break;
						}
						num = nextTrackToPlayIndex;
						int num2 = nextTrackToPlayIndex + 1;
						nextTrackToPlayIndex = num2;
						break;
					}
					case EPlaybackStyle.RANDOM:
						num = Random.Range(0, musicTracks.Length);
						ended = tracksAlreadyPlayed.Count >= musicTracks.Length;
						if (ended)
						{
							break;
						}
						while (tracksAlreadyPlayed.Contains(num))
						{
							num++;
							if (num >= musicTracks.Length)
							{
								num = 0;
							}
						}
						tracksAlreadyPlayed.Add(num);
						break;
					case EPlaybackStyle.LOOPED_SEQUENCE:
					{
						if (nextTrackToPlayIndex >= musicTracks.Length)
						{
							nextTrackToPlayIndex = 0;
						}
						num = nextTrackToPlayIndex;
						int num2 = nextTrackToPlayIndex + 1;
						nextTrackToPlayIndex = num2;
						break;
					}
					case EPlaybackStyle.LOOPED_RANDOM:
						num = Random.Range(0, musicTracks.Length);
						while (tracksAlreadyPlayed.Contains(num))
						{
							num++;
							if (num >= musicTracks.Length)
							{
								num = 0;
							}
						}
						if (tracksAlreadyPlayed.Count + 1 >= musicTracks.Length)
						{
							tracksAlreadyPlayed.Clear();
						}
						if (!tracksAlreadyPlayed.Contains(num))
						{
							tracksAlreadyPlayed.Add(num);
						}
						break;
					}
					if (!ended && num >= 0 && num < musicTracks.Length)
					{
						_currentMusicTrack = musicTracks[num];
						PlaySound(_musicSource, musicTracks[num], _musicVolume);
						_musicEndMark = Time.time + musicTracks[num].Clip.length;
						if (_musicWaitTrigger)
						{
							_musicWaitTrigger = false;
							StopMusic(immediate: true);
						}
					}
				}
			}
			yield return new WaitForSeconds(1f);
		}
		StopMusic();
	}

	public AudioSource PlaySfxAudioEntry(AudioEntry sound, bool loop, float delay, Transform source)
	{
		AudioSource origin = null;
		if (sound != null)
		{
			if (source == null)
			{
				origin = GetFreeSfxSource();
			}
			else
			{
				origin = source.GetComponent<AudioSource>();
			}
			if (origin != null)
			{
				if (_sfxPlayback.Find((PlayInstance x) => x.Source == origin) != null)
				{
					PlaySoundClip(origin, sound, _sfxVolume);
				}
				else
				{
					PlaySound(origin, sound, _sfxVolume, loop, delay);
					_sfxPlayback.Add(new PlayInstance(origin, Time.time, origin.clip.length));
				}
			}
		}
		return origin;
	}

	public AudioSource PlaySFX(AudioClip sound, bool loop, float delay, float volume, float pitch, Transform source)
	{
		AudioSource origin = null;
		if (sound != null)
		{
			if (source == null)
			{
				origin = GetFreeSfxSource();
			}
			else
			{
				origin = source.GetComponent<AudioSource>();
			}
			if (origin != null)
			{
				if (_sfxPlayback.Find((PlayInstance x) => x.Source == origin) != null)
				{
					PlaySoundClip(origin, sound, _sfxVolume);
				}
				else
				{
					PlaySoundClip(origin, sound, _sfxVolume);
					_sfxPlayback.Add(new PlayInstance(origin, Time.time, origin.clip.length));
				}
			}
		}
		return origin;
	}

	public AudioEntry LoadFromGroup(string name)
	{
		if (_audioCollection != null)
		{
			return _audioCollection.GetRandomEntry(name);
		}
		Debug.LogError($"Could not load audio from group '{name}'.");
		return null;
	}

	public AudioEntry Load(string name)
	{
		if (_audioCollection != null)
		{
			return _audioCollection.GetEntry(name);
		}
		Debug.LogError($"Could not load audio '{name}'.");
		return null;
	}

	public bool IsAudioClipPlaying(string name)
	{
		for (int i = 0; i < _sfxPlayback.Count; i++)
		{
			if (_sfxPlayback[i].Source.clip.name == name)
			{
				return true;
			}
		}
		return false;
	}

	public void CrossIn(float time, AudioSource src)
	{
		StartCoroutine(DoCross(crossIn: true, time, src));
	}

	private IEnumerator DoCross(bool crossIn, float time, AudioSource src)
	{
		if (!(src != null) || !(src.clip != null) || !src.isPlaying)
		{
			yield break;
		}
		float target = (crossIn ? src.volume : 0f);
		float origin = (crossIn ? 0f : src.volume);
		float endTime = Time.time + time;
		src.volume = origin;
		float timeElapsed = 0f;
		bool terminate = false;
		while (endTime > Time.time)
		{
			if (!src.isPlaying || _musicForcedChange)
			{
				terminate = true;
				break;
			}
			timeElapsed += Time.deltaTime;
			src.volume = Mathf.Lerp(origin, target, timeElapsed / time);
			yield return null;
		}
		if (!terminate)
		{
			src.volume = target;
			if (!crossIn)
			{
				src.Stop();
			}
		}
	}

	public void CrossOut(float time, AudioSource src)
	{
		StartCoroutine(DoCross(crossIn: false, time, src));
	}
}
