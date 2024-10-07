using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Supplies Nodes/Consumables/Remove Consumable Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent)
})]
public class RemoveConsumableNode : ResourceNode
{
	public const string ID = "EE_RemoveConsumableNode";

	private const string NODE_NAME = "Remove Consumable";

	private const string INPUT_FLOW_NAME = "In";

	private const string INPUT_AMOUNT_NAME = "Amount";

	private const string INPUT_SHOW_GRAPHIC_NAME = "Show in Starlog";

	private const string INPUT_CONSUMABLE_NAME = "Consumable object";

	private const string OUTPUT_FLOW_NAME = "Out";

	private const string OUTPUT_CURRENT_AMOUNT_NAME = "Current Amount";

	private const string OUTPUT_OPERATION_STATUS = "Is Operation Finished";

	private const int INPUT_FLOW_INDEX = 0;

	private const int INPUT_CONSUMABLE_INDEX = 1;

	private const int INPUT_AMOUNT_INDEX = 2;

	private const int INPUT_SHOW_GRAPHIC_INDEX = 3;

	private const int OUTPUT_FLOW_INDEX = 0;

	private const int OUTPUT_CURRENT_AMOUNT_INDEX = 1;

	private const int OUTPUT_OPERATION_INDEX = 2;

	[SerializeField]
	private float _amount;

	[SerializeField]
	private ConsumableRemedium _consumableRemedium;

	[SerializeField]
	private bool _showStarlogGraphic = true;

	private bool _isOperationSuccess;

	public override string GetID => "EE_RemoveConsumableNode";

	public override Node Create(Vector2 pos)
	{
		RemoveConsumableNode removeConsumableNode = ScriptableObject.CreateInstance<RemoveConsumableNode>();
		removeConsumableNode.rect = new Rect(pos.x, pos.y, 230f, 100f);
		removeConsumableNode.name = "Remove Consumable";
		removeConsumableNode.CreateMutliInput("In", "Flow");
		removeConsumableNode.CreateInput("Consumable object", "ConsumableRemedium");
		removeConsumableNode.CreateInput("Amount", "Float");
		removeConsumableNode.CreateInput("Show in Starlog", "Bool");
		removeConsumableNode.CreateOutput("Out", "Flow");
		removeConsumableNode.CreateOutput("Current Amount", "Float");
		removeConsumableNode.CreateOutput("Is Operation Finished", "Bool");
		return removeConsumableNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		RemoveConsumableNode obj = (RemoveConsumableNode)Create(rect.position + new Vector2(20f, 20f));
		obj._amount = _amount;
		obj._consumableRemedium = _consumableRemedium;
		obj._showStarlogGraphic = _showStarlogGraphic;
		return obj;
	}

	protected override void NodeEnable()
	{
	}

	protected override void OnNodeValidate()
	{
	}

	protected override void NodeGUI()
	{
	}

	public override void Execute(NodeCanvas canvas)
	{
		GetInputValue(Inputs[2], ref _amount, canvas);
		GetInputValue(Inputs[3], ref _showStarlogGraphic, canvas);
		GetInputValue(Inputs[1], ref _consumableRemedium, canvas);
		_isOperationSuccess = false;
		if (_consumableRemedium.RuntimeData.Amount > 0f)
		{
			_isOperationSuccess = true;
			float num = _consumableRemedium.RemoveAndGetRemovedAmount(_amount);
			if (_showStarlogGraphic)
			{
				TextIconJournalContent content = new TextIconJournalContent(_consumableRemedium.BaseStaticData.IconTerm, (int)num, EventContentData.ETextIconContentType.SUBTRACTION, 0);
				SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
			}
		}
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		return output switch
		{
			1 => CastValue<T>(_consumableRemedium.RuntimeData.Amount - _consumableRemedium.RuntimeData.PlannedConsumption), 
			2 => CastValue<T>(_isOperationSuccess), 
			_ => throw new NotExistingOutputException(GetID, output), 
		};
	}
}
