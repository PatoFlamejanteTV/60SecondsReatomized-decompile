using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayVideo : MonoBehaviour
{
	private float _originalOrtoSize = 1f;

	private void Awake()
	{
		_originalOrtoSize = Camera.main.orthographicSize;
	}

	private void Start()
	{
	}

	public void UpdateScaling()
	{
	}

	public void Play()
	{
	}

	public void Pause()
	{
	}

	public void Stop()
	{
	}

	public bool IsPlaying()
	{
		return false;
	}
}
