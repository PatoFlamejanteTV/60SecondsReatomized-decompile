using RG.Parsecs.EventEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Menu;

public class CreditsEyesController : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private RectTransform _rectTransform;

	[SerializeField]
	private GlobalBoolVariable _isContinueAvailable;

	[SerializeField]
	private CreditsEyeGroup _parentGroup;

	[SerializeField]
	private EEyesType _eyesType;

	[HideInInspector]
	public bool IsFree = true;

	[Range(0f, 1f)]
	[SerializeField]
	private float _chanceForLongStare = 0.5f;

	private const string LONG_STARE_TRIGGER_NAME = "LongLook";

	private const string SHORT_STARE_TRIGGER_NAME = "ShortLook";

	private const string LONG_STARE_TRIGGER_POSTAPO_NAME = "LongLookPostApo";

	private const string SHORT_STARE_TRIGGER_POSTAPO_NAME = "ShortLookPostApo";

	public RectTransform RectTransform => _rectTransform;

	public CreditsEyeGroup ParentGroup => _parentGroup;

	public void ShowAnimation()
	{
		if (!(_isContinueAvailable != null))
		{
			return;
		}
		if (_isContinueAvailable.Value)
		{
			if (Random.Range(0f, 1f) < _chanceForLongStare)
			{
				_animator.SetTrigger("LongLookPostApo");
			}
			else
			{
				_animator.SetTrigger("ShortLookPostApo");
			}
		}
		else if (Random.Range(0f, 1f) < _chanceForLongStare)
		{
			_animator.SetTrigger("LongLook");
		}
		else
		{
			_animator.SetTrigger("ShortLook");
		}
	}
}
