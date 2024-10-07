using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Core;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Node(false, "Supplies Nodes/Items/Fix Item Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent)
})]
public class FixItemNode : ResourceNode
{
	public const string ID = "EE_FixItemNode";

	[SerializeField]
	private Item _item;

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_ITEM_NAME = "Item";

	private const string OUTPUT_OUT_NAME = "Out";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_ITEM_INDEX = 1;

	private const int OUTPUT_OUT_INDEX = 0;

	private const string OUTPUT_NOT_CONNECTED_MESSAGE = "Output is not connected";

	public override string GetID => "EE_FixItemNode";

	public override Node Create(Vector2 pos)
	{
		FixItemNode fixItemNode = ScriptableObject.CreateInstance<FixItemNode>();
		fixItemNode.rect = new Rect(pos.x, pos.y, 180f, 130f);
		fixItemNode.name = "Fix Item";
		fixItemNode.CreateMutliInput("In", "Flow");
		fixItemNode.CreateInput("Item", "Item");
		fixItemNode.CreateOutput("Out", "Flow");
		return fixItemNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		FixItemNode obj = (FixItemNode)Create(rect.position + new Vector2(20f, 20f));
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
		IItem currentValue = _item;
		GetInputValue(Inputs[1], ref currentValue, canvas);
		bool flag = false;
		bool flag2 = false;
		if (currentValue is Item)
		{
			Item obj = currentValue as Item;
			flag = obj.RuntimeData.IsDamaged;
			obj.Repair();
			flag2 = obj.RuntimeData.IsDamaged;
		}
		else if (currentValue is SecondsRemedium)
		{
			SecondsRemedium obj2 = currentValue as SecondsRemedium;
			flag = obj2.SecondsRemediumRuntimeData.IsDamaged;
			obj2.Repair();
			flag2 = obj2.SecondsRemediumRuntimeData.IsDamaged;
		}
		else
		{
			currentValue = null;
		}
		if (currentValue != null && !flag2 && flag)
		{
			TextIconJournalContent content = new TextIconJournalContent(currentValue.BaseStaticData.IconTerm, 1, EventContentData.ETextIconContentType.ADDITION, 0);
			SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
		}
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
