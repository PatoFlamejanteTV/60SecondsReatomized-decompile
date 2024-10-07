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

[Node(false, "Player Input/Display Choose Character List Node", new Type[] { typeof(SurvivalEvent) })]
public class DisplayChooseCharacterListNodeV2 : PlayerDecisionNode
{
	private const string ID = "EE_DisplayChooseCharacterListNodeV2";

	private const string INPUT_CHARACTER_LIST_NAME = "Character list";

	private const string INPUT_CALL_TO_ACTION_NAME = "Call To Action";

	private const string OUTPUT_RESULT_NAME = "Result";

	private const string NODE_NAME = "Display Choose Character List";

	private const int INPUT_CHARACTER_LIST_INDEX = 1;

	private const int INPUT_CALL_TO_ACTION_INDEX = 2;

	private const int OUTPUT_RESULT_INDEX = 0;

	private List<Character> _characters;

	[SerializeField]
	private PlayerCharacterDecision _result = new PlayerCharacterDecision();

	[SerializeField]
	private LocalizedString _callToActionTerm;

	public override string GetID => "EE_DisplayChooseCharacterListNodeV2";

	public override Node Create(Vector2 pos)
	{
		DisplayChooseCharacterListNodeV2 displayChooseCharacterListNodeV = ScriptableObject.CreateInstance<DisplayChooseCharacterListNodeV2>();
		displayChooseCharacterListNodeV.rect = new Rect(pos.x, pos.y, 200f, 160f);
		displayChooseCharacterListNodeV.name = "Display Choose Character List";
		displayChooseCharacterListNodeV.CreateMutliInput("In", "Flow");
		displayChooseCharacterListNodeV.CreateInput("Character list", "CharacterList");
		displayChooseCharacterListNodeV.CreateInput("Call To Action", "LocalizedString");
		displayChooseCharacterListNodeV.CreateOutput("Result", "PlayerDecision", NodeSide.Bottom);
		return displayChooseCharacterListNodeV;
	}

	public override Node Duplicate(Vector2 pos)
	{
		return Create(rect.position + new Vector2(20f, 20f));
	}

	protected override void NodeEnable()
	{
	}

	protected override void NodeGUI()
	{
	}

	public override void Execute(NodeCanvas canvas)
	{
		_characters = GetInputValue<List<Character>>(Inputs[1], canvas);
		GetInputValue(Inputs[2], ref _callToActionTerm, canvas);
		_result.WasChosen = true;
		CharacterChoiceJournalContent content = null;
		if (_characters.Count == 1)
		{
			content = new CharacterChoiceJournalContent(new List<Character>
			{
				_characters[0],
				null,
				null,
				null
			}, _callToActionTerm);
		}
		else if (_characters.Count == 2)
		{
			content = new CharacterChoiceJournalContent(new List<Character>
			{
				_characters[0],
				_characters[1],
				null,
				null
			}, _callToActionTerm);
		}
		else if (_characters.Count == 3)
		{
			content = new CharacterChoiceJournalContent(new List<Character>
			{
				_characters[0],
				_characters[1],
				_characters[2],
				null
			}, _callToActionTerm);
		}
		else if (_characters.Count == 4)
		{
			content = new CharacterChoiceJournalContent(new List<Character>
			{
				_characters[0],
				_characters[1],
				_characters[2],
				_characters[3]
			}, _callToActionTerm);
		}
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
			else if (playerChoice.GetCharacterValue() == _characters[0])
			{
				_result.Result = _characters[0];
			}
			else if (playerChoice.GetCharacterValue() == _characters[1])
			{
				_result.Result = _characters[1];
			}
			else if (playerChoice.GetCharacterValue() == _characters[2])
			{
				_result.Result = _characters[2];
			}
			else if (playerChoice.GetCharacterValue() == _characters[3])
			{
				_result.Result = _characters[3];
			}
		}
		return CastValue<T>(_result);
	}
}
