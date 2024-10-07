using System;
using NodeEditorFramework;
using RG.Parsecs.EndGameEditor;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using UnityEngine;

namespace RG.Parsecs.NodeEditor;

[Node(false, "Utility Nodes/Cast To Term Node", new Type[]
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
public class CastToTermNode : EventNode
{
	public const string ID = "PE_CastToTerm";

	private const int INPUT_OBJECT_INDEX = 0;

	private const int OUTPUT_CHARACTER_INDEX = 0;

	private const string INPUT_OBJECT_NAME = "Object";

	private const string OUTPUT_CHARACTER_NAME = "Term";

	private const string NODE_NAME = "Cast To Term";

	public override string GetID => "PE_CastToTerm";

	public override Node Create(Vector2 pos)
	{
		CastToTermNode castToTermNode = ScriptableObject.CreateInstance<CastToTermNode>();
		castToTermNode.rect = new Rect(pos.x, pos.y, 150f, 75f);
		castToTermNode.name = "Cast To Term";
		castToTermNode.CreateInput("Object", ObjectConnection.ID);
		castToTermNode.CreateOutput("Term", "LocalizedString");
		return castToTermNode;
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
