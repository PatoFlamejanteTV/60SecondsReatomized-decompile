using System;
using NodeEditorFramework;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(true, "Legacy/Add Resource Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent)
})]
public class AddResourceNode : ResourceNode
{
	public const string ID = "EE_AddResourceNode";

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_RESOURCE_NAME = "Resource";

	private const string INPUT_VALUE_NAME = "Value";

	private const string OUTPUT_OUT_NAME = "Out";

	private const string NODE_NAME = "Add Resource";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_RESOURCE_INDEX = 1;

	private const int INPUT_VALUE_INDEX = 2;

	private const int OUTPUT_OUT_INDEX = 0;

	[SerializeField]
	private Resource _resource;

	[SerializeField]
	private int _value;

	public override string GetID => "EE_AddResourceNode";

	public override Node Create(Vector2 pos)
	{
		AddResourceNode addResourceNode = ScriptableObject.CreateInstance<AddResourceNode>();
		addResourceNode.rect = new Rect(pos.x, pos.y, 180f, 40f);
		addResourceNode.name = "Add Resource";
		addResourceNode.CreateInput("In", "Flow");
		addResourceNode.CreateInput("Resource", "Resources");
		addResourceNode.CreateInput("Value", "Int");
		addResourceNode.CreateOutput("Out", "Flow");
		return addResourceNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		AddResourceNode obj = (AddResourceNode)Create(rect.position + new Vector2(20f, 20f));
		obj._resource = _resource;
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
		GetInputValue(Inputs[1], ref _resource, canvas);
		GetInputValue(Inputs[2], ref _value, canvas);
		int amount = Singleton<ItemManager>.Instance.GetPlayerResources().AddResource(_resource, _value);
		TextIconJournalContent content = new TextIconJournalContent(_resource.IconTerm, amount, EventContentData.ETextIconContentType.ADDITION, 0);
		SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
