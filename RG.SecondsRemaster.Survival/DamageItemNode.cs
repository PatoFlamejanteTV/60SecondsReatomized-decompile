using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Core;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Node(true, "Supplies Nodes/Items/Damage Item Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent)
})]
public class DamageItemNode : ResourceNode
{
	public const string ID = "EE_DamageItemNode";

	[SerializeField]
	private Item _item;

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_ITEM_NAME = "Item";

	private const string OUTPUT_OUT_NAME = "Out";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_ITEM_INDEX = 1;

	private const int OUTPUT_OUT_INDEX = 0;

	private const string OUTPUT_NOT_CONNECTED_MESSAGE = "Output is not connected";

	public override string GetID => "EE_DamageItemNode";

	public override Node Create(Vector2 pos)
	{
		DamageItemNode damageItemNode = ScriptableObject.CreateInstance<DamageItemNode>();
		damageItemNode.rect = new Rect(pos.x, pos.y, 180f, 130f);
		damageItemNode.name = "Damage Item";
		damageItemNode.CreateMutliInput("In", "Flow");
		damageItemNode.CreateInput("Item", "Item");
		damageItemNode.CreateOutput("Out", "Flow");
		return damageItemNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		DamageItemNode obj = (DamageItemNode)Create(rect.position + new Vector2(20f, 20f));
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
		bool isAvailable = currentValue.BaseRuntimeData.IsAvailable;
		bool flag2 = false;
		if (currentValue is Item)
		{
			Item obj = currentValue as Item;
			flag2 = obj.IsDamaged();
			obj.SetDamage();
			flag = obj.IsDamaged();
		}
		else
		{
			if (!(currentValue is SecondsRemedium))
			{
				throw new UnityException("Cannot damage item: " + currentValue.BaseStaticData.ItemId);
			}
			SecondsRemedium obj2 = currentValue as SecondsRemedium;
			flag2 = obj2.IsDamaged();
			obj2.SetDamage();
			flag = obj2.IsDamaged();
		}
		if ((currentValue.BaseRuntimeData.IsAvailable && flag && !flag2) || (isAvailable && !currentValue.BaseRuntimeData.IsAvailable))
		{
			TextIconJournalContent content = new TextIconJournalContent(currentValue.BaseStaticData.IconTerm, 1, EventContentData.ETextIconContentType.SUBTRACTION, 0);
			SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
		}
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
