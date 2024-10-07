using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Supplies Nodes/Consumables/Add Consumable Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent)
})]
public class AddConsumableNode : ResourceNode
{
	public const string ID = "EE_AddConsumableNode";

	private const string NODE_NAME = "Add Consumable";

	private const string INPUT_FLOW_NAME = "In";

	private const string INPUT_AMOUNT_NAME = "Amount";

	private const string INPUT_SHOW_GRAPHIC_NAME = "Show in Starlog";

	private const string INPUT_CONSUMABLE_NAME = "Consumable object";

	private const string OUTPUT_FLOW_NAME = "Out";

	private const string OUTPUT_CURRENT_AMOUNT_NAME = "Current Amount";

	private const int INPUT_FLOW_INDEX = 0;

	private const int INPUT_CONSUMABLE_INDEX = 1;

	private const int INPUT_AMOUNT_INDEX = 2;

	private const int INPUT_SHOW_GRAPHIC_INDEX = 3;

	private const int OUTPUT_FLOW_INDEX = 0;

	private const int OUTPUT_CURRENT_AMOUNT_INDEX = 1;

	[SerializeField]
	private float _amount;

	[SerializeField]
	private ConsumableRemedium _consumableRemedium;

	[SerializeField]
	private bool _showStarlogGraphic = true;

	public override string GetID => "EE_AddConsumableNode";

	public override Node Create(Vector2 pos)
	{
		AddConsumableNode addConsumableNode = ScriptableObject.CreateInstance<AddConsumableNode>();
		addConsumableNode.rect = new Rect(pos.x, pos.y, 180f, 100f);
		addConsumableNode.name = "Add Consumable";
		addConsumableNode.CreateMutliInput("In", "Flow");
		addConsumableNode.CreateInput("Consumable object", "ConsumableRemedium");
		addConsumableNode.CreateInput("Amount", "Float");
		addConsumableNode.CreateInput("Show in Starlog", "Bool");
		addConsumableNode.CreateOutput("Out", "Flow");
		addConsumableNode.CreateOutput("Current Amount", "Float");
		return addConsumableNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		AddConsumableNode obj = (AddConsumableNode)Create(rect.position + new Vector2(20f, 20f));
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
		_consumableRemedium.Add(_amount);
		if (_showStarlogGraphic)
		{
			TextIconJournalContent content = new TextIconJournalContent(_consumableRemedium.BaseStaticData.IconTerm, (int)_amount, EventContentData.ETextIconContentType.ADDITION, 0);
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
		return CastValue<T>(_consumableRemedium.RuntimeData.Amount - _consumableRemedium.RuntimeData.PlannedConsumption);
	}
}
