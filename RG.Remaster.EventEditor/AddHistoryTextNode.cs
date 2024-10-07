using System;
using I2.Loc;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.NodeEditor;
using RG.Remaster.Survival;
using UnityEngine;

namespace RG.Remaster.EventEditor;

[Node(false, "Text Nodes/Add History Text Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(ReportEvent),
	typeof(Goal)
})]
public class AddHistoryTextNode : MessageNode
{
	private const string NODE_NAME = "Add History Text";

	private const string INPUT_FLOW = "IN";

	private const string INPUT_TERM = "Term";

	private const string INPUT_USE_CURRENT_DAY_NAME = "Use Current Day";

	private const string INPUT_DAY = "Day";

	private const string OUTPUT_FLOW = "Out";

	private const int INPUT_FLOW_INDEX = 0;

	private const int INPUT_TERM_INDEX = 1;

	private const int INPUT_DECISION_INDEX = 2;

	private const int INPUT_DAY_INDEX = 3;

	private const int OUTPUT_FLOW_INDEX = 0;

	[SerializeField]
	private LocalizedString _term;

	[SerializeField]
	private bool _useCurrentDay = true;

	[SerializeField]
	private int _dayToSet;

	public const string ID = "EE_AddHistoryTextNode";

	public override string GetID => "EE_AddHistoryTextNode";

	public override Node Create(Vector2 pos)
	{
		AddHistoryTextNode addHistoryTextNode = ScriptableObject.CreateInstance<AddHistoryTextNode>();
		addHistoryTextNode.rect = new Rect(pos.x, pos.y, 300f, 105f);
		addHistoryTextNode.name = "Add History Text";
		addHistoryTextNode.CreateMutliInput("IN", "Flow");
		addHistoryTextNode.CreateInput("Term", "LocalizedString");
		addHistoryTextNode.CreateInput("Use Current Day", "Bool");
		addHistoryTextNode.CreateInput("Day", "Int");
		addHistoryTextNode.CreateOutput("Out", "Flow");
		_dayToSet = 0;
		return addHistoryTextNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		AddHistoryTextNode obj = (AddHistoryTextNode)Create(rect.position + new Vector2(20f, 20f));
		obj._term = _term;
		obj._dayToSet = _dayToSet;
		obj._useCurrentDay = _useCurrentDay;
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
		GetInputValue(Inputs[1], ref _term, canvas);
		GetInputValue(Inputs[3], ref _dayToSet, canvas);
		GetInputValue(Inputs[2], ref _useCurrentDay, canvas);
		SimpleHistoryManager.Instance.AddEntry(_term, _dayToSet, _useCurrentDay);
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
