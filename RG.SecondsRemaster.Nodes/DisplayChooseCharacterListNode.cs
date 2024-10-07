using System;
using System.Collections.Generic;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(true, "Player Input/Display Choose Character List Node", new Type[] { typeof(SurvivalEvent) })]
public class DisplayChooseCharacterListNode : PlayerDecisionNode
{
	private const string ID = "EE_DisplayChooseCharacterListNode";

	private const string OUTPUT_RESULT_NAME = "Result";

	private const string NODE_NAME = "Display Choose Character List";

	private const int INPUT_CHARACTER_LIST_INDEX = 1;

	private const int OUTPUT_RESULT_INDEX = 0;

	private const string INPUT_CHARACTER_LIST_NAME = "Character list";

	private List<Character> _characters;

	[SerializeField]
	private PlayerCharacterDecision _result = new PlayerCharacterDecision();

	public override string GetID => "EE_DisplayChooseCharacterListNode";

	public override Node Create(Vector2 pos)
	{
		DisplayChooseCharacterListNode displayChooseCharacterListNode = ScriptableObject.CreateInstance<DisplayChooseCharacterListNode>();
		displayChooseCharacterListNode.rect = new Rect(pos.x, pos.y, 200f, 160f);
		displayChooseCharacterListNode.name = "Display Choose Character List";
		displayChooseCharacterListNode.CreateMutliInput("In", "Flow");
		displayChooseCharacterListNode.CreateInput("Character list", "CharacterList");
		displayChooseCharacterListNode.CreateOutput("Result", "PlayerDecision", NodeSide.Bottom);
		return displayChooseCharacterListNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		return Create(rect.position + new Vector2(20f, 20f));
	}

	protected override void NodeGUI()
	{
	}

	public override void Execute(NodeCanvas canvas)
	{
		_characters = GetInputValue<List<Character>>(Inputs[1], canvas);
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
			}, null);
		}
		else if (_characters.Count == 2)
		{
			content = new CharacterChoiceJournalContent(new List<Character>
			{
				_characters[0],
				_characters[1],
				null,
				null
			}, null);
		}
		else if (_characters.Count == 3)
		{
			content = new CharacterChoiceJournalContent(new List<Character>
			{
				_characters[0],
				_characters[1],
				_characters[2],
				null
			}, null);
		}
		else if (_characters.Count == 4)
		{
			content = new CharacterChoiceJournalContent(new List<Character>
			{
				_characters[0],
				_characters[1],
				_characters[2],
				_characters[3]
			}, null);
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
