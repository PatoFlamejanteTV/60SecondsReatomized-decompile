using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.NodeEditor;
using RG.Remaster.Survival;
using UnityEngine;

namespace RG.Remaster.EventEditor;

[Node(false, "Utility Nodes/Force Skin Use Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(Goal)
})]
public class ForceSkinUseNode : EventNode
{
	public const string ID = "EE_forceSkinUse";

	private const int FLOW_INPUT_INDEX = 0;

	private const int FLOW_OUTPUT_INDEX = 0;

	private const int DATALIST_INPUT_INDEX = 1;

	private const int SKIN_ID_INPUT_INDEX = 2;

	private const string NODE_NAME = "Force Skin Use";

	private const string FLOW_OUTPUT_NAME = "Out";

	private const string FLOW_INPUT_NAME = "In";

	private const string DATALIST_INPUT_NAME = "Skin Data List";

	private const string SKINID_INPUT_NAME = "Skin Id";

	[SerializeField]
	private SkinDataList _dataList;

	[SerializeField]
	private SkinId _skinId;

	public override string GetID => "EE_forceSkinUse";

	public override Node Create(Vector2 pos)
	{
		ForceSkinUseNode forceSkinUseNode = ScriptableObject.CreateInstance<ForceSkinUseNode>();
		forceSkinUseNode.rect = new Rect(pos.x, pos.y, 250f, 40f);
		forceSkinUseNode.name = "Force Skin Use";
		forceSkinUseNode.CreateMutliInput("In", "Flow");
		forceSkinUseNode.CreateInput("Skin Data List", "SkinDataList");
		forceSkinUseNode.CreateInput("Skin Id", "SkinId");
		forceSkinUseNode.CreateOutput("Out", "Flow");
		return forceSkinUseNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		ForceSkinUseNode obj = (ForceSkinUseNode)Create(rect.position + new Vector2(20f, 20f));
		obj._dataList = _dataList;
		obj._skinId = _skinId;
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
		GetInputValue(Inputs[2], ref _skinId, canvas);
		SkinManager.Instance.ForceSkinUse(_dataList, _skinId);
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
