using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(true, "Legacy/Use Item Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent)
})]
public class UseItemNode : ResourceNode
{
	public const string ID = "EE_UseItemNode";

	[SerializeField]
	private IItem _item;

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_ITEM_NAME = "Item";

	private const string OUTPUT_OUT_NAME = "Out";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_ITEM_INDEX = 1;

	private const int OUTPUT_OUT_INDEX = 0;

	private const string OUTPUT_NOT_CONNECTED_MESSAGE = "Output is not connected";

	public override string GetID => "EE_UseItemNode";

	public override Node Create(Vector2 pos)
	{
		UseItemNode useItemNode = ScriptableObject.CreateInstance<UseItemNode>();
		useItemNode.rect = new Rect(pos.x, pos.y, 180f, 130f);
		useItemNode.name = "Use Item ";
		useItemNode.CreateMutliInput("In", "Flow");
		useItemNode.CreateInput("Item", "Item");
		useItemNode.CreateOutput("Out", "Flow");
		return useItemNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		UseItemNode obj = (UseItemNode)Create(rect.position + new Vector2(20f, 20f));
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
		if (!_item.IsDamaged())
		{
			_ = _item;
			bool isAvailable = _item.BaseRuntimeData.IsAvailable;
			_item.Use();
			if (_item.IsDamaged() || (!_item.BaseRuntimeData.IsAvailable && isAvailable))
			{
				TextIconJournalContent content = new TextIconJournalContent(_item.BaseStaticData.IconTerm, 1, EventContentData.ETextIconContentType.SUBTRACTION, 0);
				SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
			}
		}
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
