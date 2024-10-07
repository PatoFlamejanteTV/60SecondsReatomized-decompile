using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Player Input/Display Choose Resource Node", new Type[] { typeof(SurvivalEvent) })]
public class DisplayChooseResourceNode : PlayerDecisionNode
{
	private const string ID = "EE_DisplayChooseResourceNode ";

	private const int OUTPUT_RESULT_INDEX = 0;

	private const string OUTPUT_RESULT_NAME = "Result";

	private const string NODE_NAME = "Display Choose Resource";

	private const string INPUT_RES_1_NAME = "Resource 1";

	private const string INPUT_RES_2_NAME = "Resource 2";

	private const string INPUT_RES_3_NAME = "Resource 3";

	private const string INPUT_RES_1_AMOUNT_NAME = "Resource 1 Amount";

	private const string INPUT_RES_2_AMOUNT_NAME = "Resource 2 Amount";

	private const string INPUT_RES_3_AMOUNT_NAME = "Resource 3 Amount";

	private const string INPUT_USE_RES_NAME = "Use Resource";

	private const int INPUT_RES_1_INDEX = 1;

	private const int INPUT_RES_1_AMOUNT_INDEX = 2;

	private const int INPUT_RES_2_INDEX = 3;

	private const int INPUT_RES_2_AMOUNT_INDEX = 4;

	private const int INPUT_RES_3_INDEX = 5;

	private const int INPUT_RES_3_AMOUNT_INDEX = 6;

	private const int INPUT_USE_RES_INDEX = 7;

	private const int RES_1_CARD_ID = 3;

	private const int RES_2_CARD_ID = 2;

	private const int RES_3_CARD_ID = 1;

	private const int NO_CHOICE_CARD_ID = 0;

	private const string RESOURCE_IS_NEGATIVE_ERROR = "Resource of id: {0} in DisplayChooseResourceNode can't be negative. Current value {1}";

	[SerializeField]
	private GameResource _resource1;

	[SerializeField]
	private GameResource _resource2;

	[SerializeField]
	private GameResource _resource3;

	[SerializeField]
	private PlayerResourceDecision _result = new PlayerResourceDecision();

	[SerializeField]
	private bool _useResource;

	public override string GetID => "EE_DisplayChooseResourceNode ";

	public override Node Create(Vector2 pos)
	{
		DisplayChooseResourceNode displayChooseResourceNode = ScriptableObject.CreateInstance<DisplayChooseResourceNode>();
		displayChooseResourceNode.rect = new Rect(pos.x, pos.y, 200f, 160f);
		displayChooseResourceNode.name = "Display Choose Resource";
		displayChooseResourceNode.CreateMutliInput("In", "Flow");
		displayChooseResourceNode.CreateInput("Resource 1", "Resources");
		displayChooseResourceNode.CreateInput("Resource 1 Amount", "Int");
		displayChooseResourceNode.CreateInput("Resource 2", "Resources");
		displayChooseResourceNode.CreateInput("Resource 2 Amount", "Int");
		displayChooseResourceNode.CreateInput("Resource 3", "Resources");
		displayChooseResourceNode.CreateInput("Resource 3 Amount", "Int");
		displayChooseResourceNode.CreateInput("Use Resource", "Bool");
		displayChooseResourceNode.CreateOutput("Result", "PlayerDecision", NodeSide.Bottom);
		return displayChooseResourceNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		DisplayChooseResourceNode obj = (DisplayChooseResourceNode)Create(rect.position + new Vector2(20f, 20f));
		obj._resource1 = new GameResource(_resource1.Resource, _resource1.Amount);
		obj._resource2 = new GameResource(_resource2.Resource, _resource2.Amount);
		obj._resource3 = new GameResource(_resource3.Resource, _resource3.Amount);
		obj._useResource = _useResource;
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
		if (_useResource)
		{
			if (_resource1.Resource != null && _resource1.Amount < 0 && !Inputs[1].isConnected)
			{
				LogMessage($"Resource of id: {1} in DisplayChooseResourceNode can't be negative. Current value {_resource1.Amount}", EMessageType.ERROR);
			}
			if (_resource2.Resource != null && _resource2.Amount < 0 && !Inputs[3].isConnected)
			{
				LogMessage($"Resource of id: {2} in DisplayChooseResourceNode can't be negative. Current value {_resource2.Amount}", EMessageType.ERROR);
			}
			if (_resource3.Resource != null && _resource3.Amount < 0 && !Inputs[5].isConnected)
			{
				LogMessage($"Resource of id: {3} in DisplayChooseResourceNode can't be negative. Current value {_resource3.Amount}", EMessageType.ERROR);
			}
		}
	}

	public override void Execute(NodeCanvas canvas)
	{
		GetInputValue(Inputs[1], ref _resource1.Resource, canvas);
		GetInputValue(Inputs[3], ref _resource2.Resource, canvas);
		GetInputValue(Inputs[5], ref _resource3.Resource, canvas);
		GetInputValue(Inputs[2], ref _resource1.Amount, canvas);
		GetInputValue(Inputs[4], ref _resource2.Amount, canvas);
		GetInputValue(Inputs[6], ref _resource3.Amount, canvas);
		GetInputValue(Inputs[7], ref _useResource, canvas);
		if (_useResource)
		{
			if (_resource1.Resource != null && _resource1.Amount < 0)
			{
				Debug.LogErrorFormat("Resource of id: {0} in DisplayChooseResourceNode can't be negative. Current value {1}", 1, _resource1.Amount);
			}
			if (_resource2.Resource != null && _resource2.Amount < 0)
			{
				Debug.LogErrorFormat("Resource of id: {0} in DisplayChooseResourceNode can't be negative. Current value {1}", 2, _resource2.Amount);
			}
			if (_resource3.Resource != null && _resource3.Amount < 0)
			{
				Debug.LogErrorFormat("Resource of id: {0} in DisplayChooseResourceNode can't be negative. Current value {1}", 3, _resource3.Amount);
			}
		}
		_result.UseResource = _useResource;
		_result.WasChosen = true;
		ParsecsEventManager.DisplayChoiceContent(_useResource, _resource1, _resource2, _resource3);
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
			_result.ChoosenNumber = playerChoice.GetCardId();
			if (playerChoice.GetCardId() == 0)
			{
				_result.Result = new GameResource(null, 0);
			}
			else if (playerChoice.GetCardId() == 3)
			{
				_result.Result = new GameResource(_resource1.Resource, _resource1.Amount);
			}
			else if (playerChoice.GetCardId() == 2)
			{
				_result.Result = new GameResource(_resource2.Resource, _resource2.Amount);
			}
			else if (playerChoice.GetCardId() == 1)
			{
				_result.Result = new GameResource(_resource3.Resource, _resource3.Amount);
			}
		}
		return CastValue<T>(_result);
	}
}
