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

[Node(true, "Legacy/Remove Food Node V2", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(Goal)
})]
public class RemoveFoodVerTwoNode : ResourceNode
{
	public const string ID = "EE_RemoveFoodVerTwoNode";

	private const string NODE_NAME = "Remove Food Node";

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_AMOUNT_NAME = "Amount";

	private const string OUTPUT_OUT_NAME = "Out";

	private const string OUTPUT_CURRENT_AMOUNT_NAME = "Current Amount";

	private const string OUTPUT_OPERATION_STATUS = "Is Operation Finished";

	private const string FOOD_ID = "item_food";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_AMOUNT_INDEX = 1;

	private const int OUTPUT_OUT_INDEX = 0;

	private const int OUTPUT_CURRENT_AMOUNT_INDEX = 1;

	private const int OUTPUT_OPERATION_INDEX = 2;

	[SerializeField]
	private int _amount;

	private bool _isOperationSuccess;

	public override string GetID => "EE_RemoveFoodVerTwoNode";

	public override Node Create(Vector2 pos)
	{
		RemoveFoodVerTwoNode removeFoodVerTwoNode = ScriptableObject.CreateInstance<RemoveFoodVerTwoNode>();
		removeFoodVerTwoNode.rect = new Rect(pos.x, pos.y, 180f, 80f);
		removeFoodVerTwoNode.name = "Remove Food Node";
		removeFoodVerTwoNode.CreateMutliInput("In", "Flow");
		removeFoodVerTwoNode.CreateInput("Amount", "Float");
		removeFoodVerTwoNode.CreateOutput("Out", "Flow");
		removeFoodVerTwoNode.CreateOutput("Current Amount", "Float");
		removeFoodVerTwoNode.CreateOutput("Is Operation Finished", "Bool");
		return removeFoodVerTwoNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		RemoveFoodVerTwoNode obj = (RemoveFoodVerTwoNode)Create(rect.position + new Vector2(20f, 20f));
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
		GetInputValue(Inputs[1], ref _amount, canvas);
		ConsumableRemedium consumableRemedium = (ConsumableRemedium)Singleton<ItemManager>.Instance.GetItem("item_food");
		_isOperationSuccess = false;
		if (consumableRemedium.RuntimeData.Amount > 0f)
		{
			_isOperationSuccess = true;
			float num = consumableRemedium.RemoveAndGetRemovedAmount(_amount);
			TextIconJournalContent content = new TextIconJournalContent(consumableRemedium.BaseStaticData.IconTerm, (int)num, EventContentData.ETextIconContentType.SUBTRACTION, 0);
			SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
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
