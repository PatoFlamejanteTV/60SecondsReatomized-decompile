using System;
using System.Collections.Generic;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Remaster/Player Input/Get Character Result List Node", new Type[] { typeof(SurvivalEvent) })]
public class SRGetCharacterResultListNode : PlayerDecisionNode
{
	private const string ID = "EE_SRGetCharacterResultListNode";

	private const string INPUT_RESULT_NAME = "Result";

	private const string INPUT_OUTPUT_CHARACTER_1_NAME = "Character 1";

	private const string INPUT_OUTPUT_CHARACTER_2_NAME = "Character 2";

	private const string INPUT_OUTPUT_CHARACTER_3_NAME = "Character 3";

	private const string INPUT_OUTPUT_CHARACTER_4_NAME = "Character 4";

	private const string OUTPUT_NO_CHOICE_NAME = "No Choice";

	private const string INPUT_CHARACTER_LIST_NAME = "CharacterList";

	private const string OUTPUT_CHARACTER_NAME = "Character";

	private const string NODE_NAME = "Get Character List Result";

	private const int INPUT_RESULT_INDEX = 1;

	private const int INPUT_CHARACTER_LIST_INDEX = 2;

	private const int OUTPUT_CHARACTER_1_INDEX = 0;

	private const int OUTPUT_CHARACTER_2_INDEX = 1;

	private const int OUTPUT_CHARACTER_3_INDEX = 2;

	private const int OUTPUT_CHARACTER_4_INDEX = 3;

	private const int OUTPUT_NO_CHOICE_INDEX = 4;

	private const int OUTPUT_CHARACTER_INDEX = 5;

	[SerializeField]
	private PlayerDecision _currentDecision;

	public override string GetID => "EE_SRGetCharacterResultListNode";

	public override Node Create(Vector2 pos)
	{
		SRGetCharacterResultListNode sRGetCharacterResultListNode = ScriptableObject.CreateInstance<SRGetCharacterResultListNode>();
		sRGetCharacterResultListNode.rect = new Rect(pos.x, pos.y, 200f, 160f);
		sRGetCharacterResultListNode.name = "Get Character List Result";
		sRGetCharacterResultListNode.CreateMutliInput("In", "Flow");
		sRGetCharacterResultListNode.CreateInput("Result", "PlayerDecision");
		sRGetCharacterResultListNode.CreateInput("CharacterList", "CharacterList");
		sRGetCharacterResultListNode.CreateOutput("Character 1", "Flow");
		sRGetCharacterResultListNode.CreateOutput("Character 2", "Flow");
		sRGetCharacterResultListNode.CreateOutput("Character 3", "Flow");
		sRGetCharacterResultListNode.CreateOutput("Character 4", "Flow");
		sRGetCharacterResultListNode.CreateOutput("No Choice", "Flow");
		sRGetCharacterResultListNode.CreateOutput("Character", "Character");
		return sRGetCharacterResultListNode;
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
		GetInputValue(Inputs[1], ref _currentDecision, canvas);
		if (!(_currentDecision is PlayerCharacterDecision))
		{
			throw new WrongDecisionTypeException(typeof(PlayerCharacterDecision), _currentDecision.GetType());
		}
		Character result = ((PlayerCharacterDecision)_currentDecision).Result;
		List<Character> inputValue = GetInputValue<List<Character>>(Inputs[2], canvas);
		if (base.ParentEvent is SurvivalEvent)
		{
			((SurvivalEvent)base.ParentEvent).WasEventSuccessful = result != null;
		}
		if (result == null)
		{
			if (!Outputs[4].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 4);
			}
			Outputs[4].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		if (inputValue.Count >= 1 && result == inputValue[0])
		{
			if (!Outputs[0].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 0);
			}
			Outputs[0].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		if (inputValue.Count >= 2 && result == inputValue[1])
		{
			if (!Outputs[1].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 1);
			}
			Outputs[1].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		if (inputValue.Count >= 3 && result == inputValue[2])
		{
			if (!Outputs[2].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 2);
			}
			Outputs[2].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		if (inputValue.Count >= 4 && result == inputValue[3])
		{
			if (!Outputs[3].isConnected)
			{
				throw new NotConnectedOutputException(GetID, 3);
			}
			Outputs[3].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
			return;
		}
		throw new WrongDecisionConnectionExcpetion(GetID);
	}

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		if (output != 5)
		{
			throw new NotExistingOutputException(GetID, output);
		}
		if (!(_currentDecision is PlayerCharacterDecision))
		{
			throw new WrongDecisionTypeException(typeof(PlayerCharacterDecision), _currentDecision.GetType());
		}
		Character result = ((PlayerCharacterDecision)_currentDecision).Result;
		return CastValue<T>(result);
	}
}
