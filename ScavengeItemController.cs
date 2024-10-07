using HighlightingSystem;
using RG.Parsecs.Common;
using RG.Remaster.Scavenge;
using RG.SecondsRemaster.Scavenge;
using UnityEngine;

public class ScavengeItemController : MonoBehaviour
{
	[SerializeField]
	private string _name = string.Empty;

	[SerializeField]
	private string _iconName = string.Empty;

	[SerializeField]
	private string _survivalName = string.Empty;

	[SerializeField]
	private int _weight;

	[SerializeField]
	private ScavengeItem _scavengeItem;

	[SerializeField]
	private ECharacter _character;

	[SerializeField]
	private bool _specialItem;

	[SerializeField]
	private bool _canBePickedUp = true;

	[SerializeField]
	private float _specialActionTimeout;

	private float _specialActionCounter;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private Transform _raycastTarget;

	[SerializeField]
	private Highlighter[] _childrenHighlighters;

	[SerializeField]
	private Highlighter _childHighlighter;

	private float _maxReachDistance = 1.5f;

	public Transform RaycastTarget => _raycastTarget;

	public float MaxReachDistance => _maxReachDistance;

	public string IconName => _iconName;

	public string SurvivalName => _survivalName;

	public int Weight => _weight;

	public bool SpecialItem => _specialItem;

	public bool CanBePickedUp
	{
		get
		{
			return _canBePickedUp;
		}
		set
		{
			_canBePickedUp = value;
		}
	}

	public bool IsCharacter => _character != ECharacter.NONE;

	public ECharacter Character => _character;

	public ScavengeItem ScavengeItem => _scavengeItem;

	public void Start()
	{
		_specialActionCounter += _specialActionTimeout;
		_childrenHighlighters = GetComponentsInChildren<Highlighter>();
		_childHighlighter = GetComponentInChildren<Highlighter>();
		_animator = GetComponent<Animator>();
		SphereCollider componentInChildren = GetComponentInChildren<SphereCollider>();
		if (componentInChildren != null)
		{
			_maxReachDistance = componentInChildren.radius;
		}
		Highlight(on: true);
	}

	public void OnEnable()
	{
		ScavengeItemManager.Instance.RegisterController(this);
	}

	private void HighlightObject(Highlighter highlighter, bool on)
	{
		if (!(highlighter != null))
		{
			return;
		}
		PlayerInteraction playerInteraction = GlobalTools.GetPlayerInteraction();
		if (playerInteraction != null)
		{
			if (on)
			{
				highlighter.ConstantOn(playerInteraction.HighlightColor);
			}
			else
			{
				highlighter.ConstantOff();
			}
		}
	}

	public void Highlight(bool on)
	{
		if (_canBePickedUp)
		{
			for (int i = 0; i < _childrenHighlighters.Length; i++)
			{
				HighlightObject(_childrenHighlighters[i], on);
			}
		}
	}

	public void TargetHighlight(bool on)
	{
		if (!_canBePickedUp)
		{
			return;
		}
		PlayerInteraction playerInteraction = GlobalTools.GetPlayerInteraction();
		GameFlow controller = GlobalTools.GetController<GameFlow>();
		if (!(_childHighlighter != null))
		{
			return;
		}
		_childHighlighter.ConstantOffImmediate();
		if (on)
		{
			if (controller.HandsController.WillItemFit(ScavengeItem))
			{
				_childHighlighter.ConstantOn(playerInteraction.TargetHighlightColor);
			}
			else
			{
				_childHighlighter.ConstantOn(playerInteraction.TargetHighlightColorInventoryFull);
			}
		}
		else
		{
			_childHighlighter.ConstantOn(playerInteraction.HighlightColor);
		}
	}

	public void PlaySound(string soundName)
	{
		AudioManager.PlaySoundAtPoint(soundName, base.transform);
	}

	public void DoSpecialAction()
	{
		if (_animator != null && _specialActionCounter <= Time.time)
		{
			_animator.SetTrigger("specialAction");
		}
	}

	public void EndSpecialAction()
	{
		if (_animator != null)
		{
			_specialActionCounter = Time.time + _specialActionTimeout;
		}
	}
}
