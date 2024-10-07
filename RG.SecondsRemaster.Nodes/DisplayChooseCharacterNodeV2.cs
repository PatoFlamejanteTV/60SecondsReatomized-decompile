using System;
using System.Collections.Generic;
using I2.Loc;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Player Input/Display Choose Character Node", new Type[] { typeof(SurvivalEvent) })]
public class DisplayChooseCharacterNodeV2 : PlayerDecisionNode
{
	private const string ID = "EE_DisplayChooseCharacterNode";

	private const string OUTPUT_RESULT_NAME = "Result";

	private const string NODE_NAME = "Display Choose Character";

	private const string INPUT_CHARACTER_1_NAME = "Character 1";

	private const string INPUT_CHARACTER_2_NAME = "Character 2";

	private const string INPUT_CHARACTER_3_NAME = "Character 3";

	private const string INPUT_CHARACTER_4_NAME = "Character 4";

	private const string INPUT_CALL_TO_ACTION_NAME = "Call To Action";

	private const int INPUT_CHARACTER_1_INDEX = 1;

	private const int INPUT_CHARACTER_2_INDEX = 2;

	private const int INPUT_CHARACTER_3_INDEX = 3;

	private const int INPUT_CHARACTER_4_INDEX = 4;

	private const int INPUT_CALL_TO_ACTION_INDEX = 5;

	private const int OUTPUT_RESULT_INDEX = 0;

	[SerializeField]
	private Character _character1;

	[SerializeField]
	private Character _character2;

	[SerializeField]
	private Character _character3;

	[SerializeField]
	private Character _character4;

	[SerializeField]
	private PlayerCharacterDecision _result = new PlayerCharacterDecision();

	[SerializeField]
	private LocalizedString _callToActionTerm;

	public override string GetID => "EE_DisplayChooseCharacterNode";

	public override Node Create(Vector2 pos)
	{
		DisplayChooseCharacterNodeV2 displayChooseCharacterNodeV = ScriptableObject.CreateInstance<DisplayChooseCharacterNodeV2>();
		displayChooseCharacterNodeV.rect = new Rect(pos.x, pos.y, 200f, 160f);
		displayChooseCharacterNodeV.name = "Display Choose Character";
		displayChooseCharacterNodeV.CreateMutliInput("In", "Flow");
		displayChooseCharacterNodeV.CreateInput("Character 1", "Character");
		displayChooseCharacterNodeV.CreateInput("Character 2", "Character");
		displayChooseCharacterNodeV.CreateInput("Character 3", "Character");
		displayChooseCharacterNodeV.CreateInput("Character 4", "Character");
		displayChooseCharacterNodeV.CreateInput("Call To Action", "LocalizedString");
		displayChooseCharacterNodeV.CreateOutput("Result", "PlayerDecision", NodeSide.Bottom);
		return displayChooseCharacterNodeV;
	}

	public override Node Duplicate(Vector2 pos)
	{
		DisplayChooseCharacterNodeV2 obj = (DisplayChooseCharacterNodeV2)Create(rect.position + new Vector2(20f, 20f));
		obj._character1 = _character1;
		obj._character2 = _character2;
		obj._character3 = _character3;
		obj._character4 = _character4;
		obj._callToActionTerm = _callToActionTerm;
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
		GetInputValue(Inputs[1], ref _character1, canvas);
		GetInputValue(Inputs[2], ref _character2, canvas);
		GetInputValue(Inputs[3], ref _character3, canvas);
		GetInputValue(Inputs[4], ref _character4, canvas);
		GetInputValue(Inputs[5], ref _callToActionTerm, canvas);
		_result.WasChosen = true;
		CharacterChoiceJournalContent content = new CharacterChoiceJournalContent(new List<Character> { _character1, _character2, _character3, _character4 }, _callToActionTerm);
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
				_result.Result = null;
			}
			else if (playerChoice.GetCharacterValue() == _character1)
			{
				_result.Result = _character1;
			}
			else if (playerChoice.GetCharacterValue() == _character2)
			{
				_result.Result = _character2;
			}
			else if (playerChoice.GetCharacterValue() == _character3)
			{
				_result.Result = _character3;
			}
			else if (playerChoice.GetCharacterValue() == _character4)
			{
				_result.Result = _character4;
			}
		}
		return CastValue<T>(_result);
	}
}
