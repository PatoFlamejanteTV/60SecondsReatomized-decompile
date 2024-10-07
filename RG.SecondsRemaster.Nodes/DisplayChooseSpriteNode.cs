using System;
using System.Collections.Generic;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Player Input/Display Choose Sprite Node", new Type[] { typeof(SurvivalEvent) })]
public class DisplayChooseSpriteNode : PlayerDecisionNode
{
	private const string ID = "EE_DisplayChooseSpriteNode ";

	private const int OUTPUT_RESULT_INDEX = 0;

	private const string OUTPUT_RESULT_NAME = "Result";

	private const string NODE_NAME = "Display Choose Sprite";

	private const string INPUT_CHOICE_1_NAME = "Choice 1";

	private const string INPUT_CHOICE_2_NAME = "Choice 2";

	private const string INPUT_CHOICE_3_NAME = "Choice 3";

	private const string INPUT_CHOICE_4_NAME = "Choice 4";

	private const int INPUT_CHOICE_1_INDEX = 1;

	private const int INPUT_CHOICE_2_INDEX = 2;

	private const int INPUT_CHOICE_3_INDEX = 3;

	private const int INPUT_CHOICE_4_INDEX = 4;

	private const int CHOICE_1_CARD_ID = 0;

	private const int CHOICE_2_CARD_ID = 1;

	private const int CHOICE_3_CARD_ID = 2;

	private const int CHOICE_4_CARD_ID = 3;

	private const int NO_CHOICE = -1;

	[SerializeField]
	private BaseActionCondition _choice1;

	[SerializeField]
	private BaseActionCondition _choice2;

	[SerializeField]
	private BaseActionCondition _choice3;

	[SerializeField]
	private BaseActionCondition _choice4;

	[SerializeField]
	private PlayerSpriteDecision _result = new PlayerSpriteDecision();

	public override string GetID => "EE_DisplayChooseSpriteNode ";

	public override Node Create(Vector2 pos)
	{
		DisplayChooseSpriteNode displayChooseSpriteNode = ScriptableObject.CreateInstance<DisplayChooseSpriteNode>();
		displayChooseSpriteNode.rect = new Rect(pos.x, pos.y, 200f, 160f);
		displayChooseSpriteNode.name = "Display Choose Sprite";
		displayChooseSpriteNode.CreateMutliInput("In", "Flow");
		displayChooseSpriteNode.CreateInput("Choice 1", "ActionCondition");
		displayChooseSpriteNode.CreateInput("Choice 2", "ActionCondition");
		displayChooseSpriteNode.CreateInput("Choice 3", "ActionCondition");
		displayChooseSpriteNode.CreateInput("Choice 4", "ActionCondition");
		displayChooseSpriteNode.CreateOutput("Result", "PlayerDecision", NodeSide.Bottom);
		return displayChooseSpriteNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		DisplayChooseSpriteNode obj = (DisplayChooseSpriteNode)Create(rect.position + new Vector2(20f, 20f));
		obj._choice1 = _choice1;
		obj._choice2 = _choice2;
		obj._choice3 = _choice3;
		obj._choice4 = _choice4;
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
		GetInputValue(Inputs[1], ref _choice1, canvas);
		GetInputValue(Inputs[2], ref _choice2, canvas);
		GetInputValue(Inputs[3], ref _choice3, canvas);
		GetInputValue(Inputs[4], ref _choice4, canvas);
		_result.WasChosen = true;
		SpriteChoiceJournalContent content = new SpriteChoiceJournalContent(new List<BaseActionCondition> { _choice1, _choice2, _choice3, _choice4 });
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
			if (playerChoice == null)
			{
				_result.ChoosenNumber = -1;
				_result.Result = null;
			}
			else
			{
				_result.ChoosenNumber = playerChoice.GetCardId();
				if (playerChoice.GetCardId() == 0)
				{
					_result.Result = _choice4;
					_result.ChoosenNumber = 3;
				}
				else if (playerChoice.GetCardId() == 1)
				{
					_result.Result = _choice3;
					_result.ChoosenNumber = 2;
				}
				else if (playerChoice.GetCardId() == 2)
				{
					_result.Result = _choice2;
					_result.ChoosenNumber = 1;
				}
				else if (playerChoice.GetCardId() == 3)
				{
					_result.Result = _choice1;
					_result.ChoosenNumber = 0;
				}
			}
		}
		return CastValue<T>(_result);
	}
}
