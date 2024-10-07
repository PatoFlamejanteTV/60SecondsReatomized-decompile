using System;
using NodeEditorFramework;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(true, "Legacy/Food/Remove Food Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(Goal)
})]
public class RemoveFoodVisualNode : ResourceNode
{
	public const string ID = "EE_RemoveFoodVisualNode";

	private const string NODE_NAME = "Remove Food Node";

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_AMOUNT_NAME = "Amount";

	private const string OUTPUT_OUT_NAME = "Out";

	private const string OUTPUT_CURRENT_AMOUNT_NAME = "Current Amount";

	private const string OUTPUT_OPERATION_STATUS = "Is Operation Finished";

	private const string INPUT_SHOW_GRAPHIC_NAME = "Show in Starlog";

	private const string FOOD_ID = "item_food";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_AMOUNT_INDEX = 1;

	private const int INPUT_SHOW_GRAPHIC_INDEX = 2;

	private const int OUTPUT_OUT_INDEX = 0;

	private const int OUTPUT_CURRENT_AMOUNT_INDEX = 1;

	private const int OUTPUT_OPERATION_INDEX = 2;

	[SerializeField]
	private int _amount;

	[SerializeField]
	private bool _showStarlogGraphic = true;

	private bool _isOperationSuccess;

	public override string GetID => "EE_RemoveFoodVisualNode";

	public override Node Create(Vector2 pos)
	{
		RemoveFoodVisualNode removeFoodVisualNode = ScriptableObject.CreateInstance<RemoveFoodVisualNode>();
		removeFoodVisualNode.rect = new Rect(pos.x, pos.y, 180f, 80f);
		removeFoodVisualNode.name = "Remove Food Node";
		removeFoodVisualNode.CreateMutliInput("In", "Flow");
		removeFoodVisualNode.CreateInput("Amount", "Float");
		removeFoodVisualNode.CreateInput("Show in Starlog", "Bool");
		removeFoodVisualNode.CreateOutput("Out", "Flow");
		removeFoodVisualNode.CreateOutput("Current Amount", "Float");
		removeFoodVisualNode.CreateOutput("Is Operation Finished", "Bool");
		return removeFoodVisualNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		RemoveFoodVisualNode obj = (RemoveFoodVisualNode)Create(rect.position + new Vector2(20f, 20f));
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
		_isOperationSuccess = false;
		if (consumableRemedium.RuntimeData.Amount > 0f)
		{
			_isOperationSuccess = true;
			float num = consumableRemedium.RemoveAndGetRemovedAmount(currentValue);
			if (_showStarlogGraphic)
			{
				TextIconJournalContent content = new TextIconJournalContent(consumableRemedium.BaseStaticData.IconTerm, (int)num, EventContentData.ETextIconContentType.SUBTRACTION, 0);
				SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
			}
		}
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		switch (output)
		{
		case 1:
		{
			ConsumableRemedium consumableRemedium = (ConsumableRemedium)Singleton<ItemManager>.Instance.GetItem("item_food");
			return CastValue<T>(consumableRemedium.RuntimeData.Amount - consumableRemedium.RuntimeData.PlannedConsumption);
		}
		case 2:
			return CastValue<T>(_isOperationSuccess);
		default:
			throw new NotExistingOutputException(GetID, output);
		}
	}
}
