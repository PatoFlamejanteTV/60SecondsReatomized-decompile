using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Core;
using UnityEngine;

namespace RG.SecondsRemaster.Survival;

[Node(false, "Supplies Nodes/Items/Damage Item Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent)
})]
public class DamageItemNodeV2 : ResourceNode
{
	public const string ID = "EE_DamageItemNodeV2";

	[SerializeField]
	private Item _item;

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_ITEM_NAME = "Item";

	private const string INPUT_SHOW_IN_STARLOG_NAME = "Show In Starlog";

	private const string OUTPUT_OUT_NAME = "Out";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_ITEM_INDEX = 1;

	private const int INPUT_SHOW_IN_STARLOG_INDEX = 2;

	private const int OUTPUT_OUT_INDEX = 0;

	private const string OUTPUT_NOT_CONNECTED_MESSAGE = "Output is not connected";

	[SerializeField]
	private bool _showInStarlog = true;

	public override string GetID => "EE_DamageItemNodeV2";

	public override Node Create(Vector2 pos)
	{
		DamageItemNodeV2 damageItemNodeV = ScriptableObject.CreateInstance<DamageItemNodeV2>();
		damageItemNodeV.rect = new Rect(pos.x, pos.y, 180f, 130f);
		damageItemNodeV.name = "Damage Item";
		damageItemNodeV.CreateMutliInput("In", "Flow");
		damageItemNodeV.CreateInput("Item", "Item");
		damageItemNodeV.CreateInput("Show In Starlog", "Bool");
		damageItemNodeV.CreateOutput("Out", "Flow");
		return damageItemNodeV;
	}

	public override Node Duplicate(Vector2 pos)
	{
		DamageItemNodeV2 obj = (DamageItemNodeV2)Create(rect.position + new Vector2(20f, 20f));
		obj._item = _item;
		obj._showInStarlog = _showInStarlog;
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
		GetInputValue(Inputs[2], ref _showInStarlog, canvas);
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
		if (_showInStarlog && ((currentValue.BaseRuntimeData.IsAvailable && flag && !flag2) || (isAvailable && !currentValue.BaseRuntimeData.IsAvailable)))
		{
			TextIconJournalContent content = new TextIconJournalContent(currentValue.BaseStaticData.IconTerm, 1, EventContentData.ETextIconContentType.SUBTRACTION, 0);
			SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
		}
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
