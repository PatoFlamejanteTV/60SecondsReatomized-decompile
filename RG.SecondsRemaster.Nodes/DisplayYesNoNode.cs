using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using RG.SecondsRemaster.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Player Input/Display Yes-No Node", new Type[] { typeof(SurvivalEvent) })]
public class DisplayYesNoNode : PlayerDecisionNode
{
	private const string ID = "EE_DisplayYesNoNode";

	private const int OUTPUT_RESULT_INDEX = 0;

	private const string OUTPUT_RESULT_NAME = "Result";

	private const string NODE_NAME = "Display Yes/No";

	[SerializeField]
	private PlayerYesNoDecision _result = new PlayerYesNoDecision();

	public override string GetID => "EE_DisplayYesNoNode";

	public override Node Create(Vector2 pos)
	{
		DisplayYesNoNode displayYesNoNode = ScriptableObject.CreateInstance<DisplayYesNoNode>();
		displayYesNoNode.rect = new Rect(pos.x, pos.y, 200f, 160f);
		displayYesNoNode.name = "Display Yes/No";
		displayYesNoNode.CreateMutliInput("In", "Flow");
		displayYesNoNode.CreateOutput("Result", "PlayerDecision", NodeSide.Bottom);
		return displayYesNoNode;
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
		_result.WasChosen = true;
		YesNoChoiceJournalContent content = new YesNoChoiceJournalContent();
		SecondsEventManager.AddJournalContent(base.ParentCanvas, content);
	}

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		if (output != 0)
		{
			throw new NotExistingOutputException(GetID, output);
		}
		if (_result == null)
		{
			Debug.LogError("Result in GetValue in DisplayYesNoNode is null");
		}
		if (_result.WasChosen)
		{
			ChoiceCardController playerChoice = EventManager.GetPlayerChoice();
			if (playerChoice == null)
			{
				Debug.LogError("playerChoiceCard in GetValue in DisplayYesNoNode is null");
			}
			switch (playerChoice.GetChoiceType())
			{
			case EPlayerChoice.YES:
				_result.ChoosenNumber = 1;
				_result.Result = true;
				break;
			case EPlayerChoice.NO:
				_result.ChoosenNumber = 0;
				_result.Result = false;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		return CastValue<T>(_result);
	}
}
