using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Supplies Nodes/Items/Use Item Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent)
})]
public class UseItemWithDurability : ResourceNode
{
	public const string ID = "EE_UseItemWithDurabailityNode";

	[SerializeField]
	private IItem _item;

	[SerializeField]
	private int _min = 10;

	[SerializeField]
	private int _max = 20;

	private const string INPUT_IN_NAME = "In";

	private const string INPUT_ITEM_NAME = "Item";

	private const string OUTPUT_OUT_NAME = "Out";

	private const string INPUT_MIN_NAME = "Min";

	private const string INPUT_MAX_NAME = "Max";

	private const int INPUT_IN_INDEX = 0;

	private const int INPUT_ITEM_INDEX = 1;

	private const int INPUT_MIN_INDEX = 2;

	private const int INPUT_MAX_INDEX = 3;

	private const int OUTPUT_OUT_INDEX = 0;

	private const string NODE_NAME = "Use Item";

	private const string OUTPUT_NOT_CONNECTED_MESSAGE = "Output is not connected";

	public override string GetID => "EE_UseItemWithDurabailityNode";

	public override Node Create(Vector2 pos)
	{
		UseItemWithDurability useItemWithDurability = ScriptableObject.CreateInstance<UseItemWithDurability>();
		useItemWithDurability.rect = new Rect(pos.x, pos.y, 180f, 130f);
		useItemWithDurability.name = "Use Item";
		useItemWithDurability.CreateMutliInput("In", "Flow");
		useItemWithDurability.CreateInput("Item", "Item");
		useItemWithDurability.CreateInput("Min", "Int");
		useItemWithDurability.CreateInput("Max", "Int");
		useItemWithDurability.CreateOutput("Out", "Flow");
		return useItemWithDurability;
	}

	public override Node Duplicate(Vector2 pos)
	{
		UseItemWithDurability obj = (UseItemWithDurability)Create(rect.position + new Vector2(20f, 20f));
		obj._item = _item;
		obj._max = _max;
		obj._min = _min;
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
		GetInputValue(Inputs[2], ref _min, canvas);
		GetInputValue(Inputs[3], ref _max, canvas);
		if (!_item.IsDamaged())
		{
			Item item = _item as Item;
			bool isAvailable = _item.BaseRuntimeData.IsAvailable;
			if (item != null)
			{
				item.UseItem(UnityEngine.Random.Range(_min, _max));
			}
			else
			{
				_item.Use();
			}
			if (_item.IsDamaged() || (!_item.BaseRuntimeData.IsAvailable && isAvailable))
			{
				TextIconJournalContent content = new TextIconJournalContent(_item.BaseStaticData.IconTerm, 1, EventContentData.ETextIconContentType.SUBTRACTION, 0);
				SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
			}
		}
		CheckAreAllFlowOutputsConnected();
		Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
	}
}
