using I2.Loc;
using RG.Parsecs.Loading;
using UnityEngine;

namespace RG.SecondsRemaster.Loading;

[CreateAssetMenu(menuName = "60 Seconds Remaster!/New Loading Screen")]
public class LoadingScreen : Poster
{
	[SerializeField]
	private LocalizedString _middleText;

	[SerializeField]
	private Color _backgroundColor;

	[SerializeField]
	private Color _spikesColor;

	public LocalizedString MiddleText => _middleText;

	public Color BackgroundColor => _backgroundColor;

	public Color SpikesColor => _spikesColor;
}
