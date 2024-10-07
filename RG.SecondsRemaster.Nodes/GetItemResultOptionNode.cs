using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Player Input/Get Item Result Node", new Type[] { typeof(SurvivalEvent) })]
public class GetItemResultOptionNode : PlayerDecisionNode
{
	private const string ID = "EE_GetItemResultOptionNode";

	private const string INPUT_RESULT_NAME = "Result";

	private const string INPUT_OUTPUT_ITEM_1_NAME = "Item 1";

	private const string INPUT_OUTPUT_ITEM_2_NAME = "Item 2";

	private const string INPUT_OUTPUT_ITEM_3_NAME = "Item 3";

	private const string OUTPUT_NO_CHOICE_NAME = "No Choice";

	private const string OUTPUT_ITEM_NAME = "Item";

	private const string NODE_NAME = "Get Item Result";

	private const string USE_ITEM_NAME = "Use item";

	private const int INPUT_RESULT_INDEX = 1;

	private const int INPUT_ITEM_1_INDEX = 2;

	private const int INPUT_USE_ITEM_1_INDEX = 3;

	private const int INPUT_ITEM_2_INDEX = 4;

	private const int INPUT_USE_ITEM_2_INDEX = 5;

	private const int INPUT_ITEM_3_INDEX = 6;

	private const int INPUT_USE_ITEM_3_INDEX = 7;

	private const int OUTPUT_ITEM_1_INDEX = 0;

	private const int OUTPUT_ITEM_2_INDEX = 1;

	private const int OUTPUT_ITEM_3_INDEX = 2;

	private const int OUTPUT_NO_CHOICE_INDEX = 3;

	private const int OUTPUT_ITEM_INDEX = 4;

	[SerializeField]
	private PlayerDecision _currentDecision;

	[SerializeField]
	private IItem _item1;

	[SerializeField]
	private bool _useItem1 = true;

	[SerializeField]
	private IItem _item2;

	[SerializeField]
	private bool _useItem2 = true;

	[SerializeField]
	private IItem _item3;

	[SerializeField]
	private bool _useItem3 = true;

	public override string GetID => "EE_GetItemResultOptionNode";

	public override Node Create(Vector2 pos)
	{
		GetItemResultOptionNode getItemResultOptionNode = ScriptableObject.CreateInstance<GetItemResultOptionNode>();
		getItemResultOptionNode.rect = new Rect(pos.x, pos.y, 200f, 160f);
		getItemResultOptionNode.name = "Get Item Result";
		getItemResultOptionNode.CreateMutliInput("In", "Flow");
		getItemResultOptionNode.CreateInput("Result", "PlayerDecision");
		getItemResultOptionNode.CreateInput("Item 1", "Item");
		getItemResultOptionNode.CreateInput("Use item", "Bool");
		getItemResultOptionNode.CreateInput("Item 2", "Item");
		getItemResultOptionNode.CreateInput("Use item", "Bool");
		getItemResultOptionNode.CreateInput("Item 3", "Item");
		getItemResultOptionNode.CreateInput("Use item", "Bool");
		getItemResultOptionNode.CreateOutput("Item 1", "Flow");
		getItemResultOptionNode.CreateOutput("Item 2", "Flow");
		getItemResultOptionNode.CreateOutput("Item 3", "Flow");
		getItemResultOptionNode.CreateOutput("No Choice", "Flow");
		getItemResultOptionNode.CreateOutput("Item", "Item");
		return getItemResultOptionNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		return Create(rect.position + new Vector2(20f, 20f));
	}

	protected override void OnNodeValidate()
	{
	}

	protected override void NodeEnable()
	{
	}

	protected override void NodeGUI()
	{
	}

	public override void Execute(NodeCanvas canvas)
	{
		GetInputValue(Inputs[1], ref _currentDecision, canvas);
		if (!(_currentDecision is PlayerItemDecision))
		{
			throw new WrongDecisionTypeException(typeof(PlayerItemDecision), _currentDecision.GetType());
		}
		IItem result = ((PlayerItemDecision)_currentDecision).Result;
		GetInputValue(Inputs[2], ref _item1, canvas);
		GetInputValue(Inputs[4], ref _item2, canvas);
		GetInputValue(Inputs[6], ref _item3, canvas);
		GetInputValue(Inputs[3], ref _useItem1, canvas);
		GetInputValue(Inputs[5], ref _useItem2, canvas);
		GetInputValue(Inputs[7], ref _useItem3, canvas);
		if (base.ParentEvent is SurvivalEvent)
		{
			((SurvivalEvent)base.ParentEvent).WasEventSuccessful = result != null;
		}
		if (result == null)
		{
			if (!Outputs[3].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 3);
			}
			Outputs[3].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		if (result == _item1)
		{
			if (_useItem1)
			{
				UseItem(result);
			}
			if (!Outputs[0].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 0);
			}
			Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		if (result == _item2)
		{
			if (_useItem2)
			{
				UseItem(result);
			}
			if (!Outputs[1].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 1);
			}
			Outputs[1].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		if (result == _item3)
		{
			if (_useItem3)
			{
				UseItem(result);
			}
			if (!Outputs[2].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 2);
			}
			Outputs[2].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		throw new WrongDecisionConnectionExcpetion(GetID);
	}

	private void UseItem(IItem chosenItem)
	{
		if (!chosenItem.IsDamaged())
		{
			Item item = chosenItem as Item;
			bool isAvailable = chosenItem.BaseRuntimeData.IsAvailable;
			if (item != null)
			{
				item.UseItem(ItemManager.ITEM_DURABAILITY_USAGE);
			}
			else
			{
				chosenItem.Use();
			}
			if (chosenItem.IsDamaged() || (!chosenItem.BaseRuntimeData.IsAvailable && isAvailable))
			{
				TextIconJournalContent content = new TextIconJournalContent(chosenItem.BaseStaticData.IconTerm, 1, EventContentData.ETextIconContentType.SUBTRACTION, 0);
				SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
			}
		}
	}

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		if (output != 4)
		{
			throw new NotExistingOutputException(GetID, output);
		}
		if (!(_currentDecision is PlayerItemDecision))
		{
			throw new WrongDecisionTypeException(typeof(PlayerItemDecision), _currentDecision.GetType());
		}
		IItem result = ((PlayerItemDecision)_currentDecision).Result;
		return CastValue<T>(result);
	}
}
