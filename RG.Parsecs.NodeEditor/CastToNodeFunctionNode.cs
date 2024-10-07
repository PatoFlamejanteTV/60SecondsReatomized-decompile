using System;
using NodeEditorFramework;
using RG.Parsecs.EndGameEditor;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using UnityEngine;

namespace RG.Parsecs.NodeEditor;

[Node(false, "Utility Nodes/Cast To Node Function Node", new Type[]
{
	typeof(Goal),
	typeof(SurvivalEvent),
	typeof(ReportEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(EndGameCanvas),
	typeof(ConditionEvent)
})]
public class CastToNodeFunctionNode : EventNode
{
	public const string ID = "PE_CastToNodeFunction";

	private const int INPUT_OBJECT_INDEX = 0;

	private const int OUTPUT_CHARACTER_INDEX = 0;

	private const string INPUT_OBJECT_NAME = "Object";

	private const string OUTPUT_CHARACTER_NAME = "Node Function";

	private const string NODE_NAME = "Cast To Node Function";

	public override string GetID => "PE_CastToNodeFunction";

	public override Node Create(Vector2 pos)
	{
		CastToNodeFunctionNode castToNodeFunctionNode = ScriptableObject.CreateInstance<CastToNodeFunctionNode>();
		castToNodeFunctionNode.rect = new Rect(pos.x, pos.y, 180f, 75f);
		castToNodeFunctionNode.name = "Cast To Node Function";
		castToNodeFunctionNode.CreateInput("Object", ObjectConnection.ID);
		castToNodeFunctionNode.CreateOutput("Node Function", "NodeFunction");
		return castToNodeFunctionNode;
	}

	public override Node Duplicate(Vector2 pos)
	{
		return Create(rect.position + new Vector2(20f, 20f));
	}

	public override T GetValue<T>(int output, NodeCanvas canvas)
	{
		if (output != 0)
		{
			throw new NotExistingOutputException(GetID, output);
		}
		return CastValue<T>(GetInputValue<object>(Inputs[0], canvas));
	}
}
