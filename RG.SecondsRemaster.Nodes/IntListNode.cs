using System;
using NodeEditorFramework;
using RG.Parsecs.EventEditor;
using RG.Parsecs.GoalEditor;
using RG.Parsecs.NodeEditor;
using UnityEngine;

namespace RG.SecondsRemaster.Nodes;

[Node(false, "Lists/Int List", new Type[]
{
	typeof(SurvivalEvent),
	typeof(SystemEvent),
	typeof(NodeFunction),
	typeof(SystemStatusEvent),
	typeof(ExpeditionEvent),
	typeof(ReportEvent),
	typeof(Goal),
	typeof(ConditionEvent)
})]
public class IntListNode : ListNodeTemplate<int>
{
	public override string GetID => "SE_IntListNode";

	protected override string ConnectionType => "Int";

	public override Node Create(Vector2 pos)
	{
		IntListNode intListNode = ScriptableObject.CreateInstance<IntListNode>();
		intListNode.rect = new Rect(pos.x, pos.y, 100f, 85f);
		intListNode.name = "Int list";
		intListNode.CreateInput("Value 0", "Int");
		intListNode.CreateInput("Value 1", "Int");
		intListNode.CreateOutput("List", ListConnection.ID);
		return intListNode;
	}
}
