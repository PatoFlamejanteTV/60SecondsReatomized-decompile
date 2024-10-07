using System;
using System.Collections.Generic;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Remaster/Player Input/Display Choose Item Node", new Type[] { typeof(SurvivalEvent) })]
public class SRDisplayChooseItemNode : PlayerDecisionNode
{
	private const string ID = "EE_SRDisplayChooseItemNode";

	private const int OUTPUT_RESULT_INDEX = 0;

	private const string OUTPUT_RESULT_NAME = "Result";

	private const string NODE_NAME = "Display Choose Item";

	private const string INPUT_ITEM_1_NAME = "Item 1";

	private const string INPUT_ITEM_2_NAME = "Item 2";

	private const string INPUT_ITEM_3_NAME = "Item 3";

	private const string INPUT_ITEM_4_NAME = "Item 4";

	private const int INPUT_ITEM_1_INDEX = 1;

	private const int INPUT_ITEM_2_INDEX = 2;

	private const int INPUT_ITEM_3_INDEX = 3;

	private const int INPUT_ITEM_4_INDEX = 4;

	[SerializeField]
	private IItem _item1;

	[SerializeField]
	private IItem _item2;

	[SerializeField]
	private IItem _item3;

	[SerializeField]
	private IItem _item4;

	[SerializeField]
	private PlayerItemDecision _result = new PlayerItemDecision();

	public override string GetID => "EE_SRDisplayChooseItemNode";

	public override Node Create(Vector2 pos)
	{
		SRDisplayChooseItemNode sRDisplayChooseItemNode = ScriptableObject.CreateInstance<SRDisplayChooseItemNode>();
		sRDisplayChooseItemNode.rect = new Rect(pos.x, pos.y, 200f, 160f);
		sRDisplayChooseItemNode.name = "Display Choose Item";
		sRDisplayChooseItemNode.CreateMutliInput("In", "Flow");
		sRDisplayChooseItemNode.CreateInput("Item 1", "Item");
		sRDisplayChooseItemNode.CreateInput("Item 2", "Item");
		sRDisplayChooseItemNode.CreateInput("Item 3", "Item");
		sRDisplayChooseItemNode.CreateInput("Item 4", "Item");
		sRDisplayChooseItemNode.CreateOutput("Result", "PlayerDecision", NodeSide.Bottom);
		return sRDisplayChooseItemNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		SRDisplayChooseItemNode obj = (SRDisplayChooseItemNode)Create(rect.position + new Vector2(20f, 20f));
		obj._item1 = _item1;
		obj._item2 = _item2;
		obj._item3 = _item3;
		obj._item3 = _item4;
		return obj;
	}

	protected override void NodeEnable()
	{
	}

	protected override void NodeGUI()
	{
	}

	public override void Execute(NodeCanvas canvas)
	{
		GetInputValue(Inputs[1], ref _item1, canvas);
		GetInputValue(Inputs[2], ref _item2, canvas);
		GetInputValue(Inputs[3], ref _item3, canvas);
		GetInputValue(Inputs[4], ref _item4, canvas);
		_result.WasChosen = true;
		ItemChoiceJournalContent content = new ItemChoiceJournalContent(new List<IItem> { _item1, _item2, _item3, _item4 });
		SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
	}

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		if (output != 0)
		{
			throw new NotExistingOutputException(GetID, output);
		}
		if (_result.WasChosen)
		{
			ChoiceCardController playerChoice = EventManager.GetPlayerChoice();
			if (playerChoice == null || playerChoice.GetChoiceType() == EPlayerChoice.NO_CHOICE)
			{
				_result.Result = null;
			}
			else if (playerChoice.GetItemValue() == _item1)
			{
				_result.ChoosenNumber = 0;
				_result.Result = _item1;
			}
			else if (playerChoice.GetItemValue() == _item2)
			{
				_result.ChoosenNumber = 1;
				_result.Result = _item2;
			}
			else if (playerChoice.GetItemValue() == _item3)
			{
				_result.ChoosenNumber = 2;
				_result.Result = _item3;
			}
			else if (playerChoice.GetItemValue() == _item4)
			{
				_result.ChoosenNumber = 3;
				_result.Result = _item4;
			}
		}
		return CastValue<T>(_result);
	}
}
