using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.NodeEditor;
using RG.Remaster.Survival;
using UnityEngine;

namespace RG.Remaster.EventEditor;

[Node(false, "Utility Nodes/Allow Skin Changes Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(Goal)
})]
public class AllowSkinChangesNode : EventNode
{
	public const string ID = "EE_allowSkinChanges";

	private const int FLOW_INPUT_INDEX = 0;

	private const int FLOW_OUTPUT_INDEX = 0;

	private const int DATALIST_INPUT_INDEX = 1;

	private const string NODE_NAME = "Allow Skin Changes";

	private const string FLOW_OUTPUT_NAME = "Out";

	private const string FLOW_INPUT_NAME = "In";

	private const string DATALIST_INPUT_NAME = "Skin Data List";

	[SerializeField]
	private SkinDataList _dataList;

	public override string GetID => "EE_allowSkinChanges";

	public override Node Create(Vector2 pos)
	{
		AllowSkinChangesNode allowSkinChangesNode = ScriptableObject.CreateInstance<AllowSkinChangesNode>();
		allowSkinChangesNode.rect = new Rect(pos.x, pos.y, 250f, 40f);
		allowSkinChangesNode.name = "Allow Skin Changes";
		allowSkinChangesNode.CreateMutliInput("In", "Flow");
		allowSkinChangesNode.CreateInput("Skin Data List", "SkinDataList");
		allowSkinChangesNode.CreateOutput("Out", "Flow");
		return allowSkinChangesNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		AllowSkinChangesNode obj = (AllowSkinChangesNode)Create(rect.position + new Vector2(20f, 20f));
		obj._dataList = _dataList;
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
		GetInputValue(Inputs[1], ref _dataList, canvas);
		SkinManager.Instance.AllowChangingSkins(_dataList);
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
