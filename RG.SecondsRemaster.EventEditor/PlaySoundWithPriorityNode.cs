using System;
using FMODUnity;
using NodeEditorFramework;
using RG.Parsecs.EndGameEditor;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using UnityEngine;

namespace RG.SecondsRemaster.EventEditor;

[Node(false, "Utility Nodes/Play Sound With Priority", new Type[]
{
	typeof(SurvivalEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ReportEvent),
	typeof(ExpeditionEvent),
	typeof(EndGameCanvas)
})]
public class PlaySoundWithPriorityNode : FlowNode
{
	private const int INTPUT_IN_INDEX = 0;

	private const int OUTTPUT_IN_INDEX = 0;

	private const string NODE_NAME = "Play Sound With Priority Node";

	[EventRef]
	[SerializeField]
	private string _event;

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

	[SerializeField]
	private CurrentSoundToPlay _currentSound;

	[SerializeField]
	private int _priority;

	private const string ERROR_INFO = "No event selected.";

	private const string WARNING_INFO = "No offeset time was set.";

	private const string VALUE_PITCH = "Pitch";

	private const string VALUE_PAN = "Pan";

	private const string EMPTY_EVENT_INFO = "No current sound selected";

	public const string ID = "EE_PlaySoundWithPriority";

	public override string GetID => "EE_PlaySoundWithPriority";

	public override Node Create(Vector2 pos)
	{
		PlaySoundWithPriorityNode playSoundWithPriorityNode = ScriptableObject.CreateInstance<PlaySoundWithPriorityNode>();
		playSoundWithPriorityNode.rect = new Rect(pos.x, pos.y, 340f, 300f);
		playSoundWithPriorityNode.name = "Play Sound With Priority Node";
		playSoundWithPriorityNode.CreateMutliInput("In", "Flow");
		playSoundWithPriorityNode.CreateOutput("Out", "Flow");
		return playSoundWithPriorityNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		PlaySoundWithPriorityNode obj = (PlaySoundWithPriorityNode)Create(rect.position + new Vector2(20f, 20f));
		obj._event = _event;
		obj._volume = _volume;
		obj._pan = _pan;
		obj._pitch = _pitch;
		obj._offset = _offset;
		obj._currentSound = _currentSound;
		obj._priority = _priority;
		return obj;
	}

	protected override void NodeEnable()
	{
	}

	protected override void NodeGUI()
	{
	}

	protected override void OnNodeValidate()
	{
	}

	public override void Execute(NodeCanvas canvas)
	{
		if (_priority >= _currentSound.EventPriority)
		{
			_currentSound.EventName = _event;
			_currentSound.EventPriority = _priority;
			_currentSound.Volume = _volume;
			_currentSound.Pan = _pan;
			_currentSound.Pitch = _pitch;
			_currentSound.Offset = _offset;
			_currentSound.OffsetCheck = _offsetCheck;
		}
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
