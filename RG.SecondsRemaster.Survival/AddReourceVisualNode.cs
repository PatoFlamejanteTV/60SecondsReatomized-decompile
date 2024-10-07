using System;
using NodeEditorFramework;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Node(false, "Supplies Nodes/Resources/Add Resource Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent)
})]
public class AddReourceVisualNode : ResourceNode
{
	public const string ID = "EE_AddResourceVisualNode";

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_RESOURCE_NAME = "Resource";

	private const string INPUT_VALUE_NAME = "Value";

	private const string OUTPUT_OUT_NAME = "Out";

	private const string NODE_NAME = "Add Resource";

	private const string INPUT_SHOW_GRAPHIC_NAME = "Show in Starlog";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_RESOURCE_INDEX = 1;

	private const int INPUT_VALUE_INDEX = 2;

	private const int INPUT_SHOW_GRAPHIC_INDEX = 3;

	private const int OUTPUT_OUT_INDEX = 0;

	[SerializeField]
	private Resource _resource;

	[SerializeField]
	private int _value;

	[SerializeField]
	private bool _showStarlogGraphic = true;

	public override string GetID => "EE_AddResourceVisualNode";

	public override Node Create(Vector2 pos)
	{
		AddReourceVisualNode addReourceVisualNode = ScriptableObject.CreateInstance<AddReourceVisualNode>();
		addReourceVisualNode.rect = new Rect(pos.x, pos.y, 180f, 40f);
		addReourceVisualNode.name = "Add Resource";
		addReourceVisualNode.CreateInput("In", "Flow");
		addReourceVisualNode.CreateInput("Resource", "Resources");
		addReourceVisualNode.CreateInput("Value", "Int");
		addReourceVisualNode.CreateInput("Show in Starlog", "Bool");
		addReourceVisualNode.CreateOutput("Out", "Flow");
		return addReourceVisualNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		AddReourceVisualNode obj = (AddReourceVisualNode)Create(rect.position + new Vector2(20f, 20f));
		obj._resource = _resource;
		obj._value = _value;
		obj._showStarlogGraphic = _showStarlogGraphic;
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
		GetInputValue(Inputs[3], ref _showStarlogGraphic, canvas);
		int amount = Singleton<ItemManager>.Instance.GetPlayerResources().AddResource(_resource, _value);
		if (_showStarlogGraphic)
		{
			TextIconJournalContent content = new TextIconJournalContent(_resource.IconTerm, amount, EventContentData.ETextIconContentType.ADDITION, 0);
			SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
		}
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
