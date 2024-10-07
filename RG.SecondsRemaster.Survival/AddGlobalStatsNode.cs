using System;
using NodeEditorFramework;
using RG.Parsecs.Common;
using RG.Parsecs.EndGameEditor;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.NodeEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Node(false, "Global Stats/Add Global Stat Node", new Type[]
{
	typeof(Goal),
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(ExpeditionEvent),
	typeof(SystemStatusEvent),
	typeof(EndGameCanvas)
})]
public class AddGlobalStatsNode : EventNode
{
	public const string ID = "GE_AddGlobalStatsNode";

	public const string NODE_NAME = "Add global stat";

	private const int INPUT_FLOW_INDEX = 0;

	private const int INPUT_VALUE_INDEX = 1;

	private const int OUTPUT_FLOW_INDEX = 0;

	private const string INPUT_FLOW_NAME = "In";

	private const string OUTPUT_FLOW_NAME = "Out";

	private const string INPUT_STAT_VALUE = "Value";

	private const int MINIMAL_VALUE = 1;

	[SerializeField]
	private string _key;

	[SerializeField]
	private int _value = 1;

	public override string GetID => "GE_AddGlobalStatsNode";

	public override Node Create(Vector2 pos)
	{
		AddGlobalStatsNode addGlobalStatsNode = ScriptableObject.CreateInstance<AddGlobalStatsNode>();
		addGlobalStatsNode.rect = new Rect(pos.x, pos.y, 300f, 200f);
		addGlobalStatsNode.name = "Add global stat";
		addGlobalStatsNode.CreateMutliInput("In", "Flow");
		addGlobalStatsNode.CreateInput("Value", "Int");
		addGlobalStatsNode.CreateOutput("Out", "Flow");
		return addGlobalStatsNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		AddGlobalStatsNode obj = (AddGlobalStatsNode)Create(pos + new Vector2(20f, 20f));
		obj._key = _key;
		obj._value = _value;
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
		GetInputValue(Inputs[1], ref _value, canvas);
		StatsManager.Instance.AddGlobalData(_key, _value);
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
