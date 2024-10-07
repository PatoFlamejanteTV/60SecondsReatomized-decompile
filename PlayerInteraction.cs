using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Rewired;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Remaster.Scavenge;
using RG.SecondsRemaster.Scavenge;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
	[SerializeField]
	private float _maxAngle = 45f;

	[SerializeField]
	private GameObject _grabEffectTemplate;

	[SerializeField]
	private GameObject _grabEffectSource;

	private bool _canInteract = true;

	private bool _isNearShelter;

	private bool _canGrab;

	private bool _canDrop;

	private bool _showGrabLimit;

	private bool _endInteractionDone;

	private ScavengeItemController _itemToGrab;

	private ScavengeItemController _currentTarget;

	private GameObject _exit;

	private float _nextInteractionTime;

	private float _nextShowGrabLimitTime;

	[SerializeField]
	private float _interactionDelay = 1f;

	[SerializeField]
	private float _showGrabLimitDelay = 1f;

	[SerializeField]
	private LayerMask _layerMask;

	[SerializeField]
	private float _targetUpdateTime = 0.05f;

	[SerializeField]
	private Color _highlightColor = Color.white;

	[SerializeField]
	private Color _targetHighlightColor = Color.red;

	[SerializeField]
	private Color _targetHighlightColorInventoryFull = Color.red;

	[SerializeField]
	private float _maxInteractionDistance = 5f;

	[EventRef]
	[SerializeField]
	private string _nopeMaleSoundName;

	[EventRef]
	[SerializeField]
	private string _nopeFemaleSoundName;

	[EventRef]
	[SerializeField]
	private string _scavengeGrabSoundName;

	[EventRef]
	[SerializeField]
	private string _scavengeDropSoundName;

	[SerializeField]
	private ThirdPersonController _thirdPersonController;

	[SerializeField]
	private GlobalIntVariable _nukeDropsSurvivedVariable;

	[SerializeField]
	private GlobalIntVariable _perishedInExplosionVariable;

	[SerializeField]
	private GlobalIntVariable _totalItemsCollectedVariable;

	[SerializeField]
	private GlobalIntVariable _mostSuppliesCollectedVariable;

	[SerializeField]
	private GlobalIntVariable _peoplePerishedFromNukeDropVariable;

	private PlayerInventory _inventory;

	private Animator _anim;

	private GameFlow _gameFlow;

	private Shelter _shelter;

	private Player _player;

	private const int PLAYER_INDEX = 0;

	public Color HighlightColor => _highlightColor;

	public Color TargetHighlightColor => _targetHighlightColor;

	public Color TargetHighlightColorInventoryFull => _targetHighlightColorInventoryFull;

	private void Awake()
	{
		_player = ReInput.players.GetPlayer(0);
	}

	private void Start()
	{
		_gameFlow = GlobalTools.GetController<GameFlow>();
		_inventory = GlobalTools.GetPlayerInventory();
		_anim = GetComponent<Animator>();
		_exit = GlobalTools.GetShelter().gameObject;
		_shelter = GlobalTools.GetShelter();
		StartCoroutine(ShowTarget());
	}

	private void Update()
	{
		if (!_gameFlow.Paused)
		{
			DoInteraction();
		}
	}

	private IEnumerator ShowTarget()
	{
		while (true)
		{
			ScavengeItemController scavengeItemController = FindClosestItemInSight();
			if (scavengeItemController != _currentTarget)
			{
				if (_currentTarget != null)
				{
					_currentTarget.TargetHighlight(on: false);
				}
				if (scavengeItemController != null)
				{
					scavengeItemController.TargetHighlight(on: true);
				}
				_currentTarget = scavengeItemController;
			}
			yield return new WaitForSeconds(_targetUpdateTime);
		}
	}

	private void OnGrabTouchdown()
	{
		AudioManager.PlaySound(_scavengeGrabSoundName);
		if (_grabEffectTemplate != null)
		{
			Vector3 position = (_itemToGrab.GetComponent<ScavengeItemController>().IsCharacter ? new Vector3(_itemToGrab.transform.position.x, _grabEffectSource.transform.position.y, _itemToGrab.transform.position.z) : _itemToGrab.transform.position);
			Object.Instantiate(_grabEffectTemplate, position, default(Quaternion)).transform.parent = Camera.main.transform;
		}
		if (_itemToGrab != null)
		{
			_itemToGrab.gameObject.SetActive(value: false);
		}
	}

	public void EnableInteraction(bool enableGrab, bool enableDrop, bool showGrabLimit)
	{
		_canGrab = enableGrab;
		_canDrop = enableDrop;
		_showGrabLimit = showGrabLimit;
	}

	private IEnumerator DeactivateGrabItem()
	{
		if (_itemToGrab != null)
		{
			_itemToGrab.gameObject.SetActive(value: false);
		}
		yield break;
	}

	private void OnGrabFinished()
	{
		_anim.SetBool("grabbing", value: false);
		_itemToGrab = null;
		_canGrab = true;
	}

	private void OnDropFinished()
	{
		_anim.SetBool("dropping", value: false);
		_shelter.DropIntoShelter(_scavengeDropSoundName);
	}

	private void OnDuckAndCoverComplete()
	{
		_endInteractionDone = true;
	}

	private void OnShelterJumpComplete()
	{
		_endInteractionDone = true;
	}

	public void JumpToShelter()
	{
		Transform transform = GameObject.FindGameObjectWithTag("JumpPoint").transform;
		UpdateGlobalStats(scavengeSurvived: true);
		base.transform.position = transform.position;
		base.transform.rotation = transform.rotation;
		if (GameSessionData.Instance.Character == ECharacter.MOM)
		{
			base.transform.position += base.transform.forward * 0.2f;
		}
		Object.Destroy(GetComponent<Rigidbody>());
		_anim.SetBool("jumping", value: true);
	}

	public void DuckAndCover()
	{
		Transform transform = GameObject.FindGameObjectWithTag("DuckPoint").transform;
		UpdateGlobalStats(scavengeSurvived: false);
		Vector3 origin = base.transform.position + new Vector3(0f, 0.5f, 0f);
		bool flag = true;
		int num = LayerMask.NameToLayer("Room");
		if (Physics.Raycast(origin, -Vector3.up, out var hitInfo) && hitInfo.collider.gameObject.layer == num)
		{
			transform.parent.GetComponent<Renderer>().material = hitInfo.collider.GetComponent<Renderer>().material;
			flag = false;
		}
		if (flag)
		{
			GameObject gameObject = null;
			float num2 = 999f;
			Vector3 position = base.transform.position;
			GameObject[] array = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].layer == num)
				{
					float num3 = Vector3.Distance(array[i].transform.position, position);
					if (num3 < num2)
					{
						gameObject = array[i];
						num2 = num3;
					}
				}
			}
			if (gameObject != null)
			{
				Material[] materials = gameObject.GetComponent<Renderer>().materials;
				for (int j = 0; j < materials.Length; j++)
				{
					if (materials[j].name.Contains("floor"))
					{
						transform.parent.GetComponent<Renderer>().material = materials[j];
						break;
					}
				}
			}
		}
		transform.parent.GetComponent<Renderer>().material.mainTextureScale = transform.parent.transform.localScale;
		base.transform.position = transform.position;
		base.transform.rotation = transform.rotation;
		Object.Destroy(GetComponent<Rigidbody>());
		_anim.SetBool("ducking", value: true);
	}

	private void UpdateGlobalStats(bool scavengeSurvived)
	{
		if (GameSessionData.Instance.Setup.GameType == EGameType.TUTORIAL || GameSessionData.Instance.Setup.GameType == EGameType.CHALLENGE_SCAVENGE)
		{
			return;
		}
		if (scavengeSurvived)
		{
			if (_nukeDropsSurvivedVariable != null)
			{
				_nukeDropsSurvivedVariable.SetValue(_nukeDropsSurvivedVariable.Value + 1);
			}
		}
		else if (_perishedInExplosionVariable != null)
		{
			_perishedInExplosionVariable.SetValue(_perishedInExplosionVariable.Value + 1);
		}
		UpdateTotalItemsCollected();
		UpdateMostSuppliesCollected();
		UpdatePeoplePerished();
	}

	private void UpdatePeoplePerished()
	{
		if (!(_peoplePerishedFromNukeDropVariable != null))
		{
			return;
		}
		List<ScavengeItem> currentInventory = _gameFlow.SurvivalTransferManager.GetCurrentInventory();
		int num = 3;
		for (int i = 0; i < currentInventory.Count; i++)
		{
			if (currentInventory[i].Character != null)
			{
				num--;
			}
		}
		if (num > 0)
		{
			_peoplePerishedFromNukeDropVariable.SetValue(_peoplePerishedFromNukeDropVariable.Value + num);
		}
	}

	private void UpdateMostSuppliesCollected()
	{
		if (_mostSuppliesCollectedVariable != null && _gameFlow.SurvivalTransferManager.GetCurrentItemsCount() > _mostSuppliesCollectedVariable.Value)
		{
			_mostSuppliesCollectedVariable.SetValue(_mostSuppliesCollectedVariable.Value + _gameFlow.SurvivalTransferManager.GetCurrentItemsCount());
		}
	}

	private void UpdateTotalItemsCollected()
	{
		if (_totalItemsCollectedVariable != null)
		{
			_totalItemsCollectedVariable.SetValue(_totalItemsCollectedVariable.Value + _gameFlow.SurvivalTransferManager.GetCurrentItemsCount());
		}
	}

	public bool EndInteractionDone()
	{
		return _endInteractionDone;
	}

	private void DoInteraction()
	{
		if (!_canInteract && !(_nextInteractionTime <= Time.time))
		{
			return;
		}
		_canInteract = true;
		if (_anim.GetBool("grabbing") || _anim.GetBool("dropping") || !(_itemToGrab == null) || !_player.GetButtonRepeating(4))
		{
			return;
		}
		if (_canDrop && _isNearShelter && !_gameFlow.HandsController.AreHandsEmpty() && GetAngleTo(_exit) < _maxAngle)
		{
			_nextInteractionTime = Time.time + _interactionDelay;
			_canInteract = false;
			_thirdPersonController.StopMovement();
			_anim.SetBool("dropping", value: true);
			_gameFlow.SurvivalTransferManager.TransferHeldItems();
			_gameFlow.HandsController.Clear();
			if (GameSessionData.Instance.Setup.GameType == EGameType.CHALLENGE_SCAVENGE && _gameFlow.IsChallengeConditionAchieved())
			{
				_gameFlow.Terminated = true;
			}
		}
		else if (_canGrab)
		{
			GrabItem();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.parent != null && other.transform.parent.gameObject == _exit)
		{
			_isNearShelter = true;
			GlobalTools.GetController<GameFlow>().ReportNearShelter(near: true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.transform.parent != null && other.transform.parent.gameObject == _exit)
		{
			_isNearShelter = false;
			GlobalTools.GetController<GameFlow>().ReportNearShelter(near: false);
		}
	}

	public bool IsPlayerNearShelter()
	{
		return _isNearShelter;
	}

	private void GrabItem()
	{
		if (!(_itemToGrab == null) || !(_currentTarget != null))
		{
			return;
		}
		ScavengeItemController scavengeItemController = FindClosestItemInSight();
		if (!(scavengeItemController != null) || !(scavengeItemController == _currentTarget))
		{
			return;
		}
		_thirdPersonController.StopMovement();
		if (_gameFlow.HandsController.AddItem(_currentTarget.ScavengeItem))
		{
			_anim.SetBool("grabbing", value: true);
			_currentTarget.TargetHighlight(on: false);
			_itemToGrab = _currentTarget;
			_canGrab = false;
			_canInteract = false;
			_currentTarget.ScavengeItem.AddHeldItem();
			_gameFlow.ReportCollectedItem(_currentTarget);
			_gameFlow.ChallengeItemsController.DisableScavengeItem(_currentTarget.ScavengeItem);
			ScavengeItemManager.Instance.UnregisterController(_currentTarget);
			_currentTarget = null;
			_nextInteractionTime = Time.time + _interactionDelay;
		}
		else if (_showGrabLimit && _nextShowGrabLimitTime <= Time.time)
		{
			_gameFlow.NoRoomText.ShowText();
			if (GameSessionData.Instance.Character == ECharacter.DAD)
			{
				AudioManager.PlaySound(_nopeMaleSoundName);
			}
			else
			{
				AudioManager.PlaySound(_nopeFemaleSoundName);
			}
			DelayShowGrabLimit();
		}
	}

	public void DelayShowGrabLimit()
	{
		_nextShowGrabLimitTime = Time.time + _showGrabLimitDelay;
	}

	private float GetAngleTo(GameObject target)
	{
		Vector3 vector = target.transform.position - base.gameObject.transform.position;
		return Vector3.Angle(new Vector3(base.gameObject.transform.forward.x, 0f, base.gameObject.transform.forward.z), new Vector3(vector.x, 0f, vector.z));
	}

	private ScavengeItemController FindClosestItemInSight()
	{
		ScavengeItemController result = null;
		float num = 9999f;
		List<ScavengeItemController> scavengeItems = ScavengeItemManager.Instance.ScavengeItems;
		for (int i = 0; i < scavengeItems.Count; i++)
		{
			if (!(scavengeItems[i] != null) || !scavengeItems[i].CanBePickedUp || !(GetAngleTo(scavengeItems[i].gameObject) < _maxAngle))
			{
				continue;
			}
			Vector3 vector = scavengeItems[i].transform.position - base.transform.position;
			vector.y = 0f;
			if (vector.magnitude <= scavengeItems[i].MaxReachDistance && vector.magnitude < num && scavengeItems[i].RaycastTarget != null)
			{
				Vector3 end = base.gameObject.transform.position + Vector3.up;
				if (Physics.Linecast(scavengeItems[i].RaycastTarget.position, end, out var hitInfo, _layerMask.value) && hitInfo.collider.gameObject == base.gameObject)
				{
					result = scavengeItems[i];
					num = vector.magnitude;
				}
			}
		}
		return result;
	}
}
