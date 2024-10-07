using System;
using NodeEditorFramework;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Supplies Nodes/Items/Add Item Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent)
})]
public class AddItemVisualNode : ResourceNode
{
	public const string ID = "EE_AddItemVisualNode";

	[SerializeField]
	private IItem _item;

	[SerializeField]
	private bool _showStarlogGraphic = true;

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_ITEM_NAME = "Item";

	private const string INPUT_SHOW_GRAPHIC_NAME = "Show in Starlog";

	private const string OUTPUT_OUT_NAME = "Out";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_ITEM_INDEX = 1;

	private const int INPUT_SHOW_GRAPHIC_INDEX = 2;

	private const int OUTPUT_OUT_INDEX = 0;

	private const string OUTPUT_NOT_CONNECTED_MESSAGE = "Output is not connected";

	public override string GetID => "EE_AddItemVisualNode";

	public override Node Create(Vector2 pos)
	{
		AddItemVisualNode addItemVisualNode = ScriptableObject.CreateInstance<AddItemVisualNode>();
		addItemVisualNode.rect = new Rect(pos.x, pos.y, 180f, 100f);
		addItemVisualNode.name = "Add Item Visual";
		addItemVisualNode.CreateMutliInput("In", "Flow");
		addItemVisualNode.CreateInput("Item", "Item");
		addItemVisualNode.CreateInput("Show in Starlog", "Bool");
		addItemVisualNode.CreateOutput("Out", "Flow");
		return addItemVisualNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		AddItemVisualNode obj = (AddItemVisualNode)Create(rect.position + new Vector2(20f, 20f));
		obj._item = _item;
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
		GetInputValue(Inputs[1], ref _item, canvas);
		GetInputValue(Inputs[2], ref _showStarlogGraphic, canvas);
		bool flag = _item is ConsumableRemedium;
		if (CraftingManager.IsCraftingOngoing() && !flag)
		{
			if (CraftingManager.InterruptCraftingIfOngoing(_item))
			{
				switch (CraftingManager.GetInterruptedOperation())
				{
				case EPlannedCraftingAction.CRAFT:
				case EPlannedCraftingAction.RECYCLE:
				case EPlannedCraftingAction.REPAIR:
					if (_showStarlogGraphic)
					{
						DisplayAdditionTextIconContent();
					}
					_item.Add();
					break;
				case EPlannedCraftingAction.UPGRADE:
					if (_showStarlogGraphic)
					{
						DisplayAdditionTextIconContent();
					}
					break;
				}
			}
			else
			{
				if (!_item.BaseRuntimeData.IsAvailable && _showStarlogGraphic)
				{
					DisplayAdditionTextIconContent();
				}
				_item.Add();
			}
		}
		else
		{
			if (flag && _showStarlogGraphic)
			{
				DisplayAdditionTextIconContent();
			}
			else if ((!_item.BaseRuntimeData.IsAvailable || _item.IsDamaged()) && _showStarlogGraphic)
			{
				DisplayAdditionTextIconContent();
			}
			_item.Add();
		}
		ItemCollectedStatsEntry itemCollectedStatsEntry = new ItemCollectedStatsEntry();
		itemCollectedStatsEntry.FromExpedition = parentCanvas is ExpeditionEvent;
		itemCollectedStatsEntry.ItemId = _item.BaseStaticData.ItemId;
		StatsManager.Instance.AddItemCollectedStatsEntry(itemCollectedStatsEntry);
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}

	private void DisplayAdditionTextIconContent()
	{
		TextIconJournalContent content = new TextIconJournalContent(_item.BaseStaticData.IconTerm, 1, EventContentData.ETextIconContentType.ADDITION, 0);
		SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
	}
}
