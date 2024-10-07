using System;
using NodeEditorFramework;
using RG.Parsecs.EndGameEditor;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Core;
using UnityEngine;

namespace RG.Remaster.EventEditor;

[Node(false, "Supplies Nodes/Items/Is Item Damaged Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(EndGameCanvas),
	typeof(ConditionEvent)
})]
public class IsItemDamagedNode : ResourceNode
{
	public const string ID = "EE_IsDamagedItemNode";

	[SerializeField]
	private Item _item;

	private const int INPUT_ITEM_INDEX = 0;

	private const int OUTPUT_IS_AVAILABLE_INDEX = 0;

	private const string INPUT_ITEM_NAME = "Item";

	private const string OUTPUT_IS_AVAILABLE_NAME = "Is Damaged";

	public override string GetID => "EE_IsDamagedItemNode";

	public override Node Create(Vector2 pos)
	{
		IsItemDamagedNode isItemDamagedNode = ScriptableObject.CreateInstance<IsItemDamagedNode>();
		isItemDamagedNode.rect = new Rect(pos.x, pos.y, 180f, 120f);
		isItemDamagedNode.name = "Is Item Damaged";
		isItemDamagedNode.CreateInput("Item", "Item");
		isItemDamagedNode.CreateOutput("Is Damaged", "Bool");
		return isItemDamagedNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		IsItemDamagedNode obj = (IsItemDamagedNode)Create(rect.position + new Vector2(20f, 20f));
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
		if (output != 0)
		{
			throw new NotExistingOutputException(GetID, output);
		}
		IItem currentValue = _item;
		GetInputValue(Inputs[0], ref currentValue, canvas);
		if (currentValue is Item)
		{
			return CastValue<T>(((Item)currentValue).RuntimeData.IsDamaged);
		}
		if (currentValue is SecondsRemedium)
		{
			return CastValue<T>(((SecondsRemedium)currentValue).SecondsRemediumRuntimeData.IsDamaged);
		}
		throw new UnityException("Non damageable item in IsItemDamageNode");
	}
}
