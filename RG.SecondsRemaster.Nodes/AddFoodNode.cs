using System;
using NodeEditorFramework;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(true, "Legacy/Add Food Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent)
})]
public class AddFoodNode : ResourceNode
{
	public const string ID = "EE_AddFoodNode";

	private const string NODE_NAME = "Add Food Node";

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_AMOUNT_NAME = "Amount";

	private const string OUTPUT_OUT_NAME = "Out";

	private const string OUTPUT_CURRENT_AMOUNT_NAME = "Current Amount";

	private const string FOOD_ID = "item_food";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_AMOUNT_INDEX = 1;

	private const int OUTPUT_OUT_INDEX = 0;

	private const int OUTPUT_CURRENT_AMOUNT_INDEX = 1;

	[SerializeField]
	private float _amount;

	public override string GetID => "EE_AddFoodNode";

	public override Node Create(Vector2 pos)
	{
		AddFoodNode addFoodNode = ScriptableObject.CreateInstance<AddFoodNode>();
		addFoodNode.rect = new Rect(pos.x, pos.y, 180f, 80f);
		addFoodNode.name = "Add Food Node";
		addFoodNode.CreateMutliInput("In", "Flow");
		addFoodNode.CreateInput("Amount", "Float");
		addFoodNode.CreateOutput("Out", "Flow");
		addFoodNode.CreateOutput("Current Amount", "Float");
		return addFoodNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		AddFoodNode obj = (AddFoodNode)Create(rect.position + new Vector2(20f, 20f));
		obj._amount = _amount;
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
		TextIconJournalContent content = new TextIconJournalContent(consumableRemedium.BaseStaticData.IconTerm, (int)currentValue, EventContentData.ETextIconContentType.ADDITION, 0);
		SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
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
