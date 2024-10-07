using System;
using NodeEditorFramework;
using RG.Parsecs.EndGameEditor;
using RG.Parsecs.EventEditor;
using RG.Parsecs.NodeEditor;
using RG.Parsecs.Survival;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Utility Nodes/Get Random VisualId Node", new Type[]
{
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(EndGameCanvas)
})]
public class GetRandomVisualIdNode : MessageNode
{
	public const string ID = "EE_GetRandomVisualIdNode";

	[SerializeField]
	private int _inputsCounter;

	private const string INPUT_VISUAL_ID_NAME = "VisualId ";

	private const string OUTPUT_RESULT_NAME = "Result";

	public override string GetID => "EE_GetRandomVisualIdNode";

	public override Node Create(Vector2 pos)
	{
		GetRandomVisualIdNode getRandomVisualIdNode = ScriptableObject.CreateInstance<GetRandomVisualIdNode>();
		getRandomVisualIdNode.rect = new Rect(pos.x, pos.y, 150f, 200f);
		getRandomVisualIdNode.name = "Get Random VisualId";
		getRandomVisualIdNode.CreateInput("VisualId 1", "VisualId");
		getRandomVisualIdNode.CreateInput("VisualId 2", "VisualId");
		getRandomVisualIdNode.CreateOutput("Result", "VisualId");
		return getRandomVisualIdNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		return Create(rect.position + new Vector2(20f, 20f));
	}

	protected override void NodeGUI()
	{
	}

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		if (output != 0)
		{
			throw new NotExistingOutputException(GetID, output);
		}
		int num = UnityEngine.Random.Range(0, Inputs.Count - 1);
		VisualId currentValue = null;
		GetInputValue(Inputs[num], ref currentValue, canvas);
		return CastValue<T>(currentValue);
	}
}
