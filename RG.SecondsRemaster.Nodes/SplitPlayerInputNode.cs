using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Player Input/Split Player Input Node", new Type[] { typeof(SurvivalEvent) })]
public class SplitPlayerInputNode : PlayerDecisionNode
{
	private const string ID = "EE_SplitPlayerInputNode";

	private const string INPUT_DECISION_NAME = "Decision";

	private const string OUTPUT_DECISION_NAME = "Output";

	private const string NODE_NAME = "Split Player Input";

	private const string OUTPUT_RESULT_NAME = "Result";

	[SerializeField]
	private PlayerDecision _currentDecision;

	public override string GetID => "EE_SplitPlayerInputNode";

	public override Node Create(Vector2 pos)
	{
		SplitPlayerInputNode splitPlayerInputNode = ScriptableObject.CreateInstance<SplitPlayerInputNode>();
		splitPlayerInputNode.rect = new Rect(pos.x, pos.y, 220f, 160f);
		splitPlayerInputNode.name = "Split Player Input";
		splitPlayerInputNode.CreateMutliInput("In", "Flow");
		splitPlayerInputNode.CreateInput("Decision", "PlayerDecision");
		splitPlayerInputNode.CreateOutput("Result", "PlayerDecision");
		splitPlayerInputNode.CreateOutput("Output", "Flow");
		return splitPlayerInputNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		return Create(rect.position + new Vector2(20f, 20f));
	}

	private void UpdateKnobsCount()
	{
	}

	protected virtual void DisplayNodeInputs(NodeInput input)
	{
	}

	protected override void NodeGUI()
	{
	}

	public override Rect GetNodeRect()
	{
		return new Rect(rect.x, rect.y, 300f, 35 + 20 * Inputs.Count);
	}

	public override void Execute(NodeCanvas canvas)
	{
		for (int i = 1; i < Inputs.Count; i++)
		{
			GetInputValue(Inputs[i], ref _currentDecision, canvas);
			if (_currentDecision != null && _currentDecision.WasChosen)
			{
				_currentDecision.WasChosen = false;
				if (!Outputs[i].isConnected)
				{
					throw new NotConnectedOutputException(GetID, i);
				}
				Outputs[i].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
				break;
			}
			if (i == Inputs.Count - 1)
			{
				if (!Outputs[i].isConnected)
				{
					throw new NotConnectedOutputException(GetID, i);
				}
				Outputs[i].GetCustomNodeAcrossConnection<ParsecsNode>().ExecuteWithErrorHandling(canvas);
				break;
			}
		}
	}

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		if (output != 0)
		{
			throw new NotExistingOutputException(GetID, output);
		}
		return CastValue<T>(_currentDecision);
	}
}
