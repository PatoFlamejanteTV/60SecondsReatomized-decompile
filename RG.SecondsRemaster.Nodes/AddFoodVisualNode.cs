using System;
using NodeEditorFramework;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(true, "Supplies Nodes/Food/Add Food Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent)
})]
public class AddFoodVisualNode : ResourceNode
{
	public const string ID = "EE_AddFoodVisualNode";

	private const string NODE_NAME = "Add Food Node";

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_AMOUNT_NAME = "Amount";

	private const string OUTPUT_OUT_NAME = "Out";

	private const string OUTPUT_CURRENT_AMOUNT_NAME = "Current Amount";

	private const string INPUT_SHOW_GRAPHIC_NAME = "Show in Starlog";

	private const string FOOD_ID = "item_food";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_AMOUNT_INDEX = 1;

	private const int INPUT_SHOW_GRAPHIC_INDEX = 2;

	private const int OUTPUT_OUT_INDEX = 0;

	private const int OUTPUT_CURRENT_AMOUNT_INDEX = 1;

	[SerializeField]
	private float _amount;

	[SerializeField]
	private bool _showStarlogGraphic = true;

	public override string GetID => "EE_AddFoodVisualNode";

	public override Node Create(Vector2 pos)
	{
		AddFoodVisualNode addFoodVisualNode = ScriptableObject.CreateInstance<AddFoodVisualNode>();
		addFoodVisualNode.rect = new Rect(pos.x, pos.y, 180f, 80f);
		addFoodVisualNode.name = "Add Food Node";
		addFoodVisualNode.CreateMutliInput("In", "Flow");
		addFoodVisualNode.CreateInput("Amount", "Float");
		addFoodVisualNode.CreateInput("Show in Starlog", "Bool");
		addFoodVisualNode.CreateOutput("Out", "Flow");
		addFoodVisualNode.CreateOutput("Current Amount", "Float");
		return addFoodVisualNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		AddFoodVisualNode obj = (AddFoodVisualNode)Create(rect.position + new Vector2(20f, 20f));
		obj._amount = _amount;
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
		float currentValue = Convert.ToSingle(_amount);
		GetInputValue(Inputs[1], ref currentValue, canvas);
		GetInputValue(Inputs[2], ref _showStarlogGraphic, canvas);
		ConsumableRemedium consumableRemedium = (ConsumableRemedium)Singleton<ItemManager>.Instance.GetItem("item_food");
		consumableRemedium.Add(currentValue);
		int num = Convert.ToInt32(currentValue);
		for (int i = 0; i < num; i++)
		{
			ItemCollectedStatsEntry itemCollectedStatsEntry = new ItemCollectedStatsEntry();
			itemCollectedStatsEntry.FromExpedition = parentCanvas is ExpeditionEvent;
			itemCollectedStatsEntry.ItemId = "item_food";
			StatsManager.Instance.AddItemCollectedStatsEntry(itemCollectedStatsEntry);
		}
		if (_showStarlogGraphic)
		{
			TextIconJournalContent content = new TextIconJournalContent(consumableRemedium.BaseStaticData.IconTerm, (int)currentValue, EventContentData.ETextIconContentType.ADDITION, 0);
			SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
		}
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		if (output != 1)
		{
			throw new NotExistingOutputException(GetID, output);
		}
		ConsumableRemedium consumableRemedium = (ConsumableRemedium)Singleton<ItemManager>.Instance.GetItem("item_food");
		return CastValue<T>(consumableRemedium.RuntimeData.Amount - consumableRemedium.RuntimeData.PlannedConsumption);
	}
}
