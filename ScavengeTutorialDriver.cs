using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using I2.Loc;
using Rewired;
using RG.Parsecs.Common;
using RG.SecondsRemaster.Scavenge;
using UnityEngine;

public class ScavengeTutorialDriver : MonoBehaviour
{
	public enum ETutorialStage
	{
		NONE,
		INTRO,
		MOVE1,
		MOVE2,
		EXPLORE,
		GRAB_SON,
		GRAB_FOOD,
		GRAB_WATER,
		CAPACITY,
		SHELTER,
		DROP,
		SPECIALS,
		ANNOUNCEMENTS,
		DROP_ANNOUNCED,
		TIMED_SCAVENGE,
		OUTRO
	}

	private delegate void Progress();

	private delegate bool ConditionalProgress();

	[SerializeField]
	private List<LocalizedString> TIMED_SCAVENGE_COLLECT_SUCCESS_TXT = new List<LocalizedString> { "Tutorial/Scavenge/tut_timed_drop_ok_01" };

	[SerializeField]
	private List<LocalizedString> TIMED_SCAVENGE_COLLECT_FAIL_TXT = new List<LocalizedString> { "Tutorial/Scavenge/tut_timed_drop_bad_01" };

	private GameObject _target;

	private GameFlow _gameFlow;

	private ETutorialStage _stage;

	private string[] _currentTexts;

	private ScavengeItemController[] _scavengeItemsController;

	private bool _stageRunning;

	private bool _showingText;

	[SerializeField]
	private float _readTimePerSymbol = 0.075f;

	[SerializeField]
	private ScavengeTutorialTexts _tutorialTexts;

	[SerializeField]
	private ScavengeItem _food;

	[SerializeField]
	private ScavengeItem _water;

	[SerializeField]
	private ScavengeItem _son;

	[SerializeField]
	private ScavengeItem _radio;

	[EventRef]
	[SerializeField]
	private string _broadcastSoundName;

	[SerializeField]
	private ScavengeItemController[] _excludedItems;

	private Player _player;

	private const int PLAYER_INDEX = 0;

	private void Awake()
	{
		_gameFlow = GlobalTools.GetController<GameFlow>();
		_player = ReInput.players.GetPlayer(0);
	}

	private void Start()
	{
		_scavengeItemsController = Object.FindObjectsOfType<ScavengeItemController>();
		StartCoroutine(RunTutorial());
	}

	public void SetGameFlow(GameFlow flow)
	{
		_gameFlow = flow;
	}

	private IEnumerator RunTutorial()
	{
		yield return new WaitForSeconds(1f);
		StartStage(ETutorialStage.INTRO);
		while (_stageRunning)
		{
			yield return new WaitForSeconds(1f);
		}
		StartStage(ETutorialStage.MOVE1);
		while (_stageRunning)
		{
			yield return new WaitForSeconds(1f);
		}
		StartStage(ETutorialStage.MOVE2);
		while (_stageRunning)
		{
			yield return new WaitForSeconds(1f);
		}
		StartStage(ETutorialStage.EXPLORE);
		while (_stageRunning)
		{
			yield return new WaitForSeconds(1f);
		}
		StartStage(ETutorialStage.GRAB_SON);
		while (_stageRunning)
		{
			yield return new WaitForSeconds(1f);
		}
		StartStage(ETutorialStage.GRAB_FOOD);
		while (_stageRunning)
		{
			yield return new WaitForSeconds(1f);
		}
		StartStage(ETutorialStage.GRAB_WATER);
		while (_stageRunning)
		{
			yield return new WaitForSeconds(1f);
		}
		StartStage(ETutorialStage.CAPACITY);
		while (_stageRunning)
		{
			yield return new WaitForSeconds(1f);
		}
		StartStage(ETutorialStage.DROP);
		while (_stageRunning)
		{
			yield return new WaitForSeconds(1f);
		}
		StartStage(ETutorialStage.SPECIALS);
		while (_stageRunning)
		{
			yield return new WaitForSeconds(1f);
		}
		StartStage(ETutorialStage.TIMED_SCAVENGE);
		while (_stageRunning)
		{
			yield return new WaitForSeconds(1f);
		}
		StartStage(ETutorialStage.OUTRO);
		while (_stageRunning)
		{
			yield return new WaitForSeconds(1f);
		}
	}

	private void BlockPlayerMovement()
	{
		GlobalTools.GetThirdPersonController().SetMovementBlocked(block: true);
	}

	private void UnblockPlayerMovement()
	{
		GlobalTools.GetThirdPersonController().SetMovementBlocked(block: false);
	}

	private void StartStage(ETutorialStage stage)
	{
		_stage = stage;
		_stageRunning = true;
		ScavengeTutorialState texts = _tutorialTexts.GetTexts(_stage);
		switch (_stage)
		{
		case ETutorialStage.INTRO:
			StartCoroutine(ShowTexts(texts.Texts, showChar: true, hideCharAfter: false, null, EndStage));
			break;
		case ETutorialStage.MOVE1:
			StartCoroutine(ShowTexts(texts.Texts, showChar: false, hideCharAfter: false, null, UnblockPlayerMovement));
			StartCoroutine(MonitorGoToMarker(texts.Goal, "MoveMarker1", 15f, texts.Success, texts.Fail));
			break;
		case ETutorialStage.MOVE2:
			StartCoroutine(ShowTexts(texts.Texts, showChar: false, hideCharAfter: false, null, UnblockPlayerMovement));
			StartCoroutine(MonitorGoToMarker(texts.Goal, "MoveMarker2", 15f, texts.Success, texts.Fail));
			break;
		case ETutorialStage.EXPLORE:
			StartCoroutine(ShowTexts(texts.Texts, showChar: false, hideCharAfter: false, null, UnblockPlayerMovement));
			StartCoroutine(MonitorGoToMarker(texts.Goal, "ExploreMarker", 15f, texts.Success, texts.Fail));
			break;
		case ETutorialStage.GRAB_SON:
			GlobalTools.GetPlayerInteraction().EnableInteraction(enableGrab: true, enableDrop: false, showGrabLimit: false);
			_gameFlow.EnableInteraction();
			StartCoroutine(ShowTexts(texts.Texts, showChar: false, hideCharAfter: false, null, UnblockPlayerMovement));
			StartCoroutine(MonitorGrabItem(texts.Goal, _son, 15f, texts.Success, texts.Fail));
			break;
		case ETutorialStage.GRAB_FOOD:
			StartCoroutine(ShowTexts(texts.Texts, showChar: false, hideCharAfter: false, null, UnblockPlayerMovement));
			StartCoroutine(MonitorGrabItem(texts.Goal, null, 15f, texts.Success, texts.Fail, "tutorial_soup"));
			break;
		case ETutorialStage.GRAB_WATER:
			StartCoroutine(ShowTexts(texts.Texts, showChar: false, hideCharAfter: false, null, UnblockPlayerMovement));
			StartCoroutine(MonitorGrabItem(texts.Goal, null, 15f, texts.Success, texts.Fail, "tutorial_water"));
			break;
		case ETutorialStage.CAPACITY:
			StartCoroutine(ShowTexts(texts.Texts, showChar: false, hideCharAfter: false, null, EndStage));
			break;
		case ETutorialStage.SHELTER:
			GlobalTools.GetShelter().Flash();
			StartCoroutine(ShowTexts(texts.Texts, showChar: false, hideCharAfter: false, null, UnblockPlayerMovement));
			StartCoroutine(MonitorGoToMarker(texts.Goal, "ShelterMarker", 15f, texts.Success, texts.Fail));
			break;
		case ETutorialStage.DROP:
			StartCoroutine(ShowTexts(texts.Texts, showChar: false, hideCharAfter: false, null, UnblockPlayerMovement));
			GlobalTools.GetPlayerInteraction().EnableInteraction(enableGrab: true, enableDrop: true, showGrabLimit: false);
			_gameFlow.OpenShelter();
			StartCoroutine(MonitorDrop(texts.Goal, -1, new ScavengeItem[3] { _food, _water, _son }, 15f, texts.Success, texts.Fail));
			break;
		case ETutorialStage.SPECIALS:
			StartCoroutine(ShowTexts(texts.Texts, showChar: false, hideCharAfter: false, null, UnblockPlayerMovement));
			StartCoroutine(MonitorGrabItem(texts.Goal, _radio, 15f, texts.Success, texts.Fail));
			break;
		case ETutorialStage.ANNOUNCEMENTS:
		{
			StartCoroutine(ShowTexts(texts.Texts, showChar: false, hideCharAfter: false, null, UnblockPlayerMovement));
			ScavengeItemController scavengeItemController = _gameFlow.ActivateAnnouncer(1, activate: true, 9999f, "suitcase");
			StartCoroutine(MonitorGrabItem(texts.Goal, (scavengeItemController == null) ? null : scavengeItemController.ScavengeItem, 15f, texts.Success, texts.Fail));
			break;
		}
		case ETutorialStage.DROP_ANNOUNCED:
			_gameFlow.ActivateAnnouncer(1, activate: false);
			StartCoroutine(ShowTexts(texts.Texts, showChar: false, hideCharAfter: false, null, UnblockPlayerMovement));
			StartCoroutine(MonitorDrop(texts.Goal, 5, null, 15f, texts.Success, texts.Fail));
			break;
		case ETutorialStage.TIMED_SCAVENGE:
			GameSessionData.Instance.Setup.PrepareTime = 0;
			GameSessionData.Instance.Setup.GameTime = 60;
			ShowGoalText(texts.Goal, show: true);
			StartCoroutine(ShowTexts(texts.Texts, showChar: false, hideCharAfter: false, null, StartMockScavenge));
			break;
		case ETutorialStage.OUTRO:
			StartCoroutine(ShowTexts(texts.Texts, showChar: false, hideCharAfter: false, null, EndTutorial));
			break;
		}
	}

	private IEnumerator MonitorMockScavenge()
	{
		Vector3 playerPos = GlobalTools.GetPlayer().transform.position;
		_gameFlow.SurvivalTransferManager.GetCurrentInventory();
		bool collectionOk = false;
		bool checkCollection = true;
		bool collectionReminder = false;
		while (_gameFlow.TheWatch.TimeLeft > 0)
		{
			if (checkCollection)
			{
				collectionOk = _gameFlow.SurvivalTransferManager.GetCurrentItemsCount() >= 9;
				checkCollection = !collectionOk;
				if (collectionOk)
				{
					StartCoroutine(ShowTexts(TIMED_SCAVENGE_COLLECT_SUCCESS_TXT, showChar: false, hideCharAfter: false));
				}
				else if (!collectionReminder && _gameFlow.TheWatch.TimeLeft < 30)
				{
					collectionReminder = true;
					StartCoroutine(ShowTexts(TIMED_SCAVENGE_COLLECT_FAIL_TXT, showChar: false, hideCharAfter: false));
				}
			}
			yield return new WaitForSeconds(1f);
		}
		BlockPlayerMovement();
		yield return new WaitForSeconds(2f);
		if (collectionOk && GlobalTools.GetPlayerInteraction().IsPlayerNearShelter())
		{
			EndStage();
			yield break;
		}
		bool ready = false;
		StartCoroutine(ShowTexts(_tutorialTexts.GetTexts(ETutorialStage.TIMED_SCAVENGE).Fail, showChar: false, hideCharAfter: false, null, delegate
		{
			ready = true;
		}));
		while (!ready)
		{
			yield return new WaitForSeconds(0.5f);
		}
		yield return _gameFlow.DoTutorialResetStart();
		GlobalTools.GetPlayer().transform.position = playerPos;
		for (int i = 0; i < _scavengeItemsController.Length; i++)
		{
			bool flag = true;
			for (int j = 0; j < _excludedItems.Length; j++)
			{
				if (_excludedItems[j] == _scavengeItemsController[i])
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				_scavengeItemsController[i].gameObject.SetActive(value: true);
			}
		}
		ScavengeItemController[] array = Object.FindObjectsOfType<ScavengeItemController>();
		for (int k = 0; k < array.Length; k++)
		{
			array[k].CanBePickedUp = false;
			array[k].Highlight(on: false);
		}
		_gameFlow.SurvivalTransferManager.ResetItems(new List<ScavengeItem> { _son, _radio, _food, _water }, new int[4] { 1, 1, 1, 1 });
		_gameFlow.HandsController.Clear();
		_gameFlow.TheWatch.ClockController.ResetRedFill();
		yield return _gameFlow.DoTutorialResetEnd();
		yield return new WaitForSeconds(2.5f);
		StartStage(ETutorialStage.TIMED_SCAVENGE);
	}

	private void StartMockScavenge()
	{
		GlobalTools.GetPlayerInteraction().EnableInteraction(enableGrab: true, enableDrop: true, showGrabLimit: true);
		UnblockPlayerMovement();
		_gameFlow.StartGame();
		ScavengeItemController[] array = Object.FindObjectsOfType<ScavengeItemController>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].CanBePickedUp = true;
			array[i].Highlight(on: true);
		}
		StartCoroutine(MonitorMockScavenge());
	}

	private void ShowGoalText(LocalizedString text, bool show)
	{
		GlobalTools.GetController<GameFlow>().ShowGoal(show, show ? text.ToString() : null);
	}

	private IEnumerator MonitorGrabItem(LocalizedString goalText, ScavengeItem item, float timeout, List<LocalizedString> successTexts, List<LocalizedString> failTexts, string specificItemOverride = null)
	{
		ShowGoalText(goalText, show: true);
		bool flag = !string.IsNullOrEmpty(specificItemOverride);
		bool flag2 = item != null;
		if (flag || flag2)
		{
			if (flag2)
			{
				ScavengeItemController[] array = Object.FindObjectsOfType<ScavengeItemController>();
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].ScavengeItem == item)
					{
						SetTarget(array[i].gameObject);
						break;
					}
				}
			}
			else if (flag)
			{
				SetTarget(GameObject.Find(specificItemOverride));
			}
			ScavengeItemController scavengeItemController = _target.GetComponent<ScavengeItemController>();
			scavengeItemController.CanBePickedUp = true;
			scavengeItemController.Highlight(on: true);
			float time = 0f;
			while (_gameFlow.HandsController.LastScavengeItemAdded != scavengeItemController.ScavengeItem)
			{
				yield return new WaitForSeconds(1f);
				if (!_showingText)
				{
					time += 1f;
					if (time > timeout)
					{
						time = 0f;
						StartCoroutine(ShowTexts(failTexts, showChar: false, hideCharAfter: false));
					}
				}
			}
			StartCoroutine(ShowTexts(successTexts, showChar: false, hideCharAfter: false, null, EndStage));
		}
		ShowGoalText(goalText, show: false);
	}

	private IEnumerator MonitorDrop(LocalizedString goalText, int itemCount, ScavengeItem[] items, float timeout, List<LocalizedString> successTexts, List<LocalizedString> failTexts)
	{
		ShowGoalText(goalText, show: true);
		if ((items != null && items.Length != 0) || itemCount > 0)
		{
			float time = 0f;
			bool flag = false;
			while (!flag)
			{
				yield return new WaitForSeconds(1f);
				flag = true;
				if (items != null)
				{
					for (int i = 0; i < items.Length; i++)
					{
						if (!_gameFlow.SurvivalTransferManager.GetCurrentInventory().Contains(items[i]))
						{
							flag = false;
							break;
						}
					}
				}
				else
				{
					flag = _gameFlow.SurvivalTransferManager.GetCurrentInventory().Count >= itemCount;
				}
				if (!_showingText)
				{
					time += 1f;
					if (time > timeout)
					{
						time = 0f;
						StartCoroutine(ShowTexts(failTexts, showChar: false, hideCharAfter: false));
					}
				}
			}
			StartCoroutine(ShowTexts(successTexts, showChar: false, hideCharAfter: false, null, EndStage));
		}
		ShowGoalText(goalText, show: false);
	}

	private IEnumerator MonitorGoToMarker(LocalizedString goalText, string markerName, float timeout, List<LocalizedString> successTexts, List<LocalizedString> failTexts)
	{
		ShowGoalText(goalText, show: true);
		if (!string.IsNullOrEmpty(markerName))
		{
			SetTarget(markerName);
			Marker marker = _target.GetComponent<Marker>();
			marker.Show(show: true);
			yield return new WaitForSeconds(0.5f);
			marker.Animate();
			float time = 0f;
			GameObject player = GlobalTools.GetPlayer();
			while (marker.CurrentUser != player)
			{
				yield return new WaitForSeconds(1f);
				if (!_showingText)
				{
					time += 1f;
					if (time > timeout)
					{
						time = 0f;
						StartCoroutine(ShowTexts(failTexts, showChar: false, hideCharAfter: false));
					}
				}
			}
			marker.Show(show: false);
			StartCoroutine(ShowTexts(successTexts, showChar: false, hideCharAfter: false, null, EndStage));
		}
		ShowGoalText(goalText, show: false);
	}

	private void EndStage()
	{
		_stageRunning = false;
	}

	private void EndTutorial()
	{
		_gameFlow.EndLevel();
	}

	private void SetTarget(string name)
	{
		GameObject gameObject = GameObject.Find(name);
		if (gameObject != null)
		{
			_target = gameObject;
		}
	}

	private void SetTarget(GameObject obj)
	{
		_target = obj;
	}

	private void ShowTarget()
	{
		Show(show: true, _target);
	}

	private void HideTarget()
	{
		Show(show: false, _target);
	}

	private void Show(bool show, GameObject obj)
	{
		if (obj != null && obj.GetComponent<Renderer>() != null)
		{
			obj.GetComponent<Renderer>().enabled = show;
		}
	}

	private IEnumerator TestLocation(GameObject target, Vector3 destination, float distance, Progress f)
	{
		bool reachedTarget = false;
		while (!reachedTarget)
		{
			yield return new WaitForSeconds(1f);
			if (Vector3.Distance(target.transform.position, destination) <= distance)
			{
				reachedTarget = true;
			}
		}
		f?.Invoke();
	}

	private IEnumerator ShowTexts(List<LocalizedString> texts, bool showChar = true, bool hideCharAfter = true, Progress startMethod = null, Progress endMethod = null, bool playSounds = false)
	{
		if (texts == null)
		{
			yield break;
		}
		while (_showingText)
		{
			yield return new WaitForSeconds(1f);
		}
		_showingText = true;
		startMethod?.Invoke();
		if (showChar)
		{
			_gameFlow.EnableGmanChar(enable: true);
			yield return new WaitForSeconds(1f);
		}
		EventInstance eventInstance = default(EventInstance);
		if (playSounds)
		{
			eventInstance = AudioManager.PlaySoundAndReturnInstance(_broadcastSoundName);
		}
		float readTimePerSymbol = _readTimePerSymbol * ((LocalizationManager.CurrentLanguage == "Chinease" || LocalizationManager.CurrentLanguage == "Japanese") ? 2.5f : 1f);
		int i = 0;
		while (i < texts.Count)
		{
			int fastForwardPrompt = 2;
			string text = texts[i];
			float textTimeout = Time.time + (float)text.Length * readTimePerSymbol;
			_gameFlow.EnableGmanText(enable: true, i == 0, text);
			int num;
			while (textTimeout > Time.time)
			{
				yield return new WaitForSeconds(0.25f);
				if (_player.GetButton(4))
				{
					num = fastForwardPrompt - 1;
					fastForwardPrompt = num;
					if (fastForwardPrompt <= 0)
					{
						break;
					}
				}
				else
				{
					fastForwardPrompt = 2;
				}
			}
			num = i + 1;
			i = num;
		}
		_gameFlow.EnableGmanText(enable: false, talk: true, string.Empty);
		if (playSounds && eventInstance.isValid())
		{
			AudioManager.StopSound(eventInstance, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (hideCharAfter)
		{
			yield return new WaitForSeconds(1f);
			_gameFlow.EnableGmanChar(enable: false);
		}
		yield return new WaitForSeconds(1f);
		endMethod?.Invoke();
		_showingText = false;
	}
}
