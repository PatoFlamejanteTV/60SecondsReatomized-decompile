using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(true, "Legacy/Get Item Result Node", new Type[] { typeof(SurvivalEvent) })]
public class GetItemResultNode : PlayerDecisionNode
{
	private const string ID = "EE_GetItemResultNode";

	private const string INPUT_RESULT_NAME = "Result";

	private const string INPUT_OUTPUT_ITEM_1_NAME = "Item 1";

	private const string INPUT_OUTPUT_ITEM_2_NAME = "Item 2";

	private const string INPUT_OUTPUT_ITEM_3_NAME = "Item 3";

	private const string OUTPUT_NO_CHOICE_NAME = "No Choice";

	private const string OUTPUT_ITEM_NAME = "Item";

	private const string NODE_NAME = "Get Item Result";

	private const int INPUT_RESULT_INDEX = 1;

	private const int INPUT_ITEM_1_INDEX = 2;

	private const int INPUT_ITEM_2_INDEX = 3;

	private const int INPUT_ITEM_3_INDEX = 4;

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
	private IItem _item2;

	[SerializeField]
	private IItem _item3;

	public override string GetID => "EE_GetItemResultNode";

	public override Node Create(Vector2 pos)
	{
		GetItemResultNode getItemResultNode = ScriptableObject.CreateInstance<GetItemResultNode>();
		getItemResultNode.rect = new Rect(pos.x, pos.y, 200f, 160f);
		getItemResultNode.name = "Get Item Result";
		getItemResultNode.CreateMutliInput("In", "Flow");
		getItemResultNode.CreateInput("Result", "PlayerDecision");
		getItemResultNode.CreateInput("Item 1", "Item");
		getItemResultNode.CreateInput("Item 2", "Item");
		getItemResultNode.CreateInput("Item 3", "Item");
		getItemResultNode.CreateOutput("Item 1", "Flow");
		getItemResultNode.CreateOutput("Item 2", "Flow");
		getItemResultNode.CreateOutput("Item 3", "Flow");
		getItemResultNode.CreateOutput("No Choice", "Flow");
		getItemResultNode.CreateOutput("Item", "Item");
		return getItemResultNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		return Create(rect.position + new Vector2(20f, 20f));
	}

	protected override void OnNodeValidate()
	{
		base.OnNodeValidate();
		if (!Inputs[1].isConnected)
		{
			LogMessage($"Result input is not connected: {GetID}", EMessageType.ERROR);
		}
		if (Outputs[0].isConnected && !Inputs[2].isConnected && _item1 == null)
		{
			LogMessage($"Item 1 Output is connected but Item 1 Input is not or it's value is null in node: {GetID}", EMessageType.ERROR);
		}
		if (Outputs[1].isConnected && !Inputs[3].isConnected && _item2 == null)
		{
			LogMessage($"Item 2 Output is connected but Item 2 Input is not or it's value is null in node: {GetID}", EMessageType.ERROR);
		}
		if (Outputs[2].isConnected && !Inputs[4].isConnected && _item3 == null)
		{
			LogMessage($"Item 3 Output is connected but Item 3 Input is not or it's value is null in node: {GetID}", EMessageType.ERROR);
		}
		if (!Outputs[3].isConnected)
		{
			LogMessage($"No Choice output is not connected: {GetID}", EMessageType.ERROR);
		}
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
		GetInputValue(Inputs[3], ref _item2, canvas);
		GetInputValue(Inputs[4], ref _item3, canvas);
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
		bool num = result.IsDamaged();
		Item item = result as Item;
		if (item != null)
		{
			item.UseItem(ItemManager.ITEM_DURABAILITY_USAGE);
		}
		else
		{
			result.Use();
		}
		if (!num)
		{
			bool isAvailable = result.BaseRuntimeData.IsAvailable;
			if (item != null)
			{
				item.UseItem(ItemManager.ITEM_DURABAILITY_USAGE);
			}
			else
			{
				result.Use();
			}
			if (result.IsDamaged() || (!result.BaseRuntimeData.IsAvailable && isAvailable))
			{
				TextIconJournalContent content = new TextIconJournalContent(result.BaseStaticData.IconTerm, 1, EventContentData.ETextIconContentType.SUBTRACTION, 0);
				SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
			}
		}
		if (result == _item1)
		{
			if (!Outputs[0].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 0);
			}
			Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		if (result == _item2)
		{
			if (!Outputs[1].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 1);
			}
			Outputs[1].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		if (result == _item3)
		{
			if (!Outputs[2].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 2);
			}
			Outputs[2].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		throw new WrongDecisionConnectionExcpetion(GetID);
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
