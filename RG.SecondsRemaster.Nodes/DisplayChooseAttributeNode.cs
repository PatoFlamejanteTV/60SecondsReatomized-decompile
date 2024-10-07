using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Player Input/Display Choose Attribute Node", new Type[] { typeof(SurvivalEvent) })]
public class DisplayChooseAttributeNode : PlayerDecisionNode
{
	private const string ID = "EE_DisplayChooseAttributeNode";

	private const string OUTPUT_RESULT_NAME = "Result";

	private const string NODE_NAME = "Display Choose Attribute";

	private const string INPUT_CHARACTER_NAME = "Character";

	private const string INPUT_IS_TEAM = "Is Team";

	private const string ATTRIBUTE_ONE_NAME = "Attribute 1";

	private const string ATTRIBUTE_TWO_NAME = "Attribute 2";

	private const string ATTRIBUTE_THREE_NAME = "Attribute 3";

	private const string ATTRIBUTE_FOUR_NAME = "Attribute 4";

	private const int CHARACTER_INPUT_INDEX = 5;

	[SerializeField]
	private CharacterAttribute _attribute1;

	[SerializeField]
	private CharacterAttribute _attribute2;

	[SerializeField]
	private CharacterAttribute _attribute3;

	[SerializeField]
	private CharacterAttribute _attribute4;

	[SerializeField]
	private Character _character;

	[SerializeField]
	private bool _isTeam;

	[SerializeField]
	private PlayerAttributeDecision _result = new PlayerAttributeDecision();

	public override string GetID => "EE_DisplayChooseAttributeNode";

	public override Node Create(Vector2 pos)
	{
		DisplayChooseAttributeNode displayChooseAttributeNode = ScriptableObject.CreateInstance<DisplayChooseAttributeNode>();
		displayChooseAttributeNode.rect = new Rect(pos.x, pos.y, 200f, 160f);
		displayChooseAttributeNode.name = "Display Choose Attribute";
		displayChooseAttributeNode.CreateMutliInput("In", "Flow");
		displayChooseAttributeNode.CreateInput("Attribute 1", "Attributes");
		displayChooseAttributeNode.CreateInput("Attribute 2", "Attributes");
		displayChooseAttributeNode.CreateInput("Attribute 3", "Attributes");
		displayChooseAttributeNode.CreateInput("Attribute 4", "Attributes");
		displayChooseAttributeNode.CreateInput("Character", "Character");
		displayChooseAttributeNode.CreateInput("Is Team", "Bool");
		displayChooseAttributeNode.CreateOutput("Result", "PlayerDecision", NodeSide.Bottom);
		return displayChooseAttributeNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		return Create(rect.position + new Vector2(20f, 20f));
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
		GetInputValue(Inputs[1], ref _attribute1, canvas);
		GetInputValue(Inputs[2], ref _attribute2, canvas);
		GetInputValue(Inputs[3], ref _attribute3, canvas);
		GetInputValue(Inputs[4], ref _attribute4, canvas);
		GetInputValue(Inputs[5], ref _character, canvas);
		GetInputValue(Inputs[6], ref _isTeam, canvas);
		_result.WasChosen = true;
		_result.IsTeam = _isTeam;
		_result.Character = _character;
		ParsecsEventManager.DisplayChoiceContent(_isTeam ? null : _character, _attribute1, _attribute2, _attribute3, _attribute4);
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
			if (playerChoice.GetAttributeValue() == _attribute1)
			{
				_result.ChoosenNumber = 0;
				_result.Result = _attribute1;
			}
			else if (playerChoice.GetAttributeValue() == _attribute2)
			{
				_result.ChoosenNumber = 1;
				_result.Result = _attribute2;
			}
			else if (playerChoice.GetAttributeValue() == _attribute3)
			{
				_result.ChoosenNumber = 2;
				_result.Result = _attribute3;
			}
			else if (playerChoice.GetAttributeValue() == _attribute4)
			{
				_result.ChoosenNumber = 3;
				_result.Result = _attribute4;
			}
		}
		return CastValue<T>(_result);
	}
}
