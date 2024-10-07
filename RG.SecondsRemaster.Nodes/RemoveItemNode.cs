using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(true, "Legacy/Remove Item Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent)
})]
public class RemoveItemNode : ResourceNode
{
	public const string ID = "EE_RemoveItemNode";

	[SerializeField]
	private IItem _item;

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_ITEM_NAME = "Item";

	private const string OUTPUT_OUT_NAME = "Out";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_ITEM_INDEX = 1;

	private const int OUTPUT_OUT_INDEX = 0;

	private const string OUTPUT_NOT_CONNECTED_MESSAGE = "Output is not connected";

	public override string GetID => "EE_RemoveItemNode";

	public override Node Create(Vector2 pos)
	{
		RemoveItemNode removeItemNode = ScriptableObject.CreateInstance<RemoveItemNode>();
		removeItemNode.rect = new Rect(pos.x, pos.y, 180f, 80f);
		removeItemNode.name = "Remove Item";
		removeItemNode.CreateMutliInput("In", "Flow");
		removeItemNode.CreateInput("Item", "Item");
		removeItemNode.CreateOutput("Out", "Flow");
		return removeItemNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		RemoveItemNode obj = (RemoveItemNode)Create(rect.position + new Vector2(20f, 20f));
		obj._item = _item;
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
		GetInputValue(Inputs[1], ref _item, canvas);
		if (_item.BaseRuntimeData.IsAvailable && !_item.IsDamaged())
		{
			TextIconJournalContent content = new TextIconJournalContent(_item.BaseStaticData.IconTerm, 1, EventContentData.ETextIconContentType.SUBTRACTION, 0);
			SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
		}
		_item.Remove();
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
