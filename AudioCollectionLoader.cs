using UnityEngine;

public class AudioCollectionLoader : MonoBehaviour
{
	[SerializeField]
	private AudioCollection _audioCollection;

	[SerializeField]
	private EPlaybackStyle _playbackStyle = EPlaybackStyle.SEQUENCE;

	[SerializeField]
	private bool _musicAutoPlay = true;

	private void Awake()
	{
		if (_audioCollection != null)
		{
			SoundManager.Instance.LoadAudioCollection(_audioCollection, _playbackStyle, _musicAutoPlay);
		}
	}

	private void Start()
	{
		Object.Destroy(this);
	}
}
