using System;
using NodeEditorFramework;
using RG.Parsecs.EndGameEditor;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Core;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Remaster/Supplies Nodes/Items/Break Item Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(EndGameCanvas),
	typeof(Goal),
	typeof(ConditionEvent)
})]
public class SRBreakItemNode : ResourceNode
{
	public const string ID = "EE_SRBreakItemNode";

	private const int INDEX_ITEM_INPUT = 0;

	private const int INDEX_OUTPUT_NAME = 0;

	private const int INDEX_OUTPUT_IS_AVAILABLE = 1;

	private const int INDEX_OUTPUT_IS_ON_EXPEDITION = 2;

	private const int INDEX_OUTPUT_ACTUAL_LEVEL = 3;

	private const int INDEX_OUTPUT_IS_MAX_LEVEL = 4;

	private const int INDEX_OUTPUT_DURABILITY = 5;

	private const int INDEX_OUTPUT_IS_DAMAGE = 6;

	private const int INDEX_OUTPUT_AMOUNT = 7;

	private const int INDEX_OUTPUT_ICON_TERM = 8;

	[SerializeField]
	private IItem _item;

	public override string GetID => "EE_SRBreakItemNode";

	public override Node Create(Vector2 pos)
	{
		SRBreakItemNode sRBreakItemNode = ScriptableObject.CreateInstance<SRBreakItemNode>();
		sRBreakItemNode.rect = new Rect(pos.x, pos.y, 180f, 155f);
		sRBreakItemNode.name = "Break Item Remaster";
		sRBreakItemNode.CreateInput("Item", "Item");
		sRBreakItemNode.CreateOutput("Name", "LocalizedString");
		sRBreakItemNode.CreateOutput("Is Available", "Bool");
		sRBreakItemNode.CreateOutput("Is On Expedition", "Bool");
		sRBreakItemNode.CreateOutput("Actual Level", "Int");
		sRBreakItemNode.CreateOutput("Is max level", "Bool");
		sRBreakItemNode.CreateOutput("Durability", "Int");
		sRBreakItemNode.CreateOutput("Is damage", "Bool");
		sRBreakItemNode.CreateOutput("Amount", "Int");
		sRBreakItemNode.CreateOutput("Icon Term", "LocalizedString");
		return sRBreakItemNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		SRBreakItemNode obj = (SRBreakItemNode)Create(rect.position + new Vector2(20f, 20f));
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

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		GetInputValue(Inputs[0], ref _item, canvas);
		switch (output)
		{
		case 0:
			return CastValue<T>(_item.BaseStaticData.Name);
		case 1:
			return CastValue<T>(_item.BaseRuntimeData.IsAvailable);
		case 2:
			return CastValue<T>(_item.BaseRuntimeData.IsOnExpedition);
		case 3:
			return CastValue<T>(_item.BaseRuntimeData.Level);
		case 4:
			return CastValue<T>(_item.IsMaxLevel());
		case 5:
			if (_item is Item)
			{
				return CastValue<T>(((Item)_item).RuntimeData.Durability);
			}
			return CastValue<T>(1);
		case 6:
			if (_item is Item)
			{
				return CastValue<T>(((Item)_item).RuntimeData.IsDamaged);
			}
			if (_item is SecondsRemedium)
			{
				return CastValue<T>(((SecondsRemedium)_item).SecondsRemediumRuntimeData.IsDamaged);
			}
			return CastValue<T>(false);
		case 7:
			if (_item is ConsumableRemedium)
			{
				ConsumableRemedium consumableRemedium = (ConsumableRemedium)_item;
				return CastValue<T>(consumableRemedium.RuntimeData.Amount - consumableRemedium.RuntimeData.PlannedConsumption);
			}
			return CastValue<T>(1);
		case 8:
			return CastValue<T>(_item.BaseStaticData.IconTerm);
		default:
			throw new NotExistingOutputException("EE_SRBreakItemNode", output);
		}
	}
}
