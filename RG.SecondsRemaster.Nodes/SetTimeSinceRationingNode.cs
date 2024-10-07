using System;
using NodeEditorFramework;
using RG.Parsecs.EndGameEditor;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.NodeEditor;
using RG.SecondsRemaster.Core;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Supplies Nodes/Consumables/Set Time Since Rationing Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(Goal),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(EndGameCanvas),
	typeof(ConditionEvent)
})]
public class SetTimeSinceRationingNode : ResourceNode
{
	public const string ID = "EE_SetTimeSinceRationingNode";

	private const string NODE_NAME = "Set Time Since Rationing";

	private const string INPUT_FLOW_NAME = "In";

	private const string INPUT_CHARACTER_NAME = "Character";

	private const string INPUT_CONSUMABLE_NAME = "Consumable Object";

	private const string INPUT_LAST_RATIONING_NAME = "Last Rationing";

	private const string OUTPUT_FLOW_NAME = "Out";

	private const int INPUT_FLOW_INDEX = 0;

	private const int INPUT_CHARACTER_INDEX = 1;

	private const int INPUT_CONSUMABLE_INDEX = 2;

	private const int INPUT_LAST_RATIONING_INDEX = 3;

	private const int OUTPUT_FLOW_INDEX = 0;

	[SerializeField]
	private SecondsCharacter _character;

	[SerializeField]
	private SecondsConsumableRemedium _consumable;

	[SerializeField]
	private int _days;

	public override string GetID => "EE_SetTimeSinceRationingNode";

	public override Node Create(Vector2 pos)
	{
		SetTimeSinceRationingNode setTimeSinceRationingNode = ScriptableObject.CreateInstance<SetTimeSinceRationingNode>();
		setTimeSinceRationingNode.rect = new Rect(pos.x, pos.y, 200f, 100f);
		setTimeSinceRationingNode.name = "Set Time Since Rationing";
		setTimeSinceRationingNode.CreateMutliInput("In", "Flow");
		setTimeSinceRationingNode.CreateInput("Character", "Character");
		setTimeSinceRationingNode.CreateInput("Consumable Object", "ConsumableRemedium");
		setTimeSinceRationingNode.CreateInput("Last Rationing", "Int");
		setTimeSinceRationingNode.CreateOutput("Out", "Flow");
		return setTimeSinceRationingNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		SetTimeSinceRationingNode obj = (SetTimeSinceRationingNode)Create(rect.position + new Vector2(20f, 20f));
		obj._character = _character;
		obj._consumable = _consumable;
		obj._days = _days;
		return obj;
	}

	protected override void NodeEnable()
	{
	}

	protected override void OnNodeValidate()
	{
	}

	protected override void NodeGUI()
	{
	}

	public override void Execute(NodeCanvas canvas)
	{
		GetInputValue(Inputs[1], ref _character, canvas);
		GetInputValue(Inputs[2], ref _consumable, canvas);
		GetInputValue(Inputs[3], ref _days, canvas);
		SecondsRationingManager.Instance.TimeRationing.SetLastRationingTime(_consumable, _character, _days);
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
